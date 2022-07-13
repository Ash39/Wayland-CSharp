using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class XdgToplevel : WaylandObject
    {
        public const string INTERFACE = "xdg_toplevel";
        public XdgToplevel(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        public void SetParent(XdgToplevel parent)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetParent, parent.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetParent}({parent.id})");
        }

        public void SetTitle(string title)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetTitle, title);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetTitle}({title})");
        }

        public void SetAppId(string app_id)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetAppId, app_id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetAppId}({app_id})");
        }

        public void ShowWindowMenu(WlSeat seat, uint serial, int x, int y)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.ShowWindowMenu, seat.id, serial, x, y);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.ShowWindowMenu}({seat.id},{serial},{x},{y})");
        }

        public void Move(WlSeat seat, uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Move, seat.id, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Move}({seat.id},{serial})");
        }

        public void Resize(WlSeat seat, uint serial, uint edges)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Resize, seat.id, serial, edges);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Resize}({seat.id},{serial},{edges})");
        }

        public void SetMaxSize(int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMaxSize, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMaxSize}({width},{height})");
        }

        public void SetMinSize(int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMinSize, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMinSize}({width},{height})");
        }

        public void SetMaximized()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMaximized);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMaximized}()");
        }

        public void UnsetMaximized()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.UnsetMaximized);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.UnsetMaximized}()");
        }

        public void SetFullscreen(WlOutput output)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetFullscreen, output.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetFullscreen}({output.id})");
        }

        public void UnsetFullscreen()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.UnsetFullscreen);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.UnsetFullscreen}()");
        }

        public void SetMinimized()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMinimized);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMinimized}()");
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            SetParent,
            SetTitle,
            SetAppId,
            ShowWindowMenu,
            Move,
            Resize,
            SetMaxSize,
            SetMinSize,
            SetMaximized,
            UnsetMaximized,
            SetFullscreen,
            UnsetFullscreen,
            SetMinimized
        }

        public Action<XdgToplevel, int, int, byte[]> configure;
        public Action<XdgToplevel> close;
        public enum EventOpcode : ushort
        {
            Configure,
            Close
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Configure:
                {
                    var width = (int)arguments[0];
                    var height = (int)arguments[1];
                    var states = (byte[])arguments[2];
                    if (this.configure != null)
                    {
                        this.configure.Invoke(this, width, height, states);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Configure}({this},{width},{height},{states})");
                    }

                    break;
                }

                case EventOpcode.Close:
                {
                    if (this.close != null)
                    {
                        this.close.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Close}({this})");
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
                    return new WaylandType[]{WaylandType.Int, WaylandType.Int, WaylandType.Array, };
                case EventOpcode.Close:
                    return new WaylandType[]{};
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
