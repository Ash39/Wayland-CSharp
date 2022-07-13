using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class WlSurface : WaylandObject
    {
        public const string INTERFACE = "wl_surface";
        public WlSurface(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        public void Attach(WlBuffer buffer, int x, int y)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Attach, buffer.id, x, y);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Attach}({buffer.id},{x},{y})");
        }

        public void Damage(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Damage, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Damage}({x},{y},{width},{height})");
        }

        public WlCallback Frame()
        {
            uint callback = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.Frame, callback);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Frame}({callback})");
            connection[callback] = new WlCallback(callback, version, connection);
            return (WlCallback)connection[callback];
        }

        public void SetOpaqueRegion(WlRegion region)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetOpaqueRegion, region.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetOpaqueRegion}({region.id})");
        }

        public void SetInputRegion(WlRegion region)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetInputRegion, region.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetInputRegion}({region.id})");
        }

        public void Commit()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Commit);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Commit}()");
        }

        public void SetBufferTransform(int transform)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetBufferTransform, transform);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetBufferTransform}({transform})");
        }

        public void SetBufferScale(int scale)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetBufferScale, scale);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetBufferScale}({scale})");
        }

        public void DamageBuffer(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.DamageBuffer, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.DamageBuffer}({x},{y},{width},{height})");
        }

        public void Offset(int x, int y)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Offset, x, y);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Offset}({x},{y})");
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            Attach,
            Damage,
            Frame,
            SetOpaqueRegion,
            SetInputRegion,
            Commit,
            SetBufferTransform,
            SetBufferScale,
            DamageBuffer,
            Offset
        }

        public Action<WlSurface, WaylandObject> enter;
        public Action<WlSurface, WaylandObject> leave;
        public enum EventOpcode : ushort
        {
            Enter,
            Leave
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Enter:
                {
                    var output = connection[(uint)arguments[0]];
                    if (this.enter != null)
                    {
                        this.enter.Invoke(this, output);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Enter}({this},{output})");
                    }

                    break;
                }

                case EventOpcode.Leave:
                {
                    var output = connection[(uint)arguments[0]];
                    if (this.leave != null)
                    {
                        this.leave.Invoke(this, output);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Leave}({this},{output})");
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
                case EventOpcode.Enter:
                    return new WaylandType[]{WaylandType.Object, };
                case EventOpcode.Leave:
                    return new WaylandType[]{WaylandType.Object, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
