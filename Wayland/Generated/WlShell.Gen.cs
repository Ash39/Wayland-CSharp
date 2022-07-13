using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlShell : WaylandObject
    {
        public const string INTERFACE = "wl_shell";
        public WlShell(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public WlShellSurface GetShellSurface(WlSurface surface)
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.GetShellSurface, id, surface.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetShellSurface}({id},{surface.id})");
            connection[id] = new WlShellSurface(id, version, connection);
            return (WlShellSurface)connection[id];
        }

        public enum RequestOpcode : ushort
        {
            GetShellSurface
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
