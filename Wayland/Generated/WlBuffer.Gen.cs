using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// content for a wl_surface
    /// </summary>
    public partial class WlBuffer : WaylandObject
    {
        public const string INTERFACE = "wl_buffer";
        public WlBuffer(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// destroy a buffer
        /// </summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        public enum RequestOpcode : ushort
        {
            Destroy
        }

        public Action<WlBuffer> release;
        public enum EventOpcode : ushort
        {
            Release
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Release:
                {
                    if (this.release != null)
                    {
                        this.release.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Release}({this})");
                    }

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        public override WaylandType[] WaylandTypes(ushort opCode)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Release:
                    return new WaylandType[]{};
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
