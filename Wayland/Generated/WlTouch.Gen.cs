using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///touchscreen input device
    ///<para>
    ///The wl_touch interface represents a touchscreen
    ///associated with a seat.
    ///</para>
    ///<para>
    ///Touch interactions can consist of one or more contacts.
    ///For each contact, a series of events is generated, starting
    ///with a down event, followed by zero or more motion events,
    ///and ending with an up event. Events relating to the same
    ///contact point can be identified by the ID of the sequence.
    ///</para>
    ///</Summary>
    public partial class WlTouch : WaylandObject
    {
        public const string INTERFACE = "wl_touch";
        public WlTouch(uint factoryId, ref uint id, WaylandConnection connection) : base(factoryId, ref id, 8, connection)
        {
        }

        ///<Summary>
        ///release the touch object
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
        ///touch down event and beginning of a touch sequence
        ///<para>
        ///A new touch point has appeared on the surface. This touch point is
        ///assigned a unique ID. Future events from this touch point reference
        ///this ID. The ID ceases to be valid after a touch up event and may be
        ///reused in the future.
        ///</para>
        ///</Summary>
        public Action<WlTouch, uint, uint, WaylandObject, int, double, double> down;
        ///<Summary>
        ///end of a touch event sequence
        ///<para>
        ///The touch point has disappeared. No further events will be sent for
        ///this touch point and the touch point's ID is released and may be
        ///reused in a future touch down event.
        ///</para>
        ///</Summary>
        public Action<WlTouch, uint, uint, int> up;
        ///<Summary>
        ///update of touch point coordinates
        ///<para>
        ///A touch point has changed coordinates.
        ///</para>
        ///</Summary>
        public Action<WlTouch, uint, int, double, double> motion;
        ///<Summary>
        ///end of touch frame event
        ///<para>
        ///Indicates the end of a set of events that logically belong together.
        ///A client is expected to accumulate the data in all events within the
        ///frame before proceeding.
        ///</para>
        ///<para>
        ///A wl_touch.frame terminates at least one event but otherwise no
        ///guarantee is provided about the set of events within a frame. A client
        ///must assume that any state not updated in a frame is unchanged from the
        ///previously known state.
        ///</para>
        ///</Summary>
        public Action<WlTouch> frame;
        ///<Summary>
        ///touch session cancelled
        ///<para>
        ///Sent if the compositor decides the touch stream is a global
        ///gesture. No further events are sent to the clients from that
        ///particular gesture. Touch cancellation applies to all touch points
        ///currently active on this client's surface. The client is
        ///responsible for finalizing the touch points, future touch points on
        ///this surface may reuse the touch point ID.
        ///</para>
        ///</Summary>
        public Action<WlTouch> cancel;
        ///<Summary>
        ///update shape of touch point
        ///<para>
        ///Sent when a touchpoint has changed its shape.
        ///</para>
        ///<para>
        ///This event does not occur on its own. It is sent before a
        ///wl_touch.frame event and carries the new shape information for
        ///any previously reported, or new touch points of that frame.
        ///</para>
        ///<para>
        ///Other events describing the touch point such as wl_touch.down,
        ///wl_touch.motion or wl_touch.orientation may be sent within the
        ///same wl_touch.frame. A client should treat these events as a single
        ///logical touch point update. The order of wl_touch.shape,
        ///wl_touch.orientation and wl_touch.motion is not guaranteed.
        ///A wl_touch.down event is guaranteed to occur before the first
        ///wl_touch.shape event for this touch ID but both events may occur within
        ///the same wl_touch.frame.
        ///</para>
        ///<para>
        ///A touchpoint shape is approximated by an ellipse through the major and
        ///minor axis length. The major axis length describes the longer diameter
        ///of the ellipse, while the minor axis length describes the shorter
        ///diameter. Major and minor are orthogonal and both are specified in
        ///surface-local coordinates. The center of the ellipse is always at the
        ///touchpoint location as reported by wl_touch.down or wl_touch.move.
        ///</para>
        ///<para>
        ///This event is only sent by the compositor if the touch device supports
        ///shape reports. The client has to make reasonable assumptions about the
        ///shape if it did not receive this event.
        ///</para>
        ///</Summary>
        public Action<WlTouch, int, double, double> shape;
        ///<Summary>
        ///update orientation of touch point
        ///<para>
        ///Sent when a touchpoint has changed its orientation.
        ///</para>
        ///<para>
        ///This event does not occur on its own. It is sent before a
        ///wl_touch.frame event and carries the new shape information for
        ///any previously reported, or new touch points of that frame.
        ///</para>
        ///<para>
        ///Other events describing the touch point such as wl_touch.down,
        ///wl_touch.motion or wl_touch.shape may be sent within the
        ///same wl_touch.frame. A client should treat these events as a single
        ///logical touch point update. The order of wl_touch.shape,
        ///wl_touch.orientation and wl_touch.motion is not guaranteed.
        ///A wl_touch.down event is guaranteed to occur before the first
        ///wl_touch.orientation event for this touch ID but both events may occur
        ///within the same wl_touch.frame.
        ///</para>
        ///<para>
        ///The orientation describes the clockwise angle of a touchpoint's major
        ///axis to the positive surface y-axis and is normalized to the -180 to
        ///+180 degree range. The granularity of orientation depends on the touch
        ///device, some devices only support binary rotation values between 0 and
        ///90 degrees.
        ///</para>
        ///<para>
        ///This event is only sent by the compositor if the touch device supports
        ///orientation reports.
        ///</para>
        ///</Summary>
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
