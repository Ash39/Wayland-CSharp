using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///pointer input device
    ///<para>
    ///The wl_pointer interface represents one or more input devices,
    ///such as mice, which control the pointer location and pointer_focus
    ///of a seat.
    ///</para>
    ///<para>
    ///The wl_pointer interface generates motion, enter and leave
    ///events for the surfaces that the pointer is located over,
    ///and button and axis events for button presses, button releases
    ///and scrolling.
    ///</para>
    ///</Summary>
    public partial class WlPointer : WaylandObject
    {
        public const string INTERFACE = "wl_pointer";
        public WlPointer(uint factoryId, ref uint id, WaylandConnection connection, uint version = 8) : base(factoryId, ref id, version, connection)
        {
        }

        ///<Summary>
        ///set the pointer surface
        ///<para>
        ///Set the pointer surface, i.e., the surface that contains the
        ///pointer image (cursor). This request gives the surface the role
        ///of a cursor. If the surface already has another role, it raises
        ///a protocol error.
        ///</para>
        ///<para>
        ///The cursor actually changes only if the pointer
        ///focus for this device is one of the requesting client's surfaces
        ///or the surface parameter is the current pointer surface. If
        ///there was a previous surface set with this request it is
        ///replaced. If surface is NULL, the pointer image is hidden.
        ///</para>
        ///<para>
        ///The parameters hotspot_x and hotspot_y define the position of
        ///the pointer surface relative to the pointer location. Its
        ///top-left corner is always at (x, y) - (hotspot_x, hotspot_y),
        ///where (x, y) are the coordinates of the pointer location, in
        ///surface-local coordinates.
        ///</para>
        ///<para>
        ///On surface.attach requests to the pointer surface, hotspot_x
        ///and hotspot_y are decremented by the x and y parameters
        ///passed to the request. Attach must be confirmed by
        ///wl_surface.commit as usual.
        ///</para>
        ///<para>
        ///The hotspot can also be updated by passing the currently set
        ///pointer surface to this request with new values for hotspot_x
        ///and hotspot_y.
        ///</para>
        ///<para>
        ///The current and pending input regions of the wl_surface are
        ///cleared, and wl_surface.set_input_region is ignored until the
        ///wl_surface is no longer used as the cursor. When the use as a
        ///cursor ends, the current and pending input regions become
        ///undefined, and the wl_surface is unmapped.
        ///</para>
        ///<para>
        ///The serial parameter must match the latest wl_pointer.enter
        ///serial number sent to the client. Otherwise the request will be
        ///ignored.
        ///</para>
        ///</Summary>
        ///<param name = "serial"> serial number of the enter event </param>
        ///<param name = "surface"> pointer surface </param>
        ///<param name = "hotspot_x"> surface-local x coordinate </param>
        ///<param name = "hotspot_y"> surface-local y coordinate </param>
        public void SetCursor(uint serial, WlSurface surface, int hotspot_x, int hotspot_y)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetCursor, serial, surface.id, hotspot_x, hotspot_y);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetCursor}({serial},{surface.id},{hotspot_x},{hotspot_y})");
        }

        ///<Summary>
        ///release the pointer object
        ///<para>
        ///Using this request a client can tell the server that it is not going to
        ///use the pointer object anymore.
        ///</para>
        ///<para>
        ///This request destroys the pointer proxy object, so clients must not call
        ///wl_pointer_destroy() after using this request.
        ///</para>
        ///</Summary>
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

        ///<Summary>
        ///enter event
        ///<para>
        ///Notification that this seat's pointer is focused on a certain
        ///surface.
        ///</para>
        ///<para>
        ///When a seat's focus enters a surface, the pointer image
        ///is undefined and a client should respond to this event by setting
        ///an appropriate pointer image with the set_cursor request.
        ///</para>
        ///</Summary>
        public Action<WlPointer, uint, WaylandObject, double, double> enter;
        ///<Summary>
        ///leave event
        ///<para>
        ///Notification that this seat's pointer is no longer focused on
        ///a certain surface.
        ///</para>
        ///<para>
        ///The leave notification is sent before the enter notification
        ///for the new focus.
        ///</para>
        ///</Summary>
        public Action<WlPointer, uint, WaylandObject> leave;
        ///<Summary>
        ///pointer motion event
        ///<para>
        ///Notification of pointer location change. The arguments
        ///surface_x and surface_y are the location relative to the
        ///focused surface.
        ///</para>
        ///</Summary>
        public Action<WlPointer, uint, double, double> motion;
        ///<Summary>
        ///pointer button event
        ///<para>
        ///Mouse button click and release notifications.
        ///</para>
        ///<para>
        ///The location of the click is given by the last motion or
        ///enter event.
        ///The time argument is a timestamp with millisecond
        ///granularity, with an undefined base.
        ///</para>
        ///<para>
        ///The button is a button code as defined in the Linux kernel's
        ///linux/input-event-codes.h header file, e.g. BTN_LEFT.
        ///</para>
        ///<para>
        ///Any 16-bit button code value is reserved for future additions to the
        ///kernel's event code list. All other button codes above 0xFFFF are
        ///currently undefined but may be used in future versions of this
        ///protocol.
        ///</para>
        ///</Summary>
        public Action<WlPointer, uint, uint, uint, ButtonStateFlag> button;
        ///<Summary>
        ///axis event
        ///<para>
        ///Scroll and other axis notifications.
        ///</para>
        ///<para>
        ///For scroll events (vertical and horizontal scroll axes), the
        ///value parameter is the length of a vector along the specified
        ///axis in a coordinate space identical to those of motion events,
        ///representing a relative movement along the specified axis.
        ///</para>
        ///<para>
        ///For devices that support movements non-parallel to axes multiple
        ///axis events will be emitted.
        ///</para>
        ///<para>
        ///When applicable, for example for touch pads, the server can
        ///choose to emit scroll events where the motion vector is
        ///equivalent to a motion event vector.
        ///</para>
        ///<para>
        ///When applicable, a client can transform its content relative to the
        ///scroll distance.
        ///</para>
        ///</Summary>
        public Action<WlPointer, uint, AxisFlag, double> axis;
        ///<Summary>
        ///end of a pointer event sequence
        ///<para>
        ///Indicates the end of a set of events that logically belong together.
        ///A client is expected to accumulate the data in all events within the
        ///frame before proceeding.
        ///</para>
        ///<para>
        ///All wl_pointer events before a wl_pointer.frame event belong
        ///logically together. For example, in a diagonal scroll motion the
        ///compositor will send an optional wl_pointer.axis_source event, two
        ///wl_pointer.axis events (horizontal and vertical) and finally a
        ///wl_pointer.frame event. The client may use this information to
        ///calculate a diagonal vector for scrolling.
        ///</para>
        ///<para>
        ///When multiple wl_pointer.axis events occur within the same frame,
        ///the motion vector is the combined motion of all events.
        ///When a wl_pointer.axis and a wl_pointer.axis_stop event occur within
        ///the same frame, this indicates that axis movement in one axis has
        ///stopped but continues in the other axis.
        ///When multiple wl_pointer.axis_stop events occur within the same
        ///frame, this indicates that these axes stopped in the same instance.
        ///</para>
        ///<para>
        ///A wl_pointer.frame event is sent for every logical event group,
        ///even if the group only contains a single wl_pointer event.
        ///Specifically, a client may get a sequence: motion, frame, button,
        ///frame, axis, frame, axis_stop, frame.
        ///</para>
        ///<para>
        ///The wl_pointer.enter and wl_pointer.leave events are logical events
        ///generated by the compositor and not the hardware. These events are
        ///also grouped by a wl_pointer.frame. When a pointer moves from one
        ///surface to another, a compositor should group the
        ///wl_pointer.leave event within the same wl_pointer.frame.
        ///However, a client must not rely on wl_pointer.leave and
        ///wl_pointer.enter being in the same wl_pointer.frame.
        ///Compositor-specific policies may require the wl_pointer.leave and
        ///wl_pointer.enter event being split across multiple wl_pointer.frame
        ///groups.
        ///</para>
        ///</Summary>
        public Action<WlPointer> frame;
        ///<Summary>
        ///axis source event
        ///<para>
        ///Source information for scroll and other axes.
        ///</para>
        ///<para>
        ///This event does not occur on its own. It is sent before a
        ///wl_pointer.frame event and carries the source information for
        ///all events within that frame.
        ///</para>
        ///<para>
        ///The source specifies how this event was generated. If the source is
        ///wl_pointer.axis_source.finger, a wl_pointer.axis_stop event will be
        ///sent when the user lifts the finger off the device.
        ///</para>
        ///<para>
        ///If the source is wl_pointer.axis_source.wheel,
        ///wl_pointer.axis_source.wheel_tilt or
        ///wl_pointer.axis_source.continuous, a wl_pointer.axis_stop event may
        ///or may not be sent. Whether a compositor sends an axis_stop event
        ///for these sources is hardware-specific and implementation-dependent;
        ///clients must not rely on receiving an axis_stop event for these
        ///scroll sources and should treat scroll sequences from these scroll
        ///sources as unterminated by default.
        ///</para>
        ///<para>
        ///This event is optional. If the source is unknown for a particular
        ///axis event sequence, no event is sent.
        ///Only one wl_pointer.axis_source event is permitted per frame.
        ///</para>
        ///<para>
        ///The order of wl_pointer.axis_discrete and wl_pointer.axis_source is
        ///not guaranteed.
        ///</para>
        ///</Summary>
        public Action<WlPointer, AxisSourceFlag> axisSource;
        ///<Summary>
        ///axis stop event
        ///<para>
        ///Stop notification for scroll and other axes.
        ///</para>
        ///<para>
        ///For some wl_pointer.axis_source types, a wl_pointer.axis_stop event
        ///is sent to notify a client that the axis sequence has terminated.
        ///This enables the client to implement kinetic scrolling.
        ///See the wl_pointer.axis_source documentation for information on when
        ///this event may be generated.
        ///</para>
        ///<para>
        ///Any wl_pointer.axis events with the same axis_source after this
        ///event should be considered as the start of a new axis motion.
        ///</para>
        ///<para>
        ///The timestamp is to be interpreted identical to the timestamp in the
        ///wl_pointer.axis event. The timestamp value may be the same as a
        ///preceding wl_pointer.axis event.
        ///</para>
        ///</Summary>
        public Action<WlPointer, uint, AxisFlag> axisStop;
        ///<Summary>
        ///axis click event
        ///<para>
        ///Discrete step information for scroll and other axes.
        ///</para>
        ///<para>
        ///This event carries the axis value of the wl_pointer.axis event in
        ///discrete steps (e.g. mouse wheel clicks).
        ///</para>
        ///<para>
        ///This event is deprecated with wl_pointer version 8 - this event is not
        ///sent to clients supporting version 8 or later.
        ///</para>
        ///<para>
        ///This event does not occur on its own, it is coupled with a
        ///wl_pointer.axis event that represents this axis value on a
        ///continuous scale. The protocol guarantees that each axis_discrete
        ///event is always followed by exactly one axis event with the same
        ///axis number within the same wl_pointer.frame. Note that the protocol
        ///allows for other events to occur between the axis_discrete and
        ///its coupled axis event, including other axis_discrete or axis
        ///events. A wl_pointer.frame must not contain more than one axis_discrete
        ///event per axis type.
        ///</para>
        ///<para>
        ///This event is optional; continuous scrolling devices
        ///like two-finger scrolling on touchpads do not have discrete
        ///steps and do not generate this event.
        ///</para>
        ///<para>
        ///The discrete value carries the directional information. e.g. a value
        ///of -2 is two steps towards the negative direction of this axis.
        ///</para>
        ///<para>
        ///The axis number is identical to the axis number in the associated
        ///axis event.
        ///</para>
        ///<para>
        ///The order of wl_pointer.axis_discrete and wl_pointer.axis_source is
        ///not guaranteed.
        ///</para>
        ///</Summary>
        public Action<WlPointer, AxisFlag, int> axisDiscrete;
        ///<Summary>
        ///axis high-resolution scroll event
        ///<para>
        ///Discrete high-resolution scroll information.
        ///</para>
        ///<para>
        ///This event carries high-resolution wheel scroll information,
        ///with each multiple of 120 representing one logical scroll step
        ///(a wheel detent). For example, an axis_value120 of 30 is one quarter of
        ///a logical scroll step in the positive direction, a value120 of
        ///-240 are two logical scroll steps in the negative direction within the
        ///same hardware event.
        ///Clients that rely on discrete scrolling should accumulate the
        ///value120 to multiples of 120 before processing the event.
        ///</para>
        ///<para>
        ///The value120 must not be zero.
        ///</para>
        ///<para>
        ///This event replaces the wl_pointer.axis_discrete event in clients
        ///supporting wl_pointer version 8 or later.
        ///</para>
        ///<para>
        ///Where a wl_pointer.axis_source event occurs in the same
        ///wl_pointer.frame, the axis source applies to this event.
        ///</para>
        ///<para>
        ///The order of wl_pointer.axis_value120 and wl_pointer.axis_source is
        ///not guaranteed.
        ///</para>
        ///</Summary>
        public Action<WlPointer, AxisFlag, int> axisValue120;
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
                    var state = (ButtonStateFlag)arguments[3];
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
                    var axis = (AxisFlag)arguments[1];
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
                    var axisSource = (AxisSourceFlag)arguments[0];
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
                    var axis = (AxisFlag)arguments[1];
                    if (this.axisStop != null)
                    {
                        this.axisStop.Invoke(this, time, axis);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.AxisStop}({this},{time},{axis})");
                    }

                    break;
                }

                case EventOpcode.AxisDiscrete:
                {
                    var axis = (AxisFlag)arguments[0];
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
                    var axis = (AxisFlag)arguments[0];
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

        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///given wl_surface has another role
            ///</Summary>
            Role = 0,
        }

        ///<Summary>
        ///physical button state
        ///<para>
        ///Describes the physical state of a button that produced the button
        ///event.
        ///</para>
        ///</Summary>
        public enum ButtonStateFlag : uint
        {
            ///<Summary>
            ///the button is not pressed
            ///</Summary>
            Released = 0,
            ///<Summary>
            ///the button is pressed
            ///</Summary>
            Pressed = 1,
        }

        ///<Summary>
        ///axis types
        ///<para>
        ///Describes the axis types of scroll events.
        ///</para>
        ///</Summary>
        public enum AxisFlag : uint
        {
            ///<Summary>
            ///vertical axis
            ///</Summary>
            VerticalScroll = 0,
            ///<Summary>
            ///horizontal axis
            ///</Summary>
            HorizontalScroll = 1,
        }

        ///<Summary>
        ///axis source types
        ///<para>
        ///Describes the source types for axis events. This indicates to the
        ///client how an axis event was physically generated; a client may
        ///adjust the user interface accordingly. For example, scroll events
        ///from a "finger" source may be in a smooth coordinate space with
        ///kinetic scrolling whereas a "wheel" source may be in discrete steps
        ///of a number of lines.
        ///</para>
        ///<para>
        ///The "continuous" axis source is a device generating events in a
        ///continuous coordinate space, but using something other than a
        ///finger. One example for this source is button-based scrolling where
        ///the vertical motion of a device is converted to scroll events while
        ///a button is held down.
        ///</para>
        ///<para>
        ///The "wheel tilt" axis source indicates that the actual device is a
        ///wheel but the scroll event is not caused by a rotation but a
        ///(usually sideways) tilt of the wheel.
        ///</para>
        ///</Summary>
        public enum AxisSourceFlag : uint
        {
            ///<Summary>
            ///a physical wheel rotation
            ///</Summary>
            Wheel = 0,
            ///<Summary>
            ///finger on a touch surface
            ///</Summary>
            Finger = 1,
            ///<Summary>
            ///continuous coordinate space
            ///</Summary>
            Continuous = 2,
            ///<Summary>
            ///a physical wheel tilt
            ///</Summary>
            WheelTilt = 3,
        }
    }
}
