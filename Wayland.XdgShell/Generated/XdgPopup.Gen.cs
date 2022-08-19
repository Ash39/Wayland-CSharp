using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///short-lived, popup surfaces for menus
    ///<para>
    ///A popup surface is a short-lived, temporary surface. It can be used to
    ///implement for example menus, popovers, tooltips and other similar user
    ///interface concepts.
    ///</para>
    ///<para>
    ///A popup can be made to take an explicit grab. See xdg_popup.grab for
    ///details.
    ///</para>
    ///<para>
    ///When the popup is dismissed, a popup_done event will be sent out, and at
    ///the same time the surface will be unmapped. See the xdg_popup.popup_done
    ///event for details.
    ///</para>
    ///<para>
    ///Explicitly destroying the xdg_popup object will also dismiss the popup and
    ///unmap the surface. Clients that want to dismiss the popup when another
    ///surface of their own is clicked should dismiss the popup using the destroy
    ///request.
    ///</para>
    ///<para>
    ///A newly created xdg_popup will be stacked on top of all previously created
    ///xdg_popup surfaces associated with the same xdg_toplevel.
    ///</para>
    ///<para>
    ///The parent of an xdg_popup must be mapped (see the xdg_surface
    ///description) before the xdg_popup itself.
    ///</para>
    ///<para>
    ///The client must call wl_surface.commit on the corresponding wl_surface
    ///for the xdg_popup state to take effect.
    ///</para>
    ///</Summary>
    public partial class XdgPopup : WaylandObject
    {
        public const string INTERFACE = "xdg_popup";
        public XdgPopup(uint factoryId, ref uint id, WaylandConnection connection, uint version = 5) : base(factoryId, ref id, version, connection)
        {
        }

        ///<Summary>
        ///remove xdg_popup interface
        ///<para>
        ///This destroys the popup. Explicitly destroying the xdg_popup
        ///object will also dismiss the popup, and unmap the surface.
        ///</para>
        ///<para>
        ///If this xdg_popup is not the "topmost" popup, a protocol error
        ///will be sent.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        ///<Summary>
        ///make the popup take an explicit grab
        ///<para>
        ///This request makes the created popup take an explicit grab. An explicit
        ///grab will be dismissed when the user dismisses the popup, or when the
        ///client destroys the xdg_popup. This can be done by the user clicking
        ///outside the surface, using the keyboard, or even locking the screen
        ///through closing the lid or a timeout.
        ///</para>
        ///<para>
        ///If the compositor denies the grab, the popup will be immediately
        ///dismissed.
        ///</para>
        ///<para>
        ///This request must be used in response to some sort of user action like a
        ///button press, key press, or touch down event. The serial number of the
        ///event should be passed as 'serial'.
        ///</para>
        ///<para>
        ///The parent of a grabbing popup must either be an xdg_toplevel surface or
        ///another xdg_popup with an explicit grab. If the parent is another
        ///xdg_popup it means that the popups are nested, with this popup now being
        ///the topmost popup.
        ///</para>
        ///<para>
        ///Nested popups must be destroyed in the reverse order they were created
        ///in, e.g. the only popup you are allowed to destroy at all times is the
        ///topmost one.
        ///</para>
        ///<para>
        ///When compositors choose to dismiss a popup, they may dismiss every
        ///nested grabbing popup as well. When a compositor dismisses popups, it
        ///will follow the same dismissing order as required from the client.
        ///</para>
        ///<para>
        ///If the topmost grabbing popup is destroyed, the grab will be returned to
        ///the parent of the popup, if that parent previously had an explicit grab.
        ///</para>
        ///<para>
        ///If the parent is a grabbing popup which has already been dismissed, this
        ///popup will be immediately dismissed. If the parent is a popup that did
        ///not take an explicit grab, an error will be raised.
        ///</para>
        ///<para>
        ///During a popup grab, the client owning the grab will receive pointer
        ///and touch events for all their surfaces as normal (similar to an
        ///"owner-events" grab in X11 parlance), while the top most grabbing popup
        ///will always have keyboard focus.
        ///</para>
        ///</Summary>
        ///<param name = "seat"> the wl_seat of the user event </param>
        ///<param name = "serial"> the serial of the user event </param>
        public void Grab(WlSeat seat, uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Grab, seat.id, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Grab}({seat.id},{serial})");
        }

        ///<Summary>
        ///recalculate the popup's location
        ///<para>
        ///Reposition an already-mapped popup. The popup will be placed given the
        ///details in the passed xdg_positioner object, and a
        ///xdg_popup.repositioned followed by xdg_popup.configure and
        ///xdg_surface.configure will be emitted in response. Any parameters set
        ///by the previous positioner will be discarded.
        ///</para>
        ///<para>
        ///The passed token will be sent in the corresponding
        ///xdg_popup.repositioned event. The new popup position will not take
        ///effect until the corresponding configure event is acknowledged by the
        ///client. See xdg_popup.repositioned for details. The token itself is
        ///opaque, and has no other special meaning.
        ///</para>
        ///<para>
        ///If multiple reposition requests are sent, the compositor may skip all
        ///but the last one.
        ///</para>
        ///<para>
        ///If the popup is repositioned in response to a configure event for its
        ///parent, the client should send an xdg_positioner.set_parent_configure
        ///and possibly an xdg_positioner.set_parent_size request to allow the
        ///compositor to properly constrain the popup.
        ///</para>
        ///<para>
        ///If the popup is repositioned together with a parent that is being
        ///resized, but not in response to a configure event, the client should
        ///send an xdg_positioner.set_parent_size request.
        ///</para>
        ///</Summary>
        ///<param name = "positioner">  </param>
        ///<param name = "token"> reposition request token </param>
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

        ///<Summary>
        ///configure the popup surface
        ///<para>
        ///This event asks the popup surface to configure itself given the
        ///configuration. The configured state should not be applied immediately.
        ///See xdg_surface.configure for details.
        ///</para>
        ///<para>
        ///The x and y arguments represent the position the popup was placed at
        ///given the xdg_positioner rule, relative to the upper left corner of the
        ///window geometry of the parent surface.
        ///</para>
        ///<para>
        ///For version 2 or older, the configure event for an xdg_popup is only
        ///ever sent once for the initial configuration. Starting with version 3,
        ///it may be sent again if the popup is setup with an xdg_positioner with
        ///set_reactive requested, or in response to xdg_popup.reposition requests.
        ///</para>
        ///</Summary>
        public Action<XdgPopup, int, int, int, int> configure;
        ///<Summary>
        ///popup interaction is done
        ///<para>
        ///The popup_done event is sent out when a popup is dismissed by the
        ///compositor. The client should destroy the xdg_popup object at this
        ///point.
        ///</para>
        ///</Summary>
        public Action<XdgPopup> popupDone;
        ///<Summary>
        ///signal the completion of a repositioned request
        ///<para>
        ///The repositioned event is sent as part of a popup configuration
        ///sequence, together with xdg_popup.configure and lastly
        ///xdg_surface.configure to notify the completion of a reposition request.
        ///</para>
        ///<para>
        ///The repositioned event is to notify about the completion of a
        ///xdg_popup.reposition request. The token argument is the token passed
        ///in the xdg_popup.reposition request.
        ///</para>
        ///<para>
        ///Immediately after this event is emitted, xdg_popup.configure and
        ///xdg_surface.configure will be sent with the updated size and position,
        ///as well as a new configure serial.
        ///</para>
        ///<para>
        ///The client should optionally update the content of the popup, but must
        ///acknowledge the new popup configuration for the new position to take
        ///effect. See xdg_surface.ack_configure for details.
        ///</para>
        ///</Summary>
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

        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///tried to grab after being mapped
            ///</Summary>
            InvalidGrab = 0,
        }
    }
}
