using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///factory for creating dmabuf-based wl_buffers
    ///<para>
    ///Following the interfaces from:
    ///https://www.khronos.org/registry/egl/extensions/EXT/EGL_EXT_image_dma_buf_import.txt
    ///https://www.khronos.org/registry/EGL/extensions/EXT/EGL_EXT_image_dma_buf_import_modifiers.txt
    ///and the Linux DRM sub-system's AddFb2 ioctl.
    ///</para>
    ///<para>
    ///This interface offers ways to create generic dmabuf-based wl_buffers.
    ///</para>
    ///<para>
    ///Clients can use the get_surface_feedback request to get dmabuf feedback
    ///for a particular surface. If the client wants to retrieve feedback not
    ///tied to a surface, they can use the get_default_feedback request.
    ///</para>
    ///<para>
    ///The following are required from clients:
    ///</para>
    ///<para>
    ///- Clients must ensure that either all data in the dma-buf is
    ///coherent for all subsequent read access or that coherency is
    ///correctly handled by the underlying kernel-side dma-buf
    ///implementation.
    ///</para>
    ///<para>
    ///- Don't make any more attachments after sending the buffer to the
    ///compositor. Making more attachments later increases the risk of
    ///the compositor not being able to use (re-import) an existing
    ///dmabuf-based wl_buffer.
    ///</para>
    ///<para>
    ///The underlying graphics stack must ensure the following:
    ///</para>
    ///<para>
    ///- The dmabuf file descriptors relayed to the server will stay valid
    ///for the whole lifetime of the wl_buffer. This means the server may
    ///at any time use those fds to import the dmabuf into any kernel
    ///sub-system that might accept it.
    ///</para>
    ///<para>
    ///However, when the underlying graphics stack fails to deliver the
    ///promise, because of e.g. a device hot-unplug which raises internal
    ///errors, after the wl_buffer has been successfully created the
    ///compositor must not raise protocol errors to the client when dmabuf
    ///import later fails.
    ///</para>
    ///<para>
    ///To create a wl_buffer from one or more dmabufs, a client creates a
    ///zwp_linux_dmabuf_params_v1 object with a zwp_linux_dmabuf_v1.create_params
    ///request. All planes required by the intended format are added with
    ///the 'add' request. Finally, a 'create' or 'create_immed' request is
    ///issued, which has the following outcome depending on the import success.
    ///</para>
    ///<para>
    ///The 'create' request,
    ///- on success, triggers a 'created' event which provides the final
    ///wl_buffer to the client.
    ///- on failure, triggers a 'failed' event to convey that the server
    ///cannot use the dmabufs received from the client.
    ///</para>
    ///<para>
    ///For the 'create_immed' request,
    ///- on success, the server immediately imports the added dmabufs to
    ///create a wl_buffer. No event is sent from the server in this case.
    ///- on failure, the server can choose to either:
    ///- terminate the client by raising a fatal error.
    ///- mark the wl_buffer as failed, and send a 'failed' event to the
    ///client. If the client uses a failed wl_buffer as an argument to any
    ///request, the behaviour is compositor implementation-defined.
    ///</para>
    ///<para>
    ///For all DRM formats and unless specified in another protocol extension,
    ///pre-multiplied alpha is used for pixel values.
    ///</para>
    ///<para>
    ///Warning! The protocol described in this file is experimental and
    ///backward incompatible changes may be made. Backward compatible changes
    ///may be added together with the corresponding interface version bump.
    ///Backward incompatible changes are done by bumping the version number in
    ///the protocol and interface names and resetting the interface version.
    ///Once the protocol is to be declared stable, the 'z' prefix and the
    ///version number in the protocol and interface names are removed and the
    ///interface version number is reset.
    ///</para>
    ///</Summary>
    public partial class ZwpLinuxDmabufV1 : WaylandObject
    {
        public const string INTERFACE = "zwp_linux_dmabuf_v1";
        public ZwpLinuxDmabufV1(uint id, WaylandConnection connection, uint version = 4) : base(id, version, connection)
        {
        }

        ///<Summary>
        ///unbind the factory
        ///<para>
        ///Objects created through this interface, especially wl_buffers, will
        ///remain valid.
        ///</para>
        ///</Summary>
        public void Destroy()
        {
            connection.Marshal(this.id, (ushort)RequestOpcode.Destroy);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "Destroy");
        }

        ///<Summary>
        ///create a temporary object for buffer parameters
        ///<para>
        ///This temporary object is used to collect multiple dmabuf handles into
        ///a single batch to create a wl_buffer. It can only be used once and
        ///should be destroyed after a 'created' or 'failed' event has been
        ///received.
        ///</para>
        ///</Summary>
        ///<returns> the new temporary </returns>
        public ZwpLinuxBufferParamsV1 CreateParams()
        {
            ZwpLinuxBufferParamsV1 wObject = connection.Create<ZwpLinuxBufferParamsV1>(0, this.version);
            uint params_id = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.CreateParams, params_id);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "CreateParams", params_id);
            return wObject;
        }

        ///<Summary>
        ///get default feedback
        ///<para>
        ///This request creates a new wp_linux_dmabuf_feedback object not bound
        ///to a particular surface. This object will deliver feedback about dmabuf
        ///parameters to use if the client doesn't support per-surface feedback
        ///(see get_surface_feedback).
        ///</para>
        ///</Summary>
        ///<returns>  </returns>
        public ZwpLinuxDmabufFeedbackV1 GetDefaultFeedback()
        {
            ZwpLinuxDmabufFeedbackV1 wObject = connection.Create<ZwpLinuxDmabufFeedbackV1>(0, this.version);
            uint id = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.GetDefaultFeedback, id);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "GetDefaultFeedback", id);
            return wObject;
        }

        ///<Summary>
        ///get feedback for a surface
        ///<para>
        ///This request creates a new wp_linux_dmabuf_feedback object for the
        ///specified wl_surface. This object will deliver feedback about dmabuf
        ///parameters to use for buffers attached to this surface.
        ///</para>
        ///<para>
        ///If the surface is destroyed before the wp_linux_dmabuf_feedback object,
        ///the feedback object becomes inert.
        ///</para>
        ///</Summary>
        ///<returns>  </returns>
        ///<param name = "surface">  </param>
        public ZwpLinuxDmabufFeedbackV1 GetSurfaceFeedback(WlSurface surface)
        {
            ZwpLinuxDmabufFeedbackV1 wObject = connection.Create<ZwpLinuxDmabufFeedbackV1>(0, this.version);
            uint id = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.GetSurfaceFeedback, id, surface.id);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "GetSurfaceFeedback", id, surface.id);
            return wObject;
        }

        public enum RequestOpcode : ushort
        {
            Destroy,
            CreateParams,
            GetDefaultFeedback,
            GetSurfaceFeedback
        }

        ///<Summary>
        ///supported buffer format
        ///<para>
        ///This event advertises one buffer format that the server supports.
        ///All the supported formats are advertised once when the client
        ///binds to this interface. A roundtrip after binding guarantees
        ///that the client has received all supported formats.
        ///</para>
        ///<para>
        ///For the definition of the format codes, see the
        ///zwp_linux_buffer_params_v1::create request.
        ///</para>
        ///<para>
        ///Starting version 4, the format event is deprecated and must not be
        ///sent by compositors. Instead, use get_default_feedback or
        ///get_surface_feedback.
        ///</para>
        ///</Summary>
        public Action<ZwpLinuxDmabufV1, uint> format;
        ///<Summary>
        ///supported buffer format modifier
        ///<para>
        ///This event advertises the formats that the server supports, along with
        ///the modifiers supported for each format. All the supported modifiers
        ///for all the supported formats are advertised once when the client
        ///binds to this interface. A roundtrip after binding guarantees that
        ///the client has received all supported format-modifier pairs.
        ///</para>
        ///<para>
        ///For legacy support, DRM_FORMAT_MOD_INVALID (that is, modifier_hi ==
        ///0x00ffffff and modifier_lo == 0xffffffff) is allowed in this event.
        ///It indicates that the server can support the format with an implicit
        ///modifier. When a plane has DRM_FORMAT_MOD_INVALID as its modifier, it
        ///is as if no explicit modifier is specified. The effective modifier
        ///will be derived from the dmabuf.
        ///</para>
        ///<para>
        ///A compositor that sends valid modifiers and DRM_FORMAT_MOD_INVALID for
        ///a given format supports both explicit modifiers and implicit modifiers.
        ///</para>
        ///<para>
        ///For the definition of the format and modifier codes, see the
        ///zwp_linux_buffer_params_v1::create and zwp_linux_buffer_params_v1::add
        ///requests.
        ///</para>
        ///<para>
        ///Starting version 4, the modifier event is deprecated and must not be
        ///sent by compositors. Instead, use get_default_feedback or
        ///get_surface_feedback.
        ///</para>
        ///</Summary>
        public Action<ZwpLinuxDmabufV1, uint, uint, uint> modifier;
        public enum EventOpcode : ushort
        {
            Format,
            Modifier
        }

        public override void Event(ushort opCode, WlType[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Format:
                {
                    var format = arguments[0].u;
                    if (this.format != null)
                    {
                        this.format.Invoke(this, format);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Format");
                    }

                    break;
                }

                case EventOpcode.Modifier:
                {
                    var format = arguments[0].u;
                    var modifierHi = arguments[1].u;
                    var modifierLo = arguments[2].u;
                    if (this.modifier != null)
                    {
                        this.modifier.Invoke(this, format, modifierHi, modifierLo);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Modifier", this, format, modifierHi, modifierLo);
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
                case EventOpcode.Format:
                    return new WaylandType[]{WaylandType.Uint, };
                case EventOpcode.Modifier:
                    return new WaylandType[]{WaylandType.Uint, WaylandType.Uint, WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException("unknown event");
            }
        }
    }
}
