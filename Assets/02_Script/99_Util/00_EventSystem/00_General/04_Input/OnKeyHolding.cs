
using UnityEngine.InputSystem;

public readonly struct OnKeyHolding : IGameEvent
{
    public readonly Key Key;

    public OnKeyHolding(Key key)
    {
        Key = key;
    }
}