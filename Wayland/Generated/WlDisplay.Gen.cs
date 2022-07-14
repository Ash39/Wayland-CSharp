using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// core global object
    /// </summary>
    public partial class WlDisplay : WaylandObject
    {
        public const string INTERFACE = "wl_display";
        public WlDisplay(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// asynchronous roundtrip
        /// </summary>
        public WlCallback Sync()
        {
            uint callback = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.Sync, callback);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Sync}({callback})");
            connection[callback] = new WlCallback(callback, version, connection);
            return (WlCallback)connection[callback];
        }

        /// <summary>
        /// get global registry object
        /// </summary>
        public WlRegistry GetRegistry()
        {
            uint registry = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.GetRegistry, registry);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetRegistry}({registry})");
            connection[registry] = new WlRegistry(registry, version, connection);
            return (WlRegistry)connection[registry];
        }

        public enum RequestOpcode : ushort
        {
            Sync,
            GetRegistry
        }

        public Action<WlDisplay, WaylandObject, uint, string> error;
        public Action<WlDisplay, uint> deleteId;
        public enum EventOpcode : ushort
        {
            Error,
            DeleteId
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Error:
                {
                    var objectId = connection[(uint)arguments[0]];
                    var code = (uint)arguments[1];
                    var message = (string)arguments[2];
                    if (this.error != null)
                    {
                        this.error.Invoke(this, objectId, code, message);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Error}({this},{objectId},{code},{message})");
                    }

                    break;
                }

                case EventOpcode.DeleteId:
                {
                    var id = (uint)arguments[0];
                    if (this.deleteId != null)
                    {
                        this.deleteId.Invoke(this, id);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.DeleteId}({this},{id})");
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
                case EventOpcode.Error:
                    return new WaylandType[]{WaylandType.Object, WaylandType.Uint, WaylandType.String, };
                case EventOpcode.DeleteId:
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
