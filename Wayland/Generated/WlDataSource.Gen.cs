using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///offer to transfer data
    ///<para>
    ///The wl_data_source object is the source side of a wl_data_offer.
    ///It is created by the source client in a data transfer and
    ///provides a way to describe the offered data and a way to respond
    ///to requests to transfer the data.
    ///</para>
    ///</Summary>
    public partial class WlDataSource : WaylandObject
    {
        public const string INTERFACE = "wl_data_source";
        public WlDataSource(uint factoryId, ref uint id, WaylandConnection connection) : base(factoryId, ref id, 3, connection)
        {
        }

        ///<Summary>
        ///add an offered mime type
        ///<para>
        ///This request adds a mime type to the set of mime types
        ///advertised to targets.  Can be called several times to offer
        ///multiple types.
        ///</para>
        ///</Summary>
        ///<param name = "mime_type"> mime type offered by the data source </param>
        public void Offer(string mime_type)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Offer, mime_type);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Offer}({mime_type})");
        }

        ///<Summary>
        ///destroy the data source
        ///<para>
        ///Destroy the data source.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        ///<Summary>
        ///set the available drag-and-drop actions
        ///<para>
        ///Sets the actions that the source side client supports for this
        ///operation. This request may trigger wl_data_source.action and
        ///wl_data_offer.action events if the compositor needs to change the
        ///selected action.
        ///</para>
        ///<para>
        ///The dnd_actions argument must contain only values expressed in the
        ///wl_data_device_manager.dnd_actions enum, otherwise it will result
        ///in a protocol error.
        ///</para>
        ///<para>
        ///This request must be made once only, and can only be made on sources
        ///used in drag-and-drop, so it must be performed before
        ///wl_data_device.start_drag. Attempting to use the source other than
        ///for drag-and-drop will raise a protocol error.
        ///</para>
        ///</Summary>
        ///<param name = "dnd_actions"> actions supported by the data source </param>
        public void SetActions(WlDataDeviceManager.DndActionFlag dnd_actions)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetActions, (uint)dnd_actions);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetActions}({(uint)dnd_actions})");
        }

        public enum RequestOpcode : ushort
        {
            Offer,
            Destroy,
            SetActions
        }

        ///<Summary>
        ///a target accepts an offered mime type
        ///<para>
        ///Sent when a target accepts pointer_focus or motion events.  If
        ///a target does not accept any of the offered types, type is NULL.
        ///</para>
        ///<para>
        ///Used for feedback during drag-and-drop.
        ///</para>
        ///</Summary>
        public Action<WlDataSource, string> target;
        ///<Summary>
        ///send the data
        ///<para>
        ///Request for data from the client.  Send the data as the
        ///specified mime type over the passed file descriptor, then
        ///close it.
        ///</para>
        ///</Summary>
        public Action<WlDataSource, string, IntPtr> send;
        ///<Summary>
        ///selection was cancelled
        ///<para>
        ///This data source is no longer valid. There are several reasons why
        ///this could happen:
        ///</para>
        ///<para>
        ///- The data source has been replaced by another data source.
        ///- The drag-and-drop operation was performed, but the drop destination
        ///did not accept any of the mime types offered through
        ///wl_data_source.target.
        ///- The drag-and-drop operation was performed, but the drop destination
        ///did not select any of the actions present in the mask offered through
        ///wl_data_source.action.
        ///- The drag-and-drop operation was performed but didn't happen over a
        ///surface.
        ///- The compositor cancelled the drag-and-drop operation (e.g. compositor
        ///dependent timeouts to avoid stale drag-and-drop transfers).
        ///</para>
        ///<para>
        ///The client should clean up and destroy this data source.
        ///</para>
        ///<para>
        ///For objects of version 2 or older, wl_data_source.cancelled will
        ///only be emitted if the data source was replaced by another data
        ///source.
        ///</para>
        ///</Summary>
        public Action<WlDataSource> cancelled;
        ///<Summary>
        ///the drag-and-drop operation physically finished
        ///<para>
        ///The user performed the drop action. This event does not indicate
        ///acceptance, wl_data_source.cancelled may still be emitted afterwards
        ///if the drop destination does not accept any mime type.
        ///</para>
        ///<para>
        ///However, this event might however not be received if the compositor
        ///cancelled the drag-and-drop operation before this event could happen.
        ///</para>
        ///<para>
        ///Note that the data_source may still be used in the future and should
        ///not be destroyed here.
        ///</para>
        ///</Summary>
        public Action<WlDataSource> dndDropPerformed;
        ///<Summary>
        ///the drag-and-drop operation concluded
        ///<para>
        ///The drop destination finished interoperating with this data
        ///source, so the client is now free to destroy this data source and
        ///free all associated data.
        ///</para>
        ///<para>
        ///If the action used to perform the operation was "move", the
        ///source can now delete the transferred data.
        ///</para>
        ///</Summary>
        public Action<WlDataSource> dndFinished;
        ///<Summary>
        ///notify the selected action
        ///<para>
        ///This event indicates the action selected by the compositor after
        ///matching the source/destination side actions. Only one action (or
        ///none) will be offered here.
        ///</para>
        ///<para>
        ///This event can be emitted multiple times during the drag-and-drop
        ///operation, mainly in response to destination side changes through
        ///wl_data_offer.set_actions, and as the data device enters/leaves
        ///surfaces.
        ///</para>
        ///<para>
        ///It is only possible to receive this event after
        ///wl_data_source.dnd_drop_performed if the drag-and-drop operation
        ///ended in an "ask" action, in which case the final wl_data_source.action
        ///event will happen immediately before wl_data_source.dnd_finished.
        ///</para>
        ///<para>
        ///Compositors may also change the selected action on the fly, mainly
        ///in response to keyboard modifier changes during the drag-and-drop
        ///operation.
        ///</para>
        ///<para>
        ///The most recent action received is always the valid one. The chosen
        ///action may change alongside negotiation (e.g. an "ask" action can turn
        ///into a "move" operation), so the effects of the final action must
        ///always be applied in wl_data_offer.dnd_finished.
        ///</para>
        ///<para>
        ///Clients can trigger cursor surface changes from this point, so
        ///they reflect the current action.
        ///</para>
        ///</Summary>
        public Action<WlDataSource, WlDataDeviceManager.DndActionFlag> action;
        public enum EventOpcode : ushort
        {
            Target,
            Send,
            Cancelled,
            DndDropPerformed,
            DndFinished,
            Action
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Target:
                {
                    var mimeType = (string)arguments[0];
                    if (this.target != null)
                    {
                        this.target.Invoke(this, mimeType);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Target}({this},{mimeType})");
                    }

                    break;
                }

                case EventOpcode.Send:
                {
                    var mimeType = (string)arguments[0];
                    var fd = (IntPtr)arguments[1];
                    if (this.send != null)
                    {
                        this.send.Invoke(this, mimeType, fd);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Send}({this},{mimeType},{fd})");
                    }

                    break;
                }

                case EventOpcode.Cancelled:
                {
                    if (this.cancelled != null)
                    {
                        this.cancelled.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Cancelled}({this})");
                    }

                    break;
                }

                case EventOpcode.DndDropPerformed:
                {
                    if (this.dndDropPerformed != null)
                    {
                        this.dndDropPerformed.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.DndDropPerformed}({this})");
                    }

                    break;
                }

                case EventOpcode.DndFinished:
                {
                    if (this.dndFinished != null)
                    {
                        this.dndFinished.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.DndFinished}({this})");
                    }

                    break;
                }

                case EventOpcode.Action:
                {
                    var dndAction = (WlDataDeviceManager.DndActionFlag)arguments[0];
                    if (this.action != null)
                    {
                        this.action.Invoke(this, dndAction);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Action}({this},{dndAction})");
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
                case EventOpcode.Target:
                    return new WaylandType[]{WaylandType.String, };
                case EventOpcode.Send:
                    return new WaylandType[]{WaylandType.String, WaylandType.Fd, };
                case EventOpcode.Cancelled:
                    return new WaylandType[]{};
                case EventOpcode.DndDropPerformed:
                    return new WaylandType[]{};
                case EventOpcode.DndFinished:
                    return new WaylandType[]{};
                case EventOpcode.Action:
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///action mask contains invalid values
            ///</Summary>
            InvalidActionMask = 0,
            ///<Summary>
            ///source doesn't accept this request
            ///</Summary>
            InvalidSource = 1,
        }
    }
}
