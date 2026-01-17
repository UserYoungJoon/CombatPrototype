using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InputManager : ManagerBase
{
    private List<Key> pressedKeys = new();

    public Key GetLatestHeldKey()
    {
        return pressedKeys.Count > 0 ? pressedKeys[^1] : Key.None;
    }

    private static readonly Key[] TrackedKeys = new Key[]
    {
        Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J, Key.K, Key.L, Key.M,
        Key.N, Key.O, Key.P, Key.Q, Key.R, Key.S, Key.T, Key.U, Key.V, Key.W, Key.X, Key.Y, Key.Z,
        Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5,
        Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9, Key.Digit0,
        Key.Numpad1, Key.Numpad2, Key.Numpad3, Key.Numpad4, Key.Numpad5,
        Key.Numpad6, Key.Numpad7, Key.Numpad8, Key.Numpad9, Key.Numpad0,
        Key.NumpadPlus, Key.NumpadMinus, Key.NumpadMultiply, Key.NumpadDivide,
        Key.NumpadEnter, Key.NumpadPeriod, Key.NumLock,
        Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6,
        Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12,
        Key.Space, Key.Enter, Key.Tab, Key.Escape, Key.Backspace, Key.Delete,
        Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt,
        Key.LeftArrow, Key.RightArrow, Key.UpArrow, Key.DownArrow,
        Key.Home, Key.End, Key.PageUp, Key.PageDown, Key.Insert,
        Key.CapsLock, Key.ScrollLock, Key.PrintScreen, Key.Pause,
        Key.Backquote, Key.Minus, Key.Equals, Key.LeftBracket, Key.RightBracket,
        Key.Backslash, Key.Semicolon, Key.Quote, Key.Comma, Key.Period, Key.Slash,
    };

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        foreach (var key in TrackedKeys)
        {
            var keyControl = keyboard[key];

            if (keyControl.wasPressedThisFrame)
            {
                pressedKeys.Add(key);
                EventBus.SendEvent(new OnKeyDown(key));
            }

            if (keyControl.wasReleasedThisFrame)
            {
                pressedKeys.Remove(key);
                EventBus.SendEvent(new OnKeyUp(key));
            }

            if (keyControl.isPressed)
                EventBus.SendEvent(new OnKeyHolding(key));
        }
    }
}