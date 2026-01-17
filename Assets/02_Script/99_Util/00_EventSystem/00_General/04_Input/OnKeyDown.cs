
using UnityEngine.InputSystem;

public readonly struct OnKeyDown : IGameEvent
{
    public readonly Key Key;

    public OnKeyDown(Key key)
    {
        Key = key;
    }
}