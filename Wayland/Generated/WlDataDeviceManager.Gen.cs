using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///data transfer interface
    ///<para>
    ///The wl_data_device_manager is a singleton global object that
    ///provides access to inter-client data transfer mechanisms such as
    ///copy-and-paste and drag-and-drop.  These mechanisms are tied to
    ///a wl_seat and this interface lets a client get a wl_data_device
    ///corresponding to a wl_seat.
    ///</para>
    ///<para>
    ///Depending on the version bound, the objects created from the bound
    ///wl_data_device_manager object will have different requirements for
    ///functioning properly. See wl_data_source.set_actions,
    ///wl_data_offer.accept and wl_data_offer.finish for details.
    ///</para>
    ///</Summary>
    public partial class WlDataDeviceManager : WaylandObject
    {
        public const string INTERFACE = "wl_data_device_manager";
        public WlDataDeviceManager(uint factoryId, ref uint id, WaylandConnection connection) : base(factoryId, ref id, 3, connection)
        {
        }

        ///<Summary>
        ///create a new data source
        ///<para>
        ///Create a new data source.
        ///</para>
        ///</Summary>
        ///<returns> data source to create </returns>
        public WlDataSource CreateDataSource()
        {
            uint id = connection.Create();
            WlDataSource wObject = new WlDataSource(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateDataSource, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreateDataSource}({id})");
            connection[id] = wObject;
            return (WlDataSource)connection[id];
        }

        ///<Summary>
        ///create a new data device
        ///<para>
        ///Create a new data device for a given seat.
        ///</para>
        ///</Summary>
        ///<returns> data device to create </returns>
        ///<param name = "seat"> seat associated with the data device </param>
        public WlDataDevice GetDataDevice(WlSeat seat)
        {
            uint id = connection.Create();
            WlDataDevice wObject = new WlDataDevice(this.id, ref id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.GetDataDevice, id, seat.id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.GetDataDevice}({id},{seat.id})");
            connection[id] = wObject;
            return (WlDataDevice)connection[id];
        }

        public enum RequestOpcode : ushort
        {
            CreateDataSource,
            GetDataDevice
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

        ///<Summary>
        ///drag and drop actions
        ///<para>
        ///This is a bitmask of the available/preferred actions in a
        ///drag-and-drop operation.
        ///</para>
        ///<para>
        ///In the compositor, the selected action is a result of matching the
        ///actions offered by the source and destination sides.  "action" events
        ///with a "none" action will be sent to both source and destination if
        ///there is no match. All further checks will effectively happen on
        ///(source actions ∩ destination actions).
        ///</para>
        ///<para>
        ///In addition, compositors may also pick different actions in
        ///reaction to key modifiers being pressed. One common design that
        ///is used in major toolkits (and the behavior recommended for
        ///compositors) is:
        ///</para>
        ///<para>
        ///- If no modifiers are pressed, the first match (in bit order)
        ///will be used.
        ///- Pressing Shift selects "move", if enabled in the mask.
        ///- Pressing Control selects "copy", if enabled in the mask.
        ///</para>
        ///<para>
        ///Behavior beyond that is considered implementation-dependent.
        ///Compositors may for example bind other modifiers (like Alt/Meta)
        ///or drags initiated with other buttons than BTN_LEFT to specific
        ///actions (e.g. "ask").
        ///</para>
        ///</Summary>
        public enum DndActionFlag : uint
        {
            ///<Summary>
            ///no action
            ///</Summary>
            None = 0,
            ///<Summary>
            ///copy action
            ///</Summary>
            Copy = 1,
            ///<Summary>
            ///move action
            ///</Summary>
            Move = 2,
            ///<Summary>
            ///ask action
            ///</Summary>
            Ask = 4,
        }
    }
}
