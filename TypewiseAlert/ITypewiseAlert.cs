namespace TypewiseAlert
{
    using System;

    public interface ITypewiseAlert<T>
    {
        BreachStatus CheckBreachAndAlert(
            AlertTarget alertTarget,
            BatterySpecification batterySpecification,
            T temperature,
            Func<string, bool> printerFunc);
    }
}
