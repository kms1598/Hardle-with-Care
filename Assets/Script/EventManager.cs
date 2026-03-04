using System;

public static class EventManager
{
    public static Action OnPowerDown;
    public static Action OnPowerRecover;
    public static Action OnBalanceLost;
    public static Action<int> OnBarrierChange;
    public static Action OnRockCollision;
    public static Action OnWarp;
    public static Action OnErrorOccur;
    public static Action OnErrorRecovery;
    public static Action OnRebooting;
    public static Action<int> OnCargoUnitBroke;

    public static void ClearAllEvent()
    {
        OnPowerDown = null;
        OnPowerRecover = null;
        OnBalanceLost = null;
        OnBarrierChange = null;
        OnRockCollision = null;
        OnWarp = null;
        OnErrorOccur = null;
        OnErrorRecovery = null;
        OnRebooting = null;
        OnCargoUnitBroke = null;
    }
}
