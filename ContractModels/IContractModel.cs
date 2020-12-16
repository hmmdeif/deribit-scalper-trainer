using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deribit_Scalper_Trainer.ContractModels
{
    public interface IContractModel
    {
        public string Name { get; set; }
        public decimal MinimumTickSize { get; set; }
        public decimal ContractSize { get; set; }
        public decimal InitialMargin(decimal btcAmount);
        public decimal MaintenanceMargin(decimal totalBtcAmount);
        public decimal MarkPrice();
        public DateTime Expiration { get; set; }
        public decimal MakerFee { get; set; }
        public decimal TakerFee { get; set; }
        public int PositionLimit { get; set; }
        public bool IsValidContractAmount(int contractAmount);
        public decimal Premium { get; set; }
        public string BaseInstrumentName { get; set; }
        public Dictionary<DateTime, Tuple<decimal, decimal>> BaseMarketPriceHistory { get; set; }
        public Dictionary<DateTime, Tuple<decimal, decimal>> MarketPriceHistory { get; set; }
        public decimal IndexPrice { get; set; }
        public decimal MarketFee(decimal btcAmount);
    }
}
