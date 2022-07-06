namespace TypewiseAlert
{
    using System;
    using System.Collections.Generic;

    public class EmailAlerter : IAlerter
    {
        private readonly string _DestinationEmailAddress;

        private readonly IDictionary<BreachType, string> _BreachMessages;

        public EmailAlerter(string destinationEmailAddress, IDictionary<BreachType, string> breachMessages)
        {
            _DestinationEmailAddress = destinationEmailAddress;

            _BreachMessages = new Dictionary<BreachType, string>();

            if (breachMessages != null)
            {
                _BreachMessages = breachMessages;
            }
        }

        public bool SendAlert(BreachType breachType, Func<string, bool> printerAction)
        {
            if (breachType.Equals(BreachType.NORMAL))
            {
                return true;
            }

            return printerAction.Invoke(FetchEmailData(breachType));
        }

        private string FetchEmailData(BreachType breachType)
        {
            var message = $"To : {_DestinationEmailAddress}\n";

            if (_BreachMessages.ContainsKey(breachType))
            {
                return message + $"{_BreachMessages[breachType]}\n";
            }

            return message;
        }
    }
}
