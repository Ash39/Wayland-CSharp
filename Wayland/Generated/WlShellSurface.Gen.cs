using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///desktop-style metadata interface
    ///<para>
    ///An interface that may be implemented by a wl_surface, for
    ///implementations that provide a desktop-style user interface.
    ///</para>
    ///<para>
    ///It provides requests to treat surfaces like toplevel, fullscreen
    ///or popup windows, move, resize or maximize them, associate
    ///metadata like title and class, etc.
    ///</para>
    ///<para>
    ///On the server side the object is automatically destroyed when
    ///the related wl_surface is destroyed. On the client side,
    ///wl_shell_surface_destroy() must be called before destroying
    ///the wl_surface object.
    ///</para>
    ///</Summary>
    public partial class WlShellSurface : WaylandObject
    {
        public const string INTERFACE = "wl_shell_surface";
        public WlShellSurface(uint factoryId, ref uint id, WaylandConnection connection) : base(factoryId, ref id, 1, connection)
        {
        }

        ///<Summary>
        ///respond to a ping event
        ///<para>
        ///A client must respond to a ping event with a pong request or
        ///the client may be deemed unresponsive.
        ///</para>
        ///</Summary>
        ///<param name = "serial"> serial number of the ping event </param>
        public void Pong(uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Pong, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Pong}({serial})");
        }

        ///<Summary>
        ///start an interactive move
        ///<para>
        ///Start a pointer-driven move of the surface.
        ///</para>
        ///<para>
        ///This request must be used in response to a button press event.
        ///The server may ignore move requests depending on the state of
        ///the surface (e.g. fullscreen or maximized).
        ///</para>
        ///</Summary>
        ///<param name = "seat"> seat whose pointer is used </param>
        ///<param name = "serial"> serial number of the implicit grab on the pointer </param>
        public void Move(WlSeat seat, uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Move, seat.id, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Move}({seat.id},{serial})");
        }

        ///<Summary>
        ///start an interactive resize
        ///<para>
        ///Start a pointer-driven resizing of the surface.
        ///</para>
        ///<para>
        ///This request must be used in response to a button press event.
        ///The server may ignore resize requests depending on the state of
        ///the surface (e.g. fullscreen or maximized).
        ///</para>
        ///</Summary>
        ///<param name = "seat"> seat whose pointer is used </param>
        ///<param name = "serial"> serial number of the implicit grab on the pointer </param>
        ///<param name = "edges"> which edge or corner is being dragged </param>
        public void Resize(WlSeat seat, uint serial, ResizeFlag edges)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Resize, seat.id, serial, (uint)edges);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Resize}({seat.id},{serial},{(uint)edges})");
        }

        ///<Summary>
        ///make the surface a toplevel surface
        ///<para>
        ///Map the surface as a toplevel surface.
        ///</para>
        ///<para>
        ///A toplevel surface is not fullscreen, maximized or transient.
        ///</para>
        ///</Summary>
        public void SetToplevel()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetToplevel);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetToplevel}()");
        }

        ///<Summary>
        ///make the surface a transient surface
        ///<para>
        ///Map the surface relative to an existing surface.
        ///</para>
        ///<para>
        ///The x and y arguments specify the location of the upper left
        ///corner of the surface relative to the upper left corner of the
        ///parent surface, in surface-local coordinates.
        ///</para>
        ///<para>
        ///The flags argument controls details of the transient behaviour.
        ///</para>
        ///</Summary>
        ///<param name = "parent"> parent surface </param>
        ///<param name = "x"> surface-local x coordinate </param>
        ///<param name = "y"> surface-local y coordinate </param>
        ///<param name = "flags"> transient surface behavior </param>
        public void SetTransient(WlSurface parent, int x, int y, TransientFlag flags)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetTransient, parent.id, x, y, (uint)flags);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetTransient}({parent.id},{x},{y},{(uint)flags})");
        }

        ///<Summary>
        ///make the surface a fullscreen surface
        ///<para>
        ///Map the surface as a fullscreen surface.
        ///</para>
        ///<para>
        ///If an output parameter is given then the surface will be made
        ///fullscreen on that output. If the client does not specify the
        ///output then the compositor will apply its policy - usually
        ///choosing the output on which the surface has the biggest surface
        ///area.
        ///</para>
        ///<para>
        ///The client may specify a method to resolve a size conflict
        ///between the output size and the surface size - this is provided
        ///through the method parameter.
        ///</para>
        ///<para>
        ///The framerate parameter is used only when the method is set
        ///to "driver", to indicate the preferred framerate. A value of 0
        ///indicates that the client does not care about framerate.  The
        ///framerate is specified in mHz, that is framerate of 60000 is 60Hz.
        ///</para>
        ///<para>
        ///A method of "scale" or "driver" implies a scaling operation of
        ///the surface, either via a direct scaling operation or a change of
        ///the output mode. This will override any kind of output scaling, so
        ///that mapping a surface with a buffer size equal to the mode can
        ///fill the screen independent of buffer_scale.
        ///</para>
        ///<para>
        ///A method of "fill" means we don't scale up the buffer, however
        ///any output scale is applied. This means that you may run into
        ///an edge case where the application maps a buffer with the same
        ///size of the output mode but buffer_scale 1 (thus making a
        ///surface larger than the output). In this case it is allowed to
        ///downscale the results to fit the screen.
        ///</para>
        ///<para>
        ///The compositor must reply to this request with a configure event
        ///with the dimensions for the output on which the surface will
        ///be made fullscreen.
        ///</para>
        ///</Summary>
        ///<param name = "method"> method for resolving size conflict </param>
        ///<param name = "framerate"> framerate in mHz </param>
        ///<param name = "output"> output on which the surface is to be fullscreen </param>
        public void SetFullscreen(FullscreenMethodFlag method, uint framerate, WlOutput output)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetFullscreen, (uint)method, framerate, output.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetFullscreen}({(uint)method},{framerate},{output.id})");
        }

        ///<Summary>
        ///make the surface a popup surface
        ///<para>
        ///Map the surface as a popup.
        ///</para>
        ///<para>
        ///A popup surface is a transient surface with an added pointer
        ///grab.
        ///</para>
        ///<para>
        ///An existing implicit grab will be changed to owner-events mode,
        ///and the popup grab will continue after the implicit grab ends
        ///(i.e. releasing the mouse button does not cause the popup to
        ///be unmapped).
        ///</para>
        ///<para>
        ///The popup grab continues until the window is destroyed or a
        ///mouse button is pressed in any other client's window. A click
        ///in any of the client's surfaces is reported as normal, however,
        ///clicks in other clients' surfaces will be discarded and trigger
        ///the callback.
        ///</para>
        ///<para>
        ///The x and y arguments specify the location of the upper left
        ///corner of the surface relative to the upper left corner of the
        ///parent surface, in surface-local coordinates.
        ///</para>
        ///</Summary>
        ///<param name = "seat"> seat whose pointer is used </param>
        ///<param name = "serial"> serial number of the implicit grab on the pointer </param>
        ///<param name = "parent"> parent surface </param>
        ///<param name = "x"> surface-local x coordinate </param>
        ///<param name = "y"> surface-local y coordinate </param>
        ///<param name = "flags"> transient surface behavior </param>
        public void SetPopup(WlSeat seat, uint serial, WlSurface parent, int x, int y, TransientFlag flags)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetPopup, seat.id, serial, parent.id, x, y, (uint)flags);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetPopup}({seat.id},{serial},{parent.id},{x},{y},{(uint)flags})");
        }

        ///<Summary>
        ///make the surface a maximized surface
        ///<para>
        ///Map the surface as a maximized surface.
        ///</para>
        ///<para>
        ///If an output parameter is given then the surface will be
        ///maximized on that output. If the client does not specify the
        ///output then the compositor will apply its policy - usually
        ///choosing the output on which the surface has the biggest surface
        ///area.
        ///</para>
        ///<para>
        ///The compositor will reply with a configure event telling
        ///the expected new surface size. The operation is completed
        ///on the next buffer attach to this surface.
        ///</para>
        ///<para>
        ///A maximized surface typically fills the entire output it is
        ///bound to, except for desktop elements such as panels. This is
        ///the main difference between a maximized shell surface and a
        ///fullscreen shell surface.
        ///</para>
        ///<para>
        ///The details depend on the compositor implementation.
        ///</para>
        ///</Summary>
        ///<param name = "output"> output on which the surface is to be maximized </param>
        public void SetMaximized(WlOutput output)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetMaximized, output.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetMaximized}({output.id})");
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
        ///<param name = "title"> surface title </param>
        public void SetTitle(string title)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetTitle, title);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetTitle}({title})");
        }

        ///<Summary>
        ///set surface class
        ///<para>
        ///Set a class for the surface.
        ///</para>
        ///<para>
        ///The surface class identifies the general class of applications
        ///to which the surface belongs. A common convention is to use the
        ///file name (or the full path if it is a non-standard location) of
        ///the application's .desktop file as the class.
        ///</para>
        ///</Summary>
        ///<param name = "class_"> surface class </param>
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

        ///<Summary>
        ///ping client
        ///<para>
        ///Ping a client to check if it is receiving events and sending
        ///requests. A client is expected to reply with a pong request.
        ///</para>
        ///</Summary>
        public Action<WlShellSurface, uint> ping;
        ///<Summary>
        ///suggest resize
        ///<para>
        ///The configure event asks the client to resize its surface.
        ///</para>
        ///<para>
        ///The size is a hint, in the sense that the client is free to
        ///ignore it if it doesn't resize, pick a smaller size (to
        ///satisfy aspect ratio or resize in steps of NxM pixels).
        ///</para>
        ///<para>
        ///The edges parameter provides a hint about how the surface
        ///was resized. The client may use this information to decide
        ///how to adjust its content to the new size (e.g. a scrolling
        ///area might adjust its content position to leave the viewable
        ///content unmoved).
        ///</para>
        ///<para>
        ///The client is free to dismiss all but the last configure
        ///event it received.
        ///</para>
        ///<para>
        ///The width and height arguments specify the size of the window
        ///in surface-local coordinates.
        ///</para>
        ///</Summary>
        public Action<WlShellSurface, ResizeFlag, int, int> configure;
        ///<Summary>
        ///popup interaction is done
        ///<para>
        ///The popup_done event is sent out when a popup grab is broken,
        ///that is, when the user clicks a surface that doesn't belong
        ///to the client owning the popup surface.
        ///</para>
        ///</Summary>
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
                    var edges = (ResizeFlag)arguments[0];
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

        ///<Summary>
        ///edge values for resizing
        ///<para>
        ///These values are used to indicate which edge of a surface
        ///is being dragged in a resize operation. The server may
        ///use this information to adapt its behavior, e.g. choose
        ///an appropriate cursor image.
        ///</para>
        ///</Summary>
        public enum ResizeFlag : uint
        {
            ///<Summary>
            ///no edge
            ///</Summary>
            None = 0,
            ///<Summary>
            ///top edge
            ///</Summary>
            Top = 1,
            ///<Summary>
            ///bottom edge
            ///</Summary>
            Bottom = 2,
            ///<Summary>
            ///left edge
            ///</Summary>
            Left = 4,
            ///<Summary>
            ///top and left edges
            ///</Summary>
            TopLeft = 5,
            ///<Summary>
            ///bottom and left edges
            ///</Summary>
            BottomLeft = 6,
            ///<Summary>
            ///right edge
            ///</Summary>
            Right = 8,
            ///<Summary>
            ///top and right edges
            ///</Summary>
            TopRight = 9,
            ///<Summary>
            ///bottom and right edges
            ///</Summary>
            BottomRight = 10,
        }

        ///<Summary>
        ///details of transient behaviour
        ///<para>
        ///These flags specify details of the expected behaviour
        ///of transient surfaces. Used in the set_transient request.
        ///</para>
        ///</Summary>
        public enum TransientFlag : uint
        {
            ///<Summary>
            ///do not set keyboard focus
            ///</Summary>
            Inactive = 0x1,
        }

        ///<Summary>
        ///different method to set the surface fullscreen
        ///<para>
        ///Hints to indicate to the compositor how to deal with a conflict
        ///between the dimensions of the surface and the dimensions of the
        ///output. The compositor is free to ignore this parameter.
        ///</para>
        ///</Summary>
        public enum FullscreenMethodFlag : uint
        {
            ///<Summary>
            ///no preference, apply default policy
            ///</Summary>
            Default = 0,
            ///<Summary>
            ///scale, preserve the surface's aspect ratio and center on output
            ///</Summary>
            Scale = 1,
            ///<Summary>
            ///switch output mode to the smallest mode that can fit the surface, add black borders to compensate size mismatch
            ///</Summary>
            Driver = 2,
            ///<Summary>
            ///no upscaling, center on output and add black borders to compensate size mismatch
            ///</Summary>
            Fill = 3,
        }
    }
}
