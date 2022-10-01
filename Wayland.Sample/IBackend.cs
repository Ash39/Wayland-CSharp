using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wayland.Sample
{
    public interface IBackend
    {
        Action render { get; set;}
        void CreateDisplay(Device device);
        void BindBuffer(Buffer buffer);
        void Present(Window window);
        void Complete(Window window);
    }
}