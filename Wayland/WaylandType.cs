using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayland
{
    public enum WaylandType
    {
        Int,
        Uint,
        Fixed,
        Fd,
        Object,
        NewId,
        String,
        Array,
        Handle
    }
}
