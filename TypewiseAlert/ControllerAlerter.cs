namespace TypewiseAlert
{
    using System;

    public class ControllerAlerter : IAlerter
    {
        private readonly ushort _Header;

        public ControllerAlerter(ushort header)
        {
            _Header = header;
        }

        public bool SendAlert(BreachType breachType, Func<string, bool> printerAction)
        {
            return printerAction == null ? false : printerAction.Invoke(FetchControllerData(breachType));
        }

        private string FetchControllerData(BreachType breachType)
        {
            return $"{_Header} : {breachType}\n";
        }
    }
}
