using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerUnit player;
    [SerializeField] private PlayerAction playerAnimation;
    [SerializeField] private float moveSpeed = 15f;

    private Dictionary<Key, PlayerCommand> commandDict = new();
    private HashSet<ActionBase> playingActions = new();
    private Key pendingKey;
    private Keyboard keyboard;

    private void Awake()
    {
        foreach (Key key in System.Enum.GetValues(typeof(Key)))
            commandDict[key] = PlayerCommand.None;

        keyboard = Keyboard.current;
    }

    private void OnKeyDown(OnKeyDown evt)
    {
        Key key = evt.Key;
        if (commandDict.ContainsKey(key) is false) return;

        PlayerCommand command = commandDict[key];
        if (command.Type == ePlayerCommand.None) return;

        if (player.State is ePlayerState.Attack) return;
    }

    private void OnKeyHolding(OnKeyHolding evt)
    {
        pendingKey = evt.Key;
    }

    private void OnKeyUp(OnKeyUp evt)
    {
        Key key = evt.Key;
        if (commandDict.ContainsKey(key) is false) return;

        PlayerCommand command = commandDict[key];
        if (command.Type == ePlayerCommand.None) return;
    }

    private void OnDestroy()
    {
        commandDict.Clear();
    }
}
