using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlSeat : WaylandObject
    {
        public const string INTERFACE = "wl_seat";
        public WlSeat(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public WlPointer GetPointer()
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.GetPointer, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetPointer}({id})");
            connection[id] = new WlPointer(id, version, connection);
            return (WlPointer)connection[id];
        }

        public WlKeyboard GetKeyboard()
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.GetKeyboard, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetKeyboard}({id})");
            connection[id] = new WlKeyboard(id, version, connection);
            return (WlKeyboard)connection[id];
        }

        public WlTouch GetTouch()
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.GetTouch, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetTouch}({id})");
            connection[id] = new WlTouch(id, version, connection);
            return (WlTouch)connection[id];
        }

        public void Release()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Release);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Release}()");
        }

        public enum RequestOpcode : ushort
        {
            GetPointer,
            GetKeyboard,
            GetTouch,
            Release
        }

        public Action<WlSeat, uint> capabilities;
        public Action<WlSeat, string> name;
        public enum EventOpcode : ushort
        {
            Capabilities,
            Name
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Capabilities:
                {
                    var capabilities = (uint)arguments[0];
                    if (this.capabilities != null)
                    {
                        this.capabilities.Invoke(this, capabilities);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Capabilities}({this},{capabilities})");
                    }

                    break;
                }

                case EventOpcode.Name:
                {
                    var name = (string)arguments[0];
                    if (this.name != null)
                    {
                        this.name.Invoke(this, name);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Name}({this},{name})");
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
                case EventOpcode.Capabilities:
                    return new WaylandType[]{WaylandType.Uint, };
                case EventOpcode.Name:
                    return new WaylandType[]{WaylandType.String, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
