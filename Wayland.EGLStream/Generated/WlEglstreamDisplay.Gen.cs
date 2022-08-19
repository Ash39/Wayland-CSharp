using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlEglstreamDisplay : WaylandObject
    {
        public const string INTERFACE = "wl_eglstream_display";
        public WlEglstreamDisplay(uint factoryId, ref uint id, WaylandConnection connection, uint version = 1) : base(factoryId, ref id, version, connection)
        {
        }

        ///<Summary>
        ///Create a wl_buffer from the given handle
        ///<para>
        ///Create a wl_buffer corresponding to given handle. The attributes list
        ///may be used to define additional EGLStream connection data (e.g inet
        ///address/port). The server can create its EGLStream handle using the
        ///information encoded in the wl_buffer.
        ///</para>
        ///</Summary>
        ///<returns> New ID </returns>
        ///<param name = "width"> Stream framebuffer width </param>
        ///<param name = "height"> Stream framebuffer height </param>
        ///<param name = "handle"> Handle for the stream creation </param>
        ///<param name = "type"> Handle type </param>
        ///<param name = "attribs"> Stream extra connection attribs </param>
        public WlBuffer CreateStream(int width, int height, IntPtr handle, int type, byte[] attribs)
        {
            uint id = connection.Create();
            WlBuffer wObject = new WlBuffer(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateStream, id, width, height, handle, type, attribs);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreateStream}({id},{width},{height},{handle},{type},{attribs})");
            connection[id] = wObject;
            return (WlBuffer)connection[id];
        }

        ///<Summary>
        ///change the swap interval of an EGLStream consumer
        ///<para>
        ///Set the swap interval for the consumer of the given EGLStream. The swap
        ///interval is silently clamped to the valid range on the server side.
        ///</para>
        ///</Summary>
        ///<param name = "stream"> wl_buffer corresponding to an EGLStream </param>
        ///<param name = "interval"> new swap interval </param>
        public void SwapInterval(WlBuffer stream, int interval)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SwapInterval, stream.id, interval);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SwapInterval}({stream.id},{interval})");
        }

        public enum RequestOpcode : ushort
        {
            CreateStream,
            SwapInterval
        }

        ///<Summary>
        ///Server capabilities event
        ///<para>
        ///The capabilities event is sent out at wl_eglstream_display binding
        ///time. It allows the server to advertise what features it supports so
        ///clients may know what is safe to be used.
        ///</para>
        ///</Summary>
        public Action<WlEglstreamDisplay, int> caps;
        ///<Summary>
        ///Server Swap interval override event
        ///<para>
        ///The swapinterval_override event is sent out whenever a client requests
        ///a swapinterval setting through swap_interval() and there is an override
        ///in place that will make such request to be ignored.
        ///The swapinterval_override event will provide the override value so
        ///that the client is made aware of it.
        ///</para>
        ///</Summary>
        public Action<WlEglstreamDisplay, int, WaylandObject> swapintervalOverride;
        public enum EventOpcode : ushort
        {
            Caps,
            SwapintervalOverride
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Caps:
                {
                    var caps = (int)arguments[0];
                    if (this.caps != null)
                    {
                        this.caps.Invoke(this, caps);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Caps}({this},{caps})");
                    }

                    break;
                }

                case EventOpcode.SwapintervalOverride:
                {
                    var swapinterval = (int)arguments[0];
                    var stream = connection[(uint)arguments[1]];
                    if (this.swapintervalOverride != null)
                    {
                        this.swapintervalOverride.Invoke(this, swapinterval, stream);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.SwapintervalOverride}({this},{swapinterval},{stream})");
                    }

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        public override WaylandType[] WaylandTypes(ushort opCode)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Caps:
                    return new WaylandType[]{WaylandType.Int, };
                case EventOpcode.SwapintervalOverride:
                    return new WaylandType[]{WaylandType.Int, WaylandType.Object, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        ///<Summary>
        ///wl_eglstream_display capability codes
        ///<para>
        ///This enum values should be used as bit masks.
        ///</para>
        ///<para>
        ///- stream_fd:     The server supports EGLStream connections as described
        ///in EGL_KHR_stream_cross_process_fd
        ///</para>
        ///<para>
        ///- stream_inet:   The server supports EGLStream inet connections as
        ///described in EGL_NV_stream_socket.
        ///</para>
        ///<para>
        ///- stream_socket: The server supports EGLStream unix socket connections
        ///as described in EGL_NV_stream_socket.
        ///</para>
        ///</Summary>
        public enum CapFlag : uint
        {
            ///<Summary>
            ///Stream connection with FD
            ///</Summary>
            StreamFd = 1,
            ///<Summary>
            ///Stream inet connection
            ///</Summary>
            StreamInet = 2,
            ///<Summary>
            ///Stream unix connection
            ///</Summary>
            StreamSocket = 4,
        }
    }
}
