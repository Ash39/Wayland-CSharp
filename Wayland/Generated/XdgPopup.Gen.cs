using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// short-lived, popup surfaces for menus
    /// </summary>
    public partial class XdgPopup : WaylandObject
    {
        public const string INTERFACE = "xdg_popup";
        public XdgPopup(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// remove xdg_popup interface
        /// </summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        /// <summary>
        /// make the popup take an explicit grab
        /// </summary>
        public void Grab(WlSeat seat, uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Grab, seat.id, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Grab}({seat.id},{serial})");
        }

        /// <summary>
        /// recalculate the popup's location
        /// </summary>
        public void Reposition(XdgPositioner positioner, uint token)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Reposition, positioner.id, token);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Reposition}({positioner.id},{token})");
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            Grab,
            Reposition
        }

        public Action<XdgPopup, int, int, int, int> configure;
        public Action<XdgPopup> popupDone;
        public Action<XdgPopup, uint> repositioned;
        public enum EventOpcode : ushort
        {
            Configure,
            PopupDone,
            Repositioned
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Configure:
                {
                    var x = (int)arguments[0];
                    var y = (int)arguments[1];
                    var width = (int)arguments[2];
                    var height = (int)arguments[3];
                    if (this.configure != null)
                    {
                        this.configure.Invoke(this, x, y, width, height);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Configure}({this},{x},{y},{width},{height})");
                    }

                    break;
                }

                case EventOpcode.PopupDone:
                {
                    if (this.popupDone != null)
                    {
                        this.popupDone.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.PopupDone}({this})");
                    }

                    break;
                }

                case EventOpcode.Repositioned:
                {
                    var token = (uint)arguments[0];
                    if (this.repositioned != null)
                    {
                        this.repositioned.Invoke(this, token);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Repositioned}({this},{token})");
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
                case EventOpcode.Configure:
                    return new WaylandType[]{WaylandType.Int, WaylandType.Int, WaylandType.Int, WaylandType.Int, };
                case EventOpcode.PopupDone:
                    return new WaylandType[]{};
                case EventOpcode.Repositioned:
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
