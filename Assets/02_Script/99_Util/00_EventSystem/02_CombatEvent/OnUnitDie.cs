
public readonly struct OnUnitDie : IGameEvent
{
    public readonly Unit KillerUnit;
    public readonly Unit DeadUnit;

    public OnUnitDie(Unit killerUnit, Unit deadUnit)
    {
        KillerUnit = killerUnit;
        DeadUnit = deadUnit;
    }
}