using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlDrm : WaylandObject
    {
        public const string INTERFACE = "wl_drm";
        public WlDrm(uint factoryId, ref uint id, WaylandConnection connection, uint version = 2) : base(factoryId, ref id, version, connection)
        {
        }

        ///<param name = "id">  </param>
        public void Authenticate(uint id)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Authenticate, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Authenticate}({id})");
        }

        ///<returns>  </returns>
        ///<param name = "name">  </param>
        ///<param name = "width">  </param>
        ///<param name = "height">  </param>
        ///<param name = "stride">  </param>
        ///<param name = "format">  </param>
        public WlBuffer CreateBuffer(uint name, int width, int height, uint stride, uint format)
        {
            uint id = connection.Create();
            WlBuffer wObject = new WlBuffer(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateBuffer, id, name, width, height, stride, format);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreateBuffer}({id},{name},{width},{height},{stride},{format})");
            connection[id] = wObject;
            return (WlBuffer)connection[id];
        }

        ///<returns>  </returns>
        ///<param name = "name">  </param>
        ///<param name = "width">  </param>
        ///<param name = "height">  </param>
        ///<param name = "format">  </param>
        ///<param name = "offset0">  </param>
        ///<param name = "stride0">  </param>
        ///<param name = "offset1">  </param>
        ///<param name = "stride1">  </param>
        ///<param name = "offset2">  </param>
        ///<param name = "stride2">  </param>
        public WlBuffer CreatePlanarBuffer(uint name, int width, int height, uint format, int offset0, int stride0, int offset1, int stride1, int offset2, int stride2)
        {
            uint id = connection.Create();
            WlBuffer wObject = new WlBuffer(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.CreatePlanarBuffer, id, name, width, height, format, offset0, stride0, offset1, stride1, offset2, stride2);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreatePlanarBuffer}({id},{name},{width},{height},{format},{offset0},{stride0},{offset1},{stride1},{offset2},{stride2})");
            connection[id] = wObject;
            return (WlBuffer)connection[id];
        }

        ///<returns>  </returns>
        ///<param name = "name">  </param>
        ///<param name = "width">  </param>
        ///<param name = "height">  </param>
        ///<param name = "format">  </param>
        ///<param name = "offset0">  </param>
        ///<param name = "stride0">  </param>
        ///<param name = "offset1">  </param>
        ///<param name = "stride1">  </param>
        ///<param name = "offset2">  </param>
        ///<param name = "stride2">  </param>
        public WlBuffer CreatePrimeBuffer(IntPtr name, int width, int height, uint format, int offset0, int stride0, int offset1, int stride1, int offset2, int stride2)
        {
            uint id = connection.Create();
            WlBuffer wObject = new WlBuffer(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.CreatePrimeBuffer, id, name, width, height, format, offset0, stride0, offset1, stride1, offset2, stride2);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreatePrimeBuffer}({id},{name},{width},{height},{format},{offset0},{stride0},{offset1},{stride1},{offset2},{stride2})");
            connection[id] = wObject;
            return (WlBuffer)connection[id];
        }

        public enum RequestOpcode : ushort
        {
            Authenticate,
            CreateBuffer,
            CreatePlanarBuffer,
            CreatePrimeBuffer
        }

        public Action<WlDrm, string> device;
        public Action<WlDrm, uint> format;
        public Action<WlDrm> authenticated;
        public Action<WlDrm, uint> capabilities;
        public enum EventOpcode : ushort
        {
            Device,
            Format,
            Authenticated,
            Capabilities
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Device:
                {
                    var name = (string)arguments[0];
                    if (this.device != null)
                    {
                        this.device.Invoke(this, name);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Device}({this},{name})");
                    }

                    break;
                }

                case EventOpcode.Format:
                {
                    var format = (uint)arguments[0];
                    if (this.format != null)
                    {
                        this.format.Invoke(this, format);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Format}({this},{format})");
                    }

                    break;
                }

                case EventOpcode.Authenticated:
                {
                    if (this.authenticated != null)
                    {
                        this.authenticated.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Authenticated}({this})");
                    }

                    break;
                }

                case EventOpcode.Capabilities:
                {
                    var value = (uint)arguments[0];
                    if (this.capabilities != null)
                    {
                        this.capabilities.Invoke(this, value);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Capabilities}({this},{value})");
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
                case EventOpcode.Device:
                    return new WaylandType[]{WaylandType.String, };
                case EventOpcode.Format:
                    return new WaylandType[]{WaylandType.Uint, };
                case EventOpcode.Authenticated:
                    return new WaylandType[]{};
                case EventOpcode.Capabilities:
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///
            ///</Summary>
            AuthenticateFail = 0,
            ///<Summary>
            ///
            ///</Summary>
            InvalidFormat = 1,
            ///<Summary>
            ///
            ///</Summary>
            InvalidName = 2,
        }

        public enum FormatFlag : uint
        {
            ///<Summary>
            ///
            ///</Summary>
            C8 = 0x20203843,
            ///<Summary>
            ///
            ///</Summary>
            Rgb332 = 0x38424752,
            ///<Summary>
            ///
            ///</Summary>
            Bgr233 = 0x38524742,
            ///<Summary>
            ///
            ///</Summary>
            Xrgb4444 = 0x32315258,
            ///<Summary>
            ///
            ///</Summary>
            Xbgr4444 = 0x32314258,
            ///<Summary>
            ///
            ///</Summary>
            Rgbx4444 = 0x32315852,
            ///<Summary>
            ///
            ///</Summary>
            Bgrx4444 = 0x32315842,
            ///<Summary>
            ///
            ///</Summary>
            Argb4444 = 0x32315241,
            ///<Summary>
            ///
            ///</Summary>
            Abgr4444 = 0x32314241,
            ///<Summary>
            ///
            ///</Summary>
            Rgba4444 = 0x32314152,
            ///<Summary>
            ///
            ///</Summary>
            Bgra4444 = 0x32314142,
            ///<Summary>
            ///
            ///</Summary>
            Xrgb1555 = 0x35315258,
            ///<Summary>
            ///
            ///</Summary>
            Xbgr1555 = 0x35314258,
            ///<Summary>
            ///
            ///</Summary>
            Rgbx5551 = 0x35315852,
            ///<Summary>
            ///
            ///</Summary>
            Bgrx5551 = 0x35315842,
            ///<Summary>
            ///
            ///</Summary>
            Argb1555 = 0x35315241,
            ///<Summary>
            ///
            ///</Summary>
            Abgr1555 = 0x35314241,
            ///<Summary>
            ///
            ///</Summary>
            Rgba5551 = 0x35314152,
            ///<Summary>
            ///
            ///</Summary>
            Bgra5551 = 0x35314142,
            ///<Summary>
            ///
            ///</Summary>
            Rgb565 = 0x36314752,
            ///<Summary>
            ///
            ///</Summary>
            Bgr565 = 0x36314742,
            ///<Summary>
            ///
            ///</Summary>
            Rgb888 = 0x34324752,
            ///<Summary>
            ///
            ///</Summary>
            Bgr888 = 0x34324742,
            ///<Summary>
            ///
            ///</Summary>
            Xrgb8888 = 0x34325258,
            ///<Summary>
            ///
            ///</Summary>
            Xbgr8888 = 0x34324258,
            ///<Summary>
            ///
            ///</Summary>
            Rgbx8888 = 0x34325852,
            ///<Summary>
            ///
            ///</Summary>
            Bgrx8888 = 0x34325842,
            ///<Summary>
            ///
            ///</Summary>
            Argb8888 = 0x34325241,
            ///<Summary>
            ///
            ///</Summary>
            Abgr8888 = 0x34324241,
            ///<Summary>
            ///
            ///</Summary>
            Rgba8888 = 0x34324152,
            ///<Summary>
            ///
            ///</Summary>
            Bgra8888 = 0x34324142,
            ///<Summary>
            ///
            ///</Summary>
            Xrgb2101010 = 0x30335258,
            ///<Summary>
            ///
            ///</Summary>
            Xbgr2101010 = 0x30334258,
            ///<Summary>
            ///
            ///</Summary>
            Rgbx1010102 = 0x30335852,
            ///<Summary>
            ///
            ///</Summary>
            Bgrx1010102 = 0x30335842,
            ///<Summary>
            ///
            ///</Summary>
            Argb2101010 = 0x30335241,
            ///<Summary>
            ///
            ///</Summary>
            Abgr2101010 = 0x30334241,
            ///<Summary>
            ///
            ///</Summary>
            Rgba1010102 = 0x30334152,
            ///<Summary>
            ///
            ///</Summary>
            Bgra1010102 = 0x30334142,
            ///<Summary>
            ///
            ///</Summary>
            Yuyv = 0x56595559,
            ///<Summary>
            ///
            ///</Summary>
            Yvyu = 0x55595659,
            ///<Summary>
            ///
            ///</Summary>
            Uyvy = 0x59565955,
            ///<Summary>
            ///
            ///</Summary>
            Vyuy = 0x59555956,
            ///<Summary>
            ///
            ///</Summary>
            Ayuv = 0x56555941,
            ///<Summary>
            ///
            ///</Summary>
            Xyuv8888 = 0x56555958,
            ///<Summary>
            ///
            ///</Summary>
            Nv12 = 0x3231564e,
            ///<Summary>
            ///
            ///</Summary>
            Nv21 = 0x3132564e,
            ///<Summary>
            ///
            ///</Summary>
            Nv16 = 0x3631564e,
            ///<Summary>
            ///
            ///</Summary>
            Nv61 = 0x3136564e,
            ///<Summary>
            ///
            ///</Summary>
            Yuv410 = 0x39565559,
            ///<Summary>
            ///
            ///</Summary>
            Yvu410 = 0x39555659,
            ///<Summary>
            ///
            ///</Summary>
            Yuv411 = 0x31315559,
            ///<Summary>
            ///
            ///</Summary>
            Yvu411 = 0x31315659,
            ///<Summary>
            ///
            ///</Summary>
            Yuv420 = 0x32315559,
            ///<Summary>
            ///
            ///</Summary>
            Yvu420 = 0x32315659,
            ///<Summary>
            ///
            ///</Summary>
            Yuv422 = 0x36315559,
            ///<Summary>
            ///
            ///</Summary>
            Yvu422 = 0x36315659,
            ///<Summary>
            ///
            ///</Summary>
            Yuv444 = 0x34325559,
            ///<Summary>
            ///
            ///</Summary>
            Yvu444 = 0x34325659,
            ///<Summary>
            ///
            ///</Summary>
            Abgr16f = 0x48344241,
            ///<Summary>
            ///
            ///</Summary>
            Xbgr16f = 0x48344258,
        }

        ///<Summary>
        ///wl_drm capability bitmask
        ///<para>
        ///Bitmask of capabilities.
        ///</para>
        ///</Summary>
        public enum CapabilityFlag : uint
        {
            ///<Summary>
            ///wl_drm prime available
            ///</Summary>
            Prime = 1,
        }
    }
}
