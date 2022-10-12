using System.Runtime.InteropServices;
using System.Diagnostics;
using StbImageSharp;
using System;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using OpenGL;

namespace Wayland.Sample 
{
    internal class Program
    {
        private static Window window;
        private static readonly Stopwatch _watchRender = new Stopwatch();

        static void Main(string[] args)
        {
            window = new Window();
            window.Create("Game Window", 1280, 720, GraphicsApi.OpenGlES);

            double RenderFrequency = 60.00;
            
            _watchRender.Start();
            
            while(true)
            {
                var elapsed = _watchRender.Elapsed.TotalSeconds;
                var renderPeriod = RenderFrequency == 0 ? 0 : 1 / RenderFrequency;
                if (elapsed > 0 && elapsed >= renderPeriod)
                {
                    _watchRender.Restart();
                    window.PollEvents();
                    Gl.Viewport(0,0,1280,720);
                    Gl.ClearColor( c += 0.001f, 0 , 0, 1);
                    if(c >= 1)
                        c = 0;
                    Gl.Clear(ClearBufferMask.ColorBufferBit);

                    window.Present();
                }
                
            }

        }


        static float c = 0;

    }
}