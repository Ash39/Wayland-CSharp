using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// desktop-style metadata interface
    /// </summary>
    public partial class WlShellSurface : WaylandObject
    {
        public const string INTERFACE = "wl_shell_surface";
        public WlShellSurface(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// respond to a ping event
        /// </summary>
        public void Pong(uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Pong, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Pong}({serial})");
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
        /// make the surface a toplevel surface
        /// </summary>
        public void SetToplevel()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetToplevel);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetToplevel}()");
        }

        /// <summary>
        /// make the surface a transient surface
        /// </summary>
        public void SetTransient(WlSurface parent, int x, int y, uint flags)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetTransient, parent.id, x, y, flags);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetTransient}({parent.id},{x},{y},{flags})");
        }

        /// <summary>
        /// make the surface a fullscreen surface
        /// </summary>
        public void SetFullscreen(uint method, uint framerate, WlOutput output)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetFullscreen, method, framerate, output.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetFullscreen}({method},{framerate},{output.id})");
        }

        /// <summary>
        /// make the surface a popup surface
        /// </summary>
        public void SetPopup(WlSeat seat, uint serial, WlSurface parent, int x, int y, uint flags)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetPopup, seat.id, serial, parent.id, x, y, flags);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetPopup}({seat.id},{serial},{parent.id},{x},{y},{flags})");
        }

        /// <summary>
        /// make the surface a maximized surface
        /// </summary>
        public void SetMaximized(WlOutput output)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMaximized, output.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMaximized}({output.id})");
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
        /// set surface class
        /// </summary>
        public void SetClass(string class_)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetClass, class_);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetClass}({class_})");
        }

        public enum RequestOpcode : ushort
        {
            Pong,
            Move,
            Resize,
            SetToplevel,
            SetTransient,
            SetFullscreen,
            SetPopup,
            SetMaximized,
            SetTitle,
            SetClass
        }

        public Action<WlShellSurface, uint> ping;
        public Action<WlShellSurface, uint, int, int> configure;
        public Action<WlShellSurface> popupDone;
        public enum EventOpcode : ushort
        {
            Ping,
            Configure,
            PopupDone
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Ping:
                {
                    var serial = (uint)arguments[0];
                    if (this.ping != null)
                    {
                        this.ping.Invoke(this, serial);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Ping}({this},{serial})");
                    }

                    break;
                }

                case EventOpcode.Configure:
                {
                    var edges = (uint)arguments[0];
                    var width = (int)arguments[1];
                    var height = (int)arguments[2];
                    if (this.configure != null)
                    {
                        this.configure.Invoke(this, edges, width, height);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Configure}({this},{edges},{width},{height})");
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

                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        public override WaylandType[] WaylandTypes(ushort opCode)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Ping:
                    return new WaylandType[]{WaylandType.Uint, };
                case EventOpcode.Configure:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Int, WaylandType.Int, };
                case EventOpcode.PopupDone:
                    return new WaylandType[]{};
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
