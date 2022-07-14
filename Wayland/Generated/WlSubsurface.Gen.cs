using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// sub-surface interface to a wl_surface
    /// </summary>
    public partial class WlSubsurface : WaylandObject
    {
        public const string INTERFACE = "wl_subsurface";
        public WlSubsurface(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// remove sub-surface interface
        /// </summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        /// <summary>
        /// reposition the sub-surface
        /// </summary>
        public void SetPosition(int x, int y)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetPosition, x, y);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetPosition}({x},{y})");
        }

        /// <summary>
        /// restack the sub-surface
        /// </summary>
        public void PlaceAbove(WlSurface sibling)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.PlaceAbove, sibling.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.PlaceAbove}({sibling.id})");
        }

        /// <summary>
        /// restack the sub-surface
        /// </summary>
        public void PlaceBelow(WlSurface sibling)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.PlaceBelow, sibling.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.PlaceBelow}({sibling.id})");
        }

        /// <summary>
        /// set sub-surface to synchronized mode
        /// </summary>
        public void SetSync()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetSync);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetSync}()");
        }

        /// <summary>
        /// set sub-surface to desynchronized mode
        /// </summary>
        public void SetDesync()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetDesync);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetDesync}()");
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            SetPosition,
            PlaceAbove,
            PlaceBelow,
            SetSync,
            SetDesync
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
