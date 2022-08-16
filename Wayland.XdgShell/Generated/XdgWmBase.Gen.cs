using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///create desktop-style surfaces
    ///<para>
    ///The xdg_wm_base interface is exposed as a global object enabling clients
    ///to turn their wl_surfaces into windows in a desktop environment. It
    ///defines the basic functionality needed for clients and the compositor to
    ///create windows that can be dragged, resized, maximized, etc, as well as
    ///creating transient windows such as popup menus.
    ///</para>
    ///</Summary>
    public partial class XdgWmBase : WaylandObject
    {
        public const string INTERFACE = "xdg_wm_base";
        public XdgWmBase(uint factoryId, ref uint id, WaylandConnection connection) : base(factoryId, ref id, 5, connection)
        {
        }

        ///<Summary>
        ///destroy xdg_wm_base
        ///<para>
        ///Destroy this xdg_wm_base object.
        ///</para>
        ///<para>
        ///Destroying a bound xdg_wm_base object while there are surfaces
        ///still alive created by this xdg_wm_base object instance is illegal
        ///and will result in a protocol error.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        ///<Summary>
        ///create a positioner object
        ///<para>
        ///Create a positioner object. A positioner object is used to position
        ///surfaces relative to some parent surface. See the interface description
        ///and xdg_surface.get_popup for details.
        ///</para>
        ///</Summary>
        ///<returns>  </returns>
        public XdgPositioner CreatePositioner()
        {
            uint id = connection.Create();
            XdgPositioner wObject = new XdgPositioner(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.CreatePositioner, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreatePositioner}({id})");
            connection[id] = wObject;
            return (XdgPositioner)connection[id];
        }

        ///<Summary>
        ///create a shell surface from a surface
        ///<para>
        ///This creates an xdg_surface for the given surface. While xdg_surface
        ///itself is not a role, the corresponding surface may only be assigned
        ///a role extending xdg_surface, such as xdg_toplevel or xdg_popup. It is
        ///illegal to create an xdg_surface for a wl_surface which already has an
        ///assigned role and this will result in a protocol error.
        ///</para>
        ///<para>
        ///This creates an xdg_surface for the given surface. An xdg_surface is
        ///used as basis to define a role to a given surface, such as xdg_toplevel
        ///or xdg_popup. It also manages functionality shared between xdg_surface
        ///based surface roles.
        ///</para>
        ///<para>
        ///See the documentation of xdg_surface for more details about what an
        ///xdg_surface is and how it is used.
        ///</para>
        ///</Summary>
        ///<returns>  </returns>
        ///<param name = "surface">  </param>
        public XdgSurface GetXdgSurface(WlSurface surface)
        {
            uint id = connection.Create();
            XdgSurface wObject = new XdgSurface(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.GetXdgSurface, id, surface.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetXdgSurface}({id},{surface.id})");
            connection[id] = wObject;
            return (XdgSurface)connection[id];
        }

        ///<Summary>
        ///respond to a ping event
        ///<para>
        ///A client must respond to a ping event with a pong request or
        ///the client may be deemed unresponsive. See xdg_wm_base.ping.
        ///</para>
        ///</Summary>
        ///<param name = "serial"> serial of the ping event </param>
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

        ///<Summary>
        ///check if the client is alive
        ///<para>
        ///The ping event asks the client if it's still alive. Pass the
        ///serial specified in the event back to the compositor by sending
        ///a "pong" request back with the specified serial. See xdg_wm_base.pong.
        ///</para>
        ///<para>
        ///Compositors can use this to determine if the client is still
        ///alive. It's unspecified what will happen if the client doesn't
        ///respond to the ping request, or in what timeframe. Clients should
        ///try to respond in a reasonable amount of time.
        ///</para>
        ///<para>
        ///A compositor is free to ping in any way it wants, but a client must
        ///always respond to any xdg_wm_base object it created.
        ///</para>
        ///</Summary>
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

        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///given wl_surface has another role
            ///</Summary>
            Role = 0,
            ///<Summary>
            ///xdg_wm_base was destroyed before children
            ///</Summary>
            DefunctSurfaces = 1,
            ///<Summary>
            ///the client tried to map or destroy a non-topmost popup
            ///</Summary>
            NotTheTopmostPopup = 2,
            ///<Summary>
            ///the client specified an invalid popup parent surface
            ///</Summary>
            InvalidPopupParent = 3,
            ///<Summary>
            ///the client provided an invalid surface state
            ///</Summary>
            InvalidSurfaceState = 4,
            ///<Summary>
            ///the client provided an invalid positioner
            ///</Summary>
            InvalidPositioner = 5,
        }
    }
}
