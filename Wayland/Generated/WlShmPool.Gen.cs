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
        public WlShmPool(uint id, WaylandConnection connection, uint version = 1) : base(id, version, connection)
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
            WlBuffer wObject = connection.Create<WlBuffer>(0, this.version);
            uint id = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateBuffer, id, offset, width, height, stride, (uint)format);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "CreateBuffer", id, offset, width, height, stride, (uint)format);
            return wObject;
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
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "Destroy");
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
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "Resize", size);
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

        public override void Event(ushort opCode, WlType[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }

        public override WaylandType[] WaylandTypes(ushort opCode)
        {
            switch ((EventOpcode)opCode)
            {
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }
    }
}
