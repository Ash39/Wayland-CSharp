using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class XdgSurface : WaylandObject
    {
        public const string INTERFACE = "xdg_surface";
        public XdgSurface(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        public XdgToplevel GetToplevel()
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.GetToplevel, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetToplevel}({id})");
            connection[id] = new XdgToplevel(id, version, connection);
            return (XdgToplevel)connection[id];
        }

        public XdgPopup GetPopup(XdgSurface parent, XdgPositioner positioner)
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.GetPopup, id, parent.id, positioner.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetPopup}({id},{parent.id},{positioner.id})");
            connection[id] = new XdgPopup(id, version, connection);
            return (XdgPopup)connection[id];
        }

        public void SetWindowGeometry(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetWindowGeometry, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetWindowGeometry}({x},{y},{width},{height})");
        }

        public void AckConfigure(uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.AckConfigure, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.AckConfigure}({serial})");
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            GetToplevel,
            GetPopup,
            SetWindowGeometry,
            AckConfigure
        }

        public Action<XdgSurface, uint> configure;
        public enum EventOpcode : ushort
        {
            Configure
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Configure:
                {
                    var serial = (uint)arguments[0];
                    if (this.configure != null)
                    {
                        this.configure.Invoke(this, serial);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Configure}({this},{serial})");
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
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
