using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///compositor output region
    ///<para>
    ///An output describes part of the compositor geometry.  The
    ///compositor works in the 'compositor coordinate system' and an
    ///output corresponds to a rectangular area in that space that is
    ///actually visible.  This typically corresponds to a monitor that
    ///displays part of the compositor space.  This object is published
    ///as global during start up, or when a monitor is hotplugged.
    ///</para>
    ///</Summary>
    public partial class WlOutput : WaylandObject
    {
        public const string INTERFACE = "wl_output";
        public WlOutput(uint id, WaylandConnection connection, uint version = 4) : base(id, version, connection)
        {
        }

        ///<Summary>
        ///release the output object
        ///<para>
        ///Using this request a client can tell the server that it is not going to
        ///use the output object anymore.
        ///</para>
        ///</Summary>
        public void Release()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Release);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "Release");
        }

        public enum RequestOpcode : ushort
        {
            Release
        }

        ///<Summary>
        ///properties of the output
        ///<para>
        ///The geometry event describes geometric properties of the output.
        ///The event is sent when binding to the output object and whenever
        ///any of the properties change.
        ///</para>
        ///<para>
        ///The physical size can be set to zero if it doesn't make sense for this
        ///output (e.g. for projectors or virtual outputs).
        ///</para>
        ///<para>
        ///The geometry event will be followed by a done event (starting from
        ///version 2).
        ///</para>
        ///<para>
        ///Note: wl_output only advertises partial information about the output
        ///position and identification. Some compositors, for instance those not
        ///implementing a desktop-style output layout or those exposing virtual
        ///outputs, might fake this information. Instead of using x and y, clients
        ///should use xdg_output.logical_position. Instead of using make and model,
        ///clients should use name and description.
        ///</para>
        ///</Summary>
        public Action<WlOutput, int, int, int, int, SubpixelFlag, string, string, TransformFlag> geometry;
        ///<Summary>
        ///advertise available modes for the output
        ///<para>
        ///The mode event describes an available mode for the output.
        ///</para>
        ///<para>
        ///The event is sent when binding to the output object and there
        ///will always be one mode, the current mode.  The event is sent
        ///again if an output changes mode, for the mode that is now
        ///current.  In other words, the current mode is always the last
        ///mode that was received with the current flag set.
        ///</para>
        ///<para>
        ///Non-current modes are deprecated. A compositor can decide to only
        ///advertise the current mode and never send other modes. Clients
        ///should not rely on non-current modes.
        ///</para>
        ///<para>
        ///The size of a mode is given in physical hardware units of
        ///the output device. This is not necessarily the same as
        ///the output size in the global compositor space. For instance,
        ///the output may be scaled, as described in wl_output.scale,
        ///or transformed, as described in wl_output.transform. Clients
        ///willing to retrieve the output size in the global compositor
        ///space should use xdg_output.logical_size instead.
        ///</para>
        ///<para>
        ///The vertical refresh rate can be set to zero if it doesn't make
        ///sense for this output (e.g. for virtual outputs).
        ///</para>
        ///<para>
        ///The mode event will be followed by a done event (starting from
        ///version 2).
        ///</para>
        ///<para>
        ///Clients should not use the refresh rate to schedule frames. Instead,
        ///they should use the wl_surface.frame event or the presentation-time
        ///protocol.
        ///</para>
        ///<para>
        ///Note: this information is not always meaningful for all outputs. Some
        ///compositors, such as those exposing virtual outputs, might fake the
        ///refresh rate or the size.
        ///</para>
        ///</Summary>
        public Action<WlOutput, ModeFlag, int, int, int> mode;
        ///<Summary>
        ///sent all information about output
        ///<para>
        ///This event is sent after all other properties have been
        ///sent after binding to the output object and after any
        ///other property changes done after that. This allows
        ///changes to the output properties to be seen as
        ///atomic, even if they happen via multiple events.
        ///</para>
        ///</Summary>
        public Action<WlOutput> done;
        ///<Summary>
        ///output scaling properties
        ///<para>
        ///This event contains scaling geometry information
        ///that is not in the geometry event. It may be sent after
        ///binding the output object or if the output scale changes
        ///later. If it is not sent, the client should assume a
        ///scale of 1.
        ///</para>
        ///<para>
        ///A scale larger than 1 means that the compositor will
        ///automatically scale surface buffers by this amount
        ///when rendering. This is used for very high resolution
        ///displays where applications rendering at the native
        ///resolution would be too small to be legible.
        ///</para>
        ///<para>
        ///It is intended that scaling aware clients track the
        ///current output of a surface, and if it is on a scaled
        ///output it should use wl_surface.set_buffer_scale with
        ///the scale of the output. That way the compositor can
        ///avoid scaling the surface, and the client can supply
        ///a higher detail image.
        ///</para>
        ///<para>
        ///The scale event will be followed by a done event.
        ///</para>
        ///</Summary>
        public Action<WlOutput, int> scale;
        ///<Summary>
        ///name of this output
        ///<para>
        ///Many compositors will assign user-friendly names to their outputs, show
        ///them to the user, allow the user to refer to an output, etc. The client
        ///may wish to know this name as well to offer the user similar behaviors.
        ///</para>
        ///<para>
        ///The name is a UTF-8 string with no convention defined for its contents.
        ///Each name is unique among all wl_output globals. The name is only
        ///guaranteed to be unique for the compositor instance.
        ///</para>
        ///<para>
        ///The same output name is used for all clients for a given wl_output
        ///global. Thus, the name can be shared across processes to refer to a
        ///specific wl_output global.
        ///</para>
        ///<para>
        ///The name is not guaranteed to be persistent across sessions, thus cannot
        ///be used to reliably identify an output in e.g. configuration files.
        ///</para>
        ///<para>
        ///Examples of names include 'HDMI-A-1', 'WL-1', 'X11-1', etc. However, do
        ///not assume that the name is a reflection of an underlying DRM connector,
        ///X11 connection, etc.
        ///</para>
        ///<para>
        ///The name event is sent after binding the output object. This event is
        ///only sent once per output object, and the name does not change over the
        ///lifetime of the wl_output global.
        ///</para>
        ///<para>
        ///Compositors may re-use the same output name if the wl_output global is
        ///destroyed and re-created later. Compositors should avoid re-using the
        ///same name if possible.
        ///</para>
        ///<para>
        ///The name event will be followed by a done event.
        ///</para>
        ///</Summary>
        public Action<WlOutput, string> name;
        ///<Summary>
        ///human-readable description of this output
        ///<para>
        ///Many compositors can produce human-readable descriptions of their
        ///outputs. The client may wish to know this description as well, e.g. for
        ///output selection purposes.
        ///</para>
        ///<para>
        ///The description is a UTF-8 string with no convention defined for its
        ///contents. The description is not guaranteed to be unique among all
        ///wl_output globals. Examples might include 'Foocorp 11" Display' or
        ///'Virtual X11 output via :1'.
        ///</para>
        ///<para>
        ///The description event is sent after binding the output object and
        ///whenever the description changes. The description is optional, and may
        ///not be sent at all.
        ///</para>
        ///<para>
        ///The description event will be followed by a done event.
        ///</para>
        ///</Summary>
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

        public override void Event(ushort opCode, WlType[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Geometry:
                {
                    var x = arguments[0].i;
                    var y = arguments[1].i;
                    var physicalWidth = arguments[2].i;
                    var physicalHeight = arguments[3].i;
                    var subpixel = (SubpixelFlag)arguments[4].u;
                    var make = arguments[5].s;
                    var model = arguments[6].s;
                    var transform = (TransformFlag)arguments[7].u;
                    if (this.geometry != null)
                    {
                        this.geometry.Invoke(this, x, y, physicalWidth, physicalHeight, subpixel, make, model, transform);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Geometry");
                    }

                    break;
                }

                case EventOpcode.Mode:
                {
                    var flags = (ModeFlag)arguments[0].u;
                    var width = arguments[1].i;
                    var height = arguments[2].i;
                    var refresh = arguments[3].i;
                    if (this.mode != null)
                    {
                        this.mode.Invoke(this, flags, width, height, refresh);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Mode", this, flags, width, height, refresh);
                    }

                    break;
                }

                case EventOpcode.Done:
                {
                    if (this.done != null)
                    {
                        this.done.Invoke(this);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Done", this);
                    }

                    break;
                }

                case EventOpcode.Scale:
                {
                    var factor = arguments[0].i;
                    if (this.scale != null)
                    {
                        this.scale.Invoke(this, factor);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Scale", this, factor);
                    }

                    break;
                }

                case EventOpcode.Name:
                {
                    var name = arguments[0].s;
                    if (this.name != null)
                    {
                        this.name.Invoke(this, name);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Name", this, name);
                    }

                    break;
                }

                case EventOpcode.Description:
                {
                    var description = arguments[0].s;
                    if (this.description != null)
                    {
                        this.description.Invoke(this, description);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Description", this, description);
                    }

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
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
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }

        ///<Summary>
        ///subpixel geometry information
        ///<para>
        ///This enumeration describes how the physical
        ///pixels on an output are laid out.
        ///</para>
        ///</Summary>
        public enum SubpixelFlag : uint
        {
            ///<Summary>
            ///unknown geometry
            ///</Summary>
            Unknown = 0,
            ///<Summary>
            ///no geometry
            ///</Summary>
            None = 1,
            ///<Summary>
            ///horizontal RGB
            ///</Summary>
            HorizontalRgb = 2,
            ///<Summary>
            ///horizontal BGR
            ///</Summary>
            HorizontalBgr = 3,
            ///<Summary>
            ///vertical RGB
            ///</Summary>
            VerticalRgb = 4,
            ///<Summary>
            ///vertical BGR
            ///</Summary>
            VerticalBgr = 5,
        }

        ///<Summary>
        ///transform from framebuffer to output
        ///<para>
        ///This describes the transform that a compositor will apply to a
        ///surface to compensate for the rotation or mirroring of an
        ///output device.
        ///</para>
        ///<para>
        ///The flipped values correspond to an initial flip around a
        ///vertical axis followed by rotation.
        ///</para>
        ///<para>
        ///The purpose is mainly to allow clients to render accordingly and
        ///tell the compositor, so that for fullscreen surfaces, the
        ///compositor will still be able to scan out directly from client
        ///surfaces.
        ///</para>
        ///</Summary>
        public enum TransformFlag : uint
        {
            ///<Summary>
            ///no transform
            ///</Summary>
            Normal = 0,
            ///<Summary>
            ///90 degrees counter-clockwise
            ///</Summary>
            _90 = 1,
            ///<Summary>
            ///180 degrees counter-clockwise
            ///</Summary>
            _180 = 2,
            ///<Summary>
            ///270 degrees counter-clockwise
            ///</Summary>
            _270 = 3,
            ///<Summary>
            ///180 degree flip around a vertical axis
            ///</Summary>
            Flipped = 4,
            ///<Summary>
            ///flip and rotate 90 degrees counter-clockwise
            ///</Summary>
            Flipped90 = 5,
            ///<Summary>
            ///flip and rotate 180 degrees counter-clockwise
            ///</Summary>
            Flipped180 = 6,
            ///<Summary>
            ///flip and rotate 270 degrees counter-clockwise
            ///</Summary>
            Flipped270 = 7,
        }

        ///<Summary>
        ///mode information
        ///<para>
        ///These flags describe properties of an output mode.
        ///They are used in the flags bitfield of the mode event.
        ///</para>
        ///</Summary>
        public enum ModeFlag : uint
        {
            ///<Summary>
            ///indicates this is the current mode
            ///</Summary>
            Current = 0x1,
            ///<Summary>
            ///indicates this is the preferred mode
            ///</Summary>
            Preferred = 0x2,
        }
    }
}
