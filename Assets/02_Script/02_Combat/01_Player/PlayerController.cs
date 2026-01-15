using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private string[] animationTriggers = { "Attack1", "Attack2", "Attack3", "Block", "DodgeAttack" };

    private Animator animator;
    private float moveInput;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // New Input System 사용
        moveInput = 0f;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            moveInput = -1f;
        else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            moveInput = 1f;

        // H키 누르면 랜덤 애니메이션 재생
        if (keyboard.hKey.wasPressedThisFrame)
        {
            PlayRandomAnimation();
        }
    }

    private void PlayRandomAnimation()
    {
        if (animator == null) return;

        int randomIndex = Random.Range(0, animationTriggers.Length);
        animator.SetTrigger(animationTriggers[randomIndex]);
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
