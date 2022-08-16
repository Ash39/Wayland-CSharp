using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///dmabuf feedback
    ///<para>
    ///This object advertises dmabuf parameters feedback. This includes the
    ///preferred devices and the supported formats/modifiers.
    ///</para>
    ///<para>
    ///The parameters are sent once when this object is created and whenever they
    ///change. The done event is always sent once after all parameters have been
    ///sent. When a single parameter changes, all parameters are re-sent by the
    ///compositor.
    ///</para>
    ///<para>
    ///Compositors can re-send the parameters when the current client buffer
    ///allocations are sub-optimal. Compositors should not re-send the
    ///parameters if re-allocating the buffers would not result in a more optimal
    ///configuration. In particular, compositors should avoid sending the exact
    ///same parameters multiple times in a row.
    ///</para>
    ///<para>
    ///The tranche_target_device and tranche_modifier events are grouped by
    ///tranches of preference. For each tranche, a tranche_target_device, one
    ///tranche_flags and one or more tranche_modifier events are sent, followed
    ///by a tranche_done event finishing the list. The tranches are sent in
    ///descending order of preference. All formats and modifiers in the same
    ///tranche have the same preference.
    ///</para>
    ///<para>
    ///To send parameters, the compositor sends one main_device event, tranches
    ///(each consisting of one tranche_target_device event, one tranche_flags
    ///event, tranche_modifier events and then a tranche_done event), then one
    ///done event.
    ///</para>
    ///</Summary>
    public partial class ZwpLinuxDmabufFeedbackV1 : WaylandObject
    {
        public const string INTERFACE = "zwp_linux_dmabuf_feedback_v1";
        public ZwpLinuxDmabufFeedbackV1(uint factoryId, ref uint id, WaylandConnection connection) : base(factoryId, ref id, 4, connection)
        {
        }

        ///<Summary>
        ///destroy the feedback object
        ///<para>
        ///Using this request a client can tell the server that it is not going to
        ///use the wp_linux_dmabuf_feedback object anymore.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        public enum RequestOpcode : ushort
        {
            Destroy
        }

        ///<Summary>
        ///all feedback has been sent
        ///<para>
        ///This event is sent after all parameters of a wp_linux_dmabuf_feedback
        ///object have been sent.
        ///</para>
        ///<para>
        ///This allows changes to the wp_linux_dmabuf_feedback parameters to be
        ///seen as atomic, even if they happen via multiple events.
        ///</para>
        ///</Summary>
        public Action<ZwpLinuxDmabufFeedbackV1> done;
        ///<Summary>
        ///format and modifier table
        ///<para>
        ///This event provides a file descriptor which can be memory-mapped to
        ///access the format and modifier table.
        ///</para>
        ///<para>
        ///The table contains a tightly packed array of consecutive format +
        ///modifier pairs. Each pair is 16 bytes wide. It contains a format as a
        ///32-bit unsigned integer, followed by 4 bytes of unused padding, and a
        ///modifier as a 64-bit unsigned integer. The native endianness is used.
        ///</para>
        ///<para>
        ///The client must map the file descriptor in read-only private mode.
        ///</para>
        ///<para>
        ///Compositors are not allowed to mutate the table file contents once this
        ///event has been sent. Instead, compositors must create a new, separate
        ///table file and re-send feedback parameters. Compositors are allowed to
        ///store duplicate format + modifier pairs in the table.
        ///</para>
        ///</Summary>
        public Action<ZwpLinuxDmabufFeedbackV1, IntPtr, uint> formatTable;
        ///<Summary>
        ///preferred main device
        ///<para>
        ///This event advertises the main device that the server prefers to use
        ///when direct scan-out to the target device isn't possible. The
        ///advertised main device may be different for each
        ///wp_linux_dmabuf_feedback object, and may change over time.
        ///</para>
        ///<para>
        ///There is exactly one main device. The compositor must send at least
        ///one preference tranche with tranche_target_device equal to main_device.
        ///</para>
        ///<para>
        ///Clients need to create buffers that the main device can import and
        ///read from, otherwise creating the dmabuf wl_buffer will fail (see the
        ///wp_linux_buffer_params.create and create_immed requests for details).
        ///The main device will also likely be kept active by the compositor,
        ///so clients can use it instead of waking up another device for power
        ///savings.
        ///</para>
        ///<para>
        ///In general the device is a DRM node. The DRM node type (primary vs.
        ///render) is unspecified. Clients must not rely on the compositor sending
        ///a particular node type. Clients cannot check two devices for equality
        ///by comparing the dev_t value.
        ///</para>
        ///<para>
        ///If explicit modifiers are not supported and the client performs buffer
        ///allocations on a different device than the main device, then the client
        ///must force the buffer to have a linear layout.
        ///</para>
        ///</Summary>
        public Action<ZwpLinuxDmabufFeedbackV1, byte[]> mainDevice;
        ///<Summary>
        ///a preference tranche has been sent
        ///<para>
        ///This event splits tranche_target_device and tranche_modifier events in
        ///preference tranches. It is sent after a set of tranche_target_device
        ///and tranche_modifier events; it represents the end of a tranche. The
        ///next tranche will have a lower preference.
        ///</para>
        ///</Summary>
        public Action<ZwpLinuxDmabufFeedbackV1> trancheDone;
        ///<Summary>
        ///target device
        ///<para>
        ///This event advertises the target device that the server prefers to use
        ///for a buffer created given this tranche. The advertised target device
        ///may be different for each preference tranche, and may change over time.
        ///</para>
        ///<para>
        ///There is exactly one target device per tranche.
        ///</para>
        ///<para>
        ///The target device may be a scan-out device, for example if the
        ///compositor prefers to directly scan-out a buffer created given this
        ///tranche. The target device may be a rendering device, for example if
        ///the compositor prefers to texture from said buffer.
        ///</para>
        ///<para>
        ///The client can use this hint to allocate the buffer in a way that makes
        ///it accessible from the target device, ideally directly. The buffer must
        ///still be accessible from the main device, either through direct import
        ///or through a potentially more expensive fallback path. If the buffer
        ///can't be directly imported from the main device then clients must be
        ///prepared for the compositor changing the tranche priority or making
        ///wl_buffer creation fail (see the wp_linux_buffer_params.create and
        ///create_immed requests for details).
        ///</para>
        ///<para>
        ///If the device is a DRM node, the DRM node type (primary vs. render) is
        ///unspecified. Clients must not rely on the compositor sending a
        ///particular node type. Clients cannot check two devices for equality by
        ///comparing the dev_t value.
        ///</para>
        ///<para>
        ///This event is tied to a preference tranche, see the tranche_done event.
        ///</para>
        ///</Summary>
        public Action<ZwpLinuxDmabufFeedbackV1, byte[]> trancheTargetDevice;
        ///<Summary>
        ///supported buffer format modifier
        ///<para>
        ///This event advertises the format + modifier combinations that the
        ///compositor supports.
        ///</para>
        ///<para>
        ///It carries an array of indices, each referring to a format + modifier
        ///pair in the last received format table (see the format_table event).
        ///Each index is a 16-bit unsigned integer in native endianness.
        ///</para>
        ///<para>
        ///For legacy support, DRM_FORMAT_MOD_INVALID is an allowed modifier.
        ///It indicates that the server can support the format with an implicit
        ///modifier. When a buffer has DRM_FORMAT_MOD_INVALID as its modifier, it
        ///is as if no explicit modifier is specified. The effective modifier
        ///will be derived from the dmabuf.
        ///</para>
        ///<para>
        ///A compositor that sends valid modifiers and DRM_FORMAT_MOD_INVALID for
        ///a given format supports both explicit modifiers and implicit modifiers.
        ///</para>
        ///<para>
        ///Compositors must not send duplicate format + modifier pairs within the
        ///same tranche or across two different tranches with the same target
        ///device and flags.
        ///</para>
        ///<para>
        ///This event is tied to a preference tranche, see the tranche_done event.
        ///</para>
        ///<para>
        ///For the definition of the format and modifier codes, see the
        ///wp_linux_buffer_params.create request.
        ///</para>
        ///</Summary>
        public Action<ZwpLinuxDmabufFeedbackV1, byte[]> trancheFormats;
        ///<Summary>
        ///tranche flags
        ///<para>
        ///This event sets tranche-specific flags.
        ///</para>
        ///<para>
        ///The scanout flag is a hint that direct scan-out may be attempted by the
        ///compositor on the target device if the client appropriately allocates a
        ///buffer. How to allocate a buffer that can be scanned out on the target
        ///device is implementation-defined.
        ///</para>
        ///<para>
        ///This event is tied to a preference tranche, see the tranche_done event.
        ///</para>
        ///</Summary>
        public Action<ZwpLinuxDmabufFeedbackV1, TrancheFlagsFlag> trancheFlags;
        public enum EventOpcode : ushort
        {
            Done,
            FormatTable,
            MainDevice,
            TrancheDone,
            TrancheTargetDevice,
            TrancheFormats,
            TrancheFlags
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Done:
                {
                    if (this.done != null)
                    {
                        this.done.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Done}({this})");
                    }

                    break;
                }

                case EventOpcode.FormatTable:
                {
                    var fd = (IntPtr)arguments[0];
                    var size = (uint)arguments[1];
                    if (this.formatTable != null)
                    {
                        this.formatTable.Invoke(this, fd, size);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.FormatTable}({this},{fd},{size})");
                    }

                    break;
                }

                case EventOpcode.MainDevice:
                {
                    var device = (byte[])arguments[0];
                    if (this.mainDevice != null)
                    {
                        this.mainDevice.Invoke(this, device);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.MainDevice}({this},{device})");
                    }

                    break;
                }

                case EventOpcode.TrancheDone:
                {
                    if (this.trancheDone != null)
                    {
                        this.trancheDone.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.TrancheDone}({this})");
                    }

                    break;
                }

                case EventOpcode.TrancheTargetDevice:
                {
                    var device = (byte[])arguments[0];
                    if (this.trancheTargetDevice != null)
                    {
                        this.trancheTargetDevice.Invoke(this, device);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.TrancheTargetDevice}({this},{device})");
                    }

                    break;
                }

                case EventOpcode.TrancheFormats:
                {
                    var indices = (byte[])arguments[0];
                    if (this.trancheFormats != null)
                    {
                        this.trancheFormats.Invoke(this, indices);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.TrancheFormats}({this},{indices})");
                    }

                    break;
                }

                case EventOpcode.TrancheFlags:
                {
                    var flags = (TrancheFlagsFlag)arguments[0];
                    if (this.trancheFlags != null)
                    {
                        this.trancheFlags.Invoke(this, flags);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.TrancheFlags}({this},{flags})");
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
                case EventOpcode.Done:
                    return new WaylandType[]{};
                case EventOpcode.FormatTable:
                    return new WaylandType[]{WaylandType.Fd, WaylandType.Uint, };
                case EventOpcode.MainDevice:
                    return new WaylandType[]{WaylandType.Array, };
                case EventOpcode.TrancheDone:
                    return new WaylandType[]{};
                case EventOpcode.TrancheTargetDevice:
                    return new WaylandType[]{WaylandType.Array, };
                case EventOpcode.TrancheFormats:
                    return new WaylandType[]{WaylandType.Array, };
                case EventOpcode.TrancheFlags:
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        public enum TrancheFlagsFlag : uint
        {
            ///<Summary>
            ///direct scan-out tranche
            ///</Summary>
            Scanout = 1,
        }
    }
}
