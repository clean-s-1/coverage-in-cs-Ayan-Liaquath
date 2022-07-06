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

        [Fact]
        public void CheckNormalBreachForPassiveCoolingWithControllerAlert()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 30, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("65261 : NORMAL\n", _PrintedMessage);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 0, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(2, _PrinterActionCallCount);

            Assert.Equal("65261 : NORMAL\n", _PrintedMessage);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 35, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(3, _PrinterActionCallCount);

            Assert.Equal("65261 : NORMAL\n", _PrintedMessage);
        }

        [Fact]
        public void CheckLowBreachForPassiveCoolingWithControllerAlert()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, -1, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_LOW, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("65261 : TOO_LOW\n", _PrintedMessage);
        }

        [Fact]
        public void CheckHighBreachForPassiveCoolingWithControllerAlert()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 55, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_HIGH, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("65261 : TOO_HIGH\n", _PrintedMessage);
        }

        [Fact]
        public void CheckNormalBreachForPassiveCoolingWithEmailAlert()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 30, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(0, _PrinterActionCallCount);

            Assert.Null(_PrintedMessage);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 0, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(0, _PrinterActionCallCount);

            Assert.Null(_PrintedMessage);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 35, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(0, _PrinterActionCallCount);

            Assert.Null(_PrintedMessage);
        }

        [Fact]
        public void CheckLowBreachForPassiveCoolingWithEmailAlert()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, -1, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_LOW, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("To : a.b@c.com\nHi, the temperature is too low\n", _PrintedMessage);
        }

        [Fact]
        public void CheckHighBreachForPassiveCoolingWithEmailAlert()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 55, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_HIGH, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("To : a.b@c.com\nHi, the temperature is too high\n", _PrintedMessage);
        }

        [Fact]
        public void CheckNormalBreachForMedCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 30, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("65261 : NORMAL\n", _PrintedMessage);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 0, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(2, _PrinterActionCallCount);

            Assert.Equal("65261 : NORMAL\n", _PrintedMessage);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 40, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(3, _PrinterActionCallCount);

            Assert.Equal("65261 : NORMAL\n", _PrintedMessage);
        }

        [Fact]
        public void CheckLowBreachForMedCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, -1, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_LOW, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("65261 : TOO_LOW\n", _PrintedMessage);
        }

        [Fact]
        public void CheckHighBreachForMedCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 41, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_HIGH, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("65261 : TOO_HIGH\n", _PrintedMessage);
        }

        [Fact]
        public void CheckNormalBreachForMedCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 30, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(0, _PrinterActionCallCount);

            Assert.Null(_PrintedMessage);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 0, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(0, _PrinterActionCallCount);

            Assert.Null(_PrintedMessage);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 40, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(0, _PrinterActionCallCount);

            Assert.Null(_PrintedMessage);
        }

        [Fact]
        public void CheckLowBreachForMedCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, -1, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_LOW, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("To : a.b@c.com\nHi, the temperature is too low\n", _PrintedMessage);
        }

        [Fact]
        public void CheckHighBreachForMedCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 55, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_HIGH, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("To : a.b@c.com\nHi, the temperature is too high\n", _PrintedMessage);
        }

        [Fact]
        public void CheckNormalBreachForHighCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 30, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("65261 : NORMAL\n", _PrintedMessage);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 0, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(2, _PrinterActionCallCount);

            Assert.Equal("65261 : NORMAL\n", _PrintedMessage);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 45, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(3, _PrinterActionCallCount);

            Assert.Equal("65261 : NORMAL\n", _PrintedMessage);
        }

        [Fact]
        public void CheckLowBreachForHighCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, -1, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_LOW, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("65261 : TOO_LOW\n", _PrintedMessage);
        }

        [Fact]
        public void CheckHighBreachForHighCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 46, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_HIGH, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("65261 : TOO_HIGH\n", _PrintedMessage);
        }

        [Fact]
        public void CheckNormalBreachForHighCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 30, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(0, _PrinterActionCallCount);

            Assert.Null(_PrintedMessage);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 0, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(0, _PrinterActionCallCount);

            Assert.Null(_PrintedMessage);

            status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 45, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.NORMAL, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(0, _PrinterActionCallCount);

            Assert.Null(_PrintedMessage);
        }

        [Fact]
        public void CheckLowBreachForHighCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, -1, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_LOW, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("To : a.b@c.com\nHi, the temperature is too low\n", _PrintedMessage);
        }

        [Fact]
        public void CheckHighBreachForHighCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 55, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_HIGH, status.BreachType);

            Assert.Equal(AlertStatus.Success, status.AlertStatus);

            Assert.Equal(1, _PrinterActionCallCount);

            Assert.Equal("To : a.b@c.com\nHi, the temperature is too high\n", _PrintedMessage);
        }

        [Fact]
        public void CheckWithInvalidPrinterAction()
        {
            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 55, null);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_HIGH, status.BreachType);

            Assert.Equal(AlertStatus.Printer_Action_Is_Invalid, status.AlertStatus);

            Assert.Equal(0, _PrinterActionCallCount);

            Assert.Null(_PrintedMessage);
        }

        [Fact]
        public void CheckWithInvalidAlerter()
        {
            var targetAlerter =
                new TargetAlerter(new Dictionary<AlertTarget, IAlerter> { { AlertTarget.TO_EMAIL, null } });

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, targetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 55, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_HIGH, status.BreachType);

            Assert.Equal(AlertStatus.Alerter_Is_Not_Valid, status.AlertStatus);

            Assert.Equal(0, _PrinterActionCallCount);

            Assert.Null(_PrintedMessage);
        }

        [Fact]
        public void CheckWithInvalidAlertTarget()
        {
            var targetAlerter =
                new TargetAlerter(new Dictionary<AlertTarget, IAlerter> { { AlertTarget.TO_EMAIL, null } });

            var typeWiseAlert = new TypewiseAlert<double>(_BreachChecker, targetAlerter);

            var status = typeWiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 55, PrinterActionCall);

            Assert.NotNull(status);

            Assert.Equal(BreachType.TOO_HIGH, status.BreachType);

            Assert.Equal(AlertStatus.Alert_Target_Is_Not_Present, status.AlertStatus);

            Assert.Equal(0, _PrinterActionCallCount);

            Assert.Null(_PrintedMessage);
        }
    }
}
