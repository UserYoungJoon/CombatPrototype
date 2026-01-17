
using UnityEngine;

public abstract class SkillBase : ActionBase
{
    protected float cooldownTime;
    protected float lastUsedTime;

    public override bool CanStart()
    {
        return Time.time >= lastUsedTime + cooldownTime;
    }
}