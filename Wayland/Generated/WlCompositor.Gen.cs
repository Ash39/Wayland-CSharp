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
        public WlCompositor(uint factoryId, ref uint id, WaylandConnection connection, uint version = 5) : base(factoryId, ref id, version, connection)
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
            uint id = connection.Create();
            WlSurface wObject = new WlSurface(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateSurface, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreateSurface}({id})");
            connection[id] = wObject;
            return (WlSurface)connection[id];
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
            uint id = connection.Create();
            WlRegion wObject = new WlRegion(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateRegion, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreateRegion}({id})");
            connection[id] = wObject;
            return (WlRegion)connection[id];
        }

        public enum RequestOpcode : ushort
        {
            CreateSurface,
            CreateRegion
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
