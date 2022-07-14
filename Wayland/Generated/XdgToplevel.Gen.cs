using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// toplevel surface
    /// </summary>
    public partial class XdgToplevel : WaylandObject
    {
        public const string INTERFACE = "xdg_toplevel";
        public XdgToplevel(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// destroy the xdg_toplevel
        /// </summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        /// <summary>
        /// set the parent of this surface
        /// </summary>
        public void SetParent(XdgToplevel parent)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetParent, parent.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetParent}({parent.id})");
        }

        /// <summary>
        /// set surface title
        /// </summary>
        public void SetTitle(string title)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetTitle, title);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetTitle}({title})");
        }

        /// <summary>
        /// set application ID
        /// </summary>
        public void SetAppId(string app_id)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetAppId, app_id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetAppId}({app_id})");
        }

        /// <summary>
        /// show the window menu
        /// </summary>
        public void ShowWindowMenu(WlSeat seat, uint serial, int x, int y)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.ShowWindowMenu, seat.id, serial, x, y);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.ShowWindowMenu}({seat.id},{serial},{x},{y})");
        }

        /// <summary>
        /// start an interactive move
        /// </summary>
        public void Move(WlSeat seat, uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Move, seat.id, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Move}({seat.id},{serial})");
        }

        /// <summary>
        /// start an interactive resize
        /// </summary>
        public void Resize(WlSeat seat, uint serial, uint edges)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Resize, seat.id, serial, edges);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Resize}({seat.id},{serial},{edges})");
        }

        /// <summary>
        /// set the maximum size
        /// </summary>
        public void SetMaxSize(int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMaxSize, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMaxSize}({width},{height})");
        }

        /// <summary>
        /// set the minimum size
        /// </summary>
        public void SetMinSize(int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMinSize, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMinSize}({width},{height})");
        }

        /// <summary>
        /// maximize the window
        /// </summary>
        public void SetMaximized()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMaximized);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMaximized}()");
        }

        /// <summary>
        /// unmaximize the window
        /// </summary>
        public void UnsetMaximized()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.UnsetMaximized);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.UnsetMaximized}()");
        }

        /// <summary>
        /// set the window as fullscreen on an output
        /// </summary>
        public void SetFullscreen(WlOutput output)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetFullscreen, output.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetFullscreen}({output.id})");
        }

        /// <summary>
        /// unset the window as fullscreen
        /// </summary>
        public void UnsetFullscreen()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.UnsetFullscreen);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.UnsetFullscreen}()");
        }

        /// <summary>
        /// set the window as minimized
        /// </summary>
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
