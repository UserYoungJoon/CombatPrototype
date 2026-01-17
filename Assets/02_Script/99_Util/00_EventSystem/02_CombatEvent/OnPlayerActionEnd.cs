

public readonly struct OnPlayerActionEnd : IGameEvent
{
    public readonly PlayerUnit Player;
    public readonly ePlayerMotionType MotionType;

    public OnPlayerActionEnd(PlayerUnit player, ePlayerMotionType motionType)
    {
        Player = player;
        MotionType = motionType;
    }
}