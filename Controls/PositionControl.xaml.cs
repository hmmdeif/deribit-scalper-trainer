#nullable enable
using Deribit_Scalper_Trainer.Controls.Models;
using Deribit_Scalper_Trainer.DeribitModels;
using System.Text.Json;
using System.Windows.Controls;

namespace Deribit_Scalper_Trainer
{
    /// <summary>
    /// Interaction logic for PositionControl.xaml
    /// </summary>
    public partial class PositionControl : UserControl
    {
        public PositionControl()
        {
            InitializeComponent();
        }

        protected PositionManager? positionManager;
        private DeribitClient? client;

        public void RegisterEvents(DeribitClient client, PositionManager positionManager)
        {
            this.positionManager = positionManager;
            this.positionManager.BalanceChangeMessage += BalanceChangeMessage;
            PositionGrid.ItemsSource = this.positionManager.GetPositions();

            this.client = client;
            this.client.ClientSubscriptionMessage += ClientSubscriptionMessage;
        }

        void BalanceChangeMessage(object? sender, BalanceChangeEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                BTCBalanceText.Text = e.Balance.ToString("N3");
            });
        }

        void ClientSubscriptionMessage(object? sender, SubscriptionEventArgs e)
        {
            if (e.Message.Contains("quote."))
            {
                var quote = JsonSerializer.Deserialize<QuoteModel>(e.Message);
                if (quote != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var balance = positionManager?.RecalculatePositions(quote.Params.Data.Instrument, quote.Params.Data.BestBidPrice, quote.Params.Data.BestAskPrice);
                        if (balance.HasValue)
                        {
                            BTCBalanceText.Text = balance.Value.ToString("N3");
                        }
                    });
                }
            }
            else if (e.Message.Contains("deribit_price_index."))
            {
                var index = JsonSerializer.Deserialize<IndexModel>(e.Message);
                Dispatcher.Invoke(() =>
                {
                    positionManager?.CheckForLiquidations();
                });
            }
        }
    }
}
