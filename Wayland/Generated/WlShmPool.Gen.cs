using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///a shared memory pool
    ///<para>
    ///The wl_shm_pool object encapsulates a piece of memory shared
    ///between the compositor and client.  Through the wl_shm_pool
    ///object, the client can allocate shared memory wl_buffer objects.
    ///All objects created through the same pool share the same
    ///underlying mapped memory. Reusing the mapped memory avoids the
    ///setup/teardown overhead and is useful when interactively resizing
    ///a surface or for many small buffers.
    ///</para>
    ///</Summary>
    public partial class WlShmPool : WaylandObject
    {
        public const string INTERFACE = "wl_shm_pool";
        public WlShmPool(uint factoryId, ref uint id, WaylandConnection connection) : base(factoryId, ref id, 1, connection)
        {
        }

        ///<Summary>
        ///create a buffer from the pool
        ///<para>
        ///Create a wl_buffer object from the pool.
        ///</para>
        ///<para>
        ///The buffer is created offset bytes into the pool and has
        ///width and height as specified.  The stride argument specifies
        ///the number of bytes from the beginning of one row to the beginning
        ///of the next.  The format is the pixel format of the buffer and
        ///must be one of those advertised through the wl_shm.format event.
        ///</para>
        ///<para>
        ///A buffer will keep a reference to the pool it was created from
        ///so it is valid to destroy the pool immediately after creating
        ///a buffer from it.
        ///</para>
        ///</Summary>
        ///<returns> buffer to create </returns>
        ///<param name = "offset"> buffer byte offset within the pool </param>
        ///<param name = "width"> buffer width, in pixels </param>
        ///<param name = "height"> buffer height, in pixels </param>
        ///<param name = "stride"> number of bytes from the beginning of one row to the beginning of the next row </param>
        ///<param name = "format"> buffer pixel format </param>
        public WlBuffer CreateBuffer(int offset, int width, int height, int stride, WlShm.FormatFlag format)
        {
            uint id = connection.Create();
            WlBuffer wObject = new WlBuffer(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateBuffer, id, offset, width, height, stride, (uint)format);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreateBuffer}({id},{offset},{width},{height},{stride},{(uint)format})");
            connection[id] = wObject;
            return (WlBuffer)connection[id];
        }

        ///<Summary>
        ///destroy the pool
        ///<para>
        ///Destroy the shared memory pool.
        ///</para>
        ///<para>
        ///The mmapped memory will be released when all
        ///buffers that have been created from this pool
        ///are gone.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        ///<Summary>
        ///change the size of the pool mapping
        ///<para>
        ///This request will cause the server to remap the backing memory
        ///for the pool from the file descriptor passed when the pool was
        ///created, but using the new size.  This request can only be
        ///used to make the pool bigger.
        ///</para>
        ///<para>
        ///This request only changes the amount of bytes that are mmapped
        ///by the server and does not touch the file corresponding to the
        ///file descriptor passed at creation time. It is the client's
        ///responsibility to ensure that the file is at least as big as
        ///the new pool size.
        ///</para>
        ///</Summary>
        ///<param name = "size"> new size of the pool, in bytes </param>
        public void Resize(int size)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Resize, size);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Resize}({size})");
        }

        public enum RequestOpcode : ushort
        {
            CreateBuffer,
            Destroy,
            Resize
        }

        public enum EventOpcode : ushort
        {
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        public override WaylandType[] WaylandTypes(ushort opCode)
        {
            switch ((EventOpcode)opCode)
            {
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
