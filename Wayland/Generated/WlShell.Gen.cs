using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///create desktop-style surfaces
    ///<para>
    ///This interface is implemented by servers that provide
    ///desktop-style user interfaces.
    ///</para>
    ///<para>
    ///It allows clients to associate a wl_shell_surface with
    ///a basic surface.
    ///</para>
    ///<para>
    ///Note! This protocol is deprecated and not intended for production use.
    ///For desktop-style user interfaces, use xdg_shell. Compositors and clients
    ///should not implement this interface.
    ///</para>
    ///</Summary>
    public partial class WlShell : WaylandObject
    {
        public const string INTERFACE = "wl_shell";
        public WlShell(uint factoryId, ref uint id, WaylandConnection connection) : base(factoryId, ref id, 1, connection)
        {
        }

        ///<Summary>
        ///create a shell surface from a surface
        ///<para>
        ///Create a shell surface for an existing surface. This gives
        ///the wl_surface the role of a shell surface. If the wl_surface
        ///already has another role, it raises a protocol error.
        ///</para>
        ///<para>
        ///Only one shell surface can be associated with a given surface.
        ///</para>
        ///</Summary>
        ///<returns> shell surface to create </returns>
        ///<param name = "surface"> surface to be given the shell surface role </param>
        public WlShellSurface GetShellSurface(WlSurface surface)
        {
            uint id = connection.Create();
            WlShellSurface wObject = new WlShellSurface(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.GetShellSurface, id, surface.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetShellSurface}({id},{surface.id})");
            connection[id] = wObject;
            return (WlShellSurface)connection[id];
        }

        public enum RequestOpcode : ushort
        {
            GetShellSurface
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
            ///given wl_surface has another role
            ///</Summary>
            Role = 0,
        }
    }
}
