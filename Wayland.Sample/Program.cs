using System.Runtime.InteropServices;
using System.Diagnostics;
using StbImageSharp;
using System;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using Wayland.Compatibility;
using OpenGL;

namespace Wayland.Sample 
{
    internal class Program
    {
        private static Window window;

        static void Main(string[] args)
        {
            window = new Window();
            window.render += Render;
            window.Create("Game Window", 1280, 720, GraphicsApi.OpenGlES);
            
            

        }


        static float c = 0;

        static void Render()
        {
            Gl.Viewport(0,0,1280,720);
            Gl.ClearColor( c += 0.001f, 0 , 0, 1);
            if(c >= 1)
                c = 0;
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }
    }
}