namespace TypewiseAlert.Test
{
    using Xunit;

    public class ControllerAlerterTest
    {
        private IAlerter _ControllerAlerter;

        private int _ControllerPrinterFuncCallCount;

        private string _ControllerPrinterFuncCallInput;

        public ControllerAlerterTest()
        {
            _ControllerAlerter = new ControllerAlerter(123);

            _ControllerPrinterFuncCallCount = 0;

            _ControllerPrinterFuncCallInput = null;
        }

        private bool ControllerPrinterFunction(string input)
        {
            _ControllerPrinterFuncCallInput = input;

            _ControllerPrinterFuncCallCount++;

            if (input.Contains(BreachType.TOO_HIGH.ToString()))
            {
                return false;
            }

            return true;
        }

        [Fact]
        public void TestControllerAlerterSuccess()
        {
            var controllerAlertStatus = _ControllerAlerter.SendAlert(BreachType.NORMAL, ControllerPrinterFunction);

            Assert.True(controllerAlertStatus);

            Assert.Equal(1, _ControllerPrinterFuncCallCount);

            Assert.Equal("123 : NORMAL\n", _ControllerPrinterFuncCallInput);
        }

        [Fact]
        public void TestControllerAlerterFailure()
        {
            var controllerAlertStatus = _ControllerAlerter.SendAlert(BreachType.TOO_HIGH, ControllerPrinterFunction);

            Assert.False(controllerAlertStatus);

            Assert.Equal(1, _ControllerPrinterFuncCallCount);

            Assert.Equal("123 : TOO_HIGH\n", _ControllerPrinterFuncCallInput);
        }

        [Fact]
        public void TestEmailAlerterWhenPrinterIsNull()
        {
            var alertStatus = _ControllerAlerter.SendAlert(BreachType.TOO_LOW, null);

            Assert.False(alertStatus);

            Assert.Equal(0, _ControllerPrinterFuncCallCount);

            Assert.Null(_ControllerPrinterFuncCallInput);
        }
    }
}
