#nullable enable
using Deribit_Scalper_Trainer.ContractModels;
using Deribit_Scalper_Trainer.DeribitModels;
using System.Linq;
using System.Windows.Controls;

namespace Deribit_Scalper_Trainer
{
    /// <summary>
    /// Interaction logic for OrderControl.xaml
    /// </summary>
    public partial class OrderControl : UserControl
    {
        public OrderControl()
        {
            InitializeComponent();
        }

        private DeribitClient? client;
        private PositionManager? positionManager;

        public void RegisterEvents(DeribitClient client, PositionManager positionManager)
        {
            this.positionManager = positionManager;
            this.client = client;
            this.client.GetInstrumentsResponseMessage += GetInstrumentsResponseMessage;
        }

        void GetInstrumentsResponseMessage(object? sender, GetInstrumentsEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var i = 0;
                foreach (var instrument in e.Results.OrderByDescending(x => x.Expiration))
                {
                    OrderChilds.ColumnDefinitions.Add(new ColumnDefinition());
                    var child = new OrderChildControl(new BTCContract(instrument.TickSize, instrument.ContractSize, instrument.TakerFee, instrument.MakerFee, instrument.Name, instrument.Expiration), positionManager, client);
                    OrderChilds.Children.Add(child);
                    Grid.SetColumn(child, i);
                    ++i;
                }
            });
        }
    }
}
