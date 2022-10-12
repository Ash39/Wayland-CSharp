using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///sub-surface compositing
    ///<para>
    ///The global interface exposing sub-surface compositing capabilities.
    ///A wl_surface, that has sub-surfaces associated, is called the
    ///parent surface. Sub-surfaces can be arbitrarily nested and create
    ///a tree of sub-surfaces.
    ///</para>
    ///<para>
    ///The root surface in a tree of sub-surfaces is the main
    ///surface. The main surface cannot be a sub-surface, because
    ///sub-surfaces must always have a parent.
    ///</para>
    ///<para>
    ///A main surface with its sub-surfaces forms a (compound) window.
    ///For window management purposes, this set of wl_surface objects is
    ///to be considered as a single window, and it should also behave as
    ///such.
    ///</para>
    ///<para>
    ///The aim of sub-surfaces is to offload some of the compositing work
    ///within a window from clients to the compositor. A prime example is
    ///a video player with decorations and video in separate wl_surface
    ///objects. This should allow the compositor to pass YUV video buffer
    ///processing to dedicated overlay hardware when possible.
    ///</para>
    ///</Summary>
    public partial class WlSubcompositor : WaylandObject
    {
        public const string INTERFACE = "wl_subcompositor";
        public WlSubcompositor(uint id, WaylandConnection connection, uint version = 1) : base(id, version, connection)
        {
        }

        ///<Summary>
        ///unbind from the subcompositor interface
        ///<para>
        ///Informs the server that the client will not be using this
        ///protocol object anymore. This does not affect any other
        ///objects, wl_subsurface objects included.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "Destroy");
        }

        ///<Summary>
        ///give a surface the role sub-surface
        ///<para>
        ///Create a sub-surface interface for the given surface, and
        ///associate it with the given parent surface. This turns a
        ///plain wl_surface into a sub-surface.
        ///</para>
        ///<para>
        ///The to-be sub-surface must not already have another role, and it
        ///must not have an existing wl_subsurface object. Otherwise a protocol
        ///error is raised.
        ///</para>
        ///<para>
        ///Adding sub-surfaces to a parent is a double-buffered operation on the
        ///parent (see wl_surface.commit). The effect of adding a sub-surface
        ///becomes visible on the next time the state of the parent surface is
        ///applied.
        ///</para>
        ///<para>
        ///This request modifies the behaviour of wl_surface.commit request on
        ///the sub-surface, see the documentation on wl_subsurface interface.
        ///</para>
        ///</Summary>
        ///<returns> the new sub-surface object ID </returns>
        ///<param name = "surface"> the surface to be turned into a sub-surface </param>
        ///<param name = "parent"> the parent surface </param>
        public WlSubsurface GetSubsurface(WlSurface surface, WlSurface parent)
        {
            WlSubsurface wObject = connection.Create<WlSubsurface>(0, this.version);
            uint id = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.GetSubsurface, id, surface.id, parent.id);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "GetSubsurface", id, surface.id, parent.id);
            return wObject;
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            GetSubsurface
        }

        public enum EventOpcode : ushort
        {
        }

        public override void Event(ushort opCode, WlType[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }

        public override WaylandType[] WaylandTypes(ushort opCode)
        {
            switch ((EventOpcode)opCode)
            {
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }

        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///the to-be sub-surface is invalid
            ///</Summary>
            BadSurface = 0,
        }
    }
}
