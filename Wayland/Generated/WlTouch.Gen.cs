using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// touchscreen input device
    /// </summary>
    public partial class WlTouch : WaylandObject
    {
        public const string INTERFACE = "wl_touch";
        public WlTouch(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// release the touch object
        /// </summary>
        public void Release()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Release);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Release}()");
        }

        public enum RequestOpcode : ushort
        {
            Release
        }

        public Action<WlTouch, uint, uint, WaylandObject, int, double, double> down;
        public Action<WlTouch, uint, uint, int> up;
        public Action<WlTouch, uint, int, double, double> motion;
        public Action<WlTouch> frame;
        public Action<WlTouch> cancel;
        public Action<WlTouch, int, double, double> shape;
        public Action<WlTouch, int, double> orientation;
        public enum EventOpcode : ushort
        {
            Down,
            Up,
            Motion,
            Frame,
            Cancel,
            Shape,
            Orientation
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Down:
                {
                    var serial = (uint)arguments[0];
                    var time = (uint)arguments[1];
                    var surface = connection[(uint)arguments[2]];
                    var id = (int)arguments[3];
                    var x = (double)arguments[4];
                    var y = (double)arguments[5];
                    if (this.down != null)
                    {
                        this.down.Invoke(this, serial, time, surface, id, x, y);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Down}({this},{serial},{time},{surface},{id},{x},{y})");
                    }

                    break;
                }

                case EventOpcode.Up:
                {
                    var serial = (uint)arguments[0];
                    var time = (uint)arguments[1];
                    var id = (int)arguments[2];
                    if (this.up != null)
                    {
                        this.up.Invoke(this, serial, time, id);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Up}({this},{serial},{time},{id})");
                    }

                    break;
                }

                case EventOpcode.Motion:
                {
                    var time = (uint)arguments[0];
                    var id = (int)arguments[1];
                    var x = (double)arguments[2];
                    var y = (double)arguments[3];
                    if (this.motion != null)
                    {
                        this.motion.Invoke(this, time, id, x, y);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Motion}({this},{time},{id},{x},{y})");
                    }

                    break;
                }

                case EventOpcode.Frame:
                {
                    if (this.frame != null)
                    {
                        this.frame.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Frame}({this})");
                    }

                    break;
                }

                case EventOpcode.Cancel:
                {
                    if (this.cancel != null)
                    {
                        this.cancel.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Cancel}({this})");
                    }

                    break;
                }

                case EventOpcode.Shape:
                {
                    var id = (int)arguments[0];
                    var major = (double)arguments[1];
                    var minor = (double)arguments[2];
                    if (this.shape != null)
                    {
                        this.shape.Invoke(this, id, major, minor);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Shape}({this},{id},{major},{minor})");
                    }

                    break;
                }

                case EventOpcode.Orientation:
                {
                    var id = (int)arguments[0];
                    var orientation = (double)arguments[1];
                    if (this.orientation != null)
                    {
                        this.orientation.Invoke(this, id, orientation);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Orientation}({this},{id},{orientation})");
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
                case EventOpcode.Down:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Uint, WaylandType.Object, WaylandType.Int, WaylandType.Fixed, WaylandType.Fixed, };
                case EventOpcode.Up:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Uint, WaylandType.Int, };
                case EventOpcode.Motion:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Int, WaylandType.Fixed, WaylandType.Fixed, };
                case EventOpcode.Frame:
                    return new WaylandType[]{};
                case EventOpcode.Cancel:
                    return new WaylandType[]{};
                case EventOpcode.Shape:
                    return new WaylandType[]{WaylandType.Int, WaylandType.Fixed, WaylandType.Fixed, };
                case EventOpcode.Orientation:
                    return new WaylandType[]{WaylandType.Int, WaylandType.Fixed, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
