namespace TypewiseAlert.Test
{
    using Xunit;

    public class BreachLimitsTest
    {
        [Fact]
        public void TestWithIntValues()
        {
            var intLimits = new BreachLimits<int>(CoolingType.PASSIVE_COOLING, 0, 45);

            Assert.Equal(CoolingType.PASSIVE_COOLING, intLimits.FetchCoolingType());

            Assert.Equal(0, intLimits.FetchLowerLimit());
            
            Assert.Equal(45, intLimits.FetchUpperLimit());
        }

        [Fact]
        public void TestWithDoubleValues()
        {
            var doubleLimits = new BreachLimits<double>(CoolingType.HI_ACTIVE_COOLING, 1.5, 4.5);

            Assert.Equal(CoolingType.HI_ACTIVE_COOLING, doubleLimits.FetchCoolingType());

            Assert.Equal(1.5, doubleLimits.FetchLowerLimit());

            Assert.Equal(4.5, doubleLimits.FetchUpperLimit());
        }
    }
}
