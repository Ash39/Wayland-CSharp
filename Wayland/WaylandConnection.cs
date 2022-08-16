using Mono.Unix.Native;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wayland
{
    public class WaylandConnection
    {
        private WaylandSocket socket;
        private Queue<uint> freeIds;
        private Queue<Action> events = new Queue<Action>();
        private List<WaylandObject> objects;

        private const uint ClientRangeBegin = 0x00000001;
        private const uint ClientRangeEnd = 0xFEFFFFFF;
        private const uint ServerRangeBegin = 0xff000000;
        private const uint ServerRangeEnd = 0xffffffff;

        internal Func<uint, (IntPtr handle, uint id, uint version)> GetHandle;
        private Action<IntPtr> DeleteHandle; 

        private uint beginRange;
        private uint endRange;

        internal int position;

        public Queue<Action> Events { get => events;}

        public WaylandConnection(string socket,bool client = true)
        {
            this.socket = new WaylandSocket(socket);
            this.GetHandle = (factoryId) => { return (IntPtr.Zero, 0, 0); };
            this.DeleteHandle = null;
            if (client) 
            {
                beginRange = ClientRangeBegin;
                endRange = ClientRangeEnd;
            }
            else
            {
                beginRange = ServerRangeBegin;
                endRange = ServerRangeEnd;
            }

            objects = new List<WaylandObject>();
            freeIds = new Queue<uint>();
        }

        public uint Create() 
        {
            if (DeleteHandle == null) 
            {
                if (freeIds.Count > 0)
                {
                    return freeIds.Dequeue();
                }
                else
                {
                    uint id = ((uint)objects.Count) + ClientRangeBegin;
                    if (id >= beginRange && id <= endRange)
                    {
                        objects.Add(null);
                        return id;
                    }
                    else
                        throw new IndexOutOfRangeException();
                }
                
            }
            else
            {
                return 0;
            }
            
        }

        public void Get(uint id) 
        {
            freeIds = new Queue<uint>(freeIds.Where(c => c != id));
        }

        internal int Flush()
        {
            position = 0;
            return socket.Flush();
        }

        public void Destroy(uint id) 
        {
            if (id >= beginRange && id <= endRange)
            {
                DeleteHandle?.Invoke(this[id].handle);
                freeIds.Enqueue(id);
                this[id] = null;
            }
            else
                throw new IndexOutOfRangeException();
        }

        public WaylandObject this[uint id]
        {
            get
            {
                if (id >= beginRange && id <= endRange)
                    return objects[(int)(id - beginRange)];
                else
                    return null;
            }
            set
            {
                if (id >= beginRange && id <= endRange)
                    objects[(int)(id - beginRange)] = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }


        public void Marshal(uint id, ushort opcode, params object[] arguments)
        {
           
            ushort size = 8;
            foreach (object argument in arguments)
            {
                switch (argument)
                {
                    case int i:
                    case uint u:
                        size += 4;
                        break;
                    case double d:
                        size += 8;
                        break;
                    case string s:
                        size += 4;
                        if (s != null)
                            size += (ushort)((Encoding.UTF8.GetByteCount(s) + 4) / 4 * 4);
                        break;
                    case byte[] a:
                        size += 4;
                        if (a != null)
                            size += (ushort)((a.Length + 3) / 4 * 4);
                        break;
                    case IntPtr h:
                        break;
                }
            }
            socket.Write(id);
            socket.Write(((uint)size << 16) | (uint)opcode);
            foreach (object argument in arguments)
            {
                switch (argument)
                {
                    case int i:
                        socket.Write(i);
                        break;
                    case uint u:
                        socket.Write(u);
                        break;
                    case double d:
                        socket.Write(d);
                        break;
                    case string s:
                        socket.Write(s);
                        break;
                    case byte[] a:
                        socket.Write(a);
                        break;
                    case IntPtr h:
                        socket.Write(h);
                        break;
                }
            }


            //mutex.ReleaseMutex();
        }

        public bool Read() 
        {
            var header = socket.ReadHeader();

            position += (int)header.length;

            if(objects.ElementAtOrDefault((int)(header.id - beginRange)) == null)
                return false;
            WaylandType[] types = this[header.id].WaylandTypes(header.opCode);

            object[] args = new object[types.Length];
            int i = 0;

            foreach (WaylandType type in types)
            {
                switch (type)
                {
                    case WaylandType.Fd:
                        args[i] = socket.ReadFd();
                        break;
                    case WaylandType.Int:
                        args[i] = socket.ReadInt();
                        break;
                    case WaylandType.Uint:
                        args[i] = socket.ReadUInt();
                        break;
                    case WaylandType.Fixed:
                        args[i] = socket.ReadDouble();
                        break;
                    case WaylandType.Object:
                        args[i] = socket.ReadUInt();
                        break;
                    case WaylandType.NewId:
                        args[i] = socket.ReadUInt();
                        break;
                    case WaylandType.String:
                        args[i] = socket.ReadString();
                        break;
                    case WaylandType.Array:
                        args[i] = socket.ReadBytes();
                        break;
                    case WaylandType.Handle:
                        args[i] = socket.ReadFd();
                        break;
                }
                i++;
            }

            events.Enqueue(() => this[header.id].Event(header.opCode, args));

            return true;
            //mutex.WaitOne();
        }

       

        public void Disconnect()
        {
            socket.Dispose();
            freeIds.Clear();
        }
    }
}
