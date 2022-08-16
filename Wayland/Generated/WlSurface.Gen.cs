using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///an onscreen surface
    ///<para>
    ///A surface is a rectangular area that may be displayed on zero
    ///or more outputs, and shown any number of times at the compositor's
    ///discretion. They can present wl_buffers, receive user input, and
    ///define a local coordinate system.
    ///</para>
    ///<para>
    ///The size of a surface (and relative positions on it) is described
    ///in surface-local coordinates, which may differ from the buffer
    ///coordinates of the pixel content, in case a buffer_transform
    ///or a buffer_scale is used.
    ///</para>
    ///<para>
    ///A surface without a "role" is fairly useless: a compositor does
    ///not know where, when or how to present it. The role is the
    ///purpose of a wl_surface. Examples of roles are a cursor for a
    ///pointer (as set by wl_pointer.set_cursor), a drag icon
    ///(wl_data_device.start_drag), a sub-surface
    ///(wl_subcompositor.get_subsurface), and a window as defined by a
    ///shell protocol (e.g. wl_shell.get_shell_surface).
    ///</para>
    ///<para>
    ///A surface can have only one role at a time. Initially a
    ///wl_surface does not have a role. Once a wl_surface is given a
    ///role, it is set permanently for the whole lifetime of the
    ///wl_surface object. Giving the current role again is allowed,
    ///unless explicitly forbidden by the relevant interface
    ///specification.
    ///</para>
    ///<para>
    ///Surface roles are given by requests in other interfaces such as
    ///wl_pointer.set_cursor. The request should explicitly mention
    ///that this request gives a role to a wl_surface. Often, this
    ///request also creates a new protocol object that represents the
    ///role and adds additional functionality to wl_surface. When a
    ///client wants to destroy a wl_surface, they must destroy this 'role
    ///object' before the wl_surface.
    ///</para>
    ///<para>
    ///Destroying the role object does not remove the role from the
    ///wl_surface, but it may stop the wl_surface from "playing the role".
    ///For instance, if a wl_subsurface object is destroyed, the wl_surface
    ///it was created for will be unmapped and forget its position and
    ///z-order. It is allowed to create a wl_subsurface for the same
    ///wl_surface again, but it is not allowed to use the wl_surface as
    ///a cursor (cursor is a different role than sub-surface, and role
    ///switching is not allowed).
    ///</para>
    ///</Summary>
    public partial class WlSurface : WaylandObject
    {
        public const string INTERFACE = "wl_surface";
        public WlSurface(uint factoryId, ref uint id, WaylandConnection connection) : base(factoryId, ref id, 5, connection)
        {
        }

        ///<Summary>
        ///delete surface
        ///<para>
        ///Deletes the surface and invalidates its object ID.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        ///<Summary>
        ///set the surface contents
        ///<para>
        ///Set a buffer as the content of this surface.
        ///</para>
        ///<para>
        ///The new size of the surface is calculated based on the buffer
        ///size transformed by the inverse buffer_transform and the
        ///inverse buffer_scale. This means that at commit time the supplied
        ///buffer size must be an integer multiple of the buffer_scale. If
        ///that's not the case, an invalid_size error is sent.
        ///</para>
        ///<para>
        ///The x and y arguments specify the location of the new pending
        ///buffer's upper left corner, relative to the current buffer's upper
        ///left corner, in surface-local coordinates. In other words, the
        ///x and y, combined with the new surface size define in which
        ///directions the surface's size changes. Setting anything other than 0
        ///as x and y arguments is discouraged, and should instead be replaced
        ///with using the separate wl_surface.offset request.
        ///</para>
        ///<para>
        ///When the bound wl_surface version is 5 or higher, passing any
        ///non-zero x or y is a protocol violation, and will result in an
        ///'invalid_offset' error being raised. To achieve equivalent semantics,
        ///use wl_surface.offset.
        ///</para>
        ///<para>
        ///Surface contents are double-buffered state, see wl_surface.commit.
        ///</para>
        ///<para>
        ///The initial surface contents are void; there is no content.
        ///wl_surface.attach assigns the given wl_buffer as the pending
        ///wl_buffer. wl_surface.commit makes the pending wl_buffer the new
        ///surface contents, and the size of the surface becomes the size
        ///calculated from the wl_buffer, as described above. After commit,
        ///there is no pending buffer until the next attach.
        ///</para>
        ///<para>
        ///Committing a pending wl_buffer allows the compositor to read the
        ///pixels in the wl_buffer. The compositor may access the pixels at
        ///any time after the wl_surface.commit request. When the compositor
        ///will not access the pixels anymore, it will send the
        ///wl_buffer.release event. Only after receiving wl_buffer.release,
        ///the client may reuse the wl_buffer. A wl_buffer that has been
        ///attached and then replaced by another attach instead of committed
        ///will not receive a release event, and is not used by the
        ///compositor.
        ///</para>
        ///<para>
        ///If a pending wl_buffer has been committed to more than one wl_surface,
        ///the delivery of wl_buffer.release events becomes undefined. A well
        ///behaved client should not rely on wl_buffer.release events in this
        ///case. Alternatively, a client could create multiple wl_buffer objects
        ///from the same backing storage or use wp_linux_buffer_release.
        ///</para>
        ///<para>
        ///Destroying the wl_buffer after wl_buffer.release does not change
        ///the surface contents. Destroying the wl_buffer before wl_buffer.release
        ///is allowed as long as the underlying buffer storage isn't re-used (this
        ///can happen e.g. on client process termination). However, if the client
        ///destroys the wl_buffer before receiving the wl_buffer.release event and
        ///mutates the underlying buffer storage, the surface contents become
        ///undefined immediately.
        ///</para>
        ///<para>
        ///If wl_surface.attach is sent with a NULL wl_buffer, the
        ///following wl_surface.commit will remove the surface content.
        ///</para>
        ///</Summary>
        ///<param name = "buffer"> buffer of surface contents </param>
        ///<param name = "x"> surface-local x coordinate </param>
        ///<param name = "y"> surface-local y coordinate </param>
        public void Attach(WlBuffer buffer, int x, int y)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Attach, buffer.id, x, y);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Attach}({buffer.id},{x},{y})");
        }

        ///<Summary>
        ///mark part of the surface damaged
        ///<para>
        ///This request is used to describe the regions where the pending
        ///buffer is different from the current surface contents, and where
        ///the surface therefore needs to be repainted. The compositor
        ///ignores the parts of the damage that fall outside of the surface.
        ///</para>
        ///<para>
        ///Damage is double-buffered state, see wl_surface.commit.
        ///</para>
        ///<para>
        ///The damage rectangle is specified in surface-local coordinates,
        ///where x and y specify the upper left corner of the damage rectangle.
        ///</para>
        ///<para>
        ///The initial value for pending damage is empty: no damage.
        ///wl_surface.damage adds pending damage: the new pending damage
        ///is the union of old pending damage and the given rectangle.
        ///</para>
        ///<para>
        ///wl_surface.commit assigns pending damage as the current damage,
        ///and clears pending damage. The server will clear the current
        ///damage as it repaints the surface.
        ///</para>
        ///<para>
        ///Note! New clients should not use this request. Instead damage can be
        ///posted with wl_surface.damage_buffer which uses buffer coordinates
        ///instead of surface coordinates.
        ///</para>
        ///</Summary>
        ///<param name = "x"> surface-local x coordinate </param>
        ///<param name = "y"> surface-local y coordinate </param>
        ///<param name = "width"> width of damage rectangle </param>
        ///<param name = "height"> height of damage rectangle </param>
        public void Damage(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Damage, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Damage}({x},{y},{width},{height})");
        }

        ///<Summary>
        ///request a frame throttling hint
        ///<para>
        ///Request a notification when it is a good time to start drawing a new
        ///frame, by creating a frame callback. This is useful for throttling
        ///redrawing operations, and driving animations.
        ///</para>
        ///<para>
        ///When a client is animating on a wl_surface, it can use the 'frame'
        ///request to get notified when it is a good time to draw and commit the
        ///next frame of animation. If the client commits an update earlier than
        ///that, it is likely that some updates will not make it to the display,
        ///and the client is wasting resources by drawing too often.
        ///</para>
        ///<para>
        ///The frame request will take effect on the next wl_surface.commit.
        ///The notification will only be posted for one frame unless
        ///requested again. For a wl_surface, the notifications are posted in
        ///the order the frame requests were committed.
        ///</para>
        ///<para>
        ///The server must send the notifications so that a client
        ///will not send excessive updates, while still allowing
        ///the highest possible update rate for clients that wait for the reply
        ///before drawing again. The server should give some time for the client
        ///to draw and commit after sending the frame callback events to let it
        ///hit the next output refresh.
        ///</para>
        ///<para>
        ///A server should avoid signaling the frame callbacks if the
        ///surface is not visible in any way, e.g. the surface is off-screen,
        ///or completely obscured by other opaque surfaces.
        ///</para>
        ///<para>
        ///The object returned by this request will be destroyed by the
        ///compositor after the callback is fired and as such the client must not
        ///attempt to use it after that point.
        ///</para>
        ///<para>
        ///The callback_data passed in the callback is the current time, in
        ///milliseconds, with an undefined base.
        ///</para>
        ///</Summary>
        ///<returns> callback object for the frame request </returns>
        public WlCallback Frame()
        {
            uint callback = connection.Create();
            WlCallback wObject = new WlCallback(this.id, ref callback, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.Frame, callback);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Frame}({callback})");
            connection[callback] = wObject;
            return (WlCallback)connection[callback];
        }

        ///<Summary>
        ///set opaque region
        ///<para>
        ///This request sets the region of the surface that contains
        ///opaque content.
        ///</para>
        ///<para>
        ///The opaque region is an optimization hint for the compositor
        ///that lets it optimize the redrawing of content behind opaque
        ///regions.  Setting an opaque region is not required for correct
        ///behaviour, but marking transparent content as opaque will result
        ///in repaint artifacts.
        ///</para>
        ///<para>
        ///The opaque region is specified in surface-local coordinates.
        ///</para>
        ///<para>
        ///The compositor ignores the parts of the opaque region that fall
        ///outside of the surface.
        ///</para>
        ///<para>
        ///Opaque region is double-buffered state, see wl_surface.commit.
        ///</para>
        ///<para>
        ///wl_surface.set_opaque_region changes the pending opaque region.
        ///wl_surface.commit copies the pending region to the current region.
        ///Otherwise, the pending and current regions are never changed.
        ///</para>
        ///<para>
        ///The initial value for an opaque region is empty. Setting the pending
        ///opaque region has copy semantics, and the wl_region object can be
        ///destroyed immediately. A NULL wl_region causes the pending opaque
        ///region to be set to empty.
        ///</para>
        ///</Summary>
        ///<param name = "region"> opaque region of the surface </param>
        public void SetOpaqueRegion(WlRegion region)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetOpaqueRegion, region.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetOpaqueRegion}({region.id})");
        }

        ///<Summary>
        ///set input region
        ///<para>
        ///This request sets the region of the surface that can receive
        ///pointer and touch events.
        ///</para>
        ///<para>
        ///Input events happening outside of this region will try the next
        ///surface in the server surface stack. The compositor ignores the
        ///parts of the input region that fall outside of the surface.
        ///</para>
        ///<para>
        ///The input region is specified in surface-local coordinates.
        ///</para>
        ///<para>
        ///Input region is double-buffered state, see wl_surface.commit.
        ///</para>
        ///<para>
        ///wl_surface.set_input_region changes the pending input region.
        ///wl_surface.commit copies the pending region to the current region.
        ///Otherwise the pending and current regions are never changed,
        ///except cursor and icon surfaces are special cases, see
        ///wl_pointer.set_cursor and wl_data_device.start_drag.
        ///</para>
        ///<para>
        ///The initial value for an input region is infinite. That means the
        ///whole surface will accept input. Setting the pending input region
        ///has copy semantics, and the wl_region object can be destroyed
        ///immediately. A NULL wl_region causes the input region to be set
        ///to infinite.
        ///</para>
        ///</Summary>
        ///<param name = "region"> input region of the surface </param>
        public void SetInputRegion(WlRegion region)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetInputRegion, region.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetInputRegion}({region.id})");
        }

        ///<Summary>
        ///commit pending surface state
        ///<para>
        ///Surface state (input, opaque, and damage regions, attached buffers,
        ///etc.) is double-buffered. Protocol requests modify the pending state,
        ///as opposed to the current state in use by the compositor. A commit
        ///request atomically applies all pending state, replacing the current
        ///state. After commit, the new pending state is as documented for each
        ///related request.
        ///</para>
        ///<para>
        ///On commit, a pending wl_buffer is applied first, and all other state
        ///second. This means that all coordinates in double-buffered state are
        ///relative to the new wl_buffer coming into use, except for
        ///wl_surface.attach itself. If there is no pending wl_buffer, the
        ///coordinates are relative to the current surface contents.
        ///</para>
        ///<para>
        ///All requests that need a commit to become effective are documented
        ///to affect double-buffered state.
        ///</para>
        ///<para>
        ///Other interfaces may add further double-buffered surface state.
        ///</para>
        ///</Summary>
        public void Commit()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Commit);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Commit}()");
        }

        ///<Summary>
        ///sets the buffer transformation
        ///<para>
        ///This request sets an optional transformation on how the compositor
        ///interprets the contents of the buffer attached to the surface. The
        ///accepted values for the transform parameter are the values for
        ///wl_output.transform.
        ///</para>
        ///<para>
        ///Buffer transform is double-buffered state, see wl_surface.commit.
        ///</para>
        ///<para>
        ///A newly created surface has its buffer transformation set to normal.
        ///</para>
        ///<para>
        ///wl_surface.set_buffer_transform changes the pending buffer
        ///transformation. wl_surface.commit copies the pending buffer
        ///transformation to the current one. Otherwise, the pending and current
        ///values are never changed.
        ///</para>
        ///<para>
        ///The purpose of this request is to allow clients to render content
        ///according to the output transform, thus permitting the compositor to
        ///use certain optimizations even if the display is rotated. Using
        ///hardware overlays and scanning out a client buffer for fullscreen
        ///surfaces are examples of such optimizations. Those optimizations are
        ///highly dependent on the compositor implementation, so the use of this
        ///request should be considered on a case-by-case basis.
        ///</para>
        ///<para>
        ///Note that if the transform value includes 90 or 270 degree rotation,
        ///the width of the buffer will become the surface height and the height
        ///of the buffer will become the surface width.
        ///</para>
        ///<para>
        ///If transform is not one of the values from the
        ///wl_output.transform enum the invalid_transform protocol error
        ///is raised.
        ///</para>
        ///</Summary>
        ///<param name = "transform"> transform for interpreting buffer contents </param>
        public void SetBufferTransform(WlOutput.TransformFlag transform)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetBufferTransform, (int)transform);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetBufferTransform}({(int)transform})");
        }

        ///<Summary>
        ///sets the buffer scaling factor
        ///<para>
        ///This request sets an optional scaling factor on how the compositor
        ///interprets the contents of the buffer attached to the window.
        ///</para>
        ///<para>
        ///Buffer scale is double-buffered state, see wl_surface.commit.
        ///</para>
        ///<para>
        ///A newly created surface has its buffer scale set to 1.
        ///</para>
        ///<para>
        ///wl_surface.set_buffer_scale changes the pending buffer scale.
        ///wl_surface.commit copies the pending buffer scale to the current one.
        ///Otherwise, the pending and current values are never changed.
        ///</para>
        ///<para>
        ///The purpose of this request is to allow clients to supply higher
        ///resolution buffer data for use on high resolution outputs. It is
        ///intended that you pick the same buffer scale as the scale of the
        ///output that the surface is displayed on. This means the compositor
        ///can avoid scaling when rendering the surface on that output.
        ///</para>
        ///<para>
        ///Note that if the scale is larger than 1, then you have to attach
        ///a buffer that is larger (by a factor of scale in each dimension)
        ///than the desired surface size.
        ///</para>
        ///<para>
        ///If scale is not positive the invalid_scale protocol error is
        ///raised.
        ///</para>
        ///</Summary>
        ///<param name = "scale"> positive scale for interpreting buffer contents </param>
        public void SetBufferScale(int scale)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetBufferScale, scale);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetBufferScale}({scale})");
        }

        ///<Summary>
        ///mark part of the surface damaged using buffer coordinates
        ///<para>
        ///This request is used to describe the regions where the pending
        ///buffer is different from the current surface contents, and where
        ///the surface therefore needs to be repainted. The compositor
        ///ignores the parts of the damage that fall outside of the surface.
        ///</para>
        ///<para>
        ///Damage is double-buffered state, see wl_surface.commit.
        ///</para>
        ///<para>
        ///The damage rectangle is specified in buffer coordinates,
        ///where x and y specify the upper left corner of the damage rectangle.
        ///</para>
        ///<para>
        ///The initial value for pending damage is empty: no damage.
        ///wl_surface.damage_buffer adds pending damage: the new pending
        ///damage is the union of old pending damage and the given rectangle.
        ///</para>
        ///<para>
        ///wl_surface.commit assigns pending damage as the current damage,
        ///and clears pending damage. The server will clear the current
        ///damage as it repaints the surface.
        ///</para>
        ///<para>
        ///This request differs from wl_surface.damage in only one way - it
        ///takes damage in buffer coordinates instead of surface-local
        ///coordinates. While this generally is more intuitive than surface
        ///coordinates, it is especially desirable when using wp_viewport
        ///or when a drawing library (like EGL) is unaware of buffer scale
        ///and buffer transform.
        ///</para>
        ///<para>
        ///Note: Because buffer transformation changes and damage requests may
        ///be interleaved in the protocol stream, it is impossible to determine
        ///the actual mapping between surface and buffer damage until
        ///wl_surface.commit time. Therefore, compositors wishing to take both
        ///kinds of damage into account will have to accumulate damage from the
        ///two requests separately and only transform from one to the other
        ///after receiving the wl_surface.commit.
        ///</para>
        ///</Summary>
        ///<param name = "x"> buffer-local x coordinate </param>
        ///<param name = "y"> buffer-local y coordinate </param>
        ///<param name = "width"> width of damage rectangle </param>
        ///<param name = "height"> height of damage rectangle </param>
        public void DamageBuffer(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.DamageBuffer, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.DamageBuffer}({x},{y},{width},{height})");
        }

        ///<Summary>
        ///set the surface contents offset
        ///<para>
        ///The x and y arguments specify the location of the new pending
        ///buffer's upper left corner, relative to the current buffer's upper
        ///left corner, in surface-local coordinates. In other words, the
        ///x and y, combined with the new surface size define in which
        ///directions the surface's size changes.
        ///</para>
        ///<para>
        ///Surface location offset is double-buffered state, see
        ///wl_surface.commit.
        ///</para>
        ///<para>
        ///This request is semantically equivalent to and the replaces the x and y
        ///arguments in the wl_surface.attach request in wl_surface versions prior
        ///to 5. See wl_surface.attach for details.
        ///</para>
        ///</Summary>
        ///<param name = "x"> surface-local x coordinate </param>
        ///<param name = "y"> surface-local y coordinate </param>
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

        ///<Summary>
        ///surface enters an output
        ///<para>
        ///This is emitted whenever a surface's creation, movement, or resizing
        ///results in some part of it being within the scanout region of an
        ///output.
        ///</para>
        ///<para>
        ///Note that a surface may be overlapping with zero or more outputs.
        ///</para>
        ///</Summary>
        public Action<WlSurface, WaylandObject> enter;
        ///<Summary>
        ///surface leaves an output
        ///<para>
        ///This is emitted whenever a surface's creation, movement, or resizing
        ///results in it no longer having any part of it within the scanout region
        ///of an output.
        ///</para>
        ///<para>
        ///Clients should not use the number of outputs the surface is on for frame
        ///throttling purposes. The surface might be hidden even if no leave event
        ///has been sent, and the compositor might expect new surface content
        ///updates even if no enter event has been sent. The frame event should be
        ///used instead.
        ///</para>
        ///</Summary>
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

        ///<Summary>
        ///wl_surface error values
        ///<para>
        ///These errors can be emitted in response to wl_surface requests.
        ///</para>
        ///</Summary>
        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///buffer scale value is invalid
            ///</Summary>
            InvalidScale = 0,
            ///<Summary>
            ///buffer transform value is invalid
            ///</Summary>
            InvalidTransform = 1,
            ///<Summary>
            ///buffer size is invalid
            ///</Summary>
            InvalidSize = 2,
            ///<Summary>
            ///buffer offset is invalid
            ///</Summary>
            InvalidOffset = 3,
        }
    }
}
