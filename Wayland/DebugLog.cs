using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayland
{
    public class DebugLog
    {
        public static void WriteLine(string @string) 
        {
            if (Environment.GetEnvironmentVariable("WAYLAND_DEBUG") == "1") 
            {
                Console.WriteLine(@string);
            }
        }
    }
}
