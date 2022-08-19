using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlEglstream : WaylandObject
    {
        public const string INTERFACE = "wl_eglstream";
        public WlEglstream(uint factoryId, ref uint id, WaylandConnection connection, uint version = 1) : base(factoryId, ref id, version, connection)
        {
        }

        public enum RequestOpcode : ushort
        {
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

        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///Bad allocation error
            ///</Summary>
            BadAlloc = 0,
            ///<Summary>
            ///Bad handle error
            ///</Summary>
            BadHandle = 1,
            ///<Summary>
            ///Bad attributes error
            ///</Summary>
            BadAttribs = 2,
            ///<Summary>
            ///Bad IP address error
            ///</Summary>
            BadAddress = 3,
        }

        ///<Summary>
        ///Stream handle type
        ///<para>
        ///- fd:     The given handle represents a file descriptor, and the
        ///EGLStream connection must be done as described in
        ///EGL_KHR_stream_cross_process_fd
        ///</para>
        ///<para>
        ///- inet:   The EGLStream connection must be done using an inet address
        ///and port as described in EGL_NV_stream_socket. The given
        ///handle can be ignored, but both inet address and port must
        ///be given as attributes.
        ///</para>
        ///<para>
        ///- socket: The given handle represents a unix socket, and the EGLStream
        ///connection must be done as described in EGL_NV_stream_socket.
        ///</para>
        ///</Summary>
        public enum HandleTypeFlag : uint
        {
            ///<Summary>
            ///File descriptor
            ///</Summary>
            Fd = 0,
            ///<Summary>
            ///Inet connection
            ///</Summary>
            Inet = 1,
            ///<Summary>
            ///Unix socket
            ///</Summary>
            Socket = 2,
        }

        ///<Summary>
        ///Stream creation attributes
        ///<para>
        ///- inet_addr:  The given attribute encodes an IPv4 address of a client
        ///socket. Both IPv4 address and port must be set at the same
        ///time.
        ///</para>
        ///<para>
        ///- inet_port:  The given attribute encodes a port of a client socket.
        ///Both IPv4 address and port must be set at the same time.
        ///</para>
        ///<para>
        ///- y_inverted: The given attribute encodes the default value for a
        ///stream's image inversion relative to wayland protocol
        ///convention. Vulkan apps will be set to 'true', while
        ///OpenGL apps will be set to 'false'.
        ///NOTE: EGL_NV_stream_origin is the authorative source of
        ///truth regarding a stream's frame orientation and should be
        ///queried for an accurate value. The given attribute is a
        ///'best guess' fallback mechanism which should only be used
        ///when a query to EGL_NV_stream_origin fails.
        ///</para>
        ///</Summary>
        public enum AttribFlag : uint
        {
            ///<Summary>
            ///Inet IPv4 address
            ///</Summary>
            InetAddr = 0,
            ///<Summary>
            ///IP port
            ///</Summary>
            InetPort = 1,
            ///<Summary>
            ///Image Y-inversion bit
            ///</Summary>
            YInverted = 2,
        }
    }
}
