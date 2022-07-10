namespace TypewiseAlert.Test
{
    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    public class BreachCheckerTest
    {
        private IList<IBreachLimits<double>> _BreachLimits;

        private IBreachChecker<double> _BreachChecker;

        public BreachCheckerTest()
        {
            _BreachLimits = new List<IBreachLimits<double>>
                                   {
                                       new BreachLimits<double>(CoolingType.PASSIVE_COOLING, 0, 35),
                                       new BreachLimits<double>(CoolingType.MED_ACTIVE_COOLING, 0, 40),
                                       new BreachLimits<double>(CoolingType.HI_ACTIVE_COOLING, 0, 45)
                                   };

            _BreachChecker = new BreachChecker<double>(_BreachLimits);
        }

        private void PerformBreachCheck(CoolingType coolingType, double temperature, BreachType breachType)
        {
            var actualBreachType = _BreachChecker.ClassifyTemperatureBreach(coolingType, temperature);

            Assert.Equal(breachType, actualBreachType);
        }

        [Fact]
        public void CheckPassiveCoolingNormalBreach()
        {
            PerformBreachCheck(CoolingType.PASSIVE_COOLING, 25, BreachType.NORMAL);
        }

        [Fact]
        public void CheckPassiveCoolingHighBreach()
        {
            PerformBreachCheck(CoolingType.PASSIVE_COOLING, 40, BreachType.TOO_HIGH);
        }

        [Fact]
        public void CheckPassiveCoolingLowBreach()
        {
            PerformBreachCheck(CoolingType.PASSIVE_COOLING, -10, BreachType.TOO_LOW);
        }

        [Fact]
        public void CheckMedCoolingNormalBreach()
        {
            PerformBreachCheck(CoolingType.MED_ACTIVE_COOLING, 35, BreachType.NORMAL);
        }

        [Fact]
        public void CheckMedCoolingHighBreach()
        {
            PerformBreachCheck(CoolingType.MED_ACTIVE_COOLING, 43, BreachType.TOO_HIGH);
        }

        [Fact]
        public void CheckMedCoolingLowBreach()
        {
            PerformBreachCheck(CoolingType.MED_ACTIVE_COOLING, -1, BreachType.TOO_LOW);
        }

        [Fact]
        public void CheckHighCoolingNormalBreach()
        {
            PerformBreachCheck(CoolingType.HI_ACTIVE_COOLING, 20, BreachType.NORMAL);
        }

        [Fact]
        public void CheckHighCoolingHighBreach()
        {
            PerformBreachCheck(CoolingType.HI_ACTIVE_COOLING, 46, BreachType.TOO_HIGH);
        }

        [Fact]
        public void CheckHighCoolingLowBreach()
        {
            PerformBreachCheck(CoolingType.HI_ACTIVE_COOLING, -15, BreachType.TOO_LOW);
        }

        [Fact]
        public void CheckBreachTypeWhenCoolingTypeDoesNotExist()
        {
            _BreachLimits.Remove(_BreachLimits.First());

            _BreachChecker = new BreachChecker<double>(_BreachLimits);

            PerformBreachCheck(CoolingType.PASSIVE_COOLING, 1, BreachType.TOO_HIGH);

            PerformBreachCheck(CoolingType.PASSIVE_COOLING, 0, BreachType.NORMAL);

            PerformBreachCheck(CoolingType.PASSIVE_COOLING, -1, BreachType.TOO_LOW);
        }

        [Fact]
        public void CheckBreachTypeWhenBreachLimitsIsNull()
        {
            _BreachChecker = new BreachChecker<double>(null);

            PerformBreachCheck(CoolingType.HI_ACTIVE_COOLING, 3, BreachType.TOO_HIGH);
        }
    }
}
