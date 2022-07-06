namespace TypewiseAlert
{
    using System;

    public class TypewiseAlert<T> : ITypewiseAlert<T>
    {
        private readonly IBreachChecker<T> _BreachChecker;

        private readonly ITargetAlerter _TargetAlerter;

        public TypewiseAlert(IBreachChecker<T> breachChecker, ITargetAlerter targetAlerter)
        {
            _BreachChecker = breachChecker;
            _TargetAlerter = targetAlerter;
        }

        public BreachStatus CheckBreachAndAlert(
            AlertTarget alertTarget, BatterySpecification batterySpecification, T temperature, Func<string, bool> printerFunc)
        {
            var breachType = _BreachChecker.ClassifyTemperatureBreach(
              batterySpecification.CoolingType, temperature);

            var alertStatus = _TargetAlerter.SendAlertToTarget(breachType, alertTarget, printerFunc);

            return new BreachStatus(breachType, alertStatus);
        }
    }
}
