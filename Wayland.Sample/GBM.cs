using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Wayland.Sample
{
    public unsafe class GBM
    {
        public static uint __gbm_fourcc_code(char a,char b,char c,char d) => ((uint)(a) | ((uint)(b) << 8) | ((uint)(c) << 16) | ((uint)(d) << 24));

        public int GBM_FORMAT_BIG_ENDIAN = (1<<31); /* format is big endian instead of little endian */

        /* color index */
        public uint GBM_FORMAT_C8	= __gbm_fourcc_code('C', '8', ' ', ' '); /* [7:0] C */

        /* 8 bpp Red */
        public uint GBM_FORMAT_R8	= __gbm_fourcc_code('R', '8', ' ', ' ') ;   /* [7:0] R */

        /* 16 bpp RG */
        public uint GBM_FORMAT_GR88	= __gbm_fourcc_code('G', 'R', '8', '8') ;   /* [15:0] G:R 8:8 little endian */

        /* 8 bpp RGB */
        public uint GBM_FORMAT_RGB332= __gbm_fourcc_code('R', 'G', 'B', '8') ;   /* [7:0] R:G:B 3:3:2 */
        public uint GBM_FORMAT_BGR233= __gbm_fourcc_code('B', 'G', 'R', '8') ;   /* [7:0] B:G:R 2:3:3 */

        /* 16 bpp RGB */
        public uint GBM_FORMAT_XRGB4444= __gbm_fourcc_code('X', 'R', '1', '2') ;   /* [15:0] x:R:G:B 4:4:4:4 little endian */
        public uint GBM_FORMAT_XBGR4444= __gbm_fourcc_code('X', 'B', '1', '2') ;   /* [15:0] x:B:G:R 4:4:4:4 little endian */
        public uint GBM_FORMAT_RGBX4444= __gbm_fourcc_code('R', 'X', '1', '2') ;   /* [15:0] R:G:B:x 4:4:4:4 little endian */
        public uint GBM_FORMAT_BGRX4444= __gbm_fourcc_code('B', 'X', '1', '2') ;   /* [15:0] B:G:R:x 4:4:4:4 little endian */

        public uint GBM_FORMAT_ARGB4444= __gbm_fourcc_code('A', 'R', '1', '2') ;   /* [15:0] A:R:G:B 4:4:4:4 little endian */
        public uint GBM_FORMAT_ABGR4444= __gbm_fourcc_code('A', 'B', '1', '2') ;   /* [15:0] A:B:G:R 4:4:4:4 little endian */
        public uint GBM_FORMAT_RGBA4444= __gbm_fourcc_code('R', 'A', '1', '2') ;   /* [15:0] R:G:B:A 4:4:4:4 little endian */
        public uint GBM_FORMAT_BGRA4444= __gbm_fourcc_code('B', 'A', '1', '2') ;   /* [15:0] B:G:R:A 4:4:4:4 little endian */

        public uint GBM_FORMAT_XRGB1555= __gbm_fourcc_code('X', 'R', '1', '5') ;   /* [15:0] x:R:G:B 1:5:5:5 little endian */
        public uint GBM_FORMAT_XBGR1555= __gbm_fourcc_code('X', 'B', '1', '5') ;   /* [15:0] x:B:G:R 1:5:5:5 little endian */
        public uint GBM_FORMAT_RGBX5551= __gbm_fourcc_code('R', 'X', '1', '5') ;   /* [15:0] R:G:B:x 5:5:5:1 little endian */
        public uint GBM_FORMAT_BGRX5551= __gbm_fourcc_code('B', 'X', '1', '5') ;   /* [15:0] B:G:R:x 5:5:5:1 little endian */

        public uint GBM_FORMAT_ARGB1555= __gbm_fourcc_code('A', 'R', '1', '5') ;   /* [15:0] A:R:G:B 1:5:5:5 little endian */
        public uint GBM_FORMAT_ABGR1555= __gbm_fourcc_code('A', 'B', '1', '5') ;   /* [15:0] A:B:G:R 1:5:5:5 little endian */
        public uint GBM_FORMAT_RGBA5551= __gbm_fourcc_code('R', 'A', '1', '5') ;   /* [15:0] R:G:B:A 5:5:5:1 little endian */
        public uint GBM_FORMAT_BGRA5551= __gbm_fourcc_code('B', 'A', '1', '5') ;   /* [15:0] B:G:R:A 5:5:5:1 little endian */

        public uint GBM_FORMAT_RGB565= __gbm_fourcc_code('R', 'G', '1', '6') ;   /* [15:0] R:G:B 5:6:5 little endian */
        public uint GBM_FORMAT_BGR565= __gbm_fourcc_code('B', 'G', '1', '6') ;   /* [15:0] B:G:R 5:6:5 little endian */

        /* 24 bpp RGB */
        public uint GBM_FORMAT_RGB888= __gbm_fourcc_code('R', 'G', '2', '4') ;   /* [23:0] R:G:B little endian */
        public uint GBM_FORMAT_BGR888= __gbm_fourcc_code('B', 'G', '2', '4') ;   /* [23:0] B:G:R little endian */

        /* 32 bpp RGB */
        public uint GBM_FORMAT_XRGB8888= __gbm_fourcc_code('X', 'R', '2', '4') ;   /* [31:0] x:R:G:B 8:8:8:8 little endian */
        public uint GBM_FORMAT_XBGR8888= __gbm_fourcc_code('X', 'B', '2', '4') ;   /* [31:0] x:B:G:R 8:8:8:8 little endian */
        public uint GBM_FORMAT_RGBX8888= __gbm_fourcc_code('R', 'X', '2', '4') ;   /* [31:0] R:G:B:x 8:8:8:8 little endian */
        public uint GBM_FORMAT_BGRX8888= __gbm_fourcc_code('B', 'X', '2', '4') ;   /* [31:0] B:G:R:x 8:8:8:8 little endian */

        public uint GBM_FORMAT_ARGB8888= __gbm_fourcc_code('A', 'R', '2', '4') ;   /* [31:0] A:R:G:B 8:8:8:8 little endian */
        public uint GBM_FORMAT_ABGR8888= __gbm_fourcc_code('A', 'B', '2', '4') ;   /* [31:0] A:B:G:R 8:8:8:8 little endian */
        public uint GBM_FORMAT_RGBA8888= __gbm_fourcc_code('R', 'A', '2', '4') ;   /* [31:0] R:G:B:A 8:8:8:8 little endian */
        public uint GBM_FORMAT_BGRA8888= __gbm_fourcc_code('B', 'A', '2', '4') ;   /* [31:0] B:G:R:A 8:8:8:8 little endian */

        public uint GBM_FORMAT_XRGB2101010= __gbm_fourcc_code('X', 'R', '3', '0') ;   /* [31:0] x:R:G:B 2:10:10:10 little endian */
        public uint GBM_FORMAT_XBGR2101010= __gbm_fourcc_code('X', 'B', '3', '0') ;   /* [31:0] x:B:G:R 2:10:10:10 little endian */
        public uint GBM_FORMAT_RGBX1010102= __gbm_fourcc_code('R', 'X', '3', '0') ;   /* [31:0] R:G:B:x 10:10:10:2 little endian */
        public uint GBM_FORMAT_BGRX1010102= __gbm_fourcc_code('B', 'X', '3', '0') ;   /* [31:0] B:G:R:x 10:10:10:2 little endian */

        public uint GBM_FORMAT_ARGB2101010= __gbm_fourcc_code('A', 'R', '3', '0') ;   /* [31:0] A:R:G:B 2:10:10:10 little endian */
        public uint GBM_FORMAT_ABGR2101010= __gbm_fourcc_code('A', 'B', '3', '0') ;   /* [31:0] A:B:G:R 2:10:10:10 little endian */
        public uint GBM_FORMAT_RGBA1010102= __gbm_fourcc_code('R', 'A', '3', '0') ;   /* [31:0] R:G:B:A 10:10:10:2 little endian */
        public uint GBM_FORMAT_BGRA1010102= __gbm_fourcc_code('B', 'A', '3', '0') ;   /* [31:0] B:G:R:A 10:10:10:2 little endian */

        /* packed YCbCr */
        public uint GBM_FORMAT_YUYV	= __gbm_fourcc_code('Y', 'U', 'Y', 'V') ;   /* [31:0] Cr0:Y1:Cb0:Y0 8:8:8:8 little endian */
        public uint GBM_FORMAT_YVYU	= __gbm_fourcc_code('Y', 'V', 'Y', 'U') ;   /* [31:0] Cb0:Y1:Cr0:Y0 8:8:8:8 little endian */
        public uint GBM_FORMAT_UYVY	= __gbm_fourcc_code('U', 'Y', 'V', 'Y') ;   /* [31:0] Y1:Cr0:Y0:Cb0 8:8:8:8 little endian */
        public uint GBM_FORMAT_VYUY	= __gbm_fourcc_code('V', 'Y', 'U', 'Y') ;   /* [31:0] Y1:Cb0:Y0:Cr0 8:8:8:8 little endian */

        public uint GBM_FORMAT_AYUV	= __gbm_fourcc_code('A', 'Y', 'U', 'V') ;   /* [31:0] A:Y:Cb:Cr 8:8:8:8 little endian */

        /*
        * 2 plane YCbCr
        * index 0 = Y plane, [7:0] Y
        * index 1 = Cr:Cb plane, [15:0] Cr:Cb little endian
        * or
        * index 1 = Cb:Cr plane, [15:0] Cb:Cr little endian
        */
        public uint GBM_FORMAT_NV12	= __gbm_fourcc_code('N', 'V', '1', '2') ;   /* 2x2 subsampled Cr:Cb plane */
        public uint GBM_FORMAT_NV21	= __gbm_fourcc_code('N', 'V', '2', '1') ;   /* 2x2 subsampled Cb:Cr plane */
        public uint GBM_FORMAT_NV16	= __gbm_fourcc_code('N', 'V', '1', '6') ;   /* 2x1 subsampled Cr:Cb plane */
        public uint GBM_FORMAT_NV61	= __gbm_fourcc_code('N', 'V', '6', '1') ;   /* 2x1 subsampled Cb:Cr plane */

        /*
        * 3 plane YCbCr
        * index 0: Y plane, [7:0] Y
        * index 1: Cb plane, [7:0] Cb
        * index 2: Cr plane, [7:0] Cr
        * or
        * index 1: Cr plane, [7:0] Cr
        * index 2: Cb plane, [7:0] Cb
        */
        public uint GBM_FORMAT_YUV410= __gbm_fourcc_code('Y', 'U', 'V', '9') ;   /* 4x4 subsampled Cb (1) and Cr (2) planes */
        public uint GBM_FORMAT_YVU410= __gbm_fourcc_code('Y', 'V', 'U', '9') ;   /* 4x4 subsampled Cr (1) and Cb (2) planes */
        public uint GBM_FORMAT_YUV411= __gbm_fourcc_code('Y', 'U', '1', '1') ;   /* 4x1 subsampled Cb (1) and Cr (2) planes */
        public uint GBM_FORMAT_YVU411= __gbm_fourcc_code('Y', 'V', '1', '1') ;   /* 4x1 subsampled Cr (1) and Cb (2) planes */
        public uint GBM_FORMAT_YUV420= __gbm_fourcc_code('Y', 'U', '1', '2') ;   /* 2x2 subsampled Cb (1) and Cr (2) planes */
        public uint GBM_FORMAT_YVU420= __gbm_fourcc_code('Y', 'V', '1', '2') ;   /* 2x2 subsampled Cr (1) and Cb (2) planes */
        public uint GBM_FORMAT_YUV422= __gbm_fourcc_code('Y', 'U', '1', '6') ;   /* 2x1 subsampled Cb (1) and Cr (2) planes */
        public uint GBM_FORMAT_YVU422= __gbm_fourcc_code('Y', 'V', '1', '6') ;   /* 2x1 subsampled Cr (1) and Cb (2) planes */
        public uint GBM_FORMAT_YUV444= __gbm_fourcc_code('Y', 'U', '2', '4') ;   /* non-subsampled Cb (1) and Cr (2) planes */
        public uint GBM_FORMAT_YVU444= __gbm_fourcc_code('Y', 'V', '2', '4') ;   /* non-subsampled Cr (1) and Cb (2) planes */


        [DllImport("libgbm.so.1")]
        public static extern int gbm_device_get_fd( gbm_device *gbm);
        [DllImport("libgbm.so.1")]
        public static extern char* gbm_device_get_backend_name( gbm_device *gbm);
        [DllImport("libgbm.so.1")]
        public static extern int gbm_device_is_format_supported( gbm_device *gbm, uint format, uint usage);
        [DllImport("libgbm.so.1")]
        public static extern int gbm_device_get_format_modifier_plane_count( gbm_device *gbm, uint format, ulong modifier);
        [DllImport("libgbm.so.1")]
        public static extern void gbm_device_destroy( gbm_device *gbm);

        [DllImport("libgbm.so.1")]
        public static extern gbm_device* gbm_create_device(int fd);
        [DllImport("libgbm.so.1")]
        public static extern gbm_bo* gbm_bo_create( gbm_device *gbm, uint width, uint height, uint format, uint flags);
        [DllImport("libgbm.so.1")]
        public static extern gbm_bo * gbm_bo_create_with_modifiers( gbm_device *gbm, uint width, uint height, uint format,  ulong *modifiers, uint count);

        public int GBM_BO_IMPORT_WL_BUFFER   =      0x5501;
        public int GBM_BO_IMPORT_EGL_IMAGE  =       0x5502;
        public int GBM_BO_IMPORT_FD           =     0x5503;
        public int GBM_BO_IMPORT_FD_MODIFIER   =    0x5504;


        [DllImport("libgbm.so.1")]
        public static extern gbm_bo* gbm_bo_import(gbm_device *gbm, uint type, void *buffer, uint usage);


        [DllImport("libgbm.so.1")]
        public static extern void * gbm_bo_map(gbm_bo *bo, int x, uint y, uint width, uint height, uint flags, uint *stride, void **map_data);
        [DllImport("libgbm.so.1")]
        public static extern void gbm_bo_unmap( gbm_bo *bo, void *map_data);
        [DllImport("libgbm.so.1")]
        public static extern uint gbm_bo_get_width( gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern uint gbm_bo_get_height( gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern uint gbm_bo_get_stride( gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern uint gbm_bo_get_stride_for_plane( gbm_bo *bo, int plane);
        [DllImport("libgbm.so.1")]
        public static extern uint gbm_bo_get_format( gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern uint gbm_bo_get_bpp( gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern uint gbm_bo_get_offset( gbm_bo *bo, int plane);
        [DllImport("libgbm.so.1")]
        public static extern gbm_device * gbm_bo_get_device( gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern  gbm_bo_handle gbm_bo_get_handle( gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern int gbm_bo_get_fd( gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern ulong gbm_bo_get_modifier( gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern int gbm_bo_get_plane_count( gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern  gbm_bo_handle gbm_bo_get_handle_for_plane( gbm_bo *bo, int plane);
        [DllImport("libgbm.so.1")]
        public static extern int gbm_bo_write( gbm_bo *bo, void *buf, IntPtr count);
        [DllImport("libgbm.so.1")]
        public static extern void gbm_bo_set_user_data( gbm_bo *bo, void *data, DestroyUserData destroy_user_data);
        [DllImport("libgbm.so.1")]
        public static extern void * gbm_bo_get_user_data( gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern void gbm_bo_destroy( gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern  gbm_surface * gbm_surface_create( gbm_device *gbm, int width, uint height, uint format, uint flags);
        [DllImport("libgbm.so.1")]
        public static extern  gbm_surface * gbm_surface_create_with_modifiers( gbm_device *gbm, uint width, uint height, uint format, ulong *modifiers, uint count);
        [DllImport("libgbm.so.1")]
        public static extern int gbm_surface_needs_lock_front_buffer( gbm_surface *surface);
        [DllImport("libgbm.so.1")]
        public static extern  gbm_bo * gbm_surface_lock_front_buffer( gbm_surface *surface);
        [DllImport("libgbm.so.1")]
        public static extern void gbm_surface_release_buffer( gbm_surface *surface, gbm_bo *bo);
        [DllImport("libgbm.so.1")]
        public static extern int gbm_surface_has_free_buffers( gbm_surface *surface);
        [DllImport("libgbm.so.1")]
        public static extern void gbm_surface_destroy( gbm_surface *surface);
    }

    public unsafe delegate void DestroyUserData(gbm_bo * bo, void* data);

    /**
    * Flags to indicate the type of mapping for the buffer - these are
    * passed into gbm_bo_map(). The caller must set the union of all the
    * flags that are appropriate.
    *
    * These flags are independent of the GBM_BO_USE_* creation flags. However,
    * mapping the buffer may require copying to/from a staging buffer.
    *
    * See also: pipe_transfer_usage
    */
    public enum gbm_bo_transfer_flags {
        /**
            * Buffer contents read back (or accessed directly) at transfer
            * create time.
            */
        GBM_BO_TRANSFER_READ       = (1 << 0),
        /**
            * Buffer contents will be written back at unmap time
            * (or modified as a result of being accessed directly).
            */
        GBM_BO_TRANSFER_WRITE      = (1 << 1),
        /**
            * Read/modify/write
            */
        GBM_BO_TRANSFER_READ_WRITE = (GBM_BO_TRANSFER_READ | GBM_BO_TRANSFER_WRITE),
    };

    public struct gbm_import_fd_data {
        public int fd;
        public uint width;
        public uint height;
        public uint stride;
        public uint format;
    };

    public unsafe struct gbm_import_fd_modifier_data {
        public uint width;
        public uint height;
        public uint format;
        public uint num_fds;

        public fixed int fds[4];
        public fixed int strides[4];
        public fixed int offsets[4];
        public ulong modifier;
    };

    [Flags]
    public enum gbm_bo_flags {
        /**
            * Buffer is going to be presented to the screen using an API such as KMS
            */
        
        GBM_BO_USE_SCANOUT      = (1 << 0),
        /**
            * Buffer is going to be used as cursor
            */
        GBM_BO_USE_CURSOR       = (1 << 1),
        /**
            * Deprecated
            */
        GBM_BO_USE_CURSOR_64X64 = GBM_BO_USE_CURSOR,
        /**
            * Buffer is to be used for rendering - for example it is going to be used
            * as the storage for a color buffer
            */
        GBM_BO_USE_RENDERING    = (1 << 2),
        /**
            * Buffer can be used for gbm_bo_write.  This is guaranteed to work
            * with GBM_BO_USE_CURSOR, but may not work for other combinations.
            */
        GBM_BO_USE_WRITE    = (1 << 3),
        /**
            * Buffer is linear, i.e. not tiled.
            */
        GBM_BO_USE_LINEAR = (1 << 4),
    }

    public enum gbm_bo_format {
        /** RGB with 8 bits per channel in a 32 bit value */
        GBM_BO_FORMAT_XRGB8888, 
        /** ARGB with 8 bits per channel in a 32 bit value */
        GBM_BO_FORMAT_ARGB8888
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct gbm_bo_handle {
        [FieldOffset(0)]
        public void *ptr;
        [FieldOffset(0)]
        public int s32;
        [FieldOffset(0)]
        public uint u32;
        [FieldOffset(0)]
        public long s64;
        [FieldOffset(0)]
        public ulong u64;
    }

    public struct gbm_device 
    {

    }
    public struct gbm_bo 
    {
        
    }
    public struct gbm_surface 
    {
        
    }

    
}