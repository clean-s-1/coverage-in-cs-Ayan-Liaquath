namespace TypewiseAlert.Test
{
    using System.Collections.Generic;

    using Xunit;

    public class TypewiseAlertTest
    {
        private readonly IBreachChecker<double> _BreachChecker;

        private readonly ITargetAlerter _TargetAlerter;

        private BatterySpecification _BatterySpecification;

        private int _PrinterActionCallCount;

        private string _PrintedMessage;

        public TypewiseAlertTest()
        {
            var breachLimits = new List<IBreachLimits<double>>
                                   {
                                       new BreachLimits<double>(CoolingType.PASSIVE_COOLING, 0, 35),
                                       new BreachLimits<double>(CoolingType.MED_ACTIVE_COOLING, 0, 40),
                                       new BreachLimits<double>(CoolingType.HI_ACTIVE_COOLING, 0, 45)
                                   };

            _BreachChecker = new BreachChecker<double>(breachLimits);

            var alerters = new Dictionary<AlertTarget, IAlerter>
                               {
                                   { AlertTarget.TO_CONTROLLER, new ControllerAlerter(0xfeed) },
                                   { AlertTarget.TO_EMAIL, new EmailAlerter(
                                       "a.b@c.com",
                                       new Dictionary<BreachType, string>
                                           {
                                               { BreachType.TOO_LOW, "Hi, the temperature is too low" },
                                               { BreachType.TOO_HIGH, "Hi, the temperature is too high" }
                                           })
                                   }
                               };

            _TargetAlerter = new TargetAlerter(alerters);

            _BatterySpecification =
                new BatterySpecification { Brand = "Philips", CoolingType = CoolingType.PASSIVE_COOLING };

            _PrinterActionCallCount = 0;

            _PrintedMessage = null;
        }

        private bool PrinterActionCall(string arg)
        {
            _PrintedMessage = arg;

            _PrinterActionCallCount++;

            return true;
        }

        private void PerformAssertion(
            BreachStatus status,
            BreachType expectedBreachType,
            AlertStatus expectedAlertStatus,
            int expectedActionCallCount,
            string printedMessage)
        {
            Assert.NotNull(status);

            Assert.Equal(expectedBreachType, status.BreachType);

            Assert.Equal(expectedAlertStatus, status.AlertStatus);

            Assert.Equal(expectedActionCallCount, _PrinterActionCallCount);

            Assert.Equal(printedMessage, _PrintedMessage);
        }

        [Fact]
        public void CheckNormalBreachForPassiveCoolingWithControllerAlert()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 30, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 1, "65261 : NORMAL\n");

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 0, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 2, "65261 : NORMAL\n");

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 35, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 3, "65261 : NORMAL\n");
        }

        [Fact]
        public void CheckLowBreachForPassiveCoolingWithControllerAlert()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, -1, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_LOW, AlertStatus.Success, 1, "65261 : TOO_LOW\n");
        }

        [Fact]
        public void CheckHighBreachForPassiveCoolingWithControllerAlert()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 55, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Success, 1, "65261 : TOO_HIGH\n");
        }

        [Fact]
        public void CheckNormalBreachForPassiveCoolingWithEmailAlert()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 30, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 0, null);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 0, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 0, null);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 35, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 0, null);
        }

        [Fact]
        public void CheckLowBreachForPassiveCoolingWithEmailAlert()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, -1, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_LOW, AlertStatus.Success, 1, "To : a.b@c.com\nHi, the temperature is too low\n");
        }

        [Fact]
        public void CheckHighBreachForPassiveCoolingWithEmailAlert()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 55, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Success, 1, "To : a.b@c.com\nHi, the temperature is too high\n");
        }

        [Fact]
        public void CheckNormalBreachForMedCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 30, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 1, "65261 : NORMAL\n");

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 0, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 2, "65261 : NORMAL\n");

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 40, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 3, "65261 : NORMAL\n");
        }

        [Fact]
        public void CheckLowBreachForMedCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, -1, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_LOW, AlertStatus.Success, 1, "65261 : TOO_LOW\n");
        }

        [Fact]
        public void CheckHighBreachForMedCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 41, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Success, 1, "65261 : TOO_HIGH\n");
        }

        [Fact]
        public void CheckNormalBreachForMedCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 30, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 0, null);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 0, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 0, null);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 40, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 0, null);
        }

        [Fact]
        public void CheckLowBreachForMedCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, -1, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_LOW, AlertStatus.Success, 1, "To : a.b@c.com\nHi, the temperature is too low\n");
        }

        [Fact]
        public void CheckHighBreachForMedCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 55, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Success, 1, "To : a.b@c.com\nHi, the temperature is too high\n");
        }

        [Fact]
        public void CheckNormalBreachForHighCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 30, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 1, "65261 : NORMAL\n");

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 0, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 2, "65261 : NORMAL\n");

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 45, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 3, "65261 : NORMAL\n");
        }

        [Fact]
        public void CheckLowBreachForHighCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, -1, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_LOW, AlertStatus.Success, 1, "65261 : TOO_LOW\n");
        }

        [Fact]
        public void CheckHighBreachForHighCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 46, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Success, 1, "65261 : TOO_HIGH\n");
        }

        [Fact]
        public void CheckNormalBreachForHighCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 30, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 0, null);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 0, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 0, null);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 45, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success, 0, null);
        }

        [Fact]
        public void CheckLowBreachForHighCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, -1, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_LOW, AlertStatus.Success, 1, "To : a.b@c.com\nHi, the temperature is too low\n");
        }

        [Fact]
        public void CheckHighBreachForHighCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 55, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Success, 1, "To : a.b@c.com\nHi, the temperature is too high\n");
        }

        [Fact]
        public void CheckWithInvalidPrinterAction()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 55, null);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Printer_Action_Is_Invalid, 0, null);
        }

        [Fact]
        public void CheckWithInvalidAlerter()
        {
            var targetAlerter =
                new TargetAlerter(new Dictionary<AlertTarget, IAlerter> { { AlertTarget.TO_EMAIL, null } });

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, targetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 55, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Alerter_Is_Not_Valid, 0, null);
        }

        [Fact]
        public void CheckWithInvalidAlertTarget()
        {
            var targetAlerter =
                new TargetAlerter(new Dictionary<AlertTarget, IAlerter> { { AlertTarget.TO_EMAIL, null } });

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, targetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 55, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Alert_Target_Is_Not_Present, 0, null);
        }
    }
}
