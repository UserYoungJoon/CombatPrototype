using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private PlayerAnimation playerAnimation;
    private float moveInput;
    private bool isGrounded;
    private bool wasGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerAnimation = GetComponent<PlayerAnimation>();
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

        wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);

        if (isGrounded && keyboard.spaceKey.wasPressedThisFrame)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerAnimation.Play(ePlayerMotion.Jump);
        }

        if (keyboard.hKey.wasPressedThisFrame)
            playerAnimation.Play(ePlayerMotion.Attack1);

        if (isGrounded && !wasGrounded)
        {
            if (moveInput != 0)
                playerAnimation.Play(ePlayerMotion.Run);
            else
                playerAnimation.Play(ePlayerMotion.Idle);
        }
        else if (isGrounded)
        {
            if (moveInput != 0 && prevInput == 0)
                playerAnimation.Play(ePlayerMotion.Run);
            else if (moveInput == 0 && prevInput != 0)
                playerAnimation.Play(ePlayerMotion.Idle);
        }
    }

    private void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveInput * moveSpeed;
        rb.linearVelocity = velocity;
    }
}
