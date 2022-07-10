namespace TypewiseAlert.Test
{
    using System.Collections.Generic;

    using Xunit;

    public class TargetAlerterTest
    {
        private IDictionary<AlertTarget, IAlerter> _Alerters;

        private ITargetAlerter _TargetAlerter;

        public TargetAlerterTest()
        {
            _Alerters = new Dictionary<AlertTarget, IAlerter> { { AlertTarget.TO_CONTROLLER, new MockAlerter() } };

            _TargetAlerter = new TargetAlerter(_Alerters);
        }

        public bool AlerterPrinter(string input)
        {
            return !input.Equals(BreachType.NORMAL.ToString());
        }

        [Fact]
        public void TestAlertSuccess()
        {
            var alertStatus = _TargetAlerter.SendAlertToTarget(
                BreachType.TOO_LOW,
                AlertTarget.TO_CONTROLLER,
                AlerterPrinter);

            Assert.Equal(AlertStatus.Success, alertStatus);
        }

        [Fact]
        public void TestAlertFailure()
        {
            var alertStatus = _TargetAlerter.SendAlertToTarget(
                BreachType.NORMAL,
                AlertTarget.TO_CONTROLLER,
                AlerterPrinter);

            Assert.Equal(AlertStatus.Failed, alertStatus);
        }

        [Fact]
        public void TestAlertWhenPrinterIsNull()
        {
            var alertStatus = _TargetAlerter.SendAlertToTarget(
                BreachType.NORMAL,
                AlertTarget.TO_CONTROLLER,
                null);

            Assert.Equal(AlertStatus.Printer_Action_Is_Invalid, alertStatus);
        }

        [Fact]
        public void TestAlertWhenAlertTargetIsNotPresent()
        {
            var alertStatus = _TargetAlerter.SendAlertToTarget(
                BreachType.NORMAL,
                AlertTarget.TO_EMAIL,
                AlerterPrinter);

            Assert.Equal(AlertStatus.Alert_Target_Is_Not_Present, alertStatus);
        }

        [Fact]
        public void TestAlertWhenAlerterIsInvalid()
        {
            _Alerters[AlertTarget.TO_CONTROLLER] = null;

            var alertStatus = _TargetAlerter.SendAlertToTarget(
                BreachType.TOO_HIGH,
                AlertTarget.TO_CONTROLLER,
                AlerterPrinter);

            Assert.Equal(AlertStatus.Alerter_Is_Not_Valid, alertStatus);
        }
    }
}
