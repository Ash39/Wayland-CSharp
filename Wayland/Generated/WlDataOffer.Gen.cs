using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlDataOffer : WaylandObject
    {
        public const string INTERFACE = "wl_data_offer";
        public WlDataOffer(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public void Accept(uint serial, string mime_type)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Accept, serial, mime_type);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Accept}({serial},{mime_type})");
        }

        public void Receive(string mime_type, IntPtr fd)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Receive, mime_type, fd);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Receive}({mime_type},{fd})");
        }

        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        public void Finish()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Finish);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Finish}()");
        }

        public void SetActions(uint dnd_actions, uint preferred_action)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetActions, dnd_actions, preferred_action);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetActions}({dnd_actions},{preferred_action})");
        }

        public enum RequestOpcode : ushort
        {
            Accept,
            Receive,
            Destroy,
            Finish,
            SetActions
        }

        public Action<WlDataOffer, string> offer;
        public Action<WlDataOffer, uint> sourceActions;
        public Action<WlDataOffer, uint> action;
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
                    var sourceActions = (uint)arguments[0];
                    if (this.sourceActions != null)
                    {
                        this.sourceActions.Invoke(this, sourceActions);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.SourceActions}({this},{sourceActions})");
                    }

                    break;
                }

                case EventOpcode.Action:
                {
                    var dndAction = (uint)arguments[0];
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
    }
}
