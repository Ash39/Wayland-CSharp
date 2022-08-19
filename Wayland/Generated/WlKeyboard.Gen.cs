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
        public WlKeyboard(uint factoryId, ref uint id, WaylandConnection connection, uint version = 8) : base(factoryId, ref id, version, connection)
        {
        }

        ///<Summary>
        ///release the keyboard object
        ///</Summary>
        public void Release()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Release);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Release}()");
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

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Keymap:
                {
                    var format = (KeymapFormatFlag)arguments[0];
                    var fd = (IntPtr)arguments[1];
                    var size = (uint)arguments[2];
                    if (this.keymap != null)
                    {
                        this.keymap.Invoke(this, format, fd, size);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Keymap}({this},{format},{fd},{size})");
                    }

                    break;
                }

                case EventOpcode.Enter:
                {
                    var serial = (uint)arguments[0];
                    var surface = connection[(uint)arguments[1]];
                    var keys = (byte[])arguments[2];
                    if (this.enter != null)
                    {
                        this.enter.Invoke(this, serial, surface, keys);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Enter}({this},{serial},{surface},{keys})");
                    }

                    break;
                }

                case EventOpcode.Leave:
                {
                    var serial = (uint)arguments[0];
                    var surface = connection[(uint)arguments[1]];
                    if (this.leave != null)
                    {
                        this.leave.Invoke(this, serial, surface);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Leave}({this},{serial},{surface})");
                    }

                    break;
                }

                case EventOpcode.Key:
                {
                    var serial = (uint)arguments[0];
                    var time = (uint)arguments[1];
                    var key = (uint)arguments[2];
                    var state = (KeyStateFlag)arguments[3];
                    if (this.key != null)
                    {
                        this.key.Invoke(this, serial, time, key, state);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Key}({this},{serial},{time},{key},{state})");
                    }

                    break;
                }

                case EventOpcode.Modifiers:
                {
                    var serial = (uint)arguments[0];
                    var modsDepressed = (uint)arguments[1];
                    var modsLatched = (uint)arguments[2];
                    var modsLocked = (uint)arguments[3];
                    var group = (uint)arguments[4];
                    if (this.modifiers != null)
                    {
                        this.modifiers.Invoke(this, serial, modsDepressed, modsLatched, modsLocked, group);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Modifiers}({this},{serial},{modsDepressed},{modsLatched},{modsLocked},{group})");
                    }

                    break;
                }

                case EventOpcode.RepeatInfo:
                {
                    var rate = (int)arguments[0];
                    var delay = (int)arguments[1];
                    if (this.repeatInfo != null)
                    {
                        this.repeatInfo.Invoke(this, rate, delay);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.RepeatInfo}({this},{rate},{delay})");
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
                    throw new ArgumentOutOfRangeException("unknown event");
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
