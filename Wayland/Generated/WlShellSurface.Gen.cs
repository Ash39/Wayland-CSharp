using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlShellSurface : WaylandObject
    {
        public const string INTERFACE = "wl_shell_surface";
        public WlShellSurface(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public void Pong(uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Pong, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Pong}({serial})");
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

        public void SetToplevel()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetToplevel);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetToplevel}()");
        }

        public void SetTransient(WlSurface parent, int x, int y, uint flags)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetTransient, parent.id, x, y, flags);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetTransient}({parent.id},{x},{y},{flags})");
        }

        public void SetFullscreen(uint method, uint framerate, WlOutput output)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetFullscreen, method, framerate, output.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetFullscreen}({method},{framerate},{output.id})");
        }

        public void SetPopup(WlSeat seat, uint serial, WlSurface parent, int x, int y, uint flags)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetPopup, seat.id, serial, parent.id, x, y, flags);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetPopup}({seat.id},{serial},{parent.id},{x},{y},{flags})");
        }

        public void SetMaximized(WlOutput output)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMaximized, output.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMaximized}({output.id})");
        }

        public void SetTitle(string title)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetTitle, title);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetTitle}({title})");
        }

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
