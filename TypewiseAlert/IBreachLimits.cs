namespace TypewiseAlert
{
    public interface IBreachLimits<T>
    {
        CoolingType FetchCoolingType();

        T FetchLowerLimit();

        T FetchUpperLimit();
    }
}
