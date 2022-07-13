using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlCompositor : WaylandObject
    {
        public const string INTERFACE = "wl_compositor";
        public WlCompositor(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public WlSurface CreateSurface()
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateSurface, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreateSurface}({id})");
            connection[id] = new WlSurface(id, version, connection);
            return (WlSurface)connection[id];
        }

        public WlRegion CreateRegion()
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateRegion, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreateRegion}({id})");
            connection[id] = new WlRegion(id, version, connection);
            return (WlRegion)connection[id];
        }

        public enum RequestOpcode : ushort
        {
            CreateSurface,
            CreateRegion
        }

        public enum EventOpcode : ushort
        {
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        public override WaylandType[] WaylandTypes(ushort opCode)
        {
            switch ((EventOpcode)opCode)
            {
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
