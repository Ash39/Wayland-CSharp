using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayland
{
    public abstract class WaylandObject
    {
        public uint id;
        public IntPtr handle;
        protected uint version { get; set; }
        protected WaylandConnection connection;

        protected WaylandObject(uint factoryId ,ref uint id, uint version, WaylandConnection connection)
        {
            var handleInfo = connection.GetHandle(factoryId);

            if (handleInfo.id > 0)
            {
                this.id = id = handleInfo.id;
                this.version = version;
            }
            else
            {
                this.id = id;
                this.version = version;
            }
            this.handle = handleInfo.handle;
            this.connection = connection;
        }

        public abstract void Event(ushort opCode, object[] objects);

        public abstract WaylandType[] WaylandTypes(ushort opCode);
    }
}
