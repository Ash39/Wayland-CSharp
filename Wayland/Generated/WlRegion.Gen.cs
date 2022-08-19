using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///region interface
    ///<para>
    ///A region object describes an area.
    ///</para>
    ///<para>
    ///Region objects are used to describe the opaque and input
    ///regions of a surface.
    ///</para>
    ///</Summary>
    public partial class WlRegion : WaylandObject
    {
        public const string INTERFACE = "wl_region";
        public WlRegion(uint factoryId, ref uint id, WaylandConnection connection, uint version = 1) : base(factoryId, ref id, version, connection)
        {
        }

        ///<Summary>
        ///destroy region
        ///<para>
        ///Destroy the region.  This will invalidate the object ID.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        ///<Summary>
        ///add rectangle to region
        ///<para>
        ///Add the specified rectangle to the region.
        ///</para>
        ///</Summary>
        ///<param name = "x"> region-local x coordinate </param>
        ///<param name = "y"> region-local y coordinate </param>
        ///<param name = "width"> rectangle width </param>
        ///<param name = "height"> rectangle height </param>
        public void Add(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Add, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Add}({x},{y},{width},{height})");
        }

        ///<Summary>
        ///subtract rectangle from region
        ///<para>
        ///Subtract the specified rectangle from the region.
        ///</para>
        ///</Summary>
        ///<param name = "x"> region-local x coordinate </param>
        ///<param name = "y"> region-local y coordinate </param>
        ///<param name = "width"> rectangle width </param>
        ///<param name = "height"> rectangle height </param>
        public void Subtract(int x, int y, int width, int height)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Subtract, x, y, width, height);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Subtract}({x},{y},{width},{height})");
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            Add,
            Subtract
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
