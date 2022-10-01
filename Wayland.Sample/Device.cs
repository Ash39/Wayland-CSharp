using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Wayland.Sample
{
    public unsafe class Device
    {
        public int drmFD;
        public gbm_device* gb_device;
        public string name;
        public string renderNode;

        public Device(string renderNode)
        {
            this.renderNode = renderNode;
        }

        public bool Connect()
        {
            FileStream stream = File.Open(renderNode, FileMode.Open, FileAccess.ReadWrite);
            drmFD = stream.SafeFileHandle.DangerousGetHandle().ToInt32();

            if(drmFD <= 0) return false;

            gb_device = GBM.gbm_create_device(drmFD);

            name = Marshal.PtrToStringAuto((IntPtr)GBM.gbm_device_get_backend_name(gb_device));

            if(gb_device == null) return false;

            return true;
        }
    }

}