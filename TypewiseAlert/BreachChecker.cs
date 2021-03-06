namespace TypewiseAlert
{
    using System;
    using System.Collections.Generic;

    public class BreachChecker<T> : IBreachChecker<T>
    {
        private readonly List<IBreachLimits<T>> _BreachLimits;

        public BreachChecker(IList<IBreachLimits<T>> breachLimits)
        {
            _BreachLimits = new List<IBreachLimits<T>>();

            if (breachLimits != null)
            {
                _BreachLimits.AddRange(breachLimits);
            }
        }

        public BreachType ClassifyTemperatureBreach(
            CoolingType coolingType,
            T temperature)
        {
            var lowerLimit = (T)Convert.ChangeType(0, typeof(T));
            var upperLimit = (T)Convert.ChangeType(0, typeof(T));

            var breachLimits = FetchBreachLimits(coolingType);

            if (breachLimits != null)
            {
                lowerLimit = breachLimits.FetchLowerLimit();
                upperLimit = breachLimits.FetchUpperLimit();
            }
            
            return FetchBreachType(temperature, lowerLimit, upperLimit);
        }

        private BreachType FetchBreachType(T value, T lowerLimit, T upperLimit)
        {
            if (Comparer<T>.Default.Compare(value, lowerLimit) < 0)
            {
                return BreachType.TOO_LOW;
            }

            if (Comparer<T>.Default.Compare(value, upperLimit) > 0)
            {
                return BreachType.TOO_HIGH;
            }

            return BreachType.NORMAL;
        }

        private IBreachLimits<T> FetchBreachLimits(CoolingType coolingType)
        {
            var breachLimits = _BreachLimits.Find(limits => limits.FetchCoolingType().Equals(coolingType));

            return breachLimits;
        }
    }
}
