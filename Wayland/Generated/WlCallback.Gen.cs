using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// callback object
    /// </summary>
    public partial class WlCallback : WaylandObject
    {
        public const string INTERFACE = "wl_callback";
        public WlCallback(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public enum RequestOpcode : ushort
        {
        }

        public Action<WlCallback, uint> done;
        public enum EventOpcode : ushort
        {
            Done
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Done:
                {
                    var callbackData = (uint)arguments[0];
                    if (this.done != null)
                    {
                        this.done.Invoke(this, callbackData);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Done}({this},{callbackData})");
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
                case EventOpcode.Done:
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
