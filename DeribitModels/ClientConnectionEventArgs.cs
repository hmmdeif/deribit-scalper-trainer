using System;

namespace Deribit_Scalper_Trainer.DeribitModels
{
    public class ClientConnectionEventArgs : EventArgs
    {
        public bool Connected { get; set; }

        public ClientConnectionEventArgs(bool connected)
        {
            Connected = connected;
        }
    }
}
