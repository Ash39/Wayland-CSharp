using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///group of input devices
    ///<para>
    ///A seat is a group of keyboards, pointer and touch devices. This
    ///object is published as a global during start up, or when such a
    ///device is hot plugged.  A seat typically has a pointer and
    ///maintains a keyboard focus and a pointer focus.
    ///</para>
    ///</Summary>
    public partial class WlSeat : WaylandObject
    {
        public const string INTERFACE = "wl_seat";
        public WlSeat(uint id, WaylandConnection connection, uint version = 8) : base(id, version, connection)
        {
        }

        ///<Summary>
        ///return pointer object
        ///<para>
        ///The ID provided will be initialized to the wl_pointer interface
        ///for this seat.
        ///</para>
        ///<para>
        ///This request only takes effect if the seat has the pointer
        ///capability, or has had the pointer capability in the past.
        ///It is a protocol violation to issue this request on a seat that has
        ///never had the pointer capability. The missing_capability error will
        ///be sent in this case.
        ///</para>
        ///</Summary>
        ///<returns> seat pointer </returns>
        public WlPointer GetPointer()
        {
            WlPointer wObject = connection.Create<WlPointer>(0, this.version);
            uint id = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.GetPointer, id);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "GetPointer", id);
            return wObject;
        }

        ///<Summary>
        ///return keyboard object
        ///<para>
        ///The ID provided will be initialized to the wl_keyboard interface
        ///for this seat.
        ///</para>
        ///<para>
        ///This request only takes effect if the seat has the keyboard
        ///capability, or has had the keyboard capability in the past.
        ///It is a protocol violation to issue this request on a seat that has
        ///never had the keyboard capability. The missing_capability error will
        ///be sent in this case.
        ///</para>
        ///</Summary>
        ///<returns> seat keyboard </returns>
        public WlKeyboard GetKeyboard()
        {
            WlKeyboard wObject = connection.Create<WlKeyboard>(0, this.version);
            uint id = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.GetKeyboard, id);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "GetKeyboard", id);
            return wObject;
        }

        ///<Summary>
        ///return touch object
        ///<para>
        ///The ID provided will be initialized to the wl_touch interface
        ///for this seat.
        ///</para>
        ///<para>
        ///This request only takes effect if the seat has the touch
        ///capability, or has had the touch capability in the past.
        ///It is a protocol violation to issue this request on a seat that has
        ///never had the touch capability. The missing_capability error will
        ///be sent in this case.
        ///</para>
        ///</Summary>
        ///<returns> seat touch interface </returns>
        public WlTouch GetTouch()
        {
            WlTouch wObject = connection.Create<WlTouch>(0, this.version);
            uint id = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.GetTouch, id);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "GetTouch", id);
            return wObject;
        }

        ///<Summary>
        ///release the seat object
        ///<para>
        ///Using this request a client can tell the server that it is not going to
        ///use the seat object anymore.
        ///</para>
        ///</Summary>
        public void Release()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Release);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "Release");
        }

        public enum RequestOpcode : ushort
        {
            GetPointer,
            GetKeyboard,
            GetTouch,
            Release
        }

        ///<Summary>
        ///seat capabilities changed
        ///<para>
        ///This is emitted whenever a seat gains or loses the pointer,
        ///keyboard or touch capabilities.  The argument is a capability
        ///enum containing the complete set of capabilities this seat has.
        ///</para>
        ///<para>
        ///When the pointer capability is added, a client may create a
        ///wl_pointer object using the wl_seat.get_pointer request. This object
        ///will receive pointer events until the capability is removed in the
        ///future.
        ///</para>
        ///<para>
        ///When the pointer capability is removed, a client should destroy the
        ///wl_pointer objects associated with the seat where the capability was
        ///removed, using the wl_pointer.release request. No further pointer
        ///events will be received on these objects.
        ///</para>
        ///<para>
        ///In some compositors, if a seat regains the pointer capability and a
        ///client has a previously obtained wl_pointer object of version 4 or
        ///less, that object may start sending pointer events again. This
        ///behavior is considered a misinterpretation of the intended behavior
        ///and must not be relied upon by the client. wl_pointer objects of
        ///version 5 or later must not send events if created before the most
        ///recent event notifying the client of an added pointer capability.
        ///</para>
        ///<para>
        ///The above behavior also applies to wl_keyboard and wl_touch with the
        ///keyboard and touch capabilities, respectively.
        ///</para>
        ///</Summary>
        public Action<WlSeat, CapabilityFlag> capabilities;
        ///<Summary>
        ///unique identifier for this seat
        ///<para>
        ///In a multi-seat configuration the seat name can be used by clients to
        ///help identify which physical devices the seat represents.
        ///</para>
        ///<para>
        ///The seat name is a UTF-8 string with no convention defined for its
        ///contents. Each name is unique among all wl_seat globals. The name is
        ///only guaranteed to be unique for the current compositor instance.
        ///</para>
        ///<para>
        ///The same seat names are used for all clients. Thus, the name can be
        ///shared across processes to refer to a specific wl_seat global.
        ///</para>
        ///<para>
        ///The name event is sent after binding to the seat global. This event is
        ///only sent once per seat object, and the name does not change over the
        ///lifetime of the wl_seat global.
        ///</para>
        ///<para>
        ///Compositors may re-use the same seat name if the wl_seat global is
        ///destroyed and re-created later.
        ///</para>
        ///</Summary>
        public Action<WlSeat, string> name;
        public enum EventOpcode : ushort
        {
            Capabilities,
            Name
        }

        public override void Event(ushort opCode, WlType[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Capabilities:
                {
                    var capabilities = (CapabilityFlag)arguments[0].u;
                    if (this.capabilities != null)
                    {
                        this.capabilities.Invoke(this, capabilities);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Capabilities");
                    }

                    break;
                }

                case EventOpcode.Name:
                {
                    var name = arguments[0].s;
                    if (this.name != null)
                    {
                        this.name.Invoke(this, name);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Name", this, name);
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
                case EventOpcode.Capabilities:
                    return new WaylandType[]{WaylandType.Uint, };
                case EventOpcode.Name:
                    return new WaylandType[]{WaylandType.String, };
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }

        ///<Summary>
        ///seat capability bitmask
        ///<para>
        ///This is a bitmask of capabilities this seat has; if a member is
        ///set, then it is present on the seat.
        ///</para>
        ///</Summary>
        public enum CapabilityFlag : uint
        {
            ///<Summary>
            ///the seat has pointer devices
            ///</Summary>
            Pointer = 1,
            ///<Summary>
            ///the seat has one or more keyboards
            ///</Summary>
            Keyboard = 2,
            ///<Summary>
            ///the seat has touch devices
            ///</Summary>
            Touch = 4,
        }

        ///<Summary>
        ///wl_seat error values
        ///<para>
        ///These errors can be emitted in response to wl_seat requests.
        ///</para>
        ///</Summary>
        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///get_pointer, get_keyboard or get_touch called on seat without the matching capability
            ///</Summary>
            MissingCapability = 0,
        }
    }
}
