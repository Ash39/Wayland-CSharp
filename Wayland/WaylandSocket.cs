using Mono.Unix.Native;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Buffers.Binary;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Wayland
{
    public class WaylandSocket
    {
        private int socket;
        private List<byte> writeBuffer;
        private byte[] readBuffer;
        private List<int> writeFdBuffer;
        private int[] readFdBuffer;
        internal int position;
        internal int fdPosition;

        public WaylandSocket(string address)
        {
            socket = Syscall.socket(UnixAddressFamily.AF_UNIX, UnixSocketType.SOCK_STREAM, 0);
            if (socket < 0)
                throw new IOException("Failed to create a socket");

            var res = Syscall.connect(socket, new SockaddrUn(address, false));
            if (res != 0)
                throw new IOException($"Failed to connect socket (handle={socket}, code={res}): {Syscall.GetLastError()}");

            this.writeBuffer = new List<byte>();
            this.writeFdBuffer = new List<int>();
        }

        private void InternalWrite() 
        {
            var cmsgbuffer = new byte[Syscall.CMSG_SPACE((ulong)writeFdBuffer.Count * sizeof(int))];
            var cmsghdr = new Cmsghdr
            {
                cmsg_len = (long)Syscall.CMSG_LEN((ulong)writeFdBuffer.Count * sizeof(int)),
                cmsg_level = UnixSocketProtocol.SOL_SOCKET,
                cmsg_type = UnixSocketControlMessage.SCM_RIGHTS,
            };

            var msghdr = new Msghdr
            {
                msg_control = cmsgbuffer,
                msg_controllen = cmsgbuffer.Length,
            };
            cmsghdr.WriteToBuffer(msghdr, 0);

            var dataOffset = Syscall.CMSG_DATA(msghdr, 0);
            for (var i = 0; i < writeFdBuffer.Count; i++)
            {
                Array.Copy(BitConverter.GetBytes(writeFdBuffer[i]), 0, msghdr.msg_control, dataOffset, sizeof(int));
                dataOffset += sizeof(int);
            }

            byte[] buffer = writeBuffer.ToArray();

            GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            
            var iovs = new[] {
                    new Iovec {
                        iov_base = bufferHandle.AddrOfPinnedObject(),
                        iov_len = (ulong)buffer.Length,
                    },
                };
            msghdr.msg_iov = iovs;
            msghdr.msg_iovlen = 1;

            // Send it
            var ret = Syscall.sendmsg(socket, msghdr, 0);
            if (ret < 0)
                Mono.Unix.UnixMarshal.ThrowExceptionForLastError();

            bufferHandle.Free();

            writeFdBuffer.Clear();
            writeBuffer.Clear();
        }

        private int Read() 
        {
            position = 0;

            Pollfd[] pollfds = new Pollfd[] { new Pollfd { events = PollEvents.POLLIN, fd = socket } };

            int result = Syscall.poll(pollfds, 20);

            if (result > 0 && !pollfds[0].revents.HasFlag(PollEvents.POLLHUP)) 
            {
                var buffer = new byte[4096];
                var cmsg = new byte[1024];
                var msghdr = new Msghdr
                {
                    msg_control = cmsg,
                    msg_controllen = cmsg.Length,
                    msg_flags = MessageFlags.MSG_CMSG_CLOEXEC,
                };

                long datalen;

                GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                var iovec = new[] {
                    new Iovec {
                        iov_base = bufferHandle.AddrOfPinnedObject(),
                        iov_len = (ulong) buffer.Length,
                    },
                };

                msghdr.msg_iov = iovec;
                msghdr.msg_iovlen = iovec.Length;
                datalen = Syscall.recvmsg(socket, msghdr, MessageFlags.MSG_DONTWAIT | MessageFlags.MSG_CMSG_CLOEXEC);

                bufferHandle.Free();

                if (datalen == 0)
                    return 0;
                if (datalen < 0)
                    Mono.Unix.UnixMarshal.ThrowExceptionForLastError();

                // Get the offset of the first message
                var offset = Syscall.CMSG_FIRSTHDR(msghdr);

                if (offset > -1)
                {
                    // Extract the bytes
                    var recvHdr = Cmsghdr.ReadFromBuffer(msghdr, offset);
                    //DebugHelper.WriteLine("Buffer has, recvHdr.cmsg_len: {0}", recvHdr.cmsg_len);

                    // See how many bytes are of file descriptors we have
                    var recvDataOffset = Syscall.CMSG_DATA(msghdr, offset);
                    //var userData = recvHdr.cmsg_len - (int)Syscall.CMSG_LEN(0);
                    var bytes = recvHdr.cmsg_len - (recvDataOffset - offset);
                    var fdCount = bytes / sizeof(int);
                    readFdBuffer = new int[fdCount];
                    //DebugHelper.WriteLine("Got {0} fds ({1} bytes)", fdCount, bytes);

                    // Extract the file descriptors
                    for (int i = 0; i < fdCount; i++)
                    {
                        readFdBuffer[i] = BitConverter.ToInt32(msghdr.msg_control, (int)(recvDataOffset + (sizeof(int) * i)));
                        //DebugHelper.WriteLine($"fd[{i}] = {fds[i]}");
                    }

                    //DebugHelper.WriteLine("Read {0} fds and {1} bytes", fds.Length, data.Length);

                    // Check that we only have a single message
                    offset = Syscall.CMSG_NXTHDR(msghdr, offset);
                    if (offset != -1)
                        System.Diagnostics.Trace.WriteLine("WARNING: more than one message detected when reading SCM_RIGHTS, only processing the first one");
                }

                readBuffer = new byte[datalen];

                Array.Copy(buffer, readBuffer, readBuffer.Length);

                return readBuffer.Length;
            }

            return 0;
        }

        public void Write(int value)
        {
            Span<byte> bytes = new byte[4];
            if(BitConverter.IsLittleEndian)
                BinaryPrimitives.WriteInt32LittleEndian(bytes, value);
            else
                BinaryPrimitives.WriteInt32BigEndian(bytes, value);

            writeBuffer.AddRange(bytes.ToArray());
        }

        public void Write(uint value)
        {
            Span<byte> bytes = new byte[4];
            if(BitConverter.IsLittleEndian)
                BinaryPrimitives.WriteUInt32LittleEndian(bytes, value);
            else
                BinaryPrimitives.WriteUInt32BigEndian(bytes, value);

            writeBuffer.AddRange(bytes.ToArray());
        }

        public unsafe void Write(double value)
        {
            Span<byte> bytes = new byte[8];
            if(BitConverter.IsLittleEndian)
                BinaryPrimitives.WriteUInt64LittleEndian(bytes, BitConverter.DoubleToUInt64Bits(value));
            else
                BinaryPrimitives.WriteUInt64BigEndian(bytes, BitConverter.DoubleToUInt64Bits(value));

            writeBuffer.AddRange(bytes.ToArray());
        }

        public void Write(string s)
        {
            if (s == null)
            {
                Write((byte[])null);
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                byte[] withNull = new byte[bytes.Length + 1];
                bytes.CopyTo(withNull, 0);
                Write(withNull);
            }
        }

        public void Write(byte[] a)
        {
            if (a == null)
            {
                Write((uint)0);
            }
            else
            {
                int paddedLength = (a.Length + 3) / 4 * 4;
                Write((uint)a.Length);

                for (int i = 0; i < a.Length; i++)
                {
                    writeBuffer.Add(a[i]);
                }
                for (int i = 0; i < paddedLength - a.Length; i++)
                {
                    writeBuffer.Add(0);
                }
            }
        }

        public void Write(IntPtr h)
        {
            int dupFd = Syscall.dup(h.ToInt32());

            if (dupFd < 0)
                throw new Exception(string.Format("dup failed: {0}", h.ToInt32()));

            writeFdBuffer.Add(dupFd);
        }

        public int Flush()
        {
            if (writeBuffer.Count == 0)
                goto Read;
            InternalWrite();

        Read:
            return Read();
        }


        private byte[] InternalRead(int size)
        {
            byte[] bytes = new byte[size];
            for (int i = 0; i < size; i++)
                bytes[i] = readBuffer[position + i];

            position += size;

            return bytes;
        }

        public int ReadInt()
        {
            if(BitConverter.IsLittleEndian)
                return BinaryPrimitives.ReadInt32LittleEndian(InternalRead(4));
            else
                return BinaryPrimitives.ReadInt32BigEndian(InternalRead(4));
        }

        public uint ReadUInt()
        {
            if(BitConverter.IsLittleEndian)
                return BinaryPrimitives.ReadUInt32LittleEndian(InternalRead(4));
            else
                return BinaryPrimitives.ReadUInt32BigEndian(InternalRead(4));
        }

        public double ReadDouble()
        {
            if(BitConverter.IsLittleEndian)
                return BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(InternalRead(8)));
            else
                return BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64BigEndian(InternalRead(8)));
        }

        public string ReadString()
        {
            byte[] bytes = ReadBytes();
            byte[] bytesWithoutNull = new byte[bytes.Length - 1];
            Array.Copy(bytes, bytesWithoutNull, bytesWithoutNull.Length);
            return Encoding.UTF8.GetString(bytesWithoutNull);
        }

        public byte[] ReadBytes()
        {
            int length = (int)ReadUInt();
            int paddedLength = (length + 3) / 4 * 4;
            byte[] bytes = InternalRead(length);
            InternalRead(paddedLength - length);
            return bytes;
        }

        public IntPtr ReadFd()
        {
            return (IntPtr)readFdBuffer[fdPosition++];
        }

        internal (uint id, ushort opCode, uint length) ReadHeader()
        {
            uint id = ReadUInt();
            uint header = ReadUInt();
            uint length = (header & 0xffff0000) >> 16;
            ushort opcode = (ushort)(header & 0x0000ffff);

            return (id, opcode, length);
        }

        internal void Dispose()
        {
            Syscall.close(socket);
        }
    }
}
