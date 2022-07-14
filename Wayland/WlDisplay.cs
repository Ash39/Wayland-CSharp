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
            WaylandConnection connection = new WaylandConnection(path);

            uint id = connection.Create();
            WlDisplay display = new WlDisplay(id, 1, connection);
            display.error += Error;
            display.deleteId += Delete;
            connection[id] = display;

            return display;
        }

        private static void Delete(WlDisplay display, uint id)
        {
            display.connection.Destroy(id);
        }

        private static void Error(WlDisplay wlDisplay,WaylandObject @object, uint code, string message)
        {
            throw new Exception($"Error {code} object {@object.id}: {message}");
        }

        public void Dispatch() 
        {
            int length = connection.Flush();

            while(length > connection.position)
            {
                if (!connection.Read())
                    break;
            }
            DispatchPending();
        }
        
        public void DispatchPending() 
        {
            while (connection.Events.Count > 0)
            {
                connection.Events.Dequeue().Invoke();
            }
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
    }
}
