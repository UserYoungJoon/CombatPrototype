using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public int LookDir { get; private set; } = 1;
    public float CurrentRotationY { get; private set; } = 0f;
    public Vector3 Direction => LookDir > 0 ? Vector3.right : Vector3.left;
    public ePlayerMotionType CurrentMotion => currentMotion;
    public AnimatorStateInfo CurrentStateInfo => animator.GetCurrentAnimatorStateInfo(0);

    [SerializeField] private PlayerUnit player;
    [SerializeField] private Animator animator;

    private Dictionary<ePlayerMotionType, AnimationEventData> eventDataDict = new();
    private HashSet<int> firedEventIndices = new();
    private ePlayerMotionType currentMotion;
    private float prevNormalizedTime;
    private bool currentIsLoop;
    private bool isPlaying;
    private float playbackSpeed;

    public void Init()
    {
        player = GetComponent<PlayerUnit>();

        animator.applyRootMotion = false;

        foreach (ePlayerMotionType motion in System.Enum.GetValues(typeof(ePlayerMotionType)))
        {
            var data = Resources.Load<AnimationEventData>($"AnimationEvents/{motion}");
            if (data != null)
                eventDataDict[motion] = data;
        }
    }

    private void Update()
    {
        if (!isPlaying) return;
        if (GameManager.Instance.CinematicManager.IsStopMotion) return;

        // 수동 업데이트
        float clipLength = GetClipLength(currentMotion);
        float delta = Time.deltaTime * playbackSpeed;

        if (!currentIsLoop)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = stateInfo.normalizedTime;
            float remainingTime = (1f - normalizedTime) * clipLength;
            delta = Mathf.Min(delta, remainingTime);
        }

        if (delta > 0f)
        {
            animator.speed = 1f;
            animator.Update(delta);
            animator.speed = 0f;
        }

        // 이벤트 처리 및 종료 체크
        var currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float currentNormTime = currentStateInfo.normalizedTime;

        if (!currentIsLoop && currentStateInfo.IsName(currentMotion.ToString()) && currentNormTime >= 1f)
        {
            isPlaying = false;
            player.EventBus.SendEvent(new OnPlayerActionEnd(player, currentMotion));
            player.ControllerComponent.EndCurrentSkill();
            return;
        }

        currentNormTime %= 1f;

        if (currentNormTime < prevNormalizedTime)
            firedEventIndices.Clear();

        if (eventDataDict.TryGetValue(currentMotion, out var eventData))
        {
            for (int i = 0; i < eventData.events.Count; i++)
            {
                if (firedEventIndices.Contains(i)) continue;

                var evt = eventData.events[i];
                if (prevNormalizedTime < evt.normalizedTime && currentNormTime >= evt.normalizedTime)
                {
                    FireEvent(evt.eventName);
                    firedEventIndices.Add(i);
                }
            }
        }

        prevNormalizedTime = currentNormTime;
    }

    public void SetLookDir(int dir)
    {
        if (dir == 0) return;
        LookDir = dir > 0 ? 1 : -1;
        CurrentRotationY = LookDir > 0 ? 90f : 270f;
        transform.rotation = Quaternion.Euler(0f, CurrentRotationY, 0f);
    }

    public void Play(ePlayerMotionType motion, bool isLoop = false, float timeScale = 1f, float blendTime = 0f)
    {
        currentMotion = motion;
        currentIsLoop = isLoop;
        prevNormalizedTime = 0f;
        isPlaying = true;
        playbackSpeed = timeScale;
        firedEventIndices.Clear();
        animator.speed = 0f;
        animator.Play(motion.ToString());
        animator.Update(0f);

        player.EventBus.SendEvent(new OnPlayerActionChanged(player, motion));
    }

    private void FireEvent(string eventName)
    {
        player.EventBus.SendEvent(new OnAnimatorEvent(eventName));
    }

    public float GetAnimationDuration(ePlayerMotionType motion, float timeScale = 1f)
    {
        return GetClipLength(motion) / timeScale;
    }

    private float GetClipLength(ePlayerMotionType motion)
    {
        string motionName = motion.ToString();
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.Contains(motionName))
                return clip.length;
        }
        return 0f;
    }

    public void JumpEnd() { }
}
