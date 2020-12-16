using System;

namespace Deribit_Scalper_Trainer.Controls.Models
{
    public class HistoricalPositionAddedEventArgs : EventArgs
    {
        public HistoricalPosition Position { get; set; }

        public HistoricalPositionAddedEventArgs(HistoricalPosition position)
        {
            Position = position;
        }
    }
}
