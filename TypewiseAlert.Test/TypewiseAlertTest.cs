namespace TypewiseAlert.Test
{
    using System.Collections.Generic;

    using Xunit;

    public class TypewiseAlertTest
    {
        private readonly IBreachChecker<double> _BreachChecker;

        private readonly ITargetAlerter _TargetAlerter;

        private BatterySpecification _BatterySpecification;

        private ITypewiseAlert<double> _TypewiseAlert;

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

            _TypewiseAlert = new TypewiseAlert<double>(_BreachChecker, _TargetAlerter);
        }

        private bool PrinterActionCall(string arg)
        {
            return true;
        }

        private void PerformAssertion(
            BreachStatus status,
            BreachType expectedBreachType,
            AlertStatus expectedAlertStatus)
        {
            Assert.NotNull(status);

            Assert.Equal(expectedBreachType, status.BreachType);

            Assert.Equal(expectedAlertStatus, status.AlertStatus);
        }

        [Fact]
        public void CheckNormalBreachForPassiveCoolingWithControllerAlert()
        {
            var passiveCoolingStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 30, PrinterActionCall);

            PerformAssertion(passiveCoolingStatus, BreachType.NORMAL, AlertStatus.Success);

            passiveCoolingStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 0, PrinterActionCall);

            PerformAssertion(passiveCoolingStatus, BreachType.NORMAL, AlertStatus.Success);

            passiveCoolingStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 35, PrinterActionCall);

            PerformAssertion(passiveCoolingStatus, BreachType.NORMAL, AlertStatus.Success);
        }

        [Fact]
        public void CheckLowBreachForPassiveCoolingWithControllerAlert()
        {
            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, -1, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_LOW, AlertStatus.Success);
        }

        [Fact]
        public void CheckHighBreachForPassiveCoolingWithControllerAlert()
        {
            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 55, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Success);
        }

        [Fact]
        public void CheckNormalBreachForPassiveCoolingWithEmailAlert()
        {
            var emailStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 20, PrinterActionCall);

            PerformAssertion(emailStatus, BreachType.NORMAL, AlertStatus.Success);

            emailStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 0, PrinterActionCall);

            PerformAssertion(emailStatus, BreachType.NORMAL, AlertStatus.Success);

            emailStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 35, PrinterActionCall);

            PerformAssertion(emailStatus, BreachType.NORMAL, AlertStatus.Success);
        }

        [Fact]
        public void CheckLowBreachForPassiveCoolingWithEmailAlert()
        {
            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, -10, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_LOW, AlertStatus.Success);
        }

        [Fact]
        public void CheckHighBreachForPassiveCoolingWithEmailAlert()
        {
            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 65, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Success);
        }

        [Fact]
        public void CheckNormalBreachForMedCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var controllerStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 10, PrinterActionCall);

            PerformAssertion(controllerStatus, BreachType.NORMAL, AlertStatus.Success);

            controllerStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 0, PrinterActionCall);

            PerformAssertion(controllerStatus, BreachType.NORMAL, AlertStatus.Success);

            controllerStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 40, PrinterActionCall);

            PerformAssertion(controllerStatus, BreachType.NORMAL, AlertStatus.Success);
        }

        [Fact]
        public void CheckLowBreachForMedCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, -2, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_LOW, AlertStatus.Success);
        }

        [Fact]
        public void CheckHighBreachForMedCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 41, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Success);
        }

        [Fact]
        public void CheckNormalBreachForMedCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var medCoolingStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 25, PrinterActionCall);

            PerformAssertion(medCoolingStatus, BreachType.NORMAL, AlertStatus.Success);

            medCoolingStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 0, PrinterActionCall);

            PerformAssertion(medCoolingStatus, BreachType.NORMAL, AlertStatus.Success);

            medCoolingStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 40, PrinterActionCall);

            PerformAssertion(medCoolingStatus, BreachType.NORMAL, AlertStatus.Success);
        }

        [Fact]
        public void CheckLowBreachForMedCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, -3, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_LOW, AlertStatus.Success);
        }

        [Fact]
        public void CheckHighBreachForMedCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.MED_ACTIVE_COOLING;

            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 60, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Success);
        }

        [Fact]
        public void CheckNormalBreachForHighCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 35, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success);

            status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 0, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success);

            status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 45, PrinterActionCall);

            PerformAssertion(status, BreachType.NORMAL, AlertStatus.Success);
        }

        [Fact]
        public void CheckLowBreachForHighCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, -4, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_LOW, AlertStatus.Success);
        }

        [Fact]
        public void CheckHighBreachForHighCoolingWithControllerAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 46, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Success);
        }

        [Fact]
        public void CheckNormalBreachForHighCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var highCoolingStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 15, PrinterActionCall);

            PerformAssertion(highCoolingStatus, BreachType.NORMAL, AlertStatus.Success);

            highCoolingStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 0, PrinterActionCall);

            PerformAssertion(highCoolingStatus, BreachType.NORMAL, AlertStatus.Success);

            highCoolingStatus = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 45, PrinterActionCall);

            PerformAssertion(highCoolingStatus, BreachType.NORMAL, AlertStatus.Success);
        }

        [Fact]
        public void CheckLowBreachForHighCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, -6, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_LOW, AlertStatus.Success);
        }

        [Fact]
        public void CheckHighBreachForHighCoolingWithEmailAlert()
        {
            _BatterySpecification.CoolingType = CoolingType.HI_ACTIVE_COOLING;

            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 50, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Success);
        }

        [Fact]
        public void CheckWithInvalidPrinterAction()
        {
            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_EMAIL, _BatterySpecification, 100, null);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Printer_Action_Is_Invalid);
        }

        [Fact]
        public void CheckWithInvalidAlerter()
        {
            var targetAlerter =
                new TargetAlerter(new Dictionary<AlertTarget, IAlerter> { { AlertTarget.TO_CONTROLLER, null } });

            _TypewiseAlert = new TypewiseAlert<double>(_BreachChecker, targetAlerter);

            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 105, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Alerter_Is_Not_Valid);
        }

        [Fact]
        public void CheckWithInvalidAlertTarget()
        {
            var targetAlerter =
                new TargetAlerter(new Dictionary<AlertTarget, IAlerter> { { AlertTarget.TO_EMAIL, null } });

            _TypewiseAlert = new TypewiseAlert<double>(_BreachChecker, targetAlerter);

            var status = _TypewiseAlert.CheckBreachAndAlert(AlertTarget.TO_CONTROLLER, _BatterySpecification, 90, PrinterActionCall);

            PerformAssertion(status, BreachType.TOO_HIGH, AlertStatus.Alert_Target_Is_Not_Present);
        }
    }
}
