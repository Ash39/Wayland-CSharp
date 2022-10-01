using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wayland.Sample
{
    public unsafe class Window
    {
        private const int NUM_BUFFERS = 3;

        public Display display;
        public Device device;
        public WlSurface surface;
        public XdgSurface xdgSurface;
        public WlCallback frameCallback;
        public XdgToplevel xdgToplevel;
        public int width, height;
        public readonly List<Buffer> buffers = new List<Buffer>();
        IBackend backend;

        public Action render;


        private void GetDevice()
        {
            device = new Device("/dev/dri/renderD128");

            if(!device.Connect())
            {
                device = new Device("/dev/dri/renderD129");

                if(!device.Connect())
                {
                    throw new Exception("No Device Available.");
                }
            }
            
        }

        public void Create(string title, int width, int height, GraphicsApi graphics)
        {
            this.width = width;
            this.height = height;

            display = new Display();
            display.Connect();

            GetDevice();

            surface = display.compositor.CreateSurface();
            xdgSurface = display.xdgWm.GetXdgSurface(surface);
            xdgSurface.configure += Configure;
            XdgToplevel xdgToplevel = xdgSurface.GetToplevel();
            xdgToplevel.SetTitle(title);
            

            

            switch(graphics)
            {
                case GraphicsApi.OpenGl:
                    backend = new OpenGlBackend();
                    break;
                case GraphicsApi.OpenGlES:
                    backend = new OpenGlESBackend();
                    break;
                case GraphicsApi.Vulkan:
                    break;
            }

            backend.render = render;

            backend.CreateDisplay(device);

            for(int i = 0; i < NUM_BUFFERS; i++ )
            {
                buffers.Add(new Buffer());
                buffers[i].CreateBuffer(device, display.dmabuf, width, height, DRMFormats.DRM_FORMAT_ARGB8888, ZwpLinuxBufferParamsV1.FlagsFlag.YInvert, DRMFormats.DRM_FORMAT_INVALID);
                backend.BindBuffer(buffers[i]);
            }

            backend.Complete(this);

            surface.Commit();

            display.display.Roundtrip();

            backend.Present(this);

            while (display.display.Dispatch())
            {
                
            }
        }

        private void Configure(XdgSurface xdgSurface, uint serial)
        {
            xdgSurface.AckConfigure(serial);

            backend.Present(this);

        }
        
        
    }
}