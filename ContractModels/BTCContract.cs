using System;
using System.Collections.Generic;
using System.Linq;

namespace Deribit_Scalper_Trainer.ContractModels
{
    public class BTCContract : BaseContractModel
    {
        public BTCContract(decimal tickSize, decimal contractSize, decimal takerFee, decimal makerFee, string name, long expiration)
        {
            MinimumTickSize = tickSize;
            ContractSize = contractSize;
            TakerFee = takerFee;
            MakerFee = makerFee;
            PositionLimit = 1000000;
            Name = name;
            BaseInstrumentName = "BTC-PERPETUAL";
            BaseMarketPriceHistory = new Dictionary<DateTime, Tuple<decimal, decimal>>();
            MarketPriceHistory = new Dictionary<DateTime, Tuple<decimal, decimal>>();
            Expiration = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(expiration);
        }

        public override decimal InitialMargin(decimal btcAmount)
        {
            return (0.01M + (btcAmount * (TakerFee / ContractSize))) * btcAmount;
        }

        public override decimal MaintenanceMargin(decimal totalBtcAmount)
        {
            return (0.00525M + (totalBtcAmount * (TakerFee / ContractSize))) * totalBtcAmount;
        }

        public override decimal MarkPrice()
        {
            var d = DateTime.UtcNow.AddSeconds(-30);
            BaseMarketPriceHistory = BaseMarketPriceHistory.Where(x => x.Key >= d).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            MarketPriceHistory = MarketPriceHistory.Where(x => x.Key >= d).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            var baseEMA = 0M;
            var i = 1M;
            foreach (var marketPriceTuple in MarketPriceHistory)
            {
                var k = 2M / (i + 1M);
                baseEMA = ((marketPriceTuple.Value.Item1 - marketPriceTuple.Value.Item2) * k) + (baseEMA * (1M - k));
                ++i;
            }

            return IndexPrice + baseEMA;
        }

        public override decimal MarketFee(decimal btcAmount)
        {
            return btcAmount * (TakerFee / ContractSize);
        }
    }
}
