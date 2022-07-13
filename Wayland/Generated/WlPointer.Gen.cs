using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlPointer : WaylandObject
    {
        public const string INTERFACE = "wl_pointer";
        public WlPointer(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public void SetCursor(uint serial, WlSurface surface, int hotspot_x, int hotspot_y)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetCursor, serial, surface.id, hotspot_x, hotspot_y);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetCursor}({serial},{surface.id},{hotspot_x},{hotspot_y})");
        }

        public void Release()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Release);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Release}()");
        }

        public enum RequestOpcode : ushort
        {
            SetCursor,
            Release
        }

        public Action<WlPointer, uint, WaylandObject, double, double> enter;
        public Action<WlPointer, uint, WaylandObject> leave;
        public Action<WlPointer, uint, double, double> motion;
        public Action<WlPointer, uint, uint, uint, uint> button;
        public Action<WlPointer, uint, uint, double> axis;
        public Action<WlPointer> frame;
        public Action<WlPointer, uint> axisSource;
        public Action<WlPointer, uint, uint> axisStop;
        public Action<WlPointer, uint, int> axisDiscrete;
        public Action<WlPointer, uint, int> axisValue120;
        public enum EventOpcode : ushort
        {
            Enter,
            Leave,
            Motion,
            Button,
            Axis,
            Frame,
            AxisSource,
            AxisStop,
            AxisDiscrete,
            AxisValue120
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Enter:
                {
                    var serial = (uint)arguments[0];
                    var surface = connection[(uint)arguments[1]];
                    var surfaceX = (double)arguments[2];
                    var surfaceY = (double)arguments[3];
                    if (this.enter != null)
                    {
                        this.enter.Invoke(this, serial, surface, surfaceX, surfaceY);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Enter}({this},{serial},{surface},{surfaceX},{surfaceY})");
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

                case EventOpcode.Motion:
                {
                    var time = (uint)arguments[0];
                    var surfaceX = (double)arguments[1];
                    var surfaceY = (double)arguments[2];
                    if (this.motion != null)
                    {
                        this.motion.Invoke(this, time, surfaceX, surfaceY);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Motion}({this},{time},{surfaceX},{surfaceY})");
                    }

                    break;
                }

                case EventOpcode.Button:
                {
                    var serial = (uint)arguments[0];
                    var time = (uint)arguments[1];
                    var button = (uint)arguments[2];
                    var state = (uint)arguments[3];
                    if (this.button != null)
                    {
                        this.button.Invoke(this, serial, time, button, state);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Button}({this},{serial},{time},{button},{state})");
                    }

                    break;
                }

                case EventOpcode.Axis:
                {
                    var time = (uint)arguments[0];
                    var axis = (uint)arguments[1];
                    var value = (double)arguments[2];
                    if (this.axis != null)
                    {
                        this.axis.Invoke(this, time, axis, value);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Axis}({this},{time},{axis},{value})");
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

                case EventOpcode.AxisSource:
                {
                    var axisSource = (uint)arguments[0];
                    if (this.axisSource != null)
                    {
                        this.axisSource.Invoke(this, axisSource);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.AxisSource}({this},{axisSource})");
                    }

                    break;
                }

                case EventOpcode.AxisStop:
                {
                    var time = (uint)arguments[0];
                    var axis = (uint)arguments[1];
                    if (this.axisStop != null)
                    {
                        this.axisStop.Invoke(this, time, axis);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.AxisStop}({this},{time},{axis})");
                    }

                    break;
                }

                case EventOpcode.AxisDiscrete:
                {
                    var axis = (uint)arguments[0];
                    var discrete = (int)arguments[1];
                    if (this.axisDiscrete != null)
                    {
                        this.axisDiscrete.Invoke(this, axis, discrete);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.AxisDiscrete}({this},{axis},{discrete})");
                    }

                    break;
                }

                case EventOpcode.AxisValue120:
                {
                    var axis = (uint)arguments[0];
                    var value120 = (int)arguments[1];
                    if (this.axisValue120 != null)
                    {
                        this.axisValue120.Invoke(this, axis, value120);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.AxisValue120}({this},{axis},{value120})");
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
                case EventOpcode.Enter:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Object, WaylandType.Fixed, WaylandType.Fixed, };
                case EventOpcode.Leave:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Object, };
                case EventOpcode.Motion:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Fixed, WaylandType.Fixed, };
                case EventOpcode.Button:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Uint, WaylandType.Uint, WaylandType.Uint, };
                case EventOpcode.Axis:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Uint, WaylandType.Fixed, };
                case EventOpcode.Frame:
                    return new WaylandType[]{};
                case EventOpcode.AxisSource:
                    return new WaylandType[]{WaylandType.Uint, };
                case EventOpcode.AxisStop:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Uint, };
                case EventOpcode.AxisDiscrete:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Int, };
                case EventOpcode.AxisValue120:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Int, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
