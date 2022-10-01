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
            if (wlinterface == WlCompositor.INTERFACE)
            {
                compositor = registery.Bind<WlCompositor>(name, wlinterface, version);
            }
            else if (wlinterface == WlShm.INTERFACE)
            {
                wl_shm = registery.Bind<WlShm>(name, wlinterface, version);
            }
            else if (wlinterface == XdgWmBase.INTERFACE)
            {
                xdgWm = registery.Bind<XdgWmBase>(name, XdgWmBase.INTERFACE, version);
            }
            else if (wlinterface == ZwpLinuxDmabufV1.INTERFACE)
            {
                dmabuf = registery.Bind<ZwpLinuxDmabufV1>(name, ZwpLinuxDmabufV1.INTERFACE, version);
                dmabuf.modifier += DmabufModifier;
            }
            else if (wlinterface == WlSeat.INTERFACE)
            {
                seat = registery.Bind<WlSeat>(name, WlSeat.INTERFACE, version);
                input = new Input();
                seat.capabilities += input.Create;
                seat.name += SeatName;
            }
            else if (wlinterface == WlDrm.INTERFACE)
            {
                wl_drm = registery.Bind<WlDrm>(name, WlDrm.INTERFACE, version);
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

        private static void DmabufModifier(ZwpLinuxDmabufV1 zwpLinuxDmabuf, uint format, uint modifier_hi, uint modifier_lo)
        {
            ulong modifier = ((ulong) modifier_hi << 32 ) | modifier_lo;
        }

    }
}