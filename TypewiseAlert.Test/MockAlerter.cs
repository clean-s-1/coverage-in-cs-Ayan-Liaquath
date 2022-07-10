namespace TypewiseAlert.Test
{
    using System;

    public class MockAlerter : IAlerter
    {
        public bool SendAlert(BreachType breachType, Func<string, bool> printerAction)
        {
            return printerAction.Invoke(breachType.ToString());
        }
    }
}
