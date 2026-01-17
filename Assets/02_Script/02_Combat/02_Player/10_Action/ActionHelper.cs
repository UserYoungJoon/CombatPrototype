
public static class ActionHelper
{
    public static ActionBase GetAction(eActionCode actionCode)
    {
        switch (actionCode)
        {
            case eActionCode.MoveLeft: return new MoveLeftAction();
            case eActionCode.MoveRight: return new MoveRightAction();
            case eActionCode.SwordAndShield_BaseAttack: return new Skill.SwordAndShield.SwordAndShieldBaseAttack();
            default: return null;
        }
    }
}