using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public int LookDir { get; private set; } = 1;
    public float CurrentRotationY { get; private set; } = 0f;
    public Vector3 Direction => LookDir > 0 ? Vector3.right : Vector3.left;

    [SerializeField] private PlayerUnit player;
    [SerializeField] private Animator animator;

    private Dictionary<ePlayerMotionType, AnimationEventData> eventDataDict = new();
    private HashSet<int> firedEventIndices = new();
    private ePlayerMotionType currentMotion;
    private float prevNormalizedTime;

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
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime % 1f;

        if (normalizedTime < prevNormalizedTime)
            firedEventIndices.Clear();

        if (eventDataDict.TryGetValue(currentMotion, out var eventData))
        {
            for (int i = 0; i < eventData.events.Count; i++)
            {
                if (firedEventIndices.Contains(i)) continue;

                var evt = eventData.events[i];
                if (prevNormalizedTime < evt.normalizedTime && normalizedTime >= evt.normalizedTime)
                {
                    FireEvent(evt.eventName);
                    firedEventIndices.Add(i);
                }
            }
        }

        prevNormalizedTime = normalizedTime;
    }

    public void SetLookDir(int dir)
    {
        if (dir == 0) return;
        LookDir = dir > 0 ? 1 : -1;
        CurrentRotationY = LookDir > 0 ? 90f : 270f;
        transform.rotation = Quaternion.Euler(0f, CurrentRotationY, 0f);
    }

    public void Play(ePlayerMotionType motion, bool isLoop = false, float timeScale = 1f, float blendTime = 0.2f)
    {
        currentMotion = motion;
        prevNormalizedTime = 0f;
        firedEventIndices.Clear();
        animator.CrossFade(motion.ToString(), blendTime);
        animator.speed = timeScale;

        player.EventBus.SendEvent(new OnPlayerActionChanged(player, motion));
    }

    private void FireEvent(string eventName)
    {
        Debug.Log($"Animation Event Fired: {eventName}");
    }

    public float GetAnimationDuration(ePlayerMotionType motion, float timeScale = 1f)
    {
        string motionName = motion.ToString();
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.Contains(motionName))
                return clip.length / timeScale;
        }
        return 0f;
    }

    public void JumpEnd() { }
}
