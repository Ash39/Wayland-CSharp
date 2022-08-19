using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///global registry object
    ///<para>
    ///The singleton global registry object.  The server has a number of
    ///global objects that are available to all clients.  These objects
    ///typically represent an actual object in the server (for example,
    ///an input device) or they are singleton objects that provide
    ///extension functionality.
    ///</para>
    ///<para>
    ///When a client creates a registry object, the registry object
    ///will emit a global event for each global currently in the
    ///registry.  Globals come and go as a result of device or
    ///monitor hotplugs, reconfiguration or other events, and the
    ///registry will send out global and global_remove events to
    ///keep the client up to date with the changes.  To mark the end
    ///of the initial burst of events, the client can use the
    ///wl_display.sync request immediately after calling
    ///wl_display.get_registry.
    ///</para>
    ///<para>
    ///A client can bind to a global object by using the bind
    ///request.  This creates a client-side handle that lets the object
    ///emit events to the client and lets the client invoke requests on
    ///the object.
    ///</para>
    ///</Summary>
    public partial class WlRegistry : WaylandObject
    {
        public const string INTERFACE = "wl_registry";
        public WlRegistry(uint factoryId, ref uint id, WaylandConnection connection, uint version = 1) : base(factoryId, ref id, version, connection)
        {
        }

        ///<Summary>
        ///bind an object to the display
        ///<para>
        ///Binds a new, client-created object to the server using the
        ///specified name as the identifier.
        ///</para>
        ///</Summary>
        ///<param name = "name"> unique numeric name of the object </param>
        ///<returns> bounded object </returns>
        public T Bind<T>(uint name, string @interface, uint version)
            where T : WaylandObject
        {
            uint id = connection.Create();
            WaylandObject wObject = (WaylandObject)Activator.CreateInstance(typeof(T), this.id, id, connection, version);
            id = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.Bind, name, @interface, version, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Bind}({name},{@interface},{version},{id})");
            connection[id] = wObject;
            return (T)connection[id];
        }

        public enum RequestOpcode : ushort
        {
            Bind
        }

        ///<Summary>
        ///announce global object
        ///<para>
        ///Notify the client of global objects.
        ///</para>
        ///<para>
        ///The event notifies the client that a global object with
        ///the given name is now available, and it implements the
        ///given version of the given interface.
        ///</para>
        ///</Summary>
        public Action<WlRegistry, uint, string, uint> global;
        ///<Summary>
        ///announce removal of global object
        ///<para>
        ///Notify the client of removed global objects.
        ///</para>
        ///<para>
        ///This event notifies the client that the global identified
        ///by name is no longer available.  If the client bound to
        ///the global using the bind request, the client should now
        ///destroy that object.
        ///</para>
        ///<para>
        ///The object remains valid and requests to the object will be
        ///ignored until the client destroys it, to avoid races between
        ///the global going away and a client sending a request to it.
        ///</para>
        ///</Summary>
        public Action<WlRegistry, uint> globalRemove;
        public enum EventOpcode : ushort
        {
            Global,
            GlobalRemove
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Global:
                {
                    var name = (uint)arguments[0];
                    var @interface = (string)arguments[1];
                    var version = (uint)arguments[2];
                    if (this.global != null)
                    {
                        this.global.Invoke(this, name, @interface, version);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Global}({this},{name},{@interface},{version})");
                    }

                    break;
                }

                case EventOpcode.GlobalRemove:
                {
                    var name = (uint)arguments[0];
                    if (this.globalRemove != null)
                    {
                        this.globalRemove.Invoke(this, name);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.GlobalRemove}({this},{name})");
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
                case EventOpcode.Global:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.String, WaylandType.Uint, };
                case EventOpcode.GlobalRemove:
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
