using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// data transfer device
    /// </summary>
    public partial class WlDataDevice : WaylandObject
    {
        public const string INTERFACE = "wl_data_device";
        public WlDataDevice(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// start drag-and-drop operation
        /// </summary>
        public void StartDrag(WlDataSource source, WlSurface origin, WlSurface icon, uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.StartDrag, source.id, origin.id, icon.id, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.StartDrag}({source.id},{origin.id},{icon.id},{serial})");
        }

        /// <summary>
        /// copy data to the selection
        /// </summary>
        public void SetSelection(WlDataSource source, uint serial)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.SetSelection, source.id, serial);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.SetSelection}({source.id},{serial})");
        }

        /// <summary>
        /// destroy data device
        /// </summary>
        public void Release()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Release);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Release}()");
        }

        public enum RequestOpcode : ushort
        {
            StartDrag,
            SetSelection,
            Release
        }

        public Action<WlDataDevice, int> dataOffer;
        public Action<WlDataDevice, uint, WaylandObject, double, double, WaylandObject> enter;
        public Action<WlDataDevice> leave;
        public Action<WlDataDevice, uint, double, double> motion;
        public Action<WlDataDevice> drop;
        public Action<WlDataDevice, WaylandObject> selection;
        public enum EventOpcode : ushort
        {
            DataOffer,
            Enter,
            Leave,
            Motion,
            Drop,
            Selection
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.DataOffer:
                {
                    var id = (int)arguments[0];
                    if (this.dataOffer != null)
                    {
                        this.dataOffer.Invoke(this, id);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.DataOffer}({this},{id})");
                    }

                    break;
                }

                case EventOpcode.Enter:
                {
                    var serial = (uint)arguments[0];
                    var surface = connection[(uint)arguments[1]];
                    var x = (double)arguments[2];
                    var y = (double)arguments[3];
                    var id = connection[(uint)arguments[4]];
                    if (this.enter != null)
                    {
                        this.enter.Invoke(this, serial, surface, x, y, id);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Enter}({this},{serial},{surface},{x},{y},{id})");
                    }

                    break;
                }

                case EventOpcode.Leave:
                {
                    if (this.leave != null)
                    {
                        this.leave.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Leave}({this})");
                    }

                    break;
                }

                case EventOpcode.Motion:
                {
                    var time = (uint)arguments[0];
                    var x = (double)arguments[1];
                    var y = (double)arguments[2];
                    if (this.motion != null)
                    {
                        this.motion.Invoke(this, time, x, y);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Motion}({this},{time},{x},{y})");
                    }

                    break;
                }

                case EventOpcode.Drop:
                {
                    if (this.drop != null)
                    {
                        this.drop.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Drop}({this})");
                    }

                    break;
                }

                case EventOpcode.Selection:
                {
                    var id = connection[(uint)arguments[0]];
                    if (this.selection != null)
                    {
                        this.selection.Invoke(this, id);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Selection}({this},{id})");
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
                case EventOpcode.DataOffer:
                    return new WaylandType[]{WaylandType.NewId, };
                case EventOpcode.Enter:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Object, WaylandType.Fixed, WaylandType.Fixed, WaylandType.Object, };
                case EventOpcode.Leave:
                    return new WaylandType[]{};
                case EventOpcode.Motion:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Fixed, WaylandType.Fixed, };
                case EventOpcode.Drop:
                    return new WaylandType[]{};
                case EventOpcode.Selection:
                    return new WaylandType[]{WaylandType.Object, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
