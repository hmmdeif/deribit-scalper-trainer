#nullable enable
using Deribit_Scalper_Trainer.ContractModels;
using Deribit_Scalper_Trainer.DeribitModels;
using System;
using System.Text.Json;
using System.Windows.Controls;

namespace Deribit_Scalper_Trainer
{
    /// <summary>
    /// Interaction logic for OrderControl.xaml
    /// </summary>
    public partial class OrderChildControl : UserControl
    {
        public OrderChildControl(IContractModel instrument, PositionManager? positionManager, DeribitClient? client)
        {
            InitializeComponent();

            Instrument = instrument;
            InstrumentNameText.Content = Instrument.Name;
            PositionManager = positionManager;
            Client = client;
            if (Client != null)
            {
                Client.ClientSubscriptionMessage += ClientSubscriptionMessage;
            }
        } 

        private readonly PositionManager? PositionManager;
        private readonly DeribitClient? Client;
        private IContractModel Instrument;
        private decimal LatestBidPrice { get; set; }
        private decimal LatestAskPrice { get; set; }

        void ClientSubscriptionMessage(object? sender, SubscriptionEventArgs e)
        {
            if (e.Message.Contains("quote."))
            {
                var quote = JsonSerializer.Deserialize<QuoteModel>(e.Message);
                if (quote != null && quote.Params.Data.Instrument == Instrument.Name)
                {
                    var dateKey = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(quote.Params.Data.Timestamp);
                    if (!Instrument.MarketPriceHistory.ContainsKey(dateKey))
                    {
                        Instrument.MarketPriceHistory.Add(dateKey, new Tuple<decimal, decimal>(quote.Params.Data.BestBidPrice, Instrument.IndexPrice));
                    }
                    Dispatcher.Invoke(() =>
                    {
                        LatestBidPrice = quote.Params.Data.BestBidPrice;
                        LatestAskPrice = quote.Params.Data.BestAskPrice;
                        LatestBidPriceText.Text = quote.Params.Data.BestBidPrice.ToString("N2");
                        LatestAskPriceText.Text = quote.Params.Data.BestAskPrice.ToString("N2");
                    });
                }
                if (quote != null && quote.Params.Data.Instrument == Instrument.BaseInstrumentName)
                {
                    var dateKey = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(quote.Params.Data.Timestamp);
                    if (!Instrument.BaseMarketPriceHistory.ContainsKey(dateKey))
                    {
                        Instrument.BaseMarketPriceHistory.Add(new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(quote.Params.Data.Timestamp), new Tuple<decimal, decimal>(quote.Params.Data.BestBidPrice, Instrument.IndexPrice));
                    }
                }
            } 
            else if (e.Message.Contains("deribit_price_index."))
            {
                var index = JsonSerializer.Deserialize<IndexModel>(e.Message);
                Dispatcher.Invoke(() =>
                {
                    IndexPriceText.Text = index?.Params.Data.Price.ToString("N2");
                    MarkPriceText.Text = Instrument.MarkPrice().ToString("N2");
                });

                if (index != null)
                {
                    Instrument.IndexPrice = index.Params.Data.Price;
                }
            }
        }

        private void Buy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (int.TryParse(BuyPositionAmount.Text, out int amount))
            {
                PositionManager?.AddPosition(ref Instrument, PositionEnum.Buy, amount, LatestAskPrice);
            }
        }

        private void Sell_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (int.TryParse(SellPositionAmount.Text, out int amount))
            {
                PositionManager?.AddPosition(ref Instrument, PositionEnum.Sell, amount, LatestBidPrice);
            }
        }
    }
}
