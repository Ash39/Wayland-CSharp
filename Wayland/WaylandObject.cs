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
        protected uint version { get; set; }
        protected WaylandConnection connection;

        protected WaylandObject(uint id, uint version, WaylandConnection connection)
        {
            this.id = id;
            this.version = version;
            this.connection = connection;
        }

        public abstract void Event(ushort opCode, WlType[] objects);

        public abstract WaylandType[] WaylandTypes(ushort opCode);
    }
}
