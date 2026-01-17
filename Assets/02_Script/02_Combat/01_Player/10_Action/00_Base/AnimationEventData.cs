using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/AnimationEventData")]
public class AnimationEventData : ScriptableObject
{
    public ePlayerMotion motion;
    public List<AnimEventInfo> events = new List<AnimEventInfo>();
}

[System.Serializable]
public class AnimEventInfo
{
    public string eventName;
    public float normalizedTime;
}
