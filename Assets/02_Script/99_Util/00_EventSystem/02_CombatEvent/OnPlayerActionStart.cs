
public readonly struct OnPlayerActionChanged : IGameEvent
{
    public readonly PlayerUnit Player;
    public readonly ePlayerMotionType MotionType;

    public OnPlayerActionChanged(PlayerUnit player, ePlayerMotionType motionType)
    {
        Player = player;
        MotionType = motionType;
    }
}