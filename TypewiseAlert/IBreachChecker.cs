namespace TypewiseAlert
{
    public interface IBreachChecker<T>
    {
        BreachType ClassifyTemperatureBreach(CoolingType coolingType, T temperature);
    }
}
