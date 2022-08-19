using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///child surface positioner
    ///<para>
    ///The xdg_positioner provides a collection of rules for the placement of a
    ///child surface relative to a parent surface. Rules can be defined to ensure
    ///the child surface remains within the visible area's borders, and to
    ///specify how the child surface changes its position, such as sliding along
    ///an axis, or flipping around a rectangle. These positioner-created rules are
    ///constrained by the requirement that a child surface must intersect with or
    ///be at least partially adjacent to its parent surface.
    ///</para>
    ///<para>
    ///See the various requests for details about possible rules.
    ///</para>
    ///<para>
    ///At the time of the request, the compositor makes a copy of the rules
    ///specified by the xdg_positioner. Thus, after the request is complete the
    ///xdg_positioner object can be destroyed or reused; further changes to the
    ///object will have no effect on previous usages.
    ///</para>
    ///<para>
    ///For an xdg_positioner object to be considered complete, it must have a
    ///non-zero size set by set_size, and a non-zero anchor rectangle set by
    ///set_anchor_rect. Passing an incomplete xdg_positioner object when
    ///positioning a surface raises an error.
    ///</para>
    ///</Summary>
    public partial class XdgPositioner : WaylandObject
    {
        public const string INTERFACE = "xdg_positioner";
        public XdgPositioner(uint factoryId, ref uint id, WaylandConnection connection, uint version = 5) : base(factoryId, ref id, version, connection)
        {
        }

        ///<Summary>
        ///destroy the xdg_positioner object
        ///<para>
        ///Notify the compositor that the xdg_positioner will no longer be used.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        ///<Summary>
        ///set the size of the to-be positioned rectangle
        ///<para>
        ///Set the size of the surface that is to be positioned with the positioner
        ///object. The size is in surface-local coordinates and corresponds to the
        ///window geometry. See xdg_surface.set_window_geometry.
        ///</para>
        ///<para>
        ///If a zero or negative size is set the invalid_input error is raised.
        ///</para>
        ///</Summary>
        ///<param name = "width"> width of positioned rectangle </param>
        ///<param name = "height"> height of positioned rectangle </param>
        public void SetSize(int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetSize, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetSize}({width},{height})");
        }

        ///<Summary>
        ///set the anchor rectangle within the parent surface
        ///<para>
        ///Specify the anchor rectangle within the parent surface that the child
        ///surface will be placed relative to. The rectangle is relative to the
        ///window geometry as defined by xdg_surface.set_window_geometry of the
        ///parent surface.
        ///</para>
        ///<para>
        ///When the xdg_positioner object is used to position a child surface, the
        ///anchor rectangle may not extend outside the window geometry of the
        ///positioned child's parent surface.
        ///</para>
        ///<para>
        ///If a negative size is set the invalid_input error is raised.
        ///</para>
        ///</Summary>
        ///<param name = "x"> x position of anchor rectangle </param>
        ///<param name = "y"> y position of anchor rectangle </param>
        ///<param name = "width"> width of anchor rectangle </param>
        ///<param name = "height"> height of anchor rectangle </param>
        public void SetAnchorRect(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetAnchorRect, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetAnchorRect}({x},{y},{width},{height})");
        }

        ///<Summary>
        ///set anchor rectangle anchor
        ///<para>
        ///Defines the anchor point for the anchor rectangle. The specified anchor
        ///is used derive an anchor point that the child surface will be
        ///positioned relative to. If a corner anchor is set (e.g. 'top_left' or
        ///'bottom_right'), the anchor point will be at the specified corner;
        ///otherwise, the derived anchor point will be centered on the specified
        ///edge, or in the center of the anchor rectangle if no edge is specified.
        ///</para>
        ///</Summary>
        ///<param name = "anchor"> anchor </param>
        public void SetAnchor(AnchorFlag anchor)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetAnchor, (uint)anchor);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetAnchor}({(uint)anchor})");
        }

        ///<Summary>
        ///set child surface gravity
        ///<para>
        ///Defines in what direction a surface should be positioned, relative to
        ///the anchor point of the parent surface. If a corner gravity is
        ///specified (e.g. 'bottom_right' or 'top_left'), then the child surface
        ///will be placed towards the specified gravity; otherwise, the child
        ///surface will be centered over the anchor point on any axis that had no
        ///gravity specified.
        ///</para>
        ///</Summary>
        ///<param name = "gravity"> gravity direction </param>
        public void SetGravity(GravityFlag gravity)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetGravity, (uint)gravity);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetGravity}({(uint)gravity})");
        }

        ///<Summary>
        ///set the adjustment to be done when constrained
        ///<para>
        ///Specify how the window should be positioned if the originally intended
        ///position caused the surface to be constrained, meaning at least
        ///partially outside positioning boundaries set by the compositor. The
        ///adjustment is set by constructing a bitmask describing the adjustment to
        ///be made when the surface is constrained on that axis.
        ///</para>
        ///<para>
        ///If no bit for one axis is set, the compositor will assume that the child
        ///surface should not change its position on that axis when constrained.
        ///</para>
        ///<para>
        ///If more than one bit for one axis is set, the order of how adjustments
        ///are applied is specified in the corresponding adjustment descriptions.
        ///</para>
        ///<para>
        ///The default adjustment is none.
        ///</para>
        ///</Summary>
        ///<param name = "constraint_adjustment"> bit mask of constraint adjustments </param>
        public void SetConstraintAdjustment(uint constraint_adjustment)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetConstraintAdjustment, constraint_adjustment);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetConstraintAdjustment}({constraint_adjustment})");
        }

        ///<Summary>
        ///set surface position offset
        ///<para>
        ///Specify the surface position offset relative to the position of the
        ///anchor on the anchor rectangle and the anchor on the surface. For
        ///example if the anchor of the anchor rectangle is at (x, y), the surface
        ///has the gravity bottom|right, and the offset is (ox, oy), the calculated
        ///surface position will be (x + ox, y + oy). The offset position of the
        ///surface is the one used for constraint testing. See
        ///set_constraint_adjustment.
        ///</para>
        ///<para>
        ///An example use case is placing a popup menu on top of a user interface
        ///element, while aligning the user interface element of the parent surface
        ///with some user interface element placed somewhere in the popup surface.
        ///</para>
        ///</Summary>
        ///<param name = "x"> surface position x offset </param>
        ///<param name = "y"> surface position y offset </param>
        public void SetOffset(int x, int y)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetOffset, x, y);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetOffset}({x},{y})");
        }

        ///<Summary>
        ///continuously reconstrain the surface
        ///<para>
        ///When set reactive, the surface is reconstrained if the conditions used
        ///for constraining changed, e.g. the parent window moved.
        ///</para>
        ///<para>
        ///If the conditions changed and the popup was reconstrained, an
        ///xdg_popup.configure event is sent with updated geometry, followed by an
        ///xdg_surface.configure event.
        ///</para>
        ///</Summary>
        public void SetReactive()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetReactive);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetReactive}()");
        }

        ///<param name = "parent_width"> future window geometry width of parent </param>
        ///<param name = "parent_height"> future window geometry height of parent </param>
        public void SetParentSize(int parent_width, int parent_height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetParentSize, parent_width, parent_height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetParentSize}({parent_width},{parent_height})");
        }

        ///<Summary>
        ///set parent configure this is a response to
        ///<para>
        ///Set the serial of an xdg_surface.configure event this positioner will be
        ///used in response to. The compositor may use this information together
        ///with set_parent_size to determine what future state the popup should be
        ///constrained using.
        ///</para>
        ///</Summary>
        ///<param name = "serial"> serial of parent configure event </param>
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

        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///invalid input provided
            ///</Summary>
            InvalidInput = 0,
        }

        public enum AnchorFlag : uint
        {
            ///<Summary>
            ///
            ///</Summary>
            None = 0,
            ///<Summary>
            ///
            ///</Summary>
            Top = 1,
            ///<Summary>
            ///
            ///</Summary>
            Bottom = 2,
            ///<Summary>
            ///
            ///</Summary>
            Left = 3,
            ///<Summary>
            ///
            ///</Summary>
            Right = 4,
            ///<Summary>
            ///
            ///</Summary>
            TopLeft = 5,
            ///<Summary>
            ///
            ///</Summary>
            BottomLeft = 6,
            ///<Summary>
            ///
            ///</Summary>
            TopRight = 7,
            ///<Summary>
            ///
            ///</Summary>
            BottomRight = 8,
        }

        public enum GravityFlag : uint
        {
            ///<Summary>
            ///
            ///</Summary>
            None = 0,
            ///<Summary>
            ///
            ///</Summary>
            Top = 1,
            ///<Summary>
            ///
            ///</Summary>
            Bottom = 2,
            ///<Summary>
            ///
            ///</Summary>
            Left = 3,
            ///<Summary>
            ///
            ///</Summary>
            Right = 4,
            ///<Summary>
            ///
            ///</Summary>
            TopLeft = 5,
            ///<Summary>
            ///
            ///</Summary>
            BottomLeft = 6,
            ///<Summary>
            ///
            ///</Summary>
            TopRight = 7,
            ///<Summary>
            ///
            ///</Summary>
            BottomRight = 8,
        }

        ///<Summary>
        ///constraint adjustments
        ///<para>
        ///The constraint adjustment value define ways the compositor will adjust
        ///the position of the surface, if the unadjusted position would result
        ///in the surface being partly constrained.
        ///</para>
        ///<para>
        ///Whether a surface is considered 'constrained' is left to the compositor
        ///to determine. For example, the surface may be partly outside the
        ///compositor's defined 'work area', thus necessitating the child surface's
        ///position be adjusted until it is entirely inside the work area.
        ///</para>
        ///<para>
        ///The adjustments can be combined, according to a defined precedence: 1)
        ///Flip, 2) Slide, 3) Resize.
        ///</para>
        ///</Summary>
        public enum ConstraintAdjustmentFlag : uint
        {
            ///<Summary>
            ///
            ///</Summary>
            None = 0,
            ///<Summary>
            ///
            ///</Summary>
            SlideX = 1,
            ///<Summary>
            ///
            ///</Summary>
            SlideY = 2,
            ///<Summary>
            ///
            ///</Summary>
            FlipX = 4,
            ///<Summary>
            ///
            ///</Summary>
            FlipY = 8,
            ///<Summary>
            ///
            ///</Summary>
            ResizeX = 16,
            ///<Summary>
            ///
            ///</Summary>
            ResizeY = 32,
        }
    }
}
