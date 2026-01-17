using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerUnit player;
    [SerializeField] private PlayerAction playerAction;
    [SerializeField] private float moveSpeed = 15f;

    private Dictionary<Key, PlayerCommand> commandDict = new();
    private Dictionary<eActionCode, ActionBase> actionDict = new();
    private HashSet<ActionBase> playingActions = new();
    private Key pendingKey;
    private Keyboard keyboard;

    public void Init()
    {
        foreach (Key key in System.Enum.GetValues(typeof(Key)))
            commandDict[key] = PlayerCommand.None;

        keyboard = Keyboard.current;

        commandDict[Key.Q] = new PlayerCommand(ePlayerCommand.Skill, (int)eActionCode.SwordAndShield_BaseAttack);
        commandDict[Key.A] = new PlayerCommand(ePlayerCommand.Base, (int)eActionCode.MoveLeft);
        commandDict[Key.D] = new PlayerCommand(ePlayerCommand.Base, (int)eActionCode.MoveRight);

        actionDict[eActionCode.MoveLeft] = ActionHelper.GetAction(eActionCode.MoveLeft);
        actionDict[eActionCode.MoveRight] = ActionHelper.GetAction(eActionCode.MoveRight);
        actionDict[eActionCode.SwordAndShield_BaseAttack] = ActionHelper.GetAction(eActionCode.SwordAndShield_BaseAttack);
        // TO DO
        // - 임시 로직, 생성자에 Player 주입하는 형식이 올바름
        actionDict[eActionCode.MoveLeft].Init(player);
        actionDict[eActionCode.MoveRight].Init(player);
        actionDict[eActionCode.SwordAndShield_BaseAttack].Init(player); 

        var eventBus = GameManager.Instance.InputManager.EventBus;
        eventBus.ConnectEvent<OnKeyDown>(OnKeyDown);
        eventBus.ConnectEvent<OnKeyHolding>(OnKeyHolding);
        eventBus.ConnectEvent<OnKeyUp>(OnKeyUp);
    }

    private void OnKeyDown(OnKeyDown evt)
    {
        Key key = evt.Key;
        if (commandDict.ContainsKey(key) is false) return;

        PlayerCommand command = commandDict[key];
        if (command.Type == ePlayerCommand.None) return;

        var action = actionDict[(eActionCode)command.Code];
        if (action == null) return;

        if (player.StateComponent.State is ePlayerState.Attack) return;

        action.Start();
        playingActions.Add(action);
    }

    private void OnKeyHolding(OnKeyHolding evt)
    {
        pendingKey = evt.Key;
        if (commandDict.ContainsKey(pendingKey) is false) return;
        PlayerCommand command = commandDict[pendingKey];

        if (command.Type == ePlayerCommand.None) return;
        var action = actionDict[(eActionCode)command.Code];

        if (action == null) return;
        action.Update();
    }

    private void OnKeyUp(OnKeyUp evt)
    {
        Key key = evt.Key;
        if (commandDict.ContainsKey(key) is false) return;

        PlayerCommand command = commandDict[key];
        if (command.Type == ePlayerCommand.None) return;

        var action = actionDict[(eActionCode)command.Code];
        if (action == null) return;
        action.End();
        playingActions.Remove(action);
    }

    private void OnDestroy()
    {
        commandDict.Clear();
    }
}
