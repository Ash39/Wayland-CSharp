using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///toplevel surface
    ///<para>
    ///This interface defines an xdg_surface role which allows a surface to,
    ///among other things, set window-like properties such as maximize,
    ///fullscreen, and minimize, set application-specific metadata like title and
    ///id, and well as trigger user interactive operations such as interactive
    ///resize and move.
    ///</para>
    ///<para>
    ///Unmapping an xdg_toplevel means that the surface cannot be shown
    ///by the compositor until it is explicitly mapped again.
    ///All active operations (e.g., move, resize) are canceled and all
    ///attributes (e.g. title, state, stacking, ...) are discarded for
    ///an xdg_toplevel surface when it is unmapped. The xdg_toplevel returns to
    ///the state it had right after xdg_surface.get_toplevel. The client
    ///can re-map the toplevel by perfoming a commit without any buffer
    ///attached, waiting for a configure event and handling it as usual (see
    ///xdg_surface description).
    ///</para>
    ///<para>
    ///Attaching a null buffer to a toplevel unmaps the surface.
    ///</para>
    ///</Summary>
    public partial class XdgToplevel : WaylandObject
    {
        public const string INTERFACE = "xdg_toplevel";
        public XdgToplevel(uint factoryId, ref uint id, WaylandConnection connection) : base(factoryId, ref id, 5, connection)
        {
        }

        ///<Summary>
        ///destroy the xdg_toplevel
        ///<para>
        ///This request destroys the role surface and unmaps the surface;
        ///see "Unmapping" behavior in interface section for details.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        ///<Summary>
        ///set the parent of this surface
        ///<para>
        ///Set the "parent" of this surface. This surface should be stacked
        ///above the parent surface and all other ancestor surfaces.
        ///</para>
        ///<para>
        ///Parent surfaces should be set on dialogs, toolboxes, or other
        ///"auxiliary" surfaces, so that the parent is raised when the dialog
        ///is raised.
        ///</para>
        ///<para>
        ///Setting a null parent for a child surface unsets its parent. Setting
        ///a null parent for a surface which currently has no parent is a no-op.
        ///</para>
        ///<para>
        ///Only mapped surfaces can have child surfaces. Setting a parent which
        ///is not mapped is equivalent to setting a null parent. If a surface
        ///becomes unmapped, its children's parent is set to the parent of
        ///the now-unmapped surface. If the now-unmapped surface has no parent,
        ///its children's parent is unset. If the now-unmapped surface becomes
        ///mapped again, its parent-child relationship is not restored.
        ///</para>
        ///</Summary>
        ///<param name = "parent">  </param>
        public void SetParent(XdgToplevel parent)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetParent, parent.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetParent}({parent.id})");
        }

        ///<Summary>
        ///set surface title
        ///<para>
        ///Set a short title for the surface.
        ///</para>
        ///<para>
        ///This string may be used to identify the surface in a task bar,
        ///window list, or other user interface elements provided by the
        ///compositor.
        ///</para>
        ///<para>
        ///The string must be encoded in UTF-8.
        ///</para>
        ///</Summary>
        ///<param name = "title">  </param>
        public void SetTitle(string title)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetTitle, title);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetTitle}({title})");
        }

        ///<Summary>
        ///set application ID
        ///<para>
        ///Set an application identifier for the surface.
        ///</para>
        ///<para>
        ///The app ID identifies the general class of applications to which
        ///the surface belongs. The compositor can use this to group multiple
        ///surfaces together, or to determine how to launch a new application.
        ///</para>
        ///<para>
        ///For D-Bus activatable applications, the app ID is used as the D-Bus
        ///service name.
        ///</para>
        ///<para>
        ///The compositor shell will try to group application surfaces together
        ///by their app ID. As a best practice, it is suggested to select app
        ///ID's that match the basename of the application's .desktop file.
        ///For example, "org.freedesktop.FooViewer" where the .desktop file is
        ///"org.freedesktop.FooViewer.desktop".
        ///</para>
        ///<para>
        ///Like other properties, a set_app_id request can be sent after the
        ///xdg_toplevel has been mapped to update the property.
        ///</para>
        ///<para>
        ///See the desktop-entry specification [0] for more details on
        ///application identifiers and how they relate to well-known D-Bus
        ///names and .desktop files.
        ///</para>
        ///<para>
        ///[0] http://standards.freedesktop.org/desktop-entry-spec/
        ///</para>
        ///</Summary>
        ///<param name = "app_id">  </param>
        public void SetAppId(string app_id)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetAppId, app_id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetAppId}({app_id})");
        }

        ///<Summary>
        ///show the window menu
        ///<para>
        ///Clients implementing client-side decorations might want to show
        ///a context menu when right-clicking on the decorations, giving the
        ///user a menu that they can use to maximize or minimize the window.
        ///</para>
        ///<para>
        ///This request asks the compositor to pop up such a window menu at
        ///the given position, relative to the local surface coordinates of
        ///the parent surface. There are no guarantees as to what menu items
        ///the window menu contains.
        ///</para>
        ///<para>
        ///This request must be used in response to some sort of user action
        ///like a button press, key press, or touch down event.
        ///</para>
        ///</Summary>
        ///<param name = "seat"> the wl_seat of the user event </param>
        ///<param name = "serial"> the serial of the user event </param>
        ///<param name = "x"> the x position to pop up the window menu at </param>
        ///<param name = "y"> the y position to pop up the window menu at </param>
        public void ShowWindowMenu(WlSeat seat, uint serial, int x, int y)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.ShowWindowMenu, seat.id, serial, x, y);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.ShowWindowMenu}({seat.id},{serial},{x},{y})");
        }

        ///<Summary>
        ///start an interactive move
        ///<para>
        ///Start an interactive, user-driven move of the surface.
        ///</para>
        ///<para>
        ///This request must be used in response to some sort of user action
        ///like a button press, key press, or touch down event. The passed
        ///serial is used to determine the type of interactive move (touch,
        ///pointer, etc).
        ///</para>
        ///<para>
        ///The server may ignore move requests depending on the state of
        ///the surface (e.g. fullscreen or maximized), or if the passed serial
        ///is no longer valid.
        ///</para>
        ///<para>
        ///If triggered, the surface will lose the focus of the device
        ///(wl_pointer, wl_touch, etc) used for the move. It is up to the
        ///compositor to visually indicate that the move is taking place, such as
        ///updating a pointer cursor, during the move. There is no guarantee
        ///that the device focus will return when the move is completed.
        ///</para>
        ///</Summary>
        ///<param name = "seat"> the wl_seat of the user event </param>
        ///<param name = "serial"> the serial of the user event </param>
        public void Move(WlSeat seat, uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Move, seat.id, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Move}({seat.id},{serial})");
        }

        ///<Summary>
        ///start an interactive resize
        ///<para>
        ///Start a user-driven, interactive resize of the surface.
        ///</para>
        ///<para>
        ///This request must be used in response to some sort of user action
        ///like a button press, key press, or touch down event. The passed
        ///serial is used to determine the type of interactive resize (touch,
        ///pointer, etc).
        ///</para>
        ///<para>
        ///The server may ignore resize requests depending on the state of
        ///the surface (e.g. fullscreen or maximized).
        ///</para>
        ///<para>
        ///If triggered, the client will receive configure events with the
        ///"resize" state enum value and the expected sizes. See the "resize"
        ///enum value for more details about what is required. The client
        ///must also acknowledge configure events using "ack_configure". After
        ///the resize is completed, the client will receive another "configure"
        ///event without the resize state.
        ///</para>
        ///<para>
        ///If triggered, the surface also will lose the focus of the device
        ///(wl_pointer, wl_touch, etc) used for the resize. It is up to the
        ///compositor to visually indicate that the resize is taking place,
        ///such as updating a pointer cursor, during the resize. There is no
        ///guarantee that the device focus will return when the resize is
        ///completed.
        ///</para>
        ///<para>
        ///The edges parameter specifies how the surface should be resized, and
        ///is one of the values of the resize_edge enum. Values not matching
        ///a variant of the enum will cause a protocol error. The compositor
        ///may use this information to update the surface position for example
        ///when dragging the top left corner. The compositor may also use
        ///this information to adapt its behavior, e.g. choose an appropriate
        ///cursor image.
        ///</para>
        ///</Summary>
        ///<param name = "seat"> the wl_seat of the user event </param>
        ///<param name = "serial"> the serial of the user event </param>
        ///<param name = "edges"> which edge or corner is being dragged </param>
        public void Resize(WlSeat seat, uint serial, ResizeEdgeFlag edges)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Resize, seat.id, serial, (uint)edges);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Resize}({seat.id},{serial},{(uint)edges})");
        }

        ///<Summary>
        ///set the maximum size
        ///<para>
        ///Set a maximum size for the window.
        ///</para>
        ///<para>
        ///The client can specify a maximum size so that the compositor does
        ///not try to configure the window beyond this size.
        ///</para>
        ///<para>
        ///The width and height arguments are in window geometry coordinates.
        ///See xdg_surface.set_window_geometry.
        ///</para>
        ///<para>
        ///Values set in this way are double-buffered. They will get applied
        ///on the next commit.
        ///</para>
        ///<para>
        ///The compositor can use this information to allow or disallow
        ///different states like maximize or fullscreen and draw accurate
        ///animations.
        ///</para>
        ///<para>
        ///Similarly, a tiling window manager may use this information to
        ///place and resize client windows in a more effective way.
        ///</para>
        ///<para>
        ///The client should not rely on the compositor to obey the maximum
        ///size. The compositor may decide to ignore the values set by the
        ///client and request a larger size.
        ///</para>
        ///<para>
        ///If never set, or a value of zero in the request, means that the
        ///client has no expected maximum size in the given dimension.
        ///As a result, a client wishing to reset the maximum size
        ///to an unspecified state can use zero for width and height in the
        ///request.
        ///</para>
        ///<para>
        ///Requesting a maximum size to be smaller than the minimum size of
        ///a surface is illegal and will result in a protocol error.
        ///</para>
        ///<para>
        ///The width and height must be greater than or equal to zero. Using
        ///strictly negative values for width and height will result in a
        ///protocol error.
        ///</para>
        ///</Summary>
        ///<param name = "width">  </param>
        ///<param name = "height">  </param>
        public void SetMaxSize(int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMaxSize, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMaxSize}({width},{height})");
        }

        ///<Summary>
        ///set the minimum size
        ///<para>
        ///Set a minimum size for the window.
        ///</para>
        ///<para>
        ///The client can specify a minimum size so that the compositor does
        ///not try to configure the window below this size.
        ///</para>
        ///<para>
        ///The width and height arguments are in window geometry coordinates.
        ///See xdg_surface.set_window_geometry.
        ///</para>
        ///<para>
        ///Values set in this way are double-buffered. They will get applied
        ///on the next commit.
        ///</para>
        ///<para>
        ///The compositor can use this information to allow or disallow
        ///different states like maximize or fullscreen and draw accurate
        ///animations.
        ///</para>
        ///<para>
        ///Similarly, a tiling window manager may use this information to
        ///place and resize client windows in a more effective way.
        ///</para>
        ///<para>
        ///The client should not rely on the compositor to obey the minimum
        ///size. The compositor may decide to ignore the values set by the
        ///client and request a smaller size.
        ///</para>
        ///<para>
        ///If never set, or a value of zero in the request, means that the
        ///client has no expected minimum size in the given dimension.
        ///As a result, a client wishing to reset the minimum size
        ///to an unspecified state can use zero for width and height in the
        ///request.
        ///</para>
        ///<para>
        ///Requesting a minimum size to be larger than the maximum size of
        ///a surface is illegal and will result in a protocol error.
        ///</para>
        ///<para>
        ///The width and height must be greater than or equal to zero. Using
        ///strictly negative values for width and height will result in a
        ///protocol error.
        ///</para>
        ///</Summary>
        ///<param name = "width">  </param>
        ///<param name = "height">  </param>
        public void SetMinSize(int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMinSize, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMinSize}({width},{height})");
        }

        ///<Summary>
        ///maximize the window
        ///<para>
        ///Maximize the surface.
        ///</para>
        ///<para>
        ///After requesting that the surface should be maximized, the compositor
        ///will respond by emitting a configure event. Whether this configure
        ///actually sets the window maximized is subject to compositor policies.
        ///The client must then update its content, drawing in the configured
        ///state. The client must also acknowledge the configure when committing
        ///the new content (see ack_configure).
        ///</para>
        ///<para>
        ///It is up to the compositor to decide how and where to maximize the
        ///surface, for example which output and what region of the screen should
        ///be used.
        ///</para>
        ///<para>
        ///If the surface was already maximized, the compositor will still emit
        ///a configure event with the "maximized" state.
        ///</para>
        ///<para>
        ///If the surface is in a fullscreen state, this request has no direct
        ///effect. It may alter the state the surface is returned to when
        ///unmaximized unless overridden by the compositor.
        ///</para>
        ///</Summary>
        public void SetMaximized()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMaximized);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMaximized}()");
        }

        ///<Summary>
        ///unmaximize the window
        ///<para>
        ///Unmaximize the surface.
        ///</para>
        ///<para>
        ///After requesting that the surface should be unmaximized, the compositor
        ///will respond by emitting a configure event. Whether this actually
        ///un-maximizes the window is subject to compositor policies.
        ///If available and applicable, the compositor will include the window
        ///geometry dimensions the window had prior to being maximized in the
        ///configure event. The client must then update its content, drawing it in
        ///the configured state. The client must also acknowledge the configure
        ///when committing the new content (see ack_configure).
        ///</para>
        ///<para>
        ///It is up to the compositor to position the surface after it was
        ///unmaximized; usually the position the surface had before maximizing, if
        ///applicable.
        ///</para>
        ///<para>
        ///If the surface was already not maximized, the compositor will still
        ///emit a configure event without the "maximized" state.
        ///</para>
        ///<para>
        ///If the surface is in a fullscreen state, this request has no direct
        ///effect. It may alter the state the surface is returned to when
        ///unmaximized unless overridden by the compositor.
        ///</para>
        ///</Summary>
        public void UnsetMaximized()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.UnsetMaximized);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.UnsetMaximized}()");
        }

        ///<Summary>
        ///set the window as fullscreen on an output
        ///<para>
        ///Make the surface fullscreen.
        ///</para>
        ///<para>
        ///After requesting that the surface should be fullscreened, the
        ///compositor will respond by emitting a configure event. Whether the
        ///client is actually put into a fullscreen state is subject to compositor
        ///policies. The client must also acknowledge the configure when
        ///committing the new content (see ack_configure).
        ///</para>
        ///<para>
        ///The output passed by the request indicates the client's preference as
        ///to which display it should be set fullscreen on. If this value is NULL,
        ///it's up to the compositor to choose which display will be used to map
        ///this surface.
        ///</para>
        ///<para>
        ///If the surface doesn't cover the whole output, the compositor will
        ///position the surface in the center of the output and compensate with
        ///with border fill covering the rest of the output. The content of the
        ///border fill is undefined, but should be assumed to be in some way that
        ///attempts to blend into the surrounding area (e.g. solid black).
        ///</para>
        ///<para>
        ///If the fullscreened surface is not opaque, the compositor must make
        ///sure that other screen content not part of the same surface tree (made
        ///up of subsurfaces, popups or similarly coupled surfaces) are not
        ///visible below the fullscreened surface.
        ///</para>
        ///</Summary>
        ///<param name = "output">  </param>
        public void SetFullscreen(WlOutput output)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetFullscreen, output.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetFullscreen}({output.id})");
        }

        ///<Summary>
        ///unset the window as fullscreen
        ///<para>
        ///Make the surface no longer fullscreen.
        ///</para>
        ///<para>
        ///After requesting that the surface should be unfullscreened, the
        ///compositor will respond by emitting a configure event.
        ///Whether this actually removes the fullscreen state of the client is
        ///subject to compositor policies.
        ///</para>
        ///<para>
        ///Making a surface unfullscreen sets states for the surface based on the following:
        ///* the state(s) it may have had before becoming fullscreen
        ///* any state(s) decided by the compositor
        ///* any state(s) requested by the client while the surface was fullscreen
        ///</para>
        ///<para>
        ///The compositor may include the previous window geometry dimensions in
        ///the configure event, if applicable.
        ///</para>
        ///<para>
        ///The client must also acknowledge the configure when committing the new
        ///content (see ack_configure).
        ///</para>
        ///</Summary>
        public void UnsetFullscreen()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.UnsetFullscreen);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.UnsetFullscreen}()");
        }

        ///<Summary>
        ///set the window as minimized
        ///<para>
        ///Request that the compositor minimize your surface. There is no
        ///way to know if the surface is currently minimized, nor is there
        ///any way to unset minimization on this surface.
        ///</para>
        ///<para>
        ///If you are looking to throttle redrawing when minimized, please
        ///instead use the wl_surface.frame event for this, as this will
        ///also work with live previews on windows in Alt-Tab, Expose or
        ///similar compositor features.
        ///</para>
        ///</Summary>
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

        ///<Summary>
        ///suggest a surface change
        ///<para>
        ///This configure event asks the client to resize its toplevel surface or
        ///to change its state. The configured state should not be applied
        ///immediately. See xdg_surface.configure for details.
        ///</para>
        ///<para>
        ///The width and height arguments specify a hint to the window
        ///about how its surface should be resized in window geometry
        ///coordinates. See set_window_geometry.
        ///</para>
        ///<para>
        ///If the width or height arguments are zero, it means the client
        ///should decide its own window dimension. This may happen when the
        ///compositor needs to configure the state of the surface but doesn't
        ///have any information about any previous or expected dimension.
        ///</para>
        ///<para>
        ///The states listed in the event specify how the width/height
        ///arguments should be interpreted, and possibly how it should be
        ///drawn.
        ///</para>
        ///<para>
        ///Clients must send an ack_configure in response to this event. See
        ///xdg_surface.configure and xdg_surface.ack_configure for details.
        ///</para>
        ///</Summary>
        public Action<XdgToplevel, int, int, byte[]> configure;
        ///<Summary>
        ///surface wants to be closed
        ///<para>
        ///The close event is sent by the compositor when the user
        ///wants the surface to be closed. This should be equivalent to
        ///the user clicking the close button in client-side decorations,
        ///if your application has any.
        ///</para>
        ///<para>
        ///This is only a request that the user intends to close the
        ///window. The client may choose to ignore this request, or show
        ///a dialog to ask the user to save their data, etc.
        ///</para>
        ///</Summary>
        public Action<XdgToplevel> close;
        ///<Summary>
        ///recommended window geometry bounds
        ///<para>
        ///The configure_bounds event may be sent prior to a xdg_toplevel.configure
        ///event to communicate the bounds a window geometry size is recommended
        ///to constrain to.
        ///</para>
        ///<para>
        ///The passed width and height are in surface coordinate space. If width
        ///and height are 0, it means bounds is unknown and equivalent to as if no
        ///configure_bounds event was ever sent for this surface.
        ///</para>
        ///<para>
        ///The bounds can for example correspond to the size of a monitor excluding
        ///any panels or other shell components, so that a surface isn't created in
        ///a way that it cannot fit.
        ///</para>
        ///<para>
        ///The bounds may change at any point, and in such a case, a new
        ///xdg_toplevel.configure_bounds will be sent, followed by
        ///xdg_toplevel.configure and xdg_surface.configure.
        ///</para>
        ///</Summary>
        public Action<XdgToplevel, int, int> configureBounds;
        ///<Summary>
        ///compositor capabilities
        ///<para>
        ///This event advertises the capabilities supported by the compositor. If
        ///a capability isn't supported, clients should hide or disable the UI
        ///elements that expose this functionality. For instance, if the
        ///compositor doesn't advertise support for minimized toplevels, a button
        ///triggering the set_minimized request should not be displayed.
        ///</para>
        ///<para>
        ///The compositor will ignore requests it doesn't support. For instance,
        ///a compositor which doesn't advertise support for minimized will ignore
        ///set_minimized requests.
        ///</para>
        ///<para>
        ///Compositors must send this event once before the first
        ///xdg_surface.configure event. When the capabilities change, compositors
        ///must send this event again and then send an xdg_surface.configure
        ///event.
        ///</para>
        ///<para>
        ///The configured state should not be applied immediately. See
        ///xdg_surface.configure for details.
        ///</para>
        ///<para>
        ///The capabilities are sent as an array of 32-bit unsigned integers in
        ///native endianness.
        ///</para>
        ///</Summary>
        public Action<XdgToplevel, byte[]> wmCapabilities;
        public enum EventOpcode : ushort
        {
            Configure,
            Close,
            ConfigureBounds,
            WmCapabilities
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

                case EventOpcode.ConfigureBounds:
                {
                    var width = (int)arguments[0];
                    var height = (int)arguments[1];
                    if (this.configureBounds != null)
                    {
                        this.configureBounds.Invoke(this, width, height);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.ConfigureBounds}({this},{width},{height})");
                    }

                    break;
                }

                case EventOpcode.WmCapabilities:
                {
                    var capabilities = (byte[])arguments[0];
                    if (this.wmCapabilities != null)
                    {
                        this.wmCapabilities.Invoke(this, capabilities);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.WmCapabilities}({this},{capabilities})");
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
                case EventOpcode.ConfigureBounds:
                    return new WaylandType[]{WaylandType.Int, WaylandType.Int, };
                case EventOpcode.WmCapabilities:
                    return new WaylandType[]{WaylandType.Array, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///provided value is         not a valid variant of the resize_edge enum
            ///</Summary>
            InvalidResizeEdge = 0,
        }

        ///<Summary>
        ///edge values for resizing
        ///<para>
        ///These values are used to indicate which edge of a surface
        ///is being dragged in a resize operation.
        ///</para>
        ///</Summary>
        public enum ResizeEdgeFlag : uint
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
            Left = 4,
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
            Right = 8,
            ///<Summary>
            ///
            ///</Summary>
            TopRight = 9,
            ///<Summary>
            ///
            ///</Summary>
            BottomRight = 10,
        }

        ///<Summary>
        ///types of state on the surface
        ///<para>
        ///The different state values used on the surface. This is designed for
        ///state values like maximized, fullscreen. It is paired with the
        ///configure event to ensure that both the client and the compositor
        ///setting the state can be synchronized.
        ///</para>
        ///<para>
        ///States set in this way are double-buffered. They will get applied on
        ///the next commit.
        ///</para>
        ///</Summary>
        public enum StateFlag : uint
        {
            ///<Summary>
            ///the surface is maximized
            ///</Summary>
            Maximized = 1,
            ///<Summary>
            ///the surface is fullscreen
            ///</Summary>
            Fullscreen = 2,
            ///<Summary>
            ///the surface is being resized
            ///</Summary>
            Resizing = 3,
            ///<Summary>
            ///the surface is now activated
            ///</Summary>
            Activated = 4,
            ///<Summary>
            ///
            ///</Summary>
            TiledLeft = 5,
            ///<Summary>
            ///
            ///</Summary>
            TiledRight = 6,
            ///<Summary>
            ///
            ///</Summary>
            TiledTop = 7,
            ///<Summary>
            ///
            ///</Summary>
            TiledBottom = 8,
        }

        public enum WmCapabilitiesFlag : uint
        {
            ///<Summary>
            ///show_window_menu is available
            ///</Summary>
            WindowMenu = 1,
            ///<Summary>
            ///set_maximized and unset_maximized are available
            ///</Summary>
            Maximize = 2,
            ///<Summary>
            ///set_fullscreen and unset_fullscreen are available
            ///</Summary>
            Fullscreen = 3,
            ///<Summary>
            ///set_minimized is available
            ///</Summary>
            Minimize = 4,
        }
    }
}
