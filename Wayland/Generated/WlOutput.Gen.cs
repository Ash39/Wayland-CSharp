using System;
using System.Collections.Generic;

namespace Wayland
{
    /// <summary>
    /// compositor output region
    /// </summary>
    public partial class WlOutput : WaylandObject
    {
        public const string INTERFACE = "wl_output";
        public WlOutput(uint id, uint version, WaylandConnection connection) : base(id, version, connection)
        {
        }

        /// <summary>
        /// release the output object
        /// </summary>
        public void Release()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Release);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Release}()");
        }

        public enum RequestOpcode : ushort
        {
            Release
        }

        public Action<WlOutput, int, int, int, int, int, string, string, int> geometry;
        public Action<WlOutput, uint, int, int, int> mode;
        public Action<WlOutput> done;
        public Action<WlOutput, int> scale;
        public Action<WlOutput, string> name;
        public Action<WlOutput, string> description;
        public enum EventOpcode : ushort
        {
            Geometry,
            Mode,
            Done,
            Scale,
            Name,
            Description
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Geometry:
                {
                    var x = (int)arguments[0];
                    var y = (int)arguments[1];
                    var physicalWidth = (int)arguments[2];
                    var physicalHeight = (int)arguments[3];
                    var subpixel = (int)arguments[4];
                    var make = (string)arguments[5];
                    var model = (string)arguments[6];
                    var transform = (int)arguments[7];
                    if (this.geometry != null)
                    {
                        this.geometry.Invoke(this, x, y, physicalWidth, physicalHeight, subpixel, make, model, transform);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Geometry}({this},{x},{y},{physicalWidth},{physicalHeight},{subpixel},{make},{model},{transform})");
                    }

                    break;
                }

                case EventOpcode.Mode:
                {
                    var flags = (uint)arguments[0];
                    var width = (int)arguments[1];
                    var height = (int)arguments[2];
                    var refresh = (int)arguments[3];
                    if (this.mode != null)
                    {
                        this.mode.Invoke(this, flags, width, height, refresh);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Mode}({this},{flags},{width},{height},{refresh})");
                    }

                    break;
                }

                case EventOpcode.Done:
                {
                    if (this.done != null)
                    {
                        this.done.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Done}({this})");
                    }

                    break;
                }

                case EventOpcode.Scale:
                {
                    var factor = (int)arguments[0];
                    if (this.scale != null)
                    {
                        this.scale.Invoke(this, factor);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Scale}({this},{factor})");
                    }

                    break;
                }

                case EventOpcode.Name:
                {
                    var name = (string)arguments[0];
                    if (this.name != null)
                    {
                        this.name.Invoke(this, name);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Name}({this},{name})");
                    }

                    break;
                }

                case EventOpcode.Description:
                {
                    var description = (string)arguments[0];
                    if (this.description != null)
                    {
                        this.description.Invoke(this, description);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Description}({this},{description})");
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
                case EventOpcode.Geometry:
                    return new WaylandType[]{WaylandType.Int, WaylandType.Int, WaylandType.Int, WaylandType.Int, WaylandType.Int, WaylandType.String, WaylandType.String, WaylandType.Int, };
                case EventOpcode.Mode:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Int, WaylandType.Int, WaylandType.Int, };
                case EventOpcode.Done:
                    return new WaylandType[]{};
                case EventOpcode.Scale:
                    return new WaylandType[]{WaylandType.Int, };
                case EventOpcode.Name:
                    return new WaylandType[]{WaylandType.String, };
                case EventOpcode.Description:
                    return new WaylandType[]{WaylandType.String, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
