using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlShmPool : WaylandObject
    {
        public const string INTERFACE = "wl_shm_pool";
        public WlShmPool(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public WlBuffer CreateBuffer(int offset, int width, int height, int stride, uint format)
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateBuffer, id, offset, width, height, stride, format);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreateBuffer}({id},{offset},{width},{height},{stride},{format})");
            connection[id] = new WlBuffer(id, version, connection);
            return (WlBuffer)connection[id];
        }

        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        public void Resize(int size)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Resize, size);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Resize}({size})");
        }

        public enum RequestOpcode : ushort
        {
            CreateBuffer,
            Destroy,
            Resize
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
