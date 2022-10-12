using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayland
{
    public partial class WlDisplay
    {
        public static WlDisplay Connect(string displayPath = null)
        {

            WaylandConnection connection = ConnectSocket(displayPath);

            WlDisplay display = connection.Create<WlDisplay>(0, 1);
            display.error += Error;
            display.deleteId += Delete;

            return display;
        }

        private static WaylandConnection ConnectSocket(string displayPath) 
        {
            displayPath ??= Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
            displayPath ??= "wayland-0";

            string path;
            if (displayPath.StartsWith('/'))
            {
                path = displayPath;
            }
            else
            {
                string xdgRuntimeDir = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
                if (xdgRuntimeDir == null)
                    throw new Exception("XDG_RUNTIME_DIR missing from environment");
                path = Path.Join(xdgRuntimeDir, displayPath);
            }

            return new WaylandConnection(path);
        }

        private static void Delete(WlDisplay display, uint id)
        {
            display.connection.Destroy(id);
        }

        private static void Error(WlDisplay wlDisplay,WaylandObject @object, uint code, string message)
        {
            if(code == 0)
                return;
            throw new Exception($"Error {code}: {message}");
        }

        public bool Dispatch(int timeout = -1)
        {
            DispatchQueue(timeout);
            return DispatchPending();
        }


        public bool DispatchQueue(int timeout = -1)
        {
            int length = connection.Flush(timeout);

            while(length > connection.Position)
            {
                if (!connection.Read())
                    break;
            }
            return connection.Events.Count != 0;
        }

        public bool DispatchPending() 
        {
            if(connection.Events.Count == 0) return false;

            while (connection.Events.Count > 0)
            {
                if (connection.Events.TryDequeue(out Action @event))
                {
                    @event.Invoke();
                }
            }
            return true;
        }

        public void Roundtrip()
        {
            WlCallback callback = Sync();
            bool isDone = false;

            callback.done += (callback,data) =>
            {
                isDone = true;
            };
            
            while (!isDone) 
            {
                Dispatch();
            }

            
        }

        public void Wait() => connection.Wait();

        public int Release() => connection.Release();
    }
}
