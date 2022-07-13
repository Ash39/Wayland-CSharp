using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlShm : WaylandObject
    {
        public const string INTERFACE = "wl_shm";
        public WlShm(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public WlShmPool CreatePool(IntPtr fd, int size)
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.CreatePool, id, fd, size);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreatePool}({id},{fd},{size})");
            connection[id] = new WlShmPool(id, version, connection);
            return (WlShmPool)connection[id];
        }

        public enum RequestOpcode : ushort
        {
            CreatePool
        }

        public Action<WlShm, uint> format;
        public enum EventOpcode : ushort
        {
            Format
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Format:
                {
                    var format = (uint)arguments[0];
                    if (this.format != null)
                    {
                        this.format.Invoke(this, format);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Format}({this},{format})");
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
                case EventOpcode.Format:
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
