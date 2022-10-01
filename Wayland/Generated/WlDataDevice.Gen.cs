using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///data transfer device
    ///<para>
    ///There is one wl_data_device per seat which can be obtained
    ///from the global wl_data_device_manager singleton.
    ///</para>
    ///<para>
    ///A wl_data_device provides access to inter-client data transfer
    ///mechanisms such as copy-and-paste and drag-and-drop.
    ///</para>
    ///</Summary>
    public partial class WlDataDevice : WaylandObject
    {
        public const string INTERFACE = "wl_data_device";
        public WlDataDevice(uint factoryId, ref uint id, WaylandConnection connection, uint version = 3) : base(factoryId, ref id, version, connection)
        {
        }

        ///<Summary>
        ///start drag-and-drop operation
        ///<para>
        ///This request asks the compositor to start a drag-and-drop
        ///operation on behalf of the client.
        ///</para>
        ///<para>
        ///The source argument is the data source that provides the data
        ///for the eventual data transfer. If source is NULL, enter, leave
        ///and motion events are sent only to the client that initiated the
        ///drag and the client is expected to handle the data passing
        ///internally. If source is destroyed, the drag-and-drop session will be
        ///cancelled.
        ///</para>
        ///<para>
        ///The origin surface is the surface where the drag originates and
        ///the client must have an active implicit grab that matches the
        ///serial.
        ///</para>
        ///<para>
        ///The icon surface is an optional (can be NULL) surface that
        ///provides an icon to be moved around with the cursor.  Initially,
        ///the top-left corner of the icon surface is placed at the cursor
        ///hotspot, but subsequent wl_surface.attach request can move the
        ///relative position. Attach requests must be confirmed with
        ///wl_surface.commit as usual. The icon surface is given the role of
        ///a drag-and-drop icon. If the icon surface already has another role,
        ///it raises a protocol error.
        ///</para>
        ///<para>
        ///The current and pending input regions of the icon wl_surface are
        ///cleared, and wl_surface.set_input_region is ignored until the
        ///wl_surface is no longer used as the icon surface. When the use
        ///as an icon ends, the current and pending input regions become
        ///undefined, and the wl_surface is unmapped.
        ///</para>
        ///</Summary>
        ///<param name = "source"> data source for the eventual transfer </param>
        ///<param name = "origin"> surface where the drag originates </param>
        ///<param name = "icon"> drag-and-drop icon surface </param>
        ///<param name = "serial"> serial number of the implicit grab on the origin </param>
        public void StartDrag(WlDataSource source, WlSurface origin, WlSurface icon, uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.StartDrag, source.id, origin.id, icon.id, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.StartDrag}({source.id},{origin.id},{icon.id},{serial})");
        }

        ///<Summary>
        ///copy data to the selection
        ///<para>
        ///This request asks the compositor to set the selection
        ///to the data from the source on behalf of the client.
        ///</para>
        ///<para>
        ///To unset the selection, set the source to NULL.
        ///</para>
        ///</Summary>
        ///<param name = "source"> data source for the selection </param>
        ///<param name = "serial"> serial number of the event that triggered this request </param>
        public void SetSelection(WlDataSource source, uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetSelection, source.id, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetSelection}({source.id},{serial})");
        }

        ///<Summary>
        ///destroy data device
        ///<para>
        ///This request destroys the data device.
        ///</para>
        ///</Summary>
        public void Release()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Release);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Release}()");
        }

        public enum RequestOpcode : ushort
        {
            StartDrag,
            SetSelection,
            Release
        }

        ///<Summary>
        ///introduce a new wl_data_offer
        ///<para>
        ///The data_offer event introduces a new wl_data_offer object,
        ///which will subsequently be used in either the
        ///data_device.enter event (for drag-and-drop) or the
        ///data_device.selection event (for selections).  Immediately
        ///following the data_device.data_offer event, the new data_offer
        ///object will send out data_offer.offer events to describe the
        ///mime types it offers.
        ///</para>
        ///</Summary>
        public Action<WlDataDevice, WlDataOffer> dataOffer;
        ///<Summary>
        ///initiate drag-and-drop session
        ///<para>
        ///This event is sent when an active drag-and-drop pointer enters
        ///a surface owned by the client.  The position of the pointer at
        ///enter time is provided by the x and y arguments, in surface-local
        ///coordinates.
        ///</para>
        ///</Summary>
        public Action<WlDataDevice, uint, WaylandObject, double, double, WaylandObject> enter;
        ///<Summary>
        ///end drag-and-drop session
        ///<para>
        ///This event is sent when the drag-and-drop pointer leaves the
        ///surface and the session ends.  The client must destroy the
        ///wl_data_offer introduced at enter time at this point.
        ///</para>
        ///</Summary>
        public Action<WlDataDevice> leave;
        ///<Summary>
        ///drag-and-drop session motion
        ///<para>
        ///This event is sent when the drag-and-drop pointer moves within
        ///the currently focused surface. The new position of the pointer
        ///is provided by the x and y arguments, in surface-local
        ///coordinates.
        ///</para>
        ///</Summary>
        public Action<WlDataDevice, uint, double, double> motion;
        ///<Summary>
        ///end drag-and-drop session successfully
        ///<para>
        ///The event is sent when a drag-and-drop operation is ended
        ///because the implicit grab is removed.
        ///</para>
        ///<para>
        ///The drag-and-drop destination is expected to honor the last action
        ///received through wl_data_offer.action, if the resulting action is
        ///"copy" or "move", the destination can still perform
        ///wl_data_offer.receive requests, and is expected to end all
        ///transfers with a wl_data_offer.finish request.
        ///</para>
        ///<para>
        ///If the resulting action is "ask", the action will not be considered
        ///final. The drag-and-drop destination is expected to perform one last
        ///wl_data_offer.set_actions request, or wl_data_offer.destroy in order
        ///to cancel the operation.
        ///</para>
        ///</Summary>
        public Action<WlDataDevice> drop;
        ///<Summary>
        ///advertise new selection
        ///<para>
        ///The selection event is sent out to notify the client of a new
        ///wl_data_offer for the selection for this device.  The
        ///data_device.data_offer and the data_offer.offer events are
        ///sent out immediately before this event to introduce the data
        ///offer object.  The selection event is sent to a client
        ///immediately before receiving keyboard focus and when a new
        ///selection is set while the client has keyboard focus.  The
        ///data_offer is valid until a new data_offer or NULL is received
        ///or until the client loses keyboard focus.  Switching surface with
        ///keyboard focus within the same client doesn't mean a new selection
        ///will be sent.  The client must destroy the previous selection
        ///data_offer, if any, upon receiving this event.
        ///</para>
        ///</Summary>
        public Action<WlDataDevice, WaylandObject> selection;
        public enum EventOpcode : ushort
        {
            DataOffer,
            Enter,
            Leave,
            Motion,
            Drop,
            Selection
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.DataOffer:
                {
                    uint new_id = (uint)arguments[0];
                    WlDataOffer id = new WlDataOffer(this.id, ref new_id, connection);
                    connection.ServerObjectAdd(id);
                    if (this.dataOffer != null)
                    {
                        this.dataOffer.Invoke(this, id);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.DataOffer}({this},{id})");
                    }

                    break;
                }

                case EventOpcode.Enter:
                {
                    var serial = (uint)arguments[0];
                    var surface = connection[(uint)arguments[1]];
                    var x = (double)arguments[2];
                    var y = (double)arguments[3];
                    var id = connection[(uint)arguments[4]];
                    if (this.enter != null)
                    {
                        this.enter.Invoke(this, serial, surface, x, y, id);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Enter}({this},{serial},{surface},{x},{y},{id})");
                    }

                    break;
                }

                case EventOpcode.Leave:
                {
                    if (this.leave != null)
                    {
                        this.leave.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Leave}({this})");
                    }

                    break;
                }

                case EventOpcode.Motion:
                {
                    var time = (uint)arguments[0];
                    var x = (double)arguments[1];
                    var y = (double)arguments[2];
                    if (this.motion != null)
                    {
                        this.motion.Invoke(this, time, x, y);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Motion}({this},{time},{x},{y})");
                    }

                    break;
                }

                case EventOpcode.Drop:
                {
                    if (this.drop != null)
                    {
                        this.drop.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Drop}({this})");
                    }

                    break;
                }

                case EventOpcode.Selection:
                {
                    var id = connection[(uint)arguments[0]];
                    if (this.selection != null)
                    {
                        this.selection.Invoke(this, id);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Selection}({this},{id})");
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
                case EventOpcode.DataOffer:
                    return new WaylandType[]{WaylandType.NewId, };
                case EventOpcode.Enter:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Object, WaylandType.Fixed, WaylandType.Fixed, WaylandType.Object, };
                case EventOpcode.Leave:
                    return new WaylandType[]{};
                case EventOpcode.Motion:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Fixed, WaylandType.Fixed, };
                case EventOpcode.Drop:
                    return new WaylandType[]{};
                case EventOpcode.Selection:
                    return new WaylandType[]{WaylandType.Object, };
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
    }
}
