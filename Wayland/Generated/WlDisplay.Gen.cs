using System;
using System.Collections.Generic;

namespace Wayland
{
    ///<Summary>
    ///core global object
    ///<para>
    ///The core global object.  This is a special singleton object.  It
    ///is used for internal Wayland protocol features.
    ///</para>
    ///</Summary>
    public partial class WlDisplay : WaylandObject
    {
        public const string INTERFACE = "wl_display";
        public WlDisplay(uint id, WaylandConnection connection, uint version = 1) : base(id, version, connection)
        {
        }

        ///<Summary>
        ///asynchronous roundtrip
        ///<para>
        ///The sync request asks the server to emit the 'done' event
        ///on the returned wl_callback object.  Since requests are
        ///handled in-order and events are delivered in-order, this can
        ///be used as a barrier to ensure all previous requests and the
        ///resulting events have been handled.
        ///</para>
        ///<para>
        ///The object returned by this request will be destroyed by the
        ///compositor after the callback is fired and as such the client must not
        ///attempt to use it after that point.
        ///</para>
        ///<para>
        ///The callback_data passed in the callback is the event serial.
        ///</para>
        ///</Summary>
        ///<returns> callback object for the sync request </returns>
        public WlCallback Sync()
        {
            WlCallback wObject = connection.Create<WlCallback>(0, this.version);
            uint callback = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.Sync, callback);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "Sync", callback);
            return wObject;
        }

        ///<Summary>
        ///get global registry object
        ///<para>
        ///This request creates a registry object that allows the client
        ///to list and bind the global objects available from the
        ///compositor.
        ///</para>
        ///<para>
        ///It should be noted that the server side resources consumed in
        ///response to a get_registry request can only be released when the
        ///client disconnects, not when the client side proxy is destroyed.
        ///Therefore, clients should invoke get_registry as infrequently as
        ///possible to avoid wasting memory.
        ///</para>
        ///</Summary>
        ///<returns> global registry object </returns>
        public WlRegistry GetRegistry()
        {
            WlRegistry wObject = connection.Create<WlRegistry>(0, this.version);
            uint registry = wObject.id;
            connection.Marshal(this.id, (ushort)RequestOpcode.GetRegistry, registry);
            DebugLog.WriteLine(DebugType.Request, INTERFACE, this.id, "GetRegistry", registry);
            return wObject;
        }

        public enum RequestOpcode : ushort
        {
            Sync,
            GetRegistry
        }

        ///<Summary>
        ///fatal error event
        ///<para>
        ///The error event is sent out when a fatal (non-recoverable)
        ///error has occurred.  The object_id argument is the object
        ///where the error occurred, most often in response to a request
        ///to that object.  The code identifies the error and is defined
        ///by the object interface.  As such, each interface defines its
        ///own set of error codes.  The message is a brief description
        ///of the error, for (debugging) convenience.
        ///</para>
        ///</Summary>
        public Action<WlDisplay, WaylandObject, uint, string> error;
        ///<Summary>
        ///acknowledge object ID deletion
        ///<para>
        ///This event is used internally by the object ID management
        ///logic. When a client deletes an object that it had created,
        ///the server will send this event to acknowledge that it has
        ///seen the delete request. When the client receives this event,
        ///it will know that it can safely reuse the object ID.
        ///</para>
        ///</Summary>
        public Action<WlDisplay, uint> deleteId;
        public enum EventOpcode : ushort
        {
            Error,
            DeleteId
        }

        public override void Event(ushort opCode, WlType[] arguments)
        {
            switch ((EventOpcode)opCode)
            {
                case EventOpcode.Error:
                {
                    var objectId = connection[arguments[0].u];
                    var code = arguments[1].u;
                    var message = arguments[2].s;
                    if (this.error != null)
                    {
                        this.error.Invoke(this, objectId, code, message);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "Error");
                    }

                    break;
                }

                case EventOpcode.DeleteId:
                {
                    var id = arguments[0].u;
                    if (this.deleteId != null)
                    {
                        this.deleteId.Invoke(this, id);
                        DebugLog.WriteLine(DebugType.Event, INTERFACE, this.id, "DeleteId", this, id);
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
                case EventOpcode.Error:
                    return new WaylandType[]{WaylandType.Object, WaylandType.Uint, WaylandType.String, };
                case EventOpcode.DeleteId:
                    return new WaylandType[]{WaylandType.Uint, };
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode), "unknown event");
            }
        }

        ///<Summary>
        ///global error values
        ///<para>
        ///These errors are global and can be emitted in response to any
        ///server request.
        ///</para>
        ///</Summary>
        public enum ErrorFlag : uint
        {
            ///<Summary>
            ///server couldn't find object
            ///</Summary>
            InvalidObject = 0,
            ///<Summary>
            ///method doesn't exist on the specified interface or malformed request
            ///</Summary>
            InvalidMethod = 1,
            ///<Summary>
            ///server is out of memory
            ///</Summary>
            NoMemory = 2,
            ///<Summary>
            ///implementation error in compositor
            ///</Summary>
            Implementation = 3,
        }
    }
}
