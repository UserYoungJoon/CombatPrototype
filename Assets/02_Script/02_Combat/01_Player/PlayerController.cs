using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private PlayerAction playerAnimation;
    private float moveInput;

    public ePlayerMotion currentMotion = ePlayerMotion.Attack1;

    private Dictionary<KeyControl, PlayerCommand> commandDict = new();

    private KeyControl pendingKey;
    private Keyboard keyboard;

    private void Awake()
    {
        foreach (KeyControl key in System.Enum.GetValues(typeof(KeyControl)))
            commandDict[key] = PlayerCommand.None;

        keyboard = Keyboard.current;
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float prevInput = moveInput;
        moveInput = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            moveInput = -1f;
        else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            moveInput = 1f;

        if (moveInput != 0)
            playerAnimation.SetLookDir((int)moveInput);

        if (keyboard.hKey.wasPressedThisFrame)
            playerAnimation.Play(currentMotion);

        if (moveInput != 0 && prevInput == 0)
            playerAnimation.Play(ePlayerMotion.Run);
        else if (moveInput == 0 && prevInput != 0)
            playerAnimation.Play(ePlayerMotion.Idle);

        transform.position += new Vector3(moveInput * moveSpeed * Time.deltaTime, 0, 0);
    }

    private void OnKeyPressed()
    {
        
    }

    private void OnDestroy()
    {
        commandDict.Clear();
    }
}
