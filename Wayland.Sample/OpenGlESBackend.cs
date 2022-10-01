using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using OpenGL;
using static Khronos.KhronosApi;

namespace Wayland.Sample
{
    public unsafe class OpenGlESBackend : IBackend
    {
        private IntPtr eglDisplay;
        private IntPtr context;

        public Action render { get; set; }

        private bool isInitated;

        public void BindBuffer(Buffer buffer)
        {
            int general_attribs = 3;
            int plane_attribs = 5;
            int entries_per_attrib = 2;
            int[] attribs = new int[(general_attribs + plane_attribs) *
                    entries_per_attrib + 1];
            uint atti = 0;
            attribs[atti++] = Egl.WIDTH;
            attribs[atti++] = buffer.width;
            attribs[atti++] = Egl.HEIGHT;
            attribs[atti++] = buffer.height;
            attribs[atti++] = Egl.LINUX_DRM_FOURCC_EXT;
            attribs[atti++] = (int)buffer.format;
            /* plane 0 */
            attribs[atti++] = Egl.DMA_BUF_PLANE0_FD_EXT;
            attribs[atti++] = buffer.dmabufFD;
            attribs[atti++] = Egl.DMA_BUF_PLANE0_OFFSET_EXT;
            attribs[atti++] = (int)0;
            attribs[atti++] = Egl.DMA_BUF_PLANE0_PITCH_EXT;
            attribs[atti++] = (int)buffer.stride;
            /* TODO: Update for dmabuf import modifiers */
            attribs[atti] = Egl.NONE;

            buffer.image = CreateImageKHR(eglDisplay, IntPtr.Zero, Egl.LINUX_DMA_BUF_EXT, IntPtr.Zero, attribs);

            Egl.MakeCurrent(eglDisplay, (IntPtr)Egl.NO_SURFACE, (IntPtr)Egl.NO_SURFACE, context);
            int error = Egl.GetError();
            buffer.glTexture = Gl.GenTexture();
            error = Egl.GetError();
            Gl.BindTexture(TextureTarget.Texture2d, buffer.glTexture);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, Gl.CLAMP_TO_EDGE);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, Gl.CLAMP_TO_EDGE);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.LINEAR);

            EGLImageTargetTexture2DOES((int)TextureTarget.Texture2d, buffer.image);
            buffer.glFBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, buffer.glFBO);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d,buffer.glTexture, 0);

            FramebufferStatus status = Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

            if(status != FramebufferStatus.FramebufferComplete)
            {
                throw new Exception($"Framebuffer Create Failed - {status.ToString()}");
            }
        }

        public void CreateDisplay(Device device)
        {
            GetPlatformDisplayEXT = Marshal.GetDelegateForFunctionPointer<eglGetPlatformDisplayEXT>(GetProcAddressCore("eglGetPlatformDisplayEXT"));
            CreateImageKHR = Marshal.GetDelegateForFunctionPointer<eglCreateImageKHR>(GetProcAddressCore("eglCreateImageKHR"));
            DestroyImageKHR = Marshal.GetDelegateForFunctionPointer<eglDestroyImageKHR>(GetProcAddressCore("eglDestroyImageKHR"));
            EGLImageTargetTexture2DOES = Marshal.GetDelegateForFunctionPointer<glEGLImageTargetTexture2DOES>(GetProcAddressCore("glEGLImageTargetTexture2DOES"));

            eglDisplay = GetPlatformDisplayEXT(Egl.PLATFORM_GBM_KHR, (IntPtr)device.gb_device, null);
            
            
            Egl.BindAPI();

            if (!Egl.Initialize(eglDisplay, null, null))
                throw new InvalidOperationException("unable to initialize EGL");

            Gl.BindAPI(new Khronos.KhronosVersion(3,2,"gles2"), new OpenGL.Gl.Extensions());
            
            int error = Egl.GetError();

            int[] config_attribs = {
                Egl.SURFACE_TYPE,Egl.WINDOW_BIT,
                Egl.RED_SIZE, 1,
                Egl.GREEN_SIZE, 1,
                Egl.BLUE_SIZE, 1,
                Egl.ALPHA_SIZE, 1,
                Egl.RENDERABLE_TYPE,(int)Egl.OPENGL_ES3_BIT,
                Egl.NONE,
            };

			int[] configCount = new int[1];
			IntPtr[] configs = new IntPtr[8];

			if (!Egl.ChooseConfig(eglDisplay, config_attribs, configs, configs.Length, configCount))
            {
                error = Egl.GetError();
            }
            int[] context_attribs = {
                Egl.CONTEXT_CLIENT_VERSION, 3,
                Egl.NONE,
            };

            context = Egl.CreateContext(eglDisplay, configs[0], IntPtr.Zero, context_attribs);
            
        }

        public void Complete(Window window)
        {
            Egl.SwapInterval(eglDisplay, 0);
            Egl.MakeCurrent(eglDisplay, (IntPtr)Egl.NO_SURFACE, (IntPtr)Egl.NO_SURFACE, context);
            isInitated = true;
        }
        
        public void Present(Window window)
        {
            if(!isInitated)
                return;

            Buffer buffer = NextBuffer(window);

            if(buffer == null)
                return;

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, buffer.glFBO);

            render?.Invoke();

            window.surface.Attach(buffer.buffer,0,0);
            window.surface.DamageBuffer(0,0, int.MaxValue, int.MaxValue);
            window.frameCallback = window.surface.Frame();
            window.frameCallback.done += (callback, time) => 
            {
                Present(window);
            };
            window.surface.Commit();
                    
            
        }

        public Buffer NextBuffer(Window window)
        {
            //return window.buffers[0];
            for(int i = 0; i < window.buffers.Count; i++)
            {
                Buffer buffer = window.buffers[i];
                if(!buffer.isBusy)
                {
                    buffer.isBusy = true;
                    return buffer;
                }
            }
            return null;
        }



        [DllImport("libEGL.so", EntryPoint = "eglGetProcAddress")]
		private static extern IntPtr GetProcAddressCore(string funcname);

        eglGetPlatformDisplayEXT GetPlatformDisplayEXT;
        eglCreateImageKHR CreateImageKHR;
        eglDestroyImageKHR DestroyImageKHR;
        glEGLImageTargetTexture2DOES EGLImageTargetTexture2DOES;

        

        private delegate IntPtr eglGetPlatformDisplayEXT(uint platform, IntPtr native_display, int[] attrib_list);
        private delegate IntPtr eglCreateImageKHR(IntPtr dpy, IntPtr ctx, uint target, IntPtr buffer, int[] attrib_list);
        private delegate bool eglDestroyImageKHR(IntPtr dpy, IntPtr image);
        private delegate void glEGLImageTargetTexture2DOES(int target, IntPtr image);
    }
}