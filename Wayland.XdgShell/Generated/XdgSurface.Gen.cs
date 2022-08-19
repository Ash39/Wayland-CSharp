using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///desktop user interface surface base interface
    ///<para>
    ///An interface that may be implemented by a wl_surface, for
    ///implementations that provide a desktop-style user interface.
    ///</para>
    ///<para>
    ///It provides a base set of functionality required to construct user
    ///interface elements requiring management by the compositor, such as
    ///toplevel windows, menus, etc. The types of functionality are split into
    ///xdg_surface roles.
    ///</para>
    ///<para>
    ///Creating an xdg_surface does not set the role for a wl_surface. In order
    ///to map an xdg_surface, the client must create a role-specific object
    ///using, e.g., get_toplevel, get_popup. The wl_surface for any given
    ///xdg_surface can have at most one role, and may not be assigned any role
    ///not based on xdg_surface.
    ///</para>
    ///<para>
    ///A role must be assigned before any other requests are made to the
    ///xdg_surface object.
    ///</para>
    ///<para>
    ///The client must call wl_surface.commit on the corresponding wl_surface
    ///for the xdg_surface state to take effect.
    ///</para>
    ///<para>
    ///Creating an xdg_surface from a wl_surface which has a buffer attached or
    ///committed is a client error, and any attempts by a client to attach or
    ///manipulate a buffer prior to the first xdg_surface.configure call must
    ///also be treated as errors.
    ///</para>
    ///<para>
    ///After creating a role-specific object and setting it up, the client must
    ///perform an initial commit without any buffer attached. The compositor
    ///will reply with an xdg_surface.configure event. The client must
    ///acknowledge it and is then allowed to attach a buffer to map the surface.
    ///</para>
    ///<para>
    ///Mapping an xdg_surface-based role surface is defined as making it
    ///possible for the surface to be shown by the compositor. Note that
    ///a mapped surface is not guaranteed to be visible once it is mapped.
    ///</para>
    ///<para>
    ///For an xdg_surface to be mapped by the compositor, the following
    ///conditions must be met:
    ///(1) the client has assigned an xdg_surface-based role to the surface
    ///(2) the client has set and committed the xdg_surface state and the
    ///role-dependent state to the surface
    ///(3) the client has committed a buffer to the surface
    ///</para>
    ///<para>
    ///A newly-unmapped surface is considered to have met condition (1) out
    ///of the 3 required conditions for mapping a surface if its role surface
    ///has not been destroyed, i.e. the client must perform the initial commit
    ///again before attaching a buffer.
    ///</para>
    ///</Summary>
    public partial class XdgSurface : WaylandObject
    {
        public const string INTERFACE = "xdg_surface";
        public XdgSurface(uint factoryId, ref uint id, WaylandConnection connection, uint version = 5) : base(factoryId, ref id, version, connection)
        {
        }

        ///<Summary>
        ///destroy the xdg_surface
        ///<para>
        ///Destroy the xdg_surface object. An xdg_surface must only be destroyed
        ///after its role object has been destroyed.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        ///<Summary>
        ///assign the xdg_toplevel surface role
        ///<para>
        ///This creates an xdg_toplevel object for the given xdg_surface and gives
        ///the associated wl_surface the xdg_toplevel role.
        ///</para>
        ///<para>
        ///See the documentation of xdg_toplevel for more details about what an
        ///xdg_toplevel is and how it is used.
        ///</para>
        ///</Summary>
        ///<returns>  </returns>
        public XdgToplevel GetToplevel()
        {
            uint id = connection.Create();
            XdgToplevel wObject = new XdgToplevel(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.GetToplevel, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetToplevel}({id})");
            connection[id] = wObject;
            return (XdgToplevel)connection[id];
        }

        ///<Summary>
        ///assign the xdg_popup surface role
        ///<para>
        ///This creates an xdg_popup object for the given xdg_surface and gives
        ///the associated wl_surface the xdg_popup role.
        ///</para>
        ///<para>
        ///If null is passed as a parent, a parent surface must be specified using
        ///some other protocol, before committing the initial state.
        ///</para>
        ///<para>
        ///See the documentation of xdg_popup for more details about what an
        ///xdg_popup is and how it is used.
        ///</para>
        ///</Summary>
        ///<returns>  </returns>
        ///<param name = "parent">  </param>
        ///<param name = "positioner">  </param>
        public XdgPopup GetPopup(XdgSurface parent, XdgPositioner positioner)
        {
            uint id = connection.Create();
            XdgPopup wObject = new XdgPopup(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.GetPopup, id, parent.id, positioner.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetPopup}({id},{parent.id},{positioner.id})");
            connection[id] = wObject;
            return (XdgPopup)connection[id];
        }

        ///<Summary>
        ///set the new window geometry
        ///<para>
        ///The window geometry of a surface is its "visible bounds" from the
        ///user's perspective. Client-side decorations often have invisible
        ///portions like drop-shadows which should be ignored for the
        ///purposes of aligning, placing and constraining windows.
        ///</para>
        ///<para>
        ///The window geometry is double buffered, and will be applied at the
        ///time wl_surface.commit of the corresponding wl_surface is called.
        ///</para>
        ///<para>
        ///When maintaining a position, the compositor should treat the (x, y)
        ///coordinate of the window geometry as the top left corner of the window.
        ///A client changing the (x, y) window geometry coordinate should in
        ///general not alter the position of the window.
        ///</para>
        ///<para>
        ///Once the window geometry of the surface is set, it is not possible to
        ///unset it, and it will remain the same until set_window_geometry is
        ///called again, even if a new subsurface or buffer is attached.
        ///</para>
        ///<para>
        ///If never set, the value is the full bounds of the surface,
        ///including any subsurfaces. This updates dynamically on every
        ///commit. This unset is meant for extremely simple clients.
        ///</para>
        ///<para>
        ///The arguments are given in the surface-local coordinate space of
        ///the wl_surface associated with this xdg_surface.
        ///</para>
        ///<para>
        ///The width and height must be greater than zero. Setting an invalid size
        ///will raise an error. When applied, the effective window geometry will be
        ///the set window geometry clamped to the bounding rectangle of the
        ///combined geometry of the surface of the xdg_surface and the associated
        ///subsurfaces.
        ///</para>
        ///</Summary>
        ///<param name = "x">  </param>
        ///<param name = "y">  </param>
        ///<param name = "width">  </param>
        ///<param name = "height">  </param>
        public void SetWindowGeometry(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetWindowGeometry, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetWindowGeometry}({x},{y},{width},{height})");
        }

        ///<Summary>
        ///ack a configure event
        ///<para>
        ///When a configure event is received, if a client commits the
        ///surface in response to the configure event, then the client
        ///must make an ack_configure request sometime before the commit
        ///request, passing along the serial of the configure event.
        ///</para>
        ///<para>
        ///For instance, for toplevel surfaces the compositor might use this
        ///information to move a surface to the top left only when the client has
        ///drawn itself for the maximized or fullscreen state.
        ///</para>
        ///<para>
        ///If the client receives multiple configure events before it
        ///can respond to one, it only has to ack the last configure event.
        ///</para>
        ///<para>
        ///A client is not required to commit immediately after sending
        ///an ack_configure request - it may even ack_configure several times
        ///before its next surface commit.
        ///</para>
        ///<para>
        ///A client may send multiple ack_configure requests before committing, but
        ///only the last request sent before a commit indicates which configure
        ///event the client really is responding to.
        ///</para>
        ///</Summary>
        ///<param name = "serial"> the serial from the configure event </param>
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

        ///<Summary>
        ///suggest a surface change
        ///<para>
        ///The configure event marks the end of a configure sequence. A configure
        ///sequence is a set of one or more events configuring the state of the
        ///xdg_surface, including the final xdg_surface.configure event.
        ///</para>
        ///<para>
        ///Where applicable, xdg_surface surface roles will during a configure
        ///sequence extend this event as a latched state sent as events before the
        ///xdg_surface.configure event. Such events should be considered to make up
        ///a set of atomically applied configuration states, where the
        ///xdg_surface.configure commits the accumulated state.
        ///</para>
        ///<para>
        ///Clients should arrange their surface for the new states, and then send
        ///an ack_configure request with the serial sent in this configure event at
        ///some point before committing the new surface.
        ///</para>
        ///<para>
        ///If the client receives multiple configure events before it can respond
        ///to one, it is free to discard all but the last event it received.
        ///</para>
        ///</Summary>
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

        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///
            ///</Summary>
            NotConstructed = 1,
            ///<Summary>
            ///
            ///</Summary>
            AlreadyConstructed = 2,
            ///<Summary>
            ///
            ///</Summary>
            UnconfiguredBuffer = 3,
        }
    }
}
