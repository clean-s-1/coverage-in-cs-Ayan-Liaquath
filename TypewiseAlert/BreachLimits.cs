namespace TypewiseAlert
{
    public class BreachLimits<T> : IBreachLimits<T>
    {
        private readonly CoolingType _CoolingType;

        private readonly T _LowerLimit;

        private readonly T _UpperLimit;

        public BreachLimits(CoolingType coolingType, T lowerLimit, T upperLimit)
        {
            _CoolingType = coolingType;
            _LowerLimit = lowerLimit;
            _UpperLimit = upperLimit;
        }

        public CoolingType FetchCoolingType()
        {
            return _CoolingType;
        }

        public T FetchLowerLimit()
        {
            return _LowerLimit;
        }

        public T FetchUpperLimit()
        {
            return _UpperLimit;
        }
    }
}
