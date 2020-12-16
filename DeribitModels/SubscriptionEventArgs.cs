using System;

namespace Deribit_Scalper_Trainer.DeribitModels
{
    public class SubscriptionEventArgs : EventArgs
    {
        public string Message { get; set; }

        public SubscriptionEventArgs(string message)
        {
            Message = message;
        }
    }
}
