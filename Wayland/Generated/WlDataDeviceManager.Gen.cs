using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlDataDeviceManager : WaylandObject
    {
        public const string INTERFACE = "wl_data_device_manager";
        public WlDataDeviceManager(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public WlDataSource CreateDataSource()
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateDataSource, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreateDataSource}({id})");
            connection[id] = new WlDataSource(id, version, connection);
            return (WlDataSource)connection[id];
        }

        public WlDataDevice GetDataDevice(WlSeat seat)
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.GetDataDevice, id, seat.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetDataDevice}({id},{seat.id})");
            connection[id] = new WlDataDevice(id, version, connection);
            return (WlDataDevice)connection[id];
        }

        public enum RequestOpcode : ushort
        {
            CreateDataSource,
            GetDataDevice
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
