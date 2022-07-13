using System;
using System.Collections.Generic;

namespace Wayland
{
    public partial class XdgPositioner : WaylandObject
    {
        public const string INTERFACE = "xdg_positioner";
        public XdgPositioner(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        public void SetSize(int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetSize, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetSize}({width},{height})");
        }

        public void SetAnchorRect(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetAnchorRect, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetAnchorRect}({x},{y},{width},{height})");
        }

        public void SetAnchor(uint anchor)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetAnchor, anchor);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetAnchor}({anchor})");
        }

        public void SetGravity(uint gravity)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetGravity, gravity);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetGravity}({gravity})");
        }

        public void SetConstraintAdjustment(uint constraint_adjustment)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetConstraintAdjustment, constraint_adjustment);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetConstraintAdjustment}({constraint_adjustment})");
        }

        public void SetOffset(int x, int y)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetOffset, x, y);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetOffset}({x},{y})");
        }

        public void SetReactive()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetReactive);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetReactive}()");
        }

        public void SetParentSize(int parent_width, int parent_height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetParentSize, parent_width, parent_height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetParentSize}({parent_width},{parent_height})");
        }

        public void SetParentConfigure(uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetParentConfigure, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetParentConfigure}({serial})");
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            SetSize,
            SetAnchorRect,
            SetAnchor,
            SetGravity,
            SetConstraintAdjustment,
            SetOffset,
            SetReactive,
            SetParentSize,
            SetParentConfigure
        }

        public enum EventOpcode : ushort
        {
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        public override WaylandType[] WaylandTypes(ushort opCode)
        {
            switch ((EventOpcode)opCode)
            {
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
