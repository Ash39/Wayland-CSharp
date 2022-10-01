using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wayland.Sample
{
    public struct InputEvents
    {
        public EventType eventType;
        public Vector2 position;
        public MouseButton button;
        public State state;
        public uint time;
        

    }

    public enum MouseButton: uint
    {
        Left = 272,
        Right,
        Middle,
        Extra1,
        Extra2,
        Extra3,
        Extra4,
        Extra5,
        Extra6
    }

    public enum State: uint 
    {
        Released,
        Pressed,
    }

    public enum ScrollAxis: uint
    {
        VerticalScroll = 0,
        HorizontalScroll = 1,
    }
}