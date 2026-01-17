using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private PlayerAnimation playerAnimation;
    private float moveInput;

    public ePlayerMotion currentMotion = ePlayerMotion.Attack1;

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
}
