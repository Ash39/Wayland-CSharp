using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///keyboard input device
    ///<para>
    ///The wl_keyboard interface represents one or more keyboards
    ///associated with a seat.
    ///</para>
    ///</Summary>
    public partial class WlKeyboard : WaylandObject
    {
        public const string INTERFACE = "wl_keyboard";
        public WlKeyboard(uint id, WaylandConnection connection, uint version = 8) : base(id, version, connection)
        {
        }

        ///<Summary>
        ///release the keyboard object
        ///</Summary>
        public void Release()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Release);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "Release");
        }

        public enum RequestOpcode : ushort
        {
            Release
        }

        ///<Summary>
        ///keyboard mapping
        ///<para>
        ///This event provides a file descriptor to the client which can be
        ///memory-mapped in read-only mode to provide a keyboard mapping
        ///description.
        ///</para>
        ///<para>
        ///From version 7 onwards, the fd must be mapped with MAP_PRIVATE by
        ///the recipient, as MAP_SHARED may fail.
        ///</para>
        ///</Summary>
        public Action<WlKeyboard, KeymapFormatFlag, IntPtr, uint> keymap;
        ///<Summary>
        ///enter event
        ///<para>
        ///Notification that this seat's keyboard focus is on a certain
        ///surface.
        ///</para>
        ///<para>
        ///The compositor must send the wl_keyboard.modifiers event after this
        ///event.
        ///</para>
        ///</Summary>
        public Action<WlKeyboard, uint, WaylandObject, byte[]> enter;
        ///<Summary>
        ///leave event
        ///<para>
        ///Notification that this seat's keyboard focus is no longer on
        ///a certain surface.
        ///</para>
        ///<para>
        ///The leave notification is sent before the enter notification
        ///for the new focus.
        ///</para>
        ///<para>
        ///After this event client must assume that all keys, including modifiers,
        ///are lifted and also it must stop key repeating if there's some going on.
        ///</para>
        ///</Summary>
        public Action<WlKeyboard, uint, WaylandObject> leave;
        ///<Summary>
        ///key event
        ///<para>
        ///A key was pressed or released.
        ///The time argument is a timestamp with millisecond
        ///granularity, with an undefined base.
        ///</para>
        ///<para>
        ///The key is a platform-specific key code that can be interpreted
        ///by feeding it to the keyboard mapping (see the keymap event).
        ///</para>
        ///<para>
        ///If this event produces a change in modifiers, then the resulting
        ///wl_keyboard.modifiers event must be sent after this event.
        ///</para>
        ///</Summary>
        public Action<WlKeyboard, uint, uint, uint, KeyStateFlag> key;
        ///<Summary>
        ///modifier and group state
        ///<para>
        ///Notifies clients that the modifier and/or group state has
        ///changed, and it should update its local state.
        ///</para>
        ///</Summary>
        public Action<WlKeyboard, uint, uint, uint, uint, uint> modifiers;
        ///<Summary>
        ///repeat rate and delay
        ///<para>
        ///Informs the client about the keyboard's repeat rate and delay.
        ///</para>
        ///<para>
        ///This event is sent as soon as the wl_keyboard object has been created,
        ///and is guaranteed to be received by the client before any key press
        ///event.
        ///</para>
        ///<para>
        ///Negative values for either rate or delay are illegal. A rate of zero
        ///will disable any repeating (regardless of the value of delay).
        ///</para>
        ///<para>
        ///This event can be sent later on as well with a new value if necessary,
        ///so clients should continue listening for the event past the creation
        ///of wl_keyboard.
        ///</para>
        ///</Summary>
        public Action<WlKeyboard, int, int> repeatInfo;
        public enum EventOpcode : ushort
        {
            Keymap,
            Enter,
            Leave,
            Key,
            Modifiers,
            RepeatInfo
        }

        public override void Event(ushort opCode, WlType[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Keymap:
                {
                    var format = (KeymapFormatFlag)arguments[0].u;
                    var fd = arguments[1].p;
                    var size = arguments[2].u;
                    if (this.keymap != null)
                    {
                        this.keymap.Invoke(this, format, fd, size);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Keymap");
                    }

                    break;
                }

                case EventOpcode.Enter:
                {
                    var serial = arguments[0].u;
                    var surface = connection[arguments[1].u];
                    var keys = arguments[2].b;
                    if (this.enter != null)
                    {
                        this.enter.Invoke(this, serial, surface, keys);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Enter", this, serial, surface, keys);
                    }

                    break;
                }

                case EventOpcode.Leave:
                {
                    var serial = arguments[0].u;
                    var surface = connection[arguments[1].u];
                    if (this.leave != null)
                    {
                        this.leave.Invoke(this, serial, surface);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Leave", this, serial, surface);
                    }

                    break;
                }

                case EventOpcode.Key:
                {
                    var serial = arguments[0].u;
                    var time = arguments[1].u;
                    var key = arguments[2].u;
                    var state = (KeyStateFlag)arguments[3].u;
                    if (this.key != null)
                    {
                        this.key.Invoke(this, serial, time, key, state);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Key", this, serial, time, key, state);
                    }

                    break;
                }

                case EventOpcode.Modifiers:
                {
                    var serial = arguments[0].u;
                    var modsDepressed = arguments[1].u;
                    var modsLatched = arguments[2].u;
                    var modsLocked = arguments[3].u;
                    var group = arguments[4].u;
                    if (this.modifiers != null)
                    {
                        this.modifiers.Invoke(this, serial, modsDepressed, modsLatched, modsLocked, group);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Modifiers", this, serial, modsDepressed, modsLatched, modsLocked, group);
                    }

                    break;
                }

                case EventOpcode.RepeatInfo:
                {
                    var rate = arguments[0].i;
                    var delay = arguments[1].i;
                    if (this.repeatInfo != null)
                    {
                        this.repeatInfo.Invoke(this, rate, delay);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "RepeatInfo", this, rate, delay);
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
                case EventOpcode.Keymap:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Fd, WaylandType.Uint, };
                case EventOpcode.Enter:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Object, WaylandType.Array, };
                case EventOpcode.Leave:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Object, };
                case EventOpcode.Key:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Uint, WaylandType.Uint, WaylandType.Uint, };
                case EventOpcode.Modifiers:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Uint, WaylandType.Uint, WaylandType.Uint, WaylandType.Uint, };
                case EventOpcode.RepeatInfo:
                    return new WaylandType[]{WaylandType.Int, WaylandType.Int, };
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }

        ///<Summary>
        ///keyboard mapping format
        ///<para>
        ///This specifies the format of the keymap provided to the
        ///client with the wl_keyboard.keymap event.
        ///</para>
        ///</Summary>
        public enum KeymapFormatFlag : uint
        {
            ///<Summary>
            ///no keymap; client must understand how to interpret the raw keycode
            ///</Summary>
            NoKeymap = 0,
            ///<Summary>
            ///libxkbcommon compatible, null-terminated string; to determine the xkb keycode, clients must add 8 to the key event keycode
            ///</Summary>
            XkbV1 = 1,
        }

        ///<Summary>
        ///physical key state
        ///<para>
        ///Describes the physical state of a key that produced the key event.
        ///</para>
        ///</Summary>
        public enum KeyStateFlag : uint
        {
            ///<Summary>
            ///key is not pressed
            ///</Summary>
            Released = 0,
            ///<Summary>
            ///key is pressed
            ///</Summary>
            Pressed = 1,
        }
    }
}
