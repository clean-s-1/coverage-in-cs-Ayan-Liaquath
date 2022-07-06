namespace TypewiseAlert
{
    public class BreachStatus
    {
        public BreachStatus(BreachType breachType, AlertStatus alertStatus)
        {
            BreachType = breachType;
            AlertStatus = alertStatus;
        }

        public BreachType BreachType { get; }

        public AlertStatus AlertStatus { get; }
    }
}
