using UnityEngine;
using UnityEngine.InputSystem;

public class MoveAction : ActionBase
{
    private float moveSpeed = 15f;
    private float moveInput;
    private bool wasMoving;

    protected override void OnUpdate()
    {
        // var keyboard = Keyboard.current;
        // if (keyboard == null) return;

        // moveInput = 0f;
        // if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        //     moveInput = -1f;
        // else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        //     moveInput = 1f;

        // if (moveInput != 0)
        //     player.ActionComponent.SetLookDir((int)moveInput);

        // bool isMoving = moveInput != 0;
        // if (isMoving && !wasMoving)
        //     player.ActionComponent.Play(ePlayerMotion.Run);
        // else if (!isMoving && wasMoving)
        //     player.ActionComponent.Play(ePlayerMotion.Idle);

        // wasMoving = isMoving;

        // player.transform.position += new Vector3(moveInput * moveSpeed * Time.deltaTime, 0, 0);
    }
}
