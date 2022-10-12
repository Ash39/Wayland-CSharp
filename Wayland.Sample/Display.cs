using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wayland.Sample
{
    public class Display
    {
        public WlDisplay display;
        public WlRegistry registry;
        public WlCompositor compositor;
        public XdgWmBase xdgWm;
        public ZwpLinuxDmabufV1 dmabuf;
        public WlShm wl_shm;
        public WlDrm wl_drm;
        public WlSeat seat;

        public Input input;

        public void Connect(string path = null)
        {
            display = WlDisplay.Connect();

            registry = display.GetRegistry();

            registry.global += globalCallback;
            registry.globalRemove += globalRemoveCallback;

            display.Roundtrip();
        }

        private void globalCallback(WlRegistry registery, uint name, string wlinterface, uint version)
        {
            switch (wlinterface)
            {
                case WlCompositor.INTERFACE:
                    compositor = registery.Bind<WlCompositor>(name, wlinterface, version);
                    break;
                case WlShm.INTERFACE:
                    wl_shm = registery.Bind<WlShm>(name, wlinterface, version);
                    break;
                case XdgWmBase.INTERFACE:
                    xdgWm = registery.Bind<XdgWmBase>(name, XdgWmBase.INTERFACE, version);
                    break;
                case ZwpLinuxDmabufV1.INTERFACE:
                    dmabuf = registery.Bind<ZwpLinuxDmabufV1>(name, ZwpLinuxDmabufV1.INTERFACE, version);
                    break;
                case WlSeat.INTERFACE:
                    seat = registery.Bind<WlSeat>(name, WlSeat.INTERFACE, version);
                    input = new Input();
                    seat.capabilities += input.Create;
                    seat.name += SeatName;
                    break;
                case WlDrm.INTERFACE:
                    wl_drm = registery.Bind<WlDrm>(name, WlDrm.INTERFACE, version);
                    break;
            }
        }

        private void SeatName(WlSeat seat, string name)
        {
        }


        private void globalRemoveCallback(WlRegistry registry, uint name)
        {
            Console.WriteLine("Object Deleted");
            //throw new NotImplementedException();
        }

        public void Wait() => display.Wait();
        
        public int Release() => display.Release();
    }
}