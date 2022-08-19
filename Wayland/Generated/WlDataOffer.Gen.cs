using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///offer to transfer data
    ///<para>
    ///A wl_data_offer represents a piece of data offered for transfer
    ///by another client (the source client).  It is used by the
    ///copy-and-paste and drag-and-drop mechanisms.  The offer
    ///describes the different mime types that the data can be
    ///converted to and provides the mechanism for transferring the
    ///data directly from the source client.
    ///</para>
    ///</Summary>
    public partial class WlDataOffer : WaylandObject
    {
        public const string INTERFACE = "wl_data_offer";
        public WlDataOffer(uint factoryId, ref uint id, WaylandConnection connection, uint version = 3) : base(factoryId, ref id, version, connection)
        {
        }

        ///<Summary>
        ///accept one of the offered mime types
        ///<para>
        ///Indicate that the client can accept the given mime type, or
        ///NULL for not accepted.
        ///</para>
        ///<para>
        ///For objects of version 2 or older, this request is used by the
        ///client to give feedback whether the client can receive the given
        ///mime type, or NULL if none is accepted; the feedback does not
        ///determine whether the drag-and-drop operation succeeds or not.
        ///</para>
        ///<para>
        ///For objects of version 3 or newer, this request determines the
        ///final result of the drag-and-drop operation. If the end result
        ///is that no mime types were accepted, the drag-and-drop operation
        ///will be cancelled and the corresponding drag source will receive
        ///wl_data_source.cancelled. Clients may still use this event in
        ///conjunction with wl_data_source.action for feedback.
        ///</para>
        ///</Summary>
        ///<param name = "serial"> serial number of the accept request </param>
        ///<param name = "mime_type"> mime type accepted by the client </param>
        public void Accept(uint serial, string mime_type)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Accept, serial, mime_type);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Accept}({serial},{mime_type})");
        }

        ///<Summary>
        ///request that the data is transferred
        ///<para>
        ///To transfer the offered data, the client issues this request
        ///and indicates the mime type it wants to receive.  The transfer
        ///happens through the passed file descriptor (typically created
        ///with the pipe system call).  The source client writes the data
        ///in the mime type representation requested and then closes the
        ///file descriptor.
        ///</para>
        ///<para>
        ///The receiving client reads from the read end of the pipe until
        ///EOF and then closes its end, at which point the transfer is
        ///complete.
        ///</para>
        ///<para>
        ///This request may happen multiple times for different mime types,
        ///both before and after wl_data_device.drop. Drag-and-drop destination
        ///clients may preemptively fetch data or examine it more closely to
        ///determine acceptance.
        ///</para>
        ///</Summary>
        ///<param name = "mime_type"> mime type desired by receiver </param>
        ///<param name = "fd"> file descriptor for data transfer </param>
        public void Receive(string mime_type, IntPtr fd)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Receive, mime_type, fd);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Receive}({mime_type},{fd})");
        }

        ///<Summary>
        ///destroy data offer
        ///<para>
        ///Destroy the data offer.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        ///<Summary>
        ///the offer will no longer be used
        ///<para>
        ///Notifies the compositor that the drag destination successfully
        ///finished the drag-and-drop operation.
        ///</para>
        ///<para>
        ///Upon receiving this request, the compositor will emit
        ///wl_data_source.dnd_finished on the drag source client.
        ///</para>
        ///<para>
        ///It is a client error to perform other requests than
        ///wl_data_offer.destroy after this one. It is also an error to perform
        ///this request after a NULL mime type has been set in
        ///wl_data_offer.accept or no action was received through
        ///wl_data_offer.action.
        ///</para>
        ///<para>
        ///If wl_data_offer.finish request is received for a non drag and drop
        ///operation, the invalid_finish protocol error is raised.
        ///</para>
        ///</Summary>
        public void Finish()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Finish);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Finish}()");
        }

        ///<Summary>
        ///set the available/preferred drag-and-drop actions
        ///<para>
        ///Sets the actions that the destination side client supports for
        ///this operation. This request may trigger the emission of
        ///wl_data_source.action and wl_data_offer.action events if the compositor
        ///needs to change the selected action.
        ///</para>
        ///<para>
        ///This request can be called multiple times throughout the
        ///drag-and-drop operation, typically in response to wl_data_device.enter
        ///or wl_data_device.motion events.
        ///</para>
        ///<para>
        ///This request determines the final result of the drag-and-drop
        ///operation. If the end result is that no action is accepted,
        ///the drag source will receive wl_data_source.cancelled.
        ///</para>
        ///<para>
        ///The dnd_actions argument must contain only values expressed in the
        ///wl_data_device_manager.dnd_actions enum, and the preferred_action
        ///argument must only contain one of those values set, otherwise it
        ///will result in a protocol error.
        ///</para>
        ///<para>
        ///While managing an "ask" action, the destination drag-and-drop client
        ///may perform further wl_data_offer.receive requests, and is expected
        ///to perform one last wl_data_offer.set_actions request with a preferred
        ///action other than "ask" (and optionally wl_data_offer.accept) before
        ///requesting wl_data_offer.finish, in order to convey the action selected
        ///by the user. If the preferred action is not in the
        ///wl_data_offer.source_actions mask, an error will be raised.
        ///</para>
        ///<para>
        ///If the "ask" action is dismissed (e.g. user cancellation), the client
        ///is expected to perform wl_data_offer.destroy right away.
        ///</para>
        ///<para>
        ///This request can only be made on drag-and-drop offers, a protocol error
        ///will be raised otherwise.
        ///</para>
        ///</Summary>
        ///<param name = "dnd_actions"> actions supported by the destination client </param>
        ///<param name = "preferred_action"> action preferred by the destination client </param>
        public void SetActions(WlDataDeviceManager.DndActionFlag dnd_actions, WlDataDeviceManager.DndActionFlag preferred_action)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetActions, (uint)dnd_actions, (uint)preferred_action);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetActions}({(uint)dnd_actions},{(uint)preferred_action})");
        }

        public enum RequestOpcode : ushort
        {
            Accept,
            Receive,
            Destroy,
            Finish,
            SetActions
        }

        ///<Summary>
        ///advertise offered mime type
        ///<para>
        ///Sent immediately after creating the wl_data_offer object.  One
        ///event per offered mime type.
        ///</para>
        ///</Summary>
        public Action<WlDataOffer, string> offer;
        ///<Summary>
        ///notify the source-side available actions
        ///<para>
        ///This event indicates the actions offered by the data source. It
        ///will be sent right after wl_data_device.enter, or anytime the source
        ///side changes its offered actions through wl_data_source.set_actions.
        ///</para>
        ///</Summary>
        public Action<WlDataOffer, WlDataDeviceManager.DndActionFlag> sourceActions;
        ///<Summary>
        ///notify the selected action
        ///<para>
        ///This event indicates the action selected by the compositor after
        ///matching the source/destination side actions. Only one action (or
        ///none) will be offered here.
        ///</para>
        ///<para>
        ///This event can be emitted multiple times during the drag-and-drop
        ///operation in response to destination side action changes through
        ///wl_data_offer.set_actions.
        ///</para>
        ///<para>
        ///This event will no longer be emitted after wl_data_device.drop
        ///happened on the drag-and-drop destination, the client must
        ///honor the last action received, or the last preferred one set
        ///through wl_data_offer.set_actions when handling an "ask" action.
        ///</para>
        ///<para>
        ///Compositors may also change the selected action on the fly, mainly
        ///in response to keyboard modifier changes during the drag-and-drop
        ///operation.
        ///</para>
        ///<para>
        ///The most recent action received is always the valid one. Prior to
        ///receiving wl_data_device.drop, the chosen action may change (e.g.
        ///due to keyboard modifiers being pressed). At the time of receiving
        ///wl_data_device.drop the drag-and-drop destination must honor the
        ///last action received.
        ///</para>
        ///<para>
        ///Action changes may still happen after wl_data_device.drop,
        ///especially on "ask" actions, where the drag-and-drop destination
        ///may choose another action afterwards. Action changes happening
        ///at this stage are always the result of inter-client negotiation, the
        ///compositor shall no longer be able to induce a different action.
        ///</para>
        ///<para>
        ///Upon "ask" actions, it is expected that the drag-and-drop destination
        ///may potentially choose a different action and/or mime type,
        ///based on wl_data_offer.source_actions and finally chosen by the
        ///user (e.g. popping up a menu with the available options). The
        ///final wl_data_offer.set_actions and wl_data_offer.accept requests
        ///must happen before the call to wl_data_offer.finish.
        ///</para>
        ///</Summary>
        public Action<WlDataOffer, WlDataDeviceManager.DndActionFlag> action;
        public enum EventOpcode : ushort
        {
            Offer,
            SourceActions,
            Action
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Offer:
                {
                    var mimeType = (string)arguments[0];
                    if (this.offer != null)
                    {
                        this.offer.Invoke(this, mimeType);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Offer}({this},{mimeType})");
                    }

                    break;
                }

                case EventOpcode.SourceActions:
                {
                    var sourceActions = (WlDataDeviceManager.DndActionFlag)arguments[0];
                    if (this.sourceActions != null)
                    {
                        this.sourceActions.Invoke(this, sourceActions);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.SourceActions}({this},{sourceActions})");
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
                case EventOpcode.Offer:
                    return new WaylandType[]{WaylandType.String, };
                case EventOpcode.SourceActions:
                    return new WaylandType[]{WaylandType.Uint, };
                case EventOpcode.Action:
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///finish request was called untimely
            ///</Summary>
            InvalidFinish = 0,
            ///<Summary>
            ///action mask contains invalid values
            ///</Summary>
            InvalidActionMask = 1,
            ///<Summary>
            ///action argument has an invalid value
            ///</Summary>
            InvalidAction = 2,
            ///<Summary>
            ///offer doesn't accept this request
            ///</Summary>
            InvalidOffer = 3,
        }
    }
}
