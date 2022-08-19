using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///callback object
    ///<para>
    ///Clients can handle the 'done' event to get notified when
    ///the related request is done.
    ///</para>
    ///</Summary>
    public partial class WlCallback : WaylandObject
    {
        public const string INTERFACE = "wl_callback";
        public WlCallback(uint factoryId, ref uint id, WaylandConnection connection, uint version = 1) : base(factoryId, ref id, version, connection)
        {
        }

        public enum RequestOpcode : ushort
        {
        }

        ///<Summary>
        ///done event
        ///<para>
        ///Notify the client when the related request is done.
        ///</para>
        ///</Summary>
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
