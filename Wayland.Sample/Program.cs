using StbImageSharp;
using System;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using Wayland.Compatibility;

namespace Wayland.Sample // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static WlDisplay display;
        static WlCompositor compositor;
        static WlShell shell;
        static WlShm wl_shm;
        static XdgWmBase xdgWm;
        static XdgSurface xdgSurface;
        static WlSurface surface;
        static ZwpLinuxDmabufV1 dmabufV1;

        static MemoryMappedFile memoryMappedFile;

        static int width, height;
        static int stride;
        static int size;

        static void Main(string[] args)
        {
            display = WlDisplay.Connect();

            WlRegistry registry = display.GetRegistry();

            registry.global += globalCallback;
            registry.globalRemove += globalRemoveCallback;

            display.Roundtrip();

            surface = compositor.CreateSurface();
            //xdgSurface = xdgWm.GetXdgSurface(surface);
            //xdgSurface.configure = Configure;
            //XdgToplevel xdgToplevel = xdgSurface.GetToplevel();
            //xdgToplevel.SetTitle("GameWindow");

            WlShellSurface shellSurface = shell.GetShellSurface(surface);
            shellSurface.ping += Ping;
            shellSurface.SetToplevel();
            shellSurface.SetTitle("Game Window");

            width = 1270;
            height = 800;
            stride = width * 4;
            size = height * stride;
            int index = 0;
            int offset = height * stride * index;

            memoryMappedFile = MemoryMappedFile.CreateNew(null, size);

            using (MemoryMappedViewStream viewStream = memoryMappedFile.CreateViewStream())
            {

                using (Stream fileStream = File.Open("Images/blue_panel.png", FileMode.Open))
                {
                    ImageResult result = ImageResult.FromStream(fileStream);
                    viewStream.Read(result.Data, 0, result.Data.Length);
                }
            };


            wl_shm.format += (@object, format) =>
            {
                Console.WriteLine("Suface Format: " + format);
            };

            WlShmPool pool = wl_shm.CreatePool(memoryMappedFile.SafeMemoryMappedFileHandle.DangerousGetHandle(), size);

            WlBuffer buffer = pool.CreateBuffer(offset, width, height, stride, WlShm.FormatFlag.Xrgb8888);

            buffer.release += (buf) =>
            {
                buf.Destroy();
            };

            pool.Destroy();

            surface.Attach(buffer, 0, 0);
            surface.Commit();

            

            while (true)
            {
                display.Dispatch();

                //surface.Damage(0, 0, width, height);
                //surface.Commit();
            }
        }

        private static void Configure(XdgSurface xdgSurface, uint serial)
        {
            xdgSurface.AckConfigure(serial);

            int index = 0;
            int offset = height * stride * index;

            wl_shm.format += (@object, format) =>
            {
                Console.WriteLine("Suface Format: " + format);
            };

            WlShmPool pool = wl_shm.CreatePool(memoryMappedFile.SafeMemoryMappedFileHandle.DangerousGetHandle(), size);

            WlBuffer buffer = pool.CreateBuffer(offset, width, height, stride, WlShm.FormatFlag.Xrgb8888);

            buffer.release += (buf) =>
            {
                buf.Destroy();
            };

            pool.Destroy();
            surface.Attach(buffer, 0, 0);
            surface.Commit();
        }

        private static void Ping(WlShellSurface shellSurface, uint serial)
        {
            shellSurface.Pong(serial);
        }

        private static void globalRemoveCallback(WlRegistry registry, uint name)
        {
            Console.WriteLine("Object Deleted");
            //throw new NotImplementedException();
        }

        private static void globalCallback(WlRegistry registery, uint name, string wlinterface, uint version)
        {
            Console.WriteLine("Avaliable Interface:" + wlinterface);
           if (wlinterface == WlCompositor.INTERFACE)
           {
               Console.WriteLine("Using Interface:" + wlinterface);
               compositor = registery.Bind<WlCompositor>(name, wlinterface, version);
           }
           else if (wlinterface == WlShell.INTERFACE)
           {
               Console.WriteLine("Using Interface:" + wlinterface);
               shell = registery.Bind<WlShell>(name, wlinterface, version);
           }
           else if (wlinterface == WlShm.INTERFACE)
           {
               Console.WriteLine("Using Interface:" + wlinterface);
               wl_shm = registery.Bind<WlShm>(name, wlinterface, version);
           }
           else if (wlinterface == XdgWmBase.INTERFACE)
           {
               Console.WriteLine("Using Interface:" + wlinterface);
               xdgWm = registery.Bind<XdgWmBase>(name, XdgWmBase.INTERFACE, version);
           }
           else if (wlinterface == ZwpLinuxDmabufV1.INTERFACE)
           {
               Console.WriteLine("Using Interface:" + wlinterface);
               dmabufV1 = registery.Bind<ZwpLinuxDmabufV1>(name, ZwpLinuxDmabufV1.INTERFACE, version);
           }

        }
    }
}