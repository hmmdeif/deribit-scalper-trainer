using System;

namespace Deribit_Scalper_Trainer.Controls.Models
{
    public class BalanceChangeEventArgs : EventArgs
    {
        public decimal Balance { get; set; }

        public BalanceChangeEventArgs(decimal balance)
        {
            Balance = balance;
        }
    }
}
