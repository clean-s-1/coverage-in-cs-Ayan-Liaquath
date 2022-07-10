namespace TypewiseAlert.Test
{
    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    public class EmailAlerterTest
    {
        private IDictionary<BreachType, string> _EmailMessages;

        private IAlerter _EmailAlerter;

        private int _EmailPrinterFuncCallCount;

        private string _EmailPrinterFuncCallInput;

        public EmailAlerterTest()
        {
            _EmailMessages = new Dictionary<BreachType, string>
            {
                { BreachType.TOO_LOW, "Hi, the temperature is too low" },
                { BreachType.TOO_HIGH, "Hi, the temperature is too high" }
            };

            _EmailAlerter = new EmailAlerter("example@example.com", _EmailMessages);

            _EmailPrinterFuncCallCount = 0;

            _EmailPrinterFuncCallInput = null;
        }

        private bool EmailPrinterFunction(string input)
        {
            _EmailPrinterFuncCallInput = input;

            _EmailPrinterFuncCallCount++;

            if (input.Equals("To : random\n"))
            {
                return false;
            }

            return true;
        }

        [Fact]
        public void TestEmailAlerterSuccess()
        {
            var emailAlertStatus = _EmailAlerter.SendAlert(BreachType.TOO_HIGH, EmailPrinterFunction);

            Assert.True(emailAlertStatus);

            Assert.Equal(1, _EmailPrinterFuncCallCount);

            Assert.Equal("To : example@example.com\nHi, the temperature is too high\n", _EmailPrinterFuncCallInput);
        }

        [Fact]
        public void TestEmailAlerterNormalBreach()
        {
            var emailAlertStatus = _EmailAlerter.SendAlert(BreachType.NORMAL, EmailPrinterFunction);

            Assert.True(emailAlertStatus);

            Assert.Equal(0, _EmailPrinterFuncCallCount);

            Assert.Null(_EmailPrinterFuncCallInput);
        }

        [Fact]
        public void TestEmailAlerterFailure()
        {
            _EmailMessages.Remove(_EmailMessages.First());

            _EmailAlerter = new EmailAlerter("random", _EmailMessages);

            var emailAlertStatus = _EmailAlerter.SendAlert(BreachType.TOO_LOW, EmailPrinterFunction);

            Assert.False(emailAlertStatus);

            Assert.Equal(1, _EmailPrinterFuncCallCount);

            Assert.Equal("To : random\n", _EmailPrinterFuncCallInput);
        }

        [Fact]
        public void TestEmailAlerterWhenPrinterIsNull()
        {
            var alertStatus = _EmailAlerter.SendAlert(BreachType.TOO_LOW, null);

            Assert.False(alertStatus);

            Assert.Equal(0, _EmailPrinterFuncCallCount);

            Assert.Null(_EmailPrinterFuncCallInput);
        }
    }
}
