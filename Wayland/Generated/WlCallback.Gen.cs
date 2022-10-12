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
        public WlCallback(uint id, WaylandConnection connection, uint version = 1) : base(id, version, connection)
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

        public override void Event(ushort opCode, WlType[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Done:
                {
                    var callbackData = arguments[0].u;
                    if (this.done != null)
                    {
                        this.done.Invoke(this, callbackData);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Done");
                    }

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }

        public override WaylandType[] WaylandTypes(ushort opCode)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Done:
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }
    }
}
