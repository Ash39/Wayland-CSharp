using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlEglstreamController : WaylandObject
    {
        public const string INTERFACE = "wl_eglstream_controller";
        public WlEglstreamController(uint factoryId, ref uint id, WaylandConnection connection, uint version = 2) : base(factoryId, ref id, version, connection)
        {
        }

        ///<Summary>
        ///Create server stream and attach consumer
        ///<para>
        ///Creates the corresponding server side EGLStream from the given wl_buffer
        ///and attaches a consumer to it.
        ///</para>
        ///</Summary>
        ///<param name = "wl_surface"> wl_surface corresponds to the client surface associated with         newly created eglstream </param>
        ///<param name = "wl_resource"> wl_resource corresponding to an EGLStream </param>
        public void AttachEglstreamConsumer(WlSurface wl_surface, WlBuffer wl_resource)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.AttachEglstreamConsumer, wl_surface.id, wl_resource.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.AttachEglstreamConsumer}({wl_surface.id},{wl_resource.id})");
        }

        ///<Summary>
        ///Create server stream and attach consumer using attributes
        ///<para>
        ///Creates the corresponding server side EGLStream from the given wl_buffer
        ///and attaches a consumer to it using the given attributes.
        ///</para>
        ///</Summary>
        ///<param name = "wl_surface"> wl_surface corresponds to the client surface associated with         newly created eglstream </param>
        ///<param name = "wl_resource"> wl_resource corresponding to an EGLStream </param>
        ///<param name = "attribs"> Stream consumer attachment attribs </param>
        public void AttachEglstreamConsumerAttribs(WlSurface wl_surface, WlBuffer wl_resource, byte[] attribs)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.AttachEglstreamConsumerAttribs, wl_surface.id, wl_resource.id, attribs);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.AttachEglstreamConsumerAttribs}({wl_surface.id},{wl_resource.id},{attribs})");
        }

        public enum RequestOpcode : ushort
        {
            AttachEglstreamConsumer,
            AttachEglstreamConsumerAttribs
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

        ///<Summary>
        ///Stream present mode
        ///<para>
        ///- dont_care: Using this enum will tell the server to make its own
        ///decisions regarding present mode.
        ///</para>
        ///<para>
        ///- fifo:      Tells the server to use a fifo present mode. The decision to
        ///use fifo synchronous is left up to the server.
        ///</para>
        ///<para>
        ///- mailbox:   Tells the server to use a mailbox present mode.
        ///</para>
        ///</Summary>
        public enum PresentModeFlag : uint
        {
            ///<Summary>
            ///Let the Server decide present mode
            ///</Summary>
            DontCare = 0,
            ///<Summary>
            ///Use a fifo present mode
            ///</Summary>
            Fifo = 1,
            ///<Summary>
            ///Use a mailbox mode
            ///</Summary>
            Mailbox = 2,
        }

        ///<Summary>
        ///Stream consumer attachment attributes
        ///<para>
        ///- present_mode: Must be one of wl_eglstream_controller_present_mode. Tells the
        ///server the desired present mode that should be used.
        ///</para>
        ///<para>
        ///- fifo_length:  Only valid when the present_mode attrib is provided and its
        ///value is specified as fifo. Tells the server the desired fifo
        ///length to be used when the desired present_mode is fifo.
        ///</para>
        ///</Summary>
        public enum AttribFlag : uint
        {
            ///<Summary>
            ///Tells the server the desired present mode
            ///</Summary>
            PresentMode = 0,
            ///<Summary>
            ///Tells the server the desired fifo length when the desired presenation_mode is fifo.
            ///</Summary>
            FifoLength = 1,
        }
    }
}
