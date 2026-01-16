using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public int LookDir { get; private set; } = 1;
    public float CurrentRotationY { get; private set; } = 0f;
    public Vector3 Direction => LookDir > 0 ? Vector3.right : Vector3.left;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetLookDir(int dir)
    {
        if (dir == 0) return;
        LookDir = dir > 0 ? 1 : -1;
        CurrentRotationY = LookDir > 0 ? 90f : 270f;
        transform.rotation = Quaternion.Euler(0f, CurrentRotationY, 0f);
    }

    public void Play(ePlayerMotion motion)
    {
        animator.Play(motion.ToString());
    }
}
