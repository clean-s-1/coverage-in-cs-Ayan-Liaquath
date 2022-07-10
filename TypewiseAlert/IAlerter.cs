namespace TypewiseAlert
{
    using System;

    public interface IAlerter
    {
        bool SendAlert(BreachType breachType, Func<string, bool> printerAction);
    }
}
