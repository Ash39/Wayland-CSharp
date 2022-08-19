using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///parameters for creating a dmabuf-based wl_buffer
    ///<para>
    ///This temporary object is a collection of dmabufs and other
    ///parameters that together form a single logical buffer. The temporary
    ///object may eventually create one wl_buffer unless cancelled by
    ///destroying it before requesting 'create'.
    ///</para>
    ///<para>
    ///Single-planar formats only require one dmabuf, however
    ///multi-planar formats may require more than one dmabuf. For all
    ///formats, an 'add' request must be called once per plane (even if the
    ///underlying dmabuf fd is identical).
    ///</para>
    ///<para>
    ///You must use consecutive plane indices ('plane_idx' argument for 'add')
    ///from zero to the number of planes used by the drm_fourcc format code.
    ///All planes required by the format must be given exactly once, but can
    ///be given in any order. Each plane index can be set only once.
    ///</para>
    ///</Summary>
    public partial class ZwpLinuxBufferParamsV1 : WaylandObject
    {
        public const string INTERFACE = "zwp_linux_buffer_params_v1";
        public ZwpLinuxBufferParamsV1(uint factoryId, ref uint id, WaylandConnection connection, uint version = 4) : base(factoryId, ref id, version, connection)
        {
        }

        ///<Summary>
        ///delete this object, used or not
        ///<para>
        ///Cleans up the temporary data sent to the server for dmabuf-based
        ///wl_buffer creation.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Destroy}()");
        }

        ///<Summary>
        ///add a dmabuf to the temporary set
        ///<para>
        ///This request adds one dmabuf to the set in this
        ///zwp_linux_buffer_params_v1.
        ///</para>
        ///<para>
        ///The 64-bit unsigned value combined from modifier_hi and modifier_lo
        ///is the dmabuf layout modifier. DRM AddFB2 ioctl calls this the
        ///fb modifier, which is defined in drm_mode.h of Linux UAPI.
        ///This is an opaque token. Drivers use this token to express tiling,
        ///compression, etc. driver-specific modifications to the base format
        ///defined by the DRM fourcc code.
        ///</para>
        ///<para>
        ///Starting from version 4, the invalid_format protocol error is sent if
        ///the format + modifier pair was not advertised as supported.
        ///</para>
        ///<para>
        ///This request raises the PLANE_IDX error if plane_idx is too large.
        ///The error PLANE_SET is raised if attempting to set a plane that
        ///was already set.
        ///</para>
        ///</Summary>
        ///<param name = "fd"> dmabuf fd </param>
        ///<param name = "plane_idx"> plane index </param>
        ///<param name = "offset"> offset in bytes </param>
        ///<param name = "stride"> stride in bytes </param>
        ///<param name = "modifier_hi"> high 32 bits of layout modifier </param>
        ///<param name = "modifier_lo"> low 32 bits of layout modifier </param>
        public void Add(IntPtr fd, uint plane_idx, uint offset, uint stride, uint modifier_hi, uint modifier_lo)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Add, fd, plane_idx, offset, stride, modifier_hi, modifier_lo);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Add}({fd},{plane_idx},{offset},{stride},{modifier_hi},{modifier_lo})");
        }

        ///<Summary>
        ///create a wl_buffer from the given dmabufs
        ///<para>
        ///This asks for creation of a wl_buffer from the added dmabuf
        ///buffers. The wl_buffer is not created immediately but returned via
        ///the 'created' event if the dmabuf sharing succeeds. The sharing
        ///may fail at runtime for reasons a client cannot predict, in
        ///which case the 'failed' event is triggered.
        ///</para>
        ///<para>
        ///The 'format' argument is a DRM_FORMAT code, as defined by the
        ///libdrm's drm_fourcc.h. The Linux kernel's DRM sub-system is the
        ///authoritative source on how the format codes should work.
        ///</para>
        ///<para>
        ///The 'flags' is a bitfield of the flags defined in enum "flags".
        ///'y_invert' means the that the image needs to be y-flipped.
        ///</para>
        ///<para>
        ///Flag 'interlaced' means that the frame in the buffer is not
        ///progressive as usual, but interlaced. An interlaced buffer as
        ///supported here must always contain both top and bottom fields.
        ///The top field always begins on the first pixel row. The temporal
        ///ordering between the two fields is top field first, unless
        ///'bottom_first' is specified. It is undefined whether 'bottom_first'
        ///is ignored if 'interlaced' is not set.
        ///</para>
        ///<para>
        ///This protocol does not convey any information about field rate,
        ///duration, or timing, other than the relative ordering between the
        ///two fields in one buffer. A compositor may have to estimate the
        ///intended field rate from the incoming buffer rate. It is undefined
        ///whether the time of receiving wl_surface.commit with a new buffer
        ///attached, applying the wl_surface state, wl_surface.frame callback
        ///trigger, presentation, or any other point in the compositor cycle
        ///is used to measure the frame or field times. There is no support
        ///for detecting missed or late frames/fields/buffers either, and
        ///there is no support whatsoever for cooperating with interlaced
        ///compositor output.
        ///</para>
        ///<para>
        ///The composited image quality resulting from the use of interlaced
        ///buffers is explicitly undefined. A compositor may use elaborate
        ///hardware features or software to deinterlace and create progressive
        ///output frames from a sequence of interlaced input buffers, or it
        ///may produce substandard image quality. However, compositors that
        ///cannot guarantee reasonable image quality in all cases are recommended
        ///to just reject all interlaced buffers.
        ///</para>
        ///<para>
        ///Any argument errors, including non-positive width or height,
        ///mismatch between the number of planes and the format, bad
        ///format, bad offset or stride, may be indicated by fatal protocol
        ///errors: INCOMPLETE, INVALID_FORMAT, INVALID_DIMENSIONS,
        ///OUT_OF_BOUNDS.
        ///</para>
        ///<para>
        ///Dmabuf import errors in the server that are not obvious client
        ///bugs are returned via the 'failed' event as non-fatal. This
        ///allows attempting dmabuf sharing and falling back in the client
        ///if it fails.
        ///</para>
        ///<para>
        ///This request can be sent only once in the object's lifetime, after
        ///which the only legal request is destroy. This object should be
        ///destroyed after issuing a 'create' request. Attempting to use this
        ///object after issuing 'create' raises ALREADY_USED protocol error.
        ///</para>
        ///<para>
        ///It is not mandatory to issue 'create'. If a client wants to
        ///cancel the buffer creation, it can just destroy this object.
        ///</para>
        ///</Summary>
        ///<param name = "width"> base plane width in pixels </param>
        ///<param name = "height"> base plane height in pixels </param>
        ///<param name = "format"> DRM_FORMAT code </param>
        ///<param name = "flags"> see enum flags </param>
        public void Create(int width, int height, uint format, FlagsFlag flags)
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Create, width, height, format, (uint)flags);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.Create}({width},{height},{format},{(uint)flags})");
        }

        ///<Summary>
        ///immediately create a wl_buffer from the given                      dmabufs
        ///<para>
        ///This asks for immediate creation of a wl_buffer by importing the
        ///added dmabufs.
        ///</para>
        ///<para>
        ///In case of import success, no event is sent from the server, and the
        ///wl_buffer is ready to be used by the client.
        ///</para>
        ///<para>
        ///Upon import failure, either of the following may happen, as seen fit
        ///by the implementation:
        ///- the client is terminated with one of the following fatal protocol
        ///errors:
        ///- INCOMPLETE, INVALID_FORMAT, INVALID_DIMENSIONS, OUT_OF_BOUNDS,
        ///in case of argument errors such as mismatch between the number
        ///of planes and the format, bad format, non-positive width or
        ///height, or bad offset or stride.
        ///- INVALID_WL_BUFFER, in case the cause for failure is unknown or
        ///plaform specific.
        ///- the server creates an invalid wl_buffer, marks it as failed and
        ///sends a 'failed' event to the client. The result of using this
        ///invalid wl_buffer as an argument in any request by the client is
        ///defined by the compositor implementation.
        ///</para>
        ///<para>
        ///This takes the same arguments as a 'create' request, and obeys the
        ///same restrictions.
        ///</para>
        ///</Summary>
        ///<returns> id for the newly created wl_buffer </returns>
        ///<param name = "width"> base plane width in pixels </param>
        ///<param name = "height"> base plane height in pixels </param>
        ///<param name = "format"> DRM_FORMAT code </param>
        ///<param name = "flags"> see enum flags </param>
        public WlBuffer CreateImmed(int width, int height, uint format, FlagsFlag flags)
        {
            uint buffer_id = connection.Create();
            WlBuffer wObject = new WlBuffer(this.id, ref buffer_id, connection);
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateImmed, buffer_id, width, height, format, (uint)flags);
            DebugLog.WriteLine($"-->{INTERFACE}@{this.id}.{RequestOpcode.CreateImmed}({buffer_id},{width},{height},{format},{(uint)flags})");
            connection[buffer_id] = wObject;
            return (WlBuffer)connection[buffer_id];
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            Add,
            Create,
            CreateImmed
        }

        ///<Summary>
        ///buffer creation succeeded
        ///<para>
        ///This event indicates that the attempted buffer creation was
        ///successful. It provides the new wl_buffer referencing the dmabuf(s).
        ///</para>
        ///<para>
        ///Upon receiving this event, the client should destroy the
        ///zlinux_dmabuf_params object.
        ///</para>
        ///</Summary>
        public Action<ZwpLinuxBufferParamsV1, int> created;
        ///<Summary>
        ///buffer creation failed
        ///<para>
        ///This event indicates that the attempted buffer creation has
        ///failed. It usually means that one of the dmabuf constraints
        ///has not been fulfilled.
        ///</para>
        ///<para>
        ///Upon receiving this event, the client should destroy the
        ///zlinux_buffer_params object.
        ///</para>
        ///</Summary>
        public Action<ZwpLinuxBufferParamsV1> failed;
        public enum EventOpcode : ushort
        {
            Created,
            Failed
        }

        public override void Event(ushort opCode, object[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Created:
                {
                    var buffer = (int)arguments[0];
                    if (this.created != null)
                    {
                        this.created.Invoke(this, buffer);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Created}({this},{buffer})");
                    }

                    break;
                }

                case EventOpcode.Failed:
                {
                    if (this.failed != null)
                    {
                        this.failed.Invoke(this);
                        DebugLog.WriteLine($"{INTERFACE}@{this.id}.{EventOpcode.Failed}({this})");
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
                case EventOpcode.Created:
                    return new WaylandType[]{WaylandType.NewId, };
                case EventOpcode.Failed:
                    return new WaylandType[]{};
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }

        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///the dmabuf_batch object has already been used to create a wl_buffer
            ///</Summary>
            AlreadyUsed = 0,
            ///<Summary>
            ///plane index out of bounds
            ///</Summary>
            PlaneIdx = 1,
            ///<Summary>
            ///the plane index was already set
            ///</Summary>
            PlaneSet = 2,
            ///<Summary>
            ///missing or too many planes to create a buffer
            ///</Summary>
            Incomplete = 3,
            ///<Summary>
            ///format not supported
            ///</Summary>
            InvalidFormat = 4,
            ///<Summary>
            ///invalid width or height
            ///</Summary>
            InvalidDimensions = 5,
            ///<Summary>
            ///offset + stride * height goes out of dmabuf bounds
            ///</Summary>
            OutOfBounds = 6,
            ///<Summary>
            ///invalid wl_buffer resulted from importing dmabufs via                the create_immed request on given buffer_params
            ///</Summary>
            InvalidWlBuffer = 7,
        }

        public enum FlagsFlag : uint
        {
            ///<Summary>
            ///contents are y-inverted
            ///</Summary>
            YInvert = 1,
            ///<Summary>
            ///content is interlaced
            ///</Summary>
            Interlaced = 2,
            ///<Summary>
            ///bottom field first
            ///</Summary>
            BottomFirst = 4,
        }
    }
}
