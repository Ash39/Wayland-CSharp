using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlKeyboard : WaylandObject
    {
        public const string INTERFACE = "wl_keyboard";
        public WlKeyboard(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public void Release()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Release);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Release}()");
        }

        public enum RequestOpcode : ushort
        {
            Release
        }

        public Action<WlKeyboard, uint, IntPtr, uint> keymap;
        public Action<WlKeyboard, uint, WaylandObject, byte[]> enter;
        public Action<WlKeyboard, uint, WaylandObject> leave;
        public Action<WlKeyboard, uint, uint, uint, uint> key;
        public Action<WlKeyboard, uint, uint, uint, uint, uint> modifiers;
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
                    var format = (uint)arguments[0];
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
                    var state = (uint)arguments[3];
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
    }
}
