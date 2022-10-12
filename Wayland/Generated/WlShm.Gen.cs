using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///shared memory support
    ///<para>
    ///A singleton global object that provides support for shared
    ///memory.
    ///</para>
    ///<para>
    ///Clients can create wl_shm_pool objects using the create_pool
    ///request.
    ///</para>
    ///<para>
    ///On binding the wl_shm object one or more format events
    ///are emitted to inform clients about the valid pixel formats
    ///that can be used for buffers.
    ///</para>
    ///</Summary>
    public partial class WlShm : WaylandObject
    {
        public const string INTERFACE = "wl_shm";
        public WlShm(uint id, WaylandConnection connection, uint version = 1) : base(id, version, connection)
        {
        }

        ///<Summary>
        ///create a shm pool
        ///<para>
        ///Create a new wl_shm_pool object.
        ///</para>
        ///<para>
        ///The pool can be used to create shared memory based buffer
        ///objects.  The server will mmap size bytes of the passed file
        ///descriptor, to use as backing memory for the pool.
        ///</para>
        ///</Summary>
        ///<returns> pool to create </returns>
        ///<param name = "fd"> file descriptor for the pool </param>
        ///<param name = "size"> pool size, in bytes </param>
        public WlShmPool CreatePool(IntPtr fd, int size)
        {
            WlShmPool wObject = connection.Create<WlShmPool>(0, this.version);
            uint id = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.CreatePool, id, fd, size);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "CreatePool", id, fd, size);
            return wObject;
        }

        public enum RequestOpcode : ushort
        {
            CreatePool
        }

        ///<Summary>
        ///pixel format description
        ///<para>
        ///Informs the client about a valid pixel format that
        ///can be used for buffers. Known formats include
        ///argb8888 and xrgb8888.
        ///</para>
        ///</Summary>
        public Action<WlShm, FormatFlag> format;
        public enum EventOpcode : ushort
        {
            Format
        }

        public override void Event(ushort opCode, WlType[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Format:
                {
                    var format = (FormatFlag)arguments[0].u;
                    if (this.format != null)
                    {
                        this.format.Invoke(this, format);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Format");
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
                case EventOpcode.Format:
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }

        ///<Summary>
        ///wl_shm error values
        ///<para>
        ///These errors can be emitted in response to wl_shm requests.
        ///</para>
        ///</Summary>
        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///buffer format is not known
            ///</Summary>
            InvalidFormat = 0,
            ///<Summary>
            ///invalid size or stride during pool or buffer creation
            ///</Summary>
            InvalidStride = 1,
            ///<Summary>
            ///mmapping the file descriptor failed
            ///</Summary>
            InvalidFd = 2,
        }

        ///<Summary>
        ///pixel formats
        ///<para>
        ///This describes the memory layout of an individual pixel.
        ///</para>
        ///<para>
        ///All renderers should support argb8888 and xrgb8888 but any other
        ///formats are optional and may not be supported by the particular
        ///renderer in use.
        ///</para>
        ///<para>
        ///The drm format codes match the macros defined in drm_fourcc.h, except
        ///argb8888 and xrgb8888. The formats actually supported by the compositor
        ///will be reported by the format event.
        ///</para>
        ///<para>
        ///For all wl_shm formats and unless specified in another protocol
        ///extension, pre-multiplied alpha is used for pixel values.
        ///</para>
        ///</Summary>
        public enum FormatFlag : uint
        {
            ///<Summary>
            ///32-bit ARGB format, [31:0] A:R:G:B 8:8:8:8 little endian
            ///</Summary>
            Argb8888 = 0,
            ///<Summary>
            ///32-bit RGB format, [31:0] x:R:G:B 8:8:8:8 little endian
            ///</Summary>
            Xrgb8888 = 1,
            ///<Summary>
            ///8-bit color index format, [7:0] C
            ///</Summary>
            C8 = 0x20203843,
            ///<Summary>
            ///8-bit RGB format, [7:0] R:G:B 3:3:2
            ///</Summary>
            Rgb332 = 0x38424752,
            ///<Summary>
            ///8-bit BGR format, [7:0] B:G:R 2:3:3
            ///</Summary>
            Bgr233 = 0x38524742,
            ///<Summary>
            ///16-bit xRGB format, [15:0] x:R:G:B 4:4:4:4 little endian
            ///</Summary>
            Xrgb4444 = 0x32315258,
            ///<Summary>
            ///16-bit xBGR format, [15:0] x:B:G:R 4:4:4:4 little endian
            ///</Summary>
            Xbgr4444 = 0x32314258,
            ///<Summary>
            ///16-bit RGBx format, [15:0] R:G:B:x 4:4:4:4 little endian
            ///</Summary>
            Rgbx4444 = 0x32315852,
            ///<Summary>
            ///16-bit BGRx format, [15:0] B:G:R:x 4:4:4:4 little endian
            ///</Summary>
            Bgrx4444 = 0x32315842,
            ///<Summary>
            ///16-bit ARGB format, [15:0] A:R:G:B 4:4:4:4 little endian
            ///</Summary>
            Argb4444 = 0x32315241,
            ///<Summary>
            ///16-bit ABGR format, [15:0] A:B:G:R 4:4:4:4 little endian
            ///</Summary>
            Abgr4444 = 0x32314241,
            ///<Summary>
            ///16-bit RBGA format, [15:0] R:G:B:A 4:4:4:4 little endian
            ///</Summary>
            Rgba4444 = 0x32314152,
            ///<Summary>
            ///16-bit BGRA format, [15:0] B:G:R:A 4:4:4:4 little endian
            ///</Summary>
            Bgra4444 = 0x32314142,
            ///<Summary>
            ///16-bit xRGB format, [15:0] x:R:G:B 1:5:5:5 little endian
            ///</Summary>
            Xrgb1555 = 0x35315258,
            ///<Summary>
            ///16-bit xBGR 1555 format, [15:0] x:B:G:R 1:5:5:5 little endian
            ///</Summary>
            Xbgr1555 = 0x35314258,
            ///<Summary>
            ///16-bit RGBx 5551 format, [15:0] R:G:B:x 5:5:5:1 little endian
            ///</Summary>
            Rgbx5551 = 0x35315852,
            ///<Summary>
            ///16-bit BGRx 5551 format, [15:0] B:G:R:x 5:5:5:1 little endian
            ///</Summary>
            Bgrx5551 = 0x35315842,
            ///<Summary>
            ///16-bit ARGB 1555 format, [15:0] A:R:G:B 1:5:5:5 little endian
            ///</Summary>
            Argb1555 = 0x35315241,
            ///<Summary>
            ///16-bit ABGR 1555 format, [15:0] A:B:G:R 1:5:5:5 little endian
            ///</Summary>
            Abgr1555 = 0x35314241,
            ///<Summary>
            ///16-bit RGBA 5551 format, [15:0] R:G:B:A 5:5:5:1 little endian
            ///</Summary>
            Rgba5551 = 0x35314152,
            ///<Summary>
            ///16-bit BGRA 5551 format, [15:0] B:G:R:A 5:5:5:1 little endian
            ///</Summary>
            Bgra5551 = 0x35314142,
            ///<Summary>
            ///16-bit RGB 565 format, [15:0] R:G:B 5:6:5 little endian
            ///</Summary>
            Rgb565 = 0x36314752,
            ///<Summary>
            ///16-bit BGR 565 format, [15:0] B:G:R 5:6:5 little endian
            ///</Summary>
            Bgr565 = 0x36314742,
            ///<Summary>
            ///24-bit RGB format, [23:0] R:G:B little endian
            ///</Summary>
            Rgb888 = 0x34324752,
            ///<Summary>
            ///24-bit BGR format, [23:0] B:G:R little endian
            ///</Summary>
            Bgr888 = 0x34324742,
            ///<Summary>
            ///32-bit xBGR format, [31:0] x:B:G:R 8:8:8:8 little endian
            ///</Summary>
            Xbgr8888 = 0x34324258,
            ///<Summary>
            ///32-bit RGBx format, [31:0] R:G:B:x 8:8:8:8 little endian
            ///</Summary>
            Rgbx8888 = 0x34325852,
            ///<Summary>
            ///32-bit BGRx format, [31:0] B:G:R:x 8:8:8:8 little endian
            ///</Summary>
            Bgrx8888 = 0x34325842,
            ///<Summary>
            ///32-bit ABGR format, [31:0] A:B:G:R 8:8:8:8 little endian
            ///</Summary>
            Abgr8888 = 0x34324241,
            ///<Summary>
            ///32-bit RGBA format, [31:0] R:G:B:A 8:8:8:8 little endian
            ///</Summary>
            Rgba8888 = 0x34324152,
            ///<Summary>
            ///32-bit BGRA format, [31:0] B:G:R:A 8:8:8:8 little endian
            ///</Summary>
            Bgra8888 = 0x34324142,
            ///<Summary>
            ///32-bit xRGB format, [31:0] x:R:G:B 2:10:10:10 little endian
            ///</Summary>
            Xrgb2101010 = 0x30335258,
            ///<Summary>
            ///32-bit xBGR format, [31:0] x:B:G:R 2:10:10:10 little endian
            ///</Summary>
            Xbgr2101010 = 0x30334258,
            ///<Summary>
            ///32-bit RGBx format, [31:0] R:G:B:x 10:10:10:2 little endian
            ///</Summary>
            Rgbx1010102 = 0x30335852,
            ///<Summary>
            ///32-bit BGRx format, [31:0] B:G:R:x 10:10:10:2 little endian
            ///</Summary>
            Bgrx1010102 = 0x30335842,
            ///<Summary>
            ///32-bit ARGB format, [31:0] A:R:G:B 2:10:10:10 little endian
            ///</Summary>
            Argb2101010 = 0x30335241,
            ///<Summary>
            ///32-bit ABGR format, [31:0] A:B:G:R 2:10:10:10 little endian
            ///</Summary>
            Abgr2101010 = 0x30334241,
            ///<Summary>
            ///32-bit RGBA format, [31:0] R:G:B:A 10:10:10:2 little endian
            ///</Summary>
            Rgba1010102 = 0x30334152,
            ///<Summary>
            ///32-bit BGRA format, [31:0] B:G:R:A 10:10:10:2 little endian
            ///</Summary>
            Bgra1010102 = 0x30334142,
            ///<Summary>
            ///packed YCbCr format, [31:0] Cr0:Y1:Cb0:Y0 8:8:8:8 little endian
            ///</Summary>
            Yuyv = 0x56595559,
            ///<Summary>
            ///packed YCbCr format, [31:0] Cb0:Y1:Cr0:Y0 8:8:8:8 little endian
            ///</Summary>
            Yvyu = 0x55595659,
            ///<Summary>
            ///packed YCbCr format, [31:0] Y1:Cr0:Y0:Cb0 8:8:8:8 little endian
            ///</Summary>
            Uyvy = 0x59565955,
            ///<Summary>
            ///packed YCbCr format, [31:0] Y1:Cb0:Y0:Cr0 8:8:8:8 little endian
            ///</Summary>
            Vyuy = 0x59555956,
            ///<Summary>
            ///packed AYCbCr format, [31:0] A:Y:Cb:Cr 8:8:8:8 little endian
            ///</Summary>
            Ayuv = 0x56555941,
            ///<Summary>
            ///2 plane YCbCr Cr:Cb format, 2x2 subsampled Cr:Cb plane
            ///</Summary>
            Nv12 = 0x3231564e,
            ///<Summary>
            ///2 plane YCbCr Cb:Cr format, 2x2 subsampled Cb:Cr plane
            ///</Summary>
            Nv21 = 0x3132564e,
            ///<Summary>
            ///2 plane YCbCr Cr:Cb format, 2x1 subsampled Cr:Cb plane
            ///</Summary>
            Nv16 = 0x3631564e,
            ///<Summary>
            ///2 plane YCbCr Cb:Cr format, 2x1 subsampled Cb:Cr plane
            ///</Summary>
            Nv61 = 0x3136564e,
            ///<Summary>
            ///3 plane YCbCr format, 4x4 subsampled Cb (1) and Cr (2) planes
            ///</Summary>
            Yuv410 = 0x39565559,
            ///<Summary>
            ///3 plane YCbCr format, 4x4 subsampled Cr (1) and Cb (2) planes
            ///</Summary>
            Yvu410 = 0x39555659,
            ///<Summary>
            ///3 plane YCbCr format, 4x1 subsampled Cb (1) and Cr (2) planes
            ///</Summary>
            Yuv411 = 0x31315559,
            ///<Summary>
            ///3 plane YCbCr format, 4x1 subsampled Cr (1) and Cb (2) planes
            ///</Summary>
            Yvu411 = 0x31315659,
            ///<Summary>
            ///3 plane YCbCr format, 2x2 subsampled Cb (1) and Cr (2) planes
            ///</Summary>
            Yuv420 = 0x32315559,
            ///<Summary>
            ///3 plane YCbCr format, 2x2 subsampled Cr (1) and Cb (2) planes
            ///</Summary>
            Yvu420 = 0x32315659,
            ///<Summary>
            ///3 plane YCbCr format, 2x1 subsampled Cb (1) and Cr (2) planes
            ///</Summary>
            Yuv422 = 0x36315559,
            ///<Summary>
            ///3 plane YCbCr format, 2x1 subsampled Cr (1) and Cb (2) planes
            ///</Summary>
            Yvu422 = 0x36315659,
            ///<Summary>
            ///3 plane YCbCr format, non-subsampled Cb (1) and Cr (2) planes
            ///</Summary>
            Yuv444 = 0x34325559,
            ///<Summary>
            ///3 plane YCbCr format, non-subsampled Cr (1) and Cb (2) planes
            ///</Summary>
            Yvu444 = 0x34325659,
            ///<Summary>
            ///[7:0] R
            ///</Summary>
            R8 = 0x20203852,
            ///<Summary>
            ///[15:0] R little endian
            ///</Summary>
            R16 = 0x20363152,
            ///<Summary>
            ///[15:0] R:G 8:8 little endian
            ///</Summary>
            Rg88 = 0x38384752,
            ///<Summary>
            ///[15:0] G:R 8:8 little endian
            ///</Summary>
            Gr88 = 0x38385247,
            ///<Summary>
            ///[31:0] R:G 16:16 little endian
            ///</Summary>
            Rg1616 = 0x32334752,
            ///<Summary>
            ///[31:0] G:R 16:16 little endian
            ///</Summary>
            Gr1616 = 0x32335247,
            ///<Summary>
            ///[63:0] x:R:G:B 16:16:16:16 little endian
            ///</Summary>
            Xrgb16161616f = 0x48345258,
            ///<Summary>
            ///[63:0] x:B:G:R 16:16:16:16 little endian
            ///</Summary>
            Xbgr16161616f = 0x48344258,
            ///<Summary>
            ///[63:0] A:R:G:B 16:16:16:16 little endian
            ///</Summary>
            Argb16161616f = 0x48345241,
            ///<Summary>
            ///[63:0] A:B:G:R 16:16:16:16 little endian
            ///</Summary>
            Abgr16161616f = 0x48344241,
            ///<Summary>
            ///[31:0] X:Y:Cb:Cr 8:8:8:8 little endian
            ///</Summary>
            Xyuv8888 = 0x56555958,
            ///<Summary>
            ///[23:0] Cr:Cb:Y 8:8:8 little endian
            ///</Summary>
            Vuy888 = 0x34325556,
            ///<Summary>
            ///Y followed by U then V, 10:10:10. Non-linear modifier only
            ///</Summary>
            Vuy101010 = 0x30335556,
            ///<Summary>
            ///[63:0] Cr0:0:Y1:0:Cb0:0:Y0:0 10:6:10:6:10:6:10:6 little endian per 2 Y pixels
            ///</Summary>
            Y210 = 0x30313259,
            ///<Summary>
            ///[63:0] Cr0:0:Y1:0:Cb0:0:Y0:0 12:4:12:4:12:4:12:4 little endian per 2 Y pixels
            ///</Summary>
            Y212 = 0x32313259,
            ///<Summary>
            ///[63:0] Cr0:Y1:Cb0:Y0 16:16:16:16 little endian per 2 Y pixels
            ///</Summary>
            Y216 = 0x36313259,
            ///<Summary>
            ///[31:0] A:Cr:Y:Cb 2:10:10:10 little endian
            ///</Summary>
            Y410 = 0x30313459,
            ///<Summary>
            ///[63:0] A:0:Cr:0:Y:0:Cb:0 12:4:12:4:12:4:12:4 little endian
            ///</Summary>
            Y412 = 0x32313459,
            ///<Summary>
            ///[63:0] A:Cr:Y:Cb 16:16:16:16 little endian
            ///</Summary>
            Y416 = 0x36313459,
            ///<Summary>
            ///[31:0] X:Cr:Y:Cb 2:10:10:10 little endian
            ///</Summary>
            Xvyu2101010 = 0x30335658,
            ///<Summary>
            ///[63:0] X:0:Cr:0:Y:0:Cb:0 12:4:12:4:12:4:12:4 little endian
            ///</Summary>
            Xvyu1216161616 = 0x36335658,
            ///<Summary>
            ///[63:0] X:Cr:Y:Cb 16:16:16:16 little endian
            ///</Summary>
            Xvyu16161616 = 0x38345658,
            ///<Summary>
            ///[63:0]   A3:A2:Y3:0:Cr0:0:Y2:0:A1:A0:Y1:0:Cb0:0:Y0:0  1:1:8:2:8:2:8:2:1:1:8:2:8:2:8:2 little endian
            ///</Summary>
            Y0l0 = 0x304c3059,
            ///<Summary>
            ///[63:0]   X3:X2:Y3:0:Cr0:0:Y2:0:X1:X0:Y1:0:Cb0:0:Y0:0  1:1:8:2:8:2:8:2:1:1:8:2:8:2:8:2 little endian
            ///</Summary>
            X0l0 = 0x304c3058,
            ///<Summary>
            ///[63:0]   A3:A2:Y3:Cr0:Y2:A1:A0:Y1:Cb0:Y0  1:1:10:10:10:1:1:10:10:10 little endian
            ///</Summary>
            Y0l2 = 0x324c3059,
            ///<Summary>
            ///[63:0]   X3:X2:Y3:Cr0:Y2:X1:X0:Y1:Cb0:Y0  1:1:10:10:10:1:1:10:10:10 little endian
            ///</Summary>
            X0l2 = 0x324c3058,
            ///<Summary>
            ///
            ///</Summary>
            Yuv4208Bit = 0x38305559,
            ///<Summary>
            ///
            ///</Summary>
            Yuv42010Bit = 0x30315559,
            ///<Summary>
            ///
            ///</Summary>
            Xrgb8888A8 = 0x38415258,
            ///<Summary>
            ///
            ///</Summary>
            Xbgr8888A8 = 0x38414258,
            ///<Summary>
            ///
            ///</Summary>
            Rgbx8888A8 = 0x38415852,
            ///<Summary>
            ///
            ///</Summary>
            Bgrx8888A8 = 0x38415842,
            ///<Summary>
            ///
            ///</Summary>
            Rgb888A8 = 0x38413852,
            ///<Summary>
            ///
            ///</Summary>
            Bgr888A8 = 0x38413842,
            ///<Summary>
            ///
            ///</Summary>
            Rgb565A8 = 0x38413552,
            ///<Summary>
            ///
            ///</Summary>
            Bgr565A8 = 0x38413542,
            ///<Summary>
            ///non-subsampled Cr:Cb plane
            ///</Summary>
            Nv24 = 0x3432564e,
            ///<Summary>
            ///non-subsampled Cb:Cr plane
            ///</Summary>
            Nv42 = 0x3234564e,
            ///<Summary>
            ///2x1 subsampled Cr:Cb plane, 10 bit per channel
            ///</Summary>
            P210 = 0x30313250,
            ///<Summary>
            ///2x2 subsampled Cr:Cb plane 10 bits per channel
            ///</Summary>
            P010 = 0x30313050,
            ///<Summary>
            ///2x2 subsampled Cr:Cb plane 12 bits per channel
            ///</Summary>
            P012 = 0x32313050,
            ///<Summary>
            ///2x2 subsampled Cr:Cb plane 16 bits per channel
            ///</Summary>
            P016 = 0x36313050,
            ///<Summary>
            ///[63:0] A:x:B:x:G:x:R:x 10:6:10:6:10:6:10:6 little endian
            ///</Summary>
            Axbxgxrx106106106106 = 0x30314241,
            ///<Summary>
            ///2x2 subsampled Cr:Cb plane
            ///</Summary>
            Nv15 = 0x3531564e,
            ///<Summary>
            ///
            ///</Summary>
            Q410 = 0x30313451,
            ///<Summary>
            ///
            ///</Summary>
            Q401 = 0x31303451,
            ///<Summary>
            ///[63:0] x:R:G:B 16:16:16:16 little endian
            ///</Summary>
            Xrgb16161616 = 0x38345258,
            ///<Summary>
            ///[63:0] x:B:G:R 16:16:16:16 little endian
            ///</Summary>
            Xbgr16161616 = 0x38344258,
            ///<Summary>
            ///[63:0] A:R:G:B 16:16:16:16 little endian
            ///</Summary>
            Argb16161616 = 0x38345241,
            ///<Summary>
            ///[63:0] A:B:G:R 16:16:16:16 little endian
            ///</Summary>
            Abgr16161616 = 0x38344241,
        }
    }
}
