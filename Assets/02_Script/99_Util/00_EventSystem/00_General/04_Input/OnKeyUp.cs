
using UnityEngine.InputSystem;

public readonly struct OnKeyUp : IGameEvent
{
    public readonly Key Key;

    public OnKeyUp(Key key)
    {
        Key = key;
    }
}