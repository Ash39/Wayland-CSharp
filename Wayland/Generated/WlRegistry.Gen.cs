using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// global registry object
    /// </summary>
    public partial class WlRegistry : WaylandObject
    {
        public const string INTERFACE = "wl_registry";
        public WlRegistry(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// bind an object to the display
        /// </summary>
        public T Bind<T>(uint name, string @interface, uint version)
            where T : WaylandObject
        {
            uint id = connection.Create();
            connection.Marshal(this.id, (ushort)RequestOpcode.Bind, name, @interface, version, id);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Bind}({name},{@interface},{version},{id})");
            connection[id] = (WaylandObject)Activator.CreateInstance(typeof(T), id, version, connection);
            return (T)connection[id];
        }

        public enum RequestOpcode : ushort
        {
            Bind
        }

        public Action<WlRegistry, uint, string, uint> global;
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
