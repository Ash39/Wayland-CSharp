using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// child surface positioner
    /// </summary>
    public partial class XdgPositioner : WaylandObject
    {
        public const string INTERFACE = "xdg_positioner";
        public XdgPositioner(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// destroy the xdg_positioner object
        /// </summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        /// <summary>
        /// set the size of the to-be positioned rectangle
        /// </summary>
        public void SetSize(int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetSize, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetSize}({width},{height})");
        }

        /// <summary>
        /// set the anchor rectangle within the parent surface
        /// </summary>
        public void SetAnchorRect(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetAnchorRect, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetAnchorRect}({x},{y},{width},{height})");
        }

        /// <summary>
        /// set anchor rectangle anchor
        /// </summary>
        public void SetAnchor(uint anchor)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetAnchor, anchor);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetAnchor}({anchor})");
        }

        /// <summary>
        /// set child surface gravity
        /// </summary>
        public void SetGravity(uint gravity)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetGravity, gravity);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetGravity}({gravity})");
        }

        /// <summary>
        /// set the adjustment to be done when constrained
        /// </summary>
        public void SetConstraintAdjustment(uint constraint_adjustment)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetConstraintAdjustment, constraint_adjustment);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetConstraintAdjustment}({constraint_adjustment})");
        }

        /// <summary>
        /// set surface position offset
        /// </summary>
        public void SetOffset(int x, int y)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetOffset, x, y);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetOffset}({x},{y})");
        }

        /// <summary>
        /// continuously reconstrain the surface
        /// </summary>
        public void SetReactive()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetReactive);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetReactive}()");
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetParentSize(int parent_width, int parent_height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetParentSize, parent_width, parent_height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetParentSize}({parent_width},{parent_height})");
        }

        /// <summary>
        /// set parent configure this is a response to
        /// </summary>
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
