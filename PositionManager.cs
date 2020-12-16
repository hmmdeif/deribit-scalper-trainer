#nullable enable
using Deribit_Scalper_Trainer.ContractModels;
using Deribit_Scalper_Trainer.Controls.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Deribit_Scalper_Trainer
{
    public class PositionManager
    {
        public PositionManager()
        {
            Positions = new ObservableCollection<Position>();
            BTCBalance = 1.0M;
        }
        protected ObservableCollection<Position> Positions { get; set; }
        protected decimal BTCBalance { get; set; }

        public ObservableCollection<Position> GetPositions()
        {
            return Positions;
        }

        public event EventHandler<BalanceChangeEventArgs>? BalanceChangeMessage;
        protected virtual void OnBalanceChangeMessage(BalanceChangeEventArgs e)
        {
            EventHandler<BalanceChangeEventArgs>? handler = BalanceChangeMessage;
            handler?.Invoke(this, e);
        }

        public event EventHandler<HistoricalPositionAddedEventArgs>? HistoricalPositionAddedMessage;
        protected virtual void OnHistoricalPositionAddedMessage(HistoricalPositionAddedEventArgs e)
        {
            EventHandler<HistoricalPositionAddedEventArgs>? handler = HistoricalPositionAddedMessage;
            handler?.Invoke(this, e);
        }

        private Tuple<decimal, decimal> CurrentMarginUsed()
        {
            var sellMargin = 0.0M;
            var buyMargin = 0.0M;
            foreach (var p in Positions)
            {
                if (p.Type == PositionEnum.Buy)
                {
                    buyMargin += p.Instrument.MaintenanceMargin(p.Amount / p.Price);
                } else
                {
                    sellMargin += p.Instrument.MaintenanceMargin(p.Amount / p.Price);
                }
                
            }
            return new Tuple<decimal, decimal>(buyMargin, sellMargin);
        }

        private decimal CalculateBTCMarginRequirement(IContractModel instrument, PositionEnum type, int amount, decimal price)
        {
            var marginUsed = CurrentMarginUsed();
            var marginRequired = instrument.InitialMargin(amount / price);
            if (type == PositionEnum.Buy)
            {
                marginRequired = marginRequired + marginUsed.Item1 - marginUsed.Item2;
            } else
            {
                marginRequired = marginRequired + marginUsed.Item2 - marginUsed.Item1;
            }

            if (marginRequired < 0)
            {
                marginRequired = 0;
            }
            return marginRequired;
        }

        private void CalculateLiquidationPrice()
        {
            if (Positions.Count > 0)
            {
                var marginUsed = CurrentMarginUsed();
                var positionType = PositionEnum.Buy;
                var maintenanceMargin = Math.Abs(marginUsed.Item1 - marginUsed.Item2);
                var netAmount = 0;
                var entryPrice = 0M;
                var instrumentUsedForCalculation = Positions.First().Instrument;
                var liquidationPrice = 0M;
                if (marginUsed.Item1 < marginUsed.Item2)
                {
                    positionType = PositionEnum.Sell;
                }

                foreach (var position in Positions)
                {
                    if (position.Type == positionType)
                    {
                        netAmount += position.Amount;
                        entryPrice = position.Price;
                        instrumentUsedForCalculation = position.Instrument;
                    }
                    else
                    {
                        netAmount -= position.Amount;
                    }
                }

                var marginRemaining = BTCBalance - maintenanceMargin;
                if (positionType == PositionEnum.Buy)
                {
                    if (netAmount == 0)
                    {
                        liquidationPrice = 0M;
                    } else
                    {
                        liquidationPrice = entryPrice / (((marginRemaining * entryPrice) / netAmount) + 1);
                    }
                }
                else
                {
                    if (netAmount == 0)
                    {
                        liquidationPrice = 100000000000;
                    } else
                    {
                        liquidationPrice = entryPrice / (1 - ((marginRemaining * entryPrice) / netAmount));
                        if (liquidationPrice <= 0)
                        {
                            liquidationPrice = 100000000000;
                        }
                    }
                }

                if (instrumentUsedForCalculation.Name != instrumentUsedForCalculation.BaseInstrumentName)
                {
                    var latestBaseMarketPrice = instrumentUsedForCalculation.BaseMarketPriceHistory.OrderByDescending(x => x.Key).First().Value.Item1;
                    var latestMarketPrice = instrumentUsedForCalculation.MarketPriceHistory.OrderByDescending(x => x.Key).First().Value.Item1;
                    liquidationPrice -= latestMarketPrice - latestBaseMarketPrice;
                }

                foreach (var position in Positions)
                {
                    position.LiquidationPrice = liquidationPrice;
                    if (position.Instrument.Name != position.Instrument.BaseInstrumentName)
                    {
                        var latestBaseMarketPrice = position.Instrument.BaseMarketPriceHistory.OrderByDescending(x => x.Key).First().Value.Item1;
                        var latestMarketPrice = position.Instrument.MarketPriceHistory.OrderByDescending(x => x.Key).First().Value.Item1;
                        position.LiquidationPrice += latestMarketPrice - latestBaseMarketPrice;
                    }
                }
            }
        }

        public bool AddPosition(ref IContractModel instrument, PositionEnum type, int amount, decimal price)
        {
            if (instrument.IsValidContractAmount(amount) && CalculateBTCMarginRequirement(instrument, type, amount, price) < BTCBalance)
            {
                var localInstrument = instrument;
                var existingPosition = Positions.FirstOrDefault(x => x.Instrument.Name == localInstrument.Name);
                if (existingPosition == null)
                {
                    Positions.Add(new Position(amount, price, type, ref instrument));
                    BTCBalance -= instrument.MarketFee(amount / price);
                    OnBalanceChangeMessage(new BalanceChangeEventArgs(BTCBalance));
                } else
                {
                    if (existingPosition.Type == type)
                    {
                        existingPosition.Amount += amount;
                        existingPosition.Price = decimal.Round((existingPosition.Amount + amount) / ((existingPosition.Amount / existingPosition.Price) + (amount / price)), 2);
                    } else
                    {
                        existingPosition.Amount -= amount;
                        var realisedPnl = existingPosition.UnrealisedPnl;
                        var positionType = existingPosition.Type;
                        if (existingPosition.Amount < 0)
                        {
                            existingPosition.Type = existingPosition.Type == PositionEnum.Buy ? PositionEnum.Sell : PositionEnum.Buy;
                            existingPosition.Amount *= -1;
                            existingPosition.Price = price;
                            existingPosition.UnrealisedPnl = 0M;
                        } else if (existingPosition.Amount == 0)
                        {
                            Positions.Remove(existingPosition);
                        } else
                        {
                            if (existingPosition.Type == PositionEnum.Buy)
                            {
                                existingPosition.UnrealisedPnl = (existingPosition.Amount / existingPosition.Price) - (existingPosition.Amount / price);
                            }
                            else
                            {
                                existingPosition.UnrealisedPnl = (existingPosition.Amount / price) - (existingPosition.Amount / existingPosition.Price);
                            }
                            realisedPnl -= existingPosition.UnrealisedPnl;
                        }

                        BTCBalance += realisedPnl - instrument.MarketFee(amount / price);
                        OnBalanceChangeMessage(new BalanceChangeEventArgs(BTCBalance));
                        OnHistoricalPositionAddedMessage(new HistoricalPositionAddedEventArgs(new HistoricalPosition(DateTime.Now, realisedPnl, positionType, ref instrument)));
                    }
                }

                CalculateLiquidationPrice();
                return true;
            }

            return false;
        }

        public decimal RecalculatePositions(string name, decimal bidPrice, decimal askPrice)
        {
            foreach (var position in Positions.Where(p => p.Instrument.Name == name))
            {
                if (position.Type == PositionEnum.Buy)
                {
                    position.UnrealisedPnl = (position.Amount / position.Price) - (position.Amount / askPrice);
                } else
                {
                    position.UnrealisedPnl = (position.Amount / bidPrice) - (position.Amount / position.Price);
                }
            }
            return BTCBalance;
        }

        public void CheckForLiquidations()
        {
            var shouldClear = false;
            var currentMargin = CurrentMarginUsed();
            foreach (var position in Positions)
            {
                var markPrice = position.Instrument.MarkPrice();
                if ((position.Type == PositionEnum.Buy && currentMargin.Item1 >= currentMargin.Item2 && markPrice < position.LiquidationPrice) || (position.Type == PositionEnum.Sell && currentMargin.Item1 < currentMargin.Item2 && markPrice > position.LiquidationPrice))
                {
                    var localInstrument = position.Instrument;
                    OnHistoricalPositionAddedMessage(new HistoricalPositionAddedEventArgs(new HistoricalPosition(DateTime.Now, BTCBalance * -1, position.Type, ref localInstrument)));
                    shouldClear = true;
                    BTCBalance = 0M;
                    OnBalanceChangeMessage(new BalanceChangeEventArgs(BTCBalance));
                    break;
                }
            }

            if (shouldClear)
            {
                Positions.Clear();
            }
        }
    }
}
