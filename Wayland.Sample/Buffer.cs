using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenGL;

namespace Wayland.Sample
{
    public unsafe class Buffer
    {
        public WlBuffer buffer;
        public bool isBusy;
        public gbm_bo* bo;
        public int dmabufFD;
        public int width, height;
        public uint stride;
        public uint format;

        public IntPtr image;
        public uint glTexture;
        public uint glFBO;

        public void CreateBuffer(Device device, ZwpLinuxDmabufV1 dmabuf,int width, int height, uint format, ZwpLinuxBufferParamsV1.FlagsFlag flag, uint modifier)
        {
            ZwpLinuxBufferParamsV1 linuxBufferParams = dmabuf.CreateParams();

            linuxBufferParams.created += BufferCreated;
            linuxBufferParams.failed += BufferFailed;

            bo = GBM.gbm_bo_create(device.gb_device, (uint)width, (uint)height, format, (uint)gbm_bo_flags.GBM_BO_USE_RENDERING);

            dmabufFD = GBM.gbm_bo_get_fd(bo);
            stride = GBM.gbm_bo_get_stride(bo);

            this.width = width;
            this.height = height;
            this.format = format;

            linuxBufferParams.Add((IntPtr)dmabufFD, 0, 0, stride, (uint)modifier >> 32, (uint)modifier & 0xffffffff);
            linuxBufferParams.Create(width, height, format, flag);

        }

        private void OpenGlBackend(Device device)
        {
            Egl.GetPlatformDisplayEXT(Egl.PLATFORM_GBM_KHR,(IntPtr)device.gb_device,null);
        }

        private void BufferCreated(ZwpLinuxBufferParamsV1 bufferParams, WlBuffer tbuffer)
        {
            this.buffer = tbuffer;
            this.buffer.release += ReleaseBuffer;

            bufferParams.Destroy();
        }

        private void ReleaseBuffer(WlBuffer buffer)
        {
            isBusy = false;
        }

        private void BufferFailed(ZwpLinuxBufferParamsV1 bufferParams)
        {
            bufferParams.Destroy();
            
        }
    }
}