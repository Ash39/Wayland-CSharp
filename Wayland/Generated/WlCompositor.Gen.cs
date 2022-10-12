using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///the compositor singleton
    ///<para>
    ///A compositor.  This object is a singleton global.  The
    ///compositor is in charge of combining the contents of multiple
    ///surfaces into one displayable output.
    ///</para>
    ///</Summary>
    public partial class WlCompositor : WaylandObject
    {
        public const string INTERFACE = "wl_compositor";
        public WlCompositor(uint id, WaylandConnection connection, uint version = 5) : base(id, version, connection)
        {
        }

        ///<Summary>
        ///create new surface
        ///<para>
        ///Ask the compositor to create a new surface.
        ///</para>
        ///</Summary>
        ///<returns> the new surface </returns>
        public WlSurface CreateSurface()
        {
            WlSurface wObject = connection.Create<WlSurface>(0, this.version);
            uint id = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateSurface, id);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "CreateSurface", id);
            return wObject;
        }

        ///<Summary>
        ///create new region
        ///<para>
        ///Ask the compositor to create a new region.
        ///</para>
        ///</Summary>
        ///<returns> the new region </returns>
        public WlRegion CreateRegion()
        {
            WlRegion wObject = connection.Create<WlRegion>(0, this.version);
            uint id = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateRegion, id);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "CreateRegion", id);
            return wObject;
        }

        public enum RequestOpcode : ushort
        {
            CreateSurface,
            CreateRegion
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
    }
}
