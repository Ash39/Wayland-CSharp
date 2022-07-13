using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class XdgWmBase : WaylandObject
    {
        public const string INTERFACE = "xdg_wm_base";
        public XdgWmBase(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        public XdgPositioner CreatePositioner()
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.CreatePositioner, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreatePositioner}({id})");
            connection[id] = new XdgPositioner(id, version, connection);
            return (XdgPositioner)connection[id];
        }

        public XdgSurface GetXdgSurface(WlSurface surface)
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.GetXdgSurface, id, surface.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetXdgSurface}({id},{surface.id})");
            connection[id] = new XdgSurface(id, version, connection);
            return (XdgSurface)connection[id];
        }

        public void Pong(uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Pong, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Pong}({serial})");
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            CreatePositioner,
            GetXdgSurface,
            Pong
        }

        public Action<XdgWmBase, uint> ping;
        public enum EventOpcode : ushort
        {
            Ping
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
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
