using UnityEngine;

public class MonsterAction : MonoBehaviour
{
    [SerializeField] private MonsterUnit monster;
    [SerializeField] private Animator animator;

    private eMonsterMotionType currentMotion;
    private bool isPlaying;
    private float playbackSpeed;

    public eMonsterMotionType CurrentMotion => currentMotion;
    public AnimatorStateInfo CurrentStateInfo => animator.GetCurrentAnimatorStateInfo(0);

    public void Init()
    {
        animator.applyRootMotion = false;
        animator.updateMode = AnimatorUpdateMode.Normal;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        animator.speed = 0f;
    }

    private void Update()
    {
        if (!isPlaying) return;

        float clipLength = GetClipLength(currentMotion);
        float delta = Time.deltaTime * playbackSpeed;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime;
        float remainingTime = (1f - normalizedTime) * clipLength;
        delta = Mathf.Min(delta, remainingTime);

        if (delta > 0f)
        {
            animator.speed = 1f;
            animator.Update(delta);
            animator.speed = 0f;
        }

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(currentMotion.ToString()) && stateInfo.normalizedTime >= 1f)
        {
            // 마지막 프레임에 고정 (1.0은 첫 프레임으로 wrap될 수 있어서 0.99 사용)
            Debug.Log($"Animation End: {currentMotion}, setting to 0.99f");
            animator.speed = 1f;
            animator.Play(currentMotion.ToString(), 0, 0.99f);
            animator.Update(0f);
            animator.speed = 0f;
            isPlaying = false;
            OnMotionEnd();
        }
    }

    public void Play(eMonsterMotionType motion, float timeScale = 1f)
    {
        currentMotion = motion;
        playbackSpeed = timeScale;
        isPlaying = true;
        animator.speed = 0f;
        animator.Play(motion.ToString());
        animator.Update(0f);
    }

    public void PlayIdle()
    {
        currentMotion = eMonsterMotionType.Idle;
        playbackSpeed = 1f;
        isPlaying = false;
        animator.speed = 1f;
        animator.Play(eMonsterMotionType.Idle.ToString());
    }

    private void OnMotionEnd()
    {
        // Die는 마지막 프레임에서 멈춤
        if (currentMotion == eMonsterMotionType.Die)
        {
            return;
        }

        // Hit 끝나면 Idle로
        if (currentMotion == eMonsterMotionType.Damage1 || currentMotion == eMonsterMotionType.Damage2)
        {
            monster.StateComponent.ChangeState(eMonsterState.Idle);
            PlayIdle();
        }
    }

    private float GetClipLength(eMonsterMotionType motion)
    {
        string motionName = motion.ToString();
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.Contains(motionName))
                return clip.length;
        }
        return 1f;
    }
}
