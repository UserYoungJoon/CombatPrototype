using UnityEngine.InputSystem;

public readonly struct PlayerCommand
{
    public readonly ePlayerCommand Type;
    public readonly int Code;
    public PlayerCommand(ePlayerCommand type, int code = 0)
    {
        Type = type;
        Code = code;
    }
}

public enum ePlayerCommand
{
    None,
    /// <summary> 이동, 점프 등 기본 조작 </summary>
    Base,
    /// <summary> 스킬 사용 </summary>
    Skill,
}