namespace TypewiseAlert
{
    using System;

    public interface ITargetAlerter
    {
        AlertStatus SendAlertToTarget(BreachType breachType, AlertTarget alertTarget, Func<string, bool> printerAction);
    }
}
