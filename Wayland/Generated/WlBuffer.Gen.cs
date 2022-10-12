using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///content for a wl_surface
    ///<para>
    ///A buffer provides the content for a wl_surface. Buffers are
    ///created through factory interfaces such as wl_shm, wp_linux_buffer_params
    ///(from the linux-dmabuf protocol extension) or similar. It has a width and
    ///a height and can be attached to a wl_surface, but the mechanism by which a
    ///client provides and updates the contents is defined by the buffer factory
    ///interface.
    ///</para>
    ///<para>
    ///If the buffer uses a format that has an alpha channel, the alpha channel
    ///is assumed to be premultiplied in the color channels unless otherwise
    ///specified.
    ///</para>
    ///</Summary>
    public partial class WlBuffer : WaylandObject
    {
        public const string INTERFACE = "wl_buffer";
        public WlBuffer(uint id, WaylandConnection connection, uint version = 1) : base(id, version, connection)
        {
        }

        ///<Summary>
        ///destroy a buffer
        ///<para>
        ///Destroy a buffer. If and how you need to release the backing
        ///storage is defined by the buffer factory interface.
        ///</para>
        ///<para>
        ///For possible side-effects to a surface, see wl_surface.attach.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "Destroy");
        }

        public enum RequestOpcode : ushort
        {
            Destroy
        }

        ///<Summary>
        ///compositor releases buffer
        ///<para>
        ///Sent when this wl_buffer is no longer used by the compositor.
        ///The client is now free to reuse or destroy this buffer and its
        ///backing storage.
        ///</para>
        ///<para>
        ///If a client receives a release event before the frame callback
        ///requested in the same wl_surface.commit that attaches this
        ///wl_buffer to a surface, then the client is immediately free to
        ///reuse the buffer and its backing storage, and does not need a
        ///second buffer for the next surface content update. Typically
        ///this is possible, when the compositor maintains a copy of the
        ///wl_surface contents, e.g. as a GL texture. This is an important
        ///optimization for GL(ES) compositors with wl_shm clients.
        ///</para>
        ///</Summary>
        public Action<WlBuffer> release;
        public enum EventOpcode : ushort
        {
            Release
        }

        public override void Event(ushort opCode, WlType[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Release:
                {
                    if (this.release != null)
                    {
                        this.release.Invoke(this);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Release");
                    }

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }

        public override WaylandType[] WaylandTypes(ushort opCode)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Release:
                    return new WaylandType[]{};
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }
    }
}
