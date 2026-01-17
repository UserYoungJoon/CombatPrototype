
using UnityEngine;

public abstract class SkillBase : ActionBase
{
    protected float cooldownTime;
    protected float lastUsedTime;

    protected sealed override void OnStart()
    {
        lastUsedTime = Time.time;
        OnStartSkill();
    }

    protected virtual void OnStartSkill()
    {
        
    }

    public override bool CanStart()
    {
        return Time.time >= lastUsedTime + cooldownTime;
    }
}