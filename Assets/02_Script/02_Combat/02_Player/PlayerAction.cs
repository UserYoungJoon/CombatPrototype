using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public int LookDir { get; private set; } = 1;
    public float CurrentRotationY { get; private set; } = 0f;
    public Vector3 Direction => LookDir > 0 ? Vector3.right : Vector3.left;

    [SerializeField] private Animator animator;

    private Dictionary<ePlayerMotion, AnimationEventData> eventDataDict = new();
    private ePlayerMotion currentMotion;
    private float prevNormalizedTime;
    private HashSet<int> firedEventIndices = new();

    private void Awake()
    {
        if (animator != null)
            animator.applyRootMotion = false;

        LoadAllEventData();
    }

    private void LoadAllEventData()
    {
        foreach (ePlayerMotion motion in System.Enum.GetValues(typeof(ePlayerMotion)))
        {
            var data = Resources.Load<AnimationEventData>($"AnimationEvents/{motion}");
            if (data != null)
                eventDataDict[motion] = data;
        }
    }

    private void Update()
    {
        if (animator == null) return;

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

    public void Play(ePlayerMotion motion, float blendTime = 0.2f)
    {
        currentMotion = motion;
        prevNormalizedTime = 0f;
        firedEventIndices.Clear();
        animator.CrossFade(motion.ToString(), blendTime);
    }

    private void FireEvent(string eventName)
    {
        Debug.Log($"Animation Event Fired: {eventName}");
    }

    public void JumpEnd() { }
}
