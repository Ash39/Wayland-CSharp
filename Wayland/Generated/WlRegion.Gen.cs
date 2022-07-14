using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// region interface
    /// </summary>
    public partial class WlRegion : WaylandObject
    {
        public const string INTERFACE = "wl_region";
        public WlRegion(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// destroy region
        /// </summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        /// <summary>
        /// add rectangle to region
        /// </summary>
        public void Add(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Add, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Add}({x},{y},{width},{height})");
        }

        /// <summary>
        /// subtract rectangle from region
        /// </summary>
        public void Subtract(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Subtract, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Subtract}({x},{y},{width},{height})");
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            Add,
            Subtract
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
