using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Wayland.Compatibility
{
    public static class WlDisplayNative
    {
        private static WaylandConnection connection;

		public unsafe static WlDisplay Connect(string displayPath = null)
        {
            MethodInfo connectMethod = typeof(WlDisplay).GetMethod("ConnectSocket", BindingFlags.Static | BindingFlags.NonPublic);

            connection = (WaylandConnection)connectMethod.Invoke(null, new object[] { displayPath });

			WaylandSocket socket = (WaylandSocket)typeof(WaylandConnection).GetField("socket", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(connection);

			IntPtr displayPtr = ConnectToFd((int)typeof(WaylandSocket).GetField("socket", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(socket));

			wl_display* dis = (wl_display*)displayPtr.ToPointer();

            connection.Get(dis->proxy.@object.id);
            WlDisplay display = new WlDisplay(0,ref dis->proxy.@object.id, connection);

			display.handle = displayPtr;

			display.error += Error;
			display.deleteId += Delete;
			connection[display.id] = display;

			Func<uint, (IntPtr handle, uint id, uint version)> GetHandle = (factoryId) => 
			{
				wl_proxy* proxy = ProxyCreate((wl_proxy*)connection[factoryId].handle, connection[factoryId].handle);

				connection.Get(proxy->@object.id);

				return ((IntPtr)proxy, proxy->@object.id, proxy->version);
			};
			Action<IntPtr> DeleteHandle = (proxy) => 
			{
				ProxyDestroy((wl_proxy*)proxy);
			};

			typeof(WaylandConnection).GetField("GetHandle", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(connection, GetHandle);
			typeof(WaylandConnection).GetField("DeleteHandle", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(connection, DeleteHandle);

			

            return display;
        }

        private static void Delete(WlDisplay display, uint id)
        {
            connection.Destroy(id);
        }

        private static void Error(WlDisplay wlDisplay, WaylandObject @object, uint code, string message)
        {
            throw new Exception($"Error {code} object {@object.id}: {message}");
        }

        [DllImport("libwayland-client.so.0", EntryPoint = "wl_display_connect_to_fd", SetLastError = true)]
        private unsafe static extern IntPtr ConnectToFd(int fd);
		
		[DllImport("libwayland-client.so.0", EntryPoint = "wl_proxy_destroy", SetLastError = true)]
        private unsafe static extern void ProxyDestroy(wl_proxy* proxy);

		[DllImport("libwayland-client.so.0", EntryPoint = "wl_display_roundtrip", SetLastError = true)]
		private unsafe static extern int RoundTrip(IntPtr display);
		
		[DllImport("libwayland-client.so.0", EntryPoint = "wl_proxy_create", SetLastError = true)]
		private unsafe static extern wl_proxy* ProxyCreate(wl_proxy* factory, IntPtr @interface);
		
		[DllImport("libwayland-client.so.0", EntryPoint = "wl_proxy_marshal_array_flags", SetLastError = true)]
		private unsafe static extern wl_proxy* MarshalArrayFlags(wl_proxy* proxy, uint opcode, IntPtr @interface, uint version, uint flags, wl_argument* args);


		private static unsafe wl_proxy* MarshalArrayConstructorVersioned(wl_proxy *proxy,uint opcode, wl_argument* args, IntPtr @interface, uint version)
		{
			return MarshalArrayFlags(proxy, opcode, @interface, version, 0, args);
		}

		private static unsafe wl_proxy* MarshalArrayConstructor(wl_proxy *proxy, uint opcode,  wl_argument* args, IntPtr @interface)
		{
			return MarshalArrayConstructorVersioned(proxy, opcode,args, @interface,proxy->version);
		}

		private static unsafe void MarshalArray(wl_proxy *proxy, uint opcode, wl_argument* args)
		{
			MarshalArrayConstructor(proxy, opcode, args, IntPtr.Zero);
		}

		[StructLayout(LayoutKind.Explicit)]
		unsafe struct wl_argument 
		{
            [FieldOffset(0)]
			public int i;           /**< `int`    */
			[FieldOffset(0)]
			public uint u;          /**< `uint`   */
			[FieldOffset(0)]
			public double f;        /**< `fixed`  */
			[FieldOffset(0)]
			public char* s;       /**< `string` */
			[FieldOffset(0)]
			public IntPtr o; /**< `object` */
			[FieldOffset(0)]
			public uint n;          /**< `new_id` */
			[FieldOffset(0)]
			public wl_array* a;  /**< `array`  */
			[FieldOffset(0)]
			public int h;           /**< `fd`     */
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		unsafe struct wl_array
		{
			/** Array size */
			public long size;
			/** Allocated space */
			public long alloc;
			/** Array data */
			public void* data;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		unsafe struct wl_interface
		{
			/** Interface name */
			public char* name;
			/** Interface version */
			public int version;
			/** Number of methods (requests) */
			public int method_count;
			/** Method (request) signatures */
			public IntPtr methods;
			/** Number of events */
			public int event_count;
			/** Event signatures */
			public IntPtr events;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		unsafe struct wl_message
		{
			/** Message name */
			public char* name;
			/** Message signature */
			public char* signature;
			/** Object argument interfaces */
			public wl_interface **types;
};

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		unsafe struct wl_proxy 
		{
			public wl_object @object;
			public IntPtr display;
			public IntPtr queue;
			public uint flags;
			public int refcount;
			public IntPtr user_data;
			public IntPtr dispatcher;
			public uint version;
			public IntPtr tag;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct wl_object
		{
			public IntPtr @interface;
			public IntPtr implementation;
			public uint id;
		};

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		unsafe struct ProtcolError {
			/* Code of the error. It can be compared to
			 * the interface's errors enumeration. */
			public uint code;
			/* interface (protocol) in which the error occurred */
			public wl_interface @interface;
			/* id of the proxy that caused the error. There's no warranty
			 * that the proxy is still valid. It's up to client how it will
			 * use it */
			public uint id;
		}


		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct wl_display {
			public wl_proxy proxy;
			public IntPtr connection;

			/* errno of the last wl_display error */
			public int last_error;

			/* When display gets an error event from some object, it stores
			 * information about it here, so that client can get this
			 * information afterwards */
			public IntPtr protocol_error;
			public int fd;
			public IntPtr objects;
			public IntPtr display_queue;
			public IntPtr default_queue;
			public IntPtr mutex;

			public int reader_count;
			public uint read_serial;
			public IntPtr reader_cond;
		}

    }
}
