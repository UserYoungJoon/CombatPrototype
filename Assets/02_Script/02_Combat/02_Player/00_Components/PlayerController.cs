using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerUnit player;
    [SerializeField] private PlayerAction playerAction;

    private Dictionary<Key, PlayerCommand> commandDict = new();
    private Dictionary<eActionCode, ActionBase> actionDict = new();
    private ActionBase playingAction;
    private Key pendingKey;
    public Key pendingKeyPress => pendingKey;

    public void Init()
    {
        foreach (Key key in System.Enum.GetValues(typeof(Key)))
            commandDict[key] = PlayerCommand.None;

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
        pendingKey = key;
        if (TryGetAction(key, out var action) is false) return;

        if (player.StateComponent.State is ePlayerState.Skill) return;
        if (action.IsRunning) return;
        if (action.CanStart() is false) return;
        if (playingAction is not null)
        {
            playingAction.End();
        }
        action.Start();
        playingAction = action;
    }

    private void OnKeyHolding(OnKeyHolding evt)
    {
        Key key = evt.Key;
        if (pendingKey == Key.None) pendingKey = key;
        if (TryGetAction(key, out var action) is false) return;
        if (action.IsRunning is false) return;
        action.KeyHolding();
    }

    private void OnKeyUp(OnKeyUp evt)
    {
        Key key = evt.Key;
        pendingKey = GameManager.Instance.InputManager.GetLatestHeldKey();

        if (TryGetAction(key, out var action) is false) return;
        action.KeyUp();
    }

    public void EndCurrentSkill()
    {
        if (playingAction != null)
        {
            playingAction.End();
            playingAction = null;
        }

        if (pendingKey != Key.None)
        {
            OnKeyDown(new OnKeyDown(pendingKey));
        }
    }

    private bool TryGetAction(Key key, out ActionBase action)
    {
        action = null;

        if (commandDict.ContainsKey(key) is false) return false;

        PlayerCommand command = commandDict[key];
        if (command.Type == ePlayerCommand.None) return false;

        action = actionDict[(eActionCode)command.Code];
        if (action == null) return false;

        return true;
    }

    private void OnDestroy()
    {
        commandDict.Clear();
    }
}
