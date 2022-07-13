using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlDataSource : WaylandObject
    {
        public const string INTERFACE = "wl_data_source";
        public WlDataSource(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public void Offer(string mime_type)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Offer, mime_type);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Offer}({mime_type})");
        }

        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        public void SetActions(uint dnd_actions)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetActions, dnd_actions);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetActions}({dnd_actions})");
        }

        public enum RequestOpcode : ushort
        {
            Offer,
            Destroy,
            SetActions
        }

        public Action<WlDataSource, string> target;
        public Action<WlDataSource, string, IntPtr> send;
        public Action<WlDataSource> cancelled;
        public Action<WlDataSource> dndDropPerformed;
        public Action<WlDataSource> dndFinished;
        public Action<WlDataSource, uint> action;
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
    }
}
