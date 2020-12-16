using System;
using System.Collections.Generic;

namespace Deribit_Scalper_Trainer.ContractModels
{
    public abstract class BaseContractModel : IContractModel
    {
        public decimal MinimumTickSize { get; set; }
        public decimal ContractSize { get; set; }
        public abstract decimal InitialMargin(decimal btcAmount);
        public abstract decimal MaintenanceMargin(decimal totalBtcAmount);
        public abstract decimal MarkPrice();
        public abstract decimal MarketFee(decimal btcAmount);
        public DateTime Expiration { get; set; }
        public decimal MakerFee { get; set; }
        public decimal TakerFee { get; set; }
        public int PositionLimit { get; set; }
        public string Name { get; set; }
        public decimal Premium { get; set; }
        public string BaseInstrumentName { get; set; }
        public Dictionary<DateTime, Tuple<decimal, decimal>> BaseMarketPriceHistory { get; set; }
        public Dictionary<DateTime, Tuple<decimal, decimal>> MarketPriceHistory { get; set; }
        public decimal IndexPrice { get; set; }

        public bool IsValidContractAmount(int contractAmount)
        {
            return contractAmount / ContractSize <= PositionLimit;
        }
    }
}
