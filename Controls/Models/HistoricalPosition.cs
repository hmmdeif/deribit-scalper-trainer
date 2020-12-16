using Deribit_Scalper_Trainer.ContractModels;
using System;

namespace Deribit_Scalper_Trainer.Controls.Models
{
    public class HistoricalPosition
    {
        public HistoricalPosition(DateTime closeDate, decimal realisedPnl, PositionEnum type, ref IContractModel instrument)
        {
            CloseDate = closeDate;
            RealisedPnl = realisedPnl;
            Type = type;
            Instrument = instrument;
        }

        public decimal RealisedPnl { get; set; }
        public PositionEnum Type { get; set; }
        public IContractModel Instrument { get; set; }
        public DateTime CloseDate { get; set; }
    }
}
