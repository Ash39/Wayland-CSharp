using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// sub-surface compositing
    /// </summary>
    public partial class WlSubcompositor : WaylandObject
    {
        public const string INTERFACE = "wl_subcompositor";
        public WlSubcompositor(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// unbind from the subcompositor interface
        /// </summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        /// <summary>
        /// give a surface the role sub-surface
        /// </summary>
        public WlSubsurface GetSubsurface(WlSurface surface, WlSurface parent)
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.GetSubsurface, id, surface.id, parent.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetSubsurface}({id},{surface.id},{parent.id})");
            connection[id] = new WlSubsurface(id, version, connection);
            return (WlSubsurface)connection[id];
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            GetSubsurface
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
