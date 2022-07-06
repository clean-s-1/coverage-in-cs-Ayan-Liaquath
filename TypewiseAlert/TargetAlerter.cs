namespace TypewiseAlert
{
    using System;
    using System.Collections.Generic;

    public class TargetAlerter : ITargetAlerter
    {
        private readonly IDictionary<AlertTarget, IAlerter> _Alerters;

        public TargetAlerter(IDictionary<AlertTarget, IAlerter> alerters)
        {
            _Alerters = new Dictionary<AlertTarget, IAlerter>();

            if (alerters != null)
            {
                _Alerters = alerters;
            }
        }

        public AlertStatus SendAlertToTarget(BreachType breachType, AlertTarget alertTarget, Func<string, bool> printerAction)
        {
            if (CheckIfAlertTargetExists(alertTarget))
            {
                return InvokeAlerter(breachType, alertTarget, printerAction);
            }

            return AlertStatus.Alert_Target_Is_Not_Present;
        }

        private AlertStatus InvokeAlerter(BreachType breachType, AlertTarget alertTarget, Func<string, bool> printerAction)
        {
            var alerter = _Alerters[alertTarget];

            if (printerAction == null)
            {
                return AlertStatus.Printer_Action_Is_Invalid;
            }

            if (CheckIfAlerterIsValid(alerter))
            {
                return FetchAlertStatus(breachType, printerAction, alerter);
            }

            return AlertStatus.Alerter_Is_Not_Valid;
        }

        private static AlertStatus FetchAlertStatus(BreachType breachType, Func<string, bool> printerAction, IAlerter alerter)
        {
            return alerter.SendAlert(breachType, printerAction) ? AlertStatus.Success : AlertStatus.Failed;
        }

        private bool CheckIfAlerterIsValid(IAlerter alerter)
        {
            return alerter != null;
        }

        private bool CheckIfAlertTargetExists(AlertTarget alertTarget)
        {
            return _Alerters.ContainsKey(alertTarget);
        }
    }
}
