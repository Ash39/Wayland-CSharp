using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Wayland.Sample
{
    public class Input
    {
        public WlKeyboard keyboard;
        public WlPointer mouse;

        public static Action<uint, MouseButton, State> mouseButton;
        public static Action<uint, Vector2> mousePosition;
        public static Action<uint, ScrollAxis, float> mouseScroll;
        public static Action<bool> focus;


        public void Create(WlSeat seat, WlSeat.CapabilityFlag capabilities)
        {
            if((capabilities.HasFlag(WlSeat.CapabilityFlag.Pointer)) && mouse == null)
            {
                mouse = seat.GetPointer();
                mouse.enter += (_,_,_,_,_) => focus?.Invoke(true);
                mouse.leave += (_,_,_) => focus?.Invoke(false);
                mouse.motion += (_, time, x, y) => mousePosition?.Invoke(time, new Vector2((float)x, (float)y));
                mouse.button += (_, _, time, button, state) => mouseButton?.Invoke(time, (MouseButton)button, (State)state);
                mouse.axis += (_, time, axis, scroll) => mouseScroll?.Invoke(time, (ScrollAxis)axis, (float)scroll);

            }

            if((capabilities.HasFlag(WlSeat.CapabilityFlag.Keyboard)) && keyboard == null)
            {
                keyboard = seat.GetKeyboard();
                
            }
        }

    }
}