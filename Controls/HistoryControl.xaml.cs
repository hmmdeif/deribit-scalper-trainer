#nullable enable
using Deribit_Scalper_Trainer.Controls.Models;
using System.Windows.Controls;

namespace Deribit_Scalper_Trainer
{
    /// <summary>
    /// Interaction logic for HistoryControl.xaml
    /// </summary>
    public partial class HistoryControl : UserControl
    {
        public HistoryControl()
        {
            InitializeComponent();
        }

        protected HistoryManager? historyManager;

        public void RegisterEvents(PositionManager positionManager, HistoryManager historyManager)
        {
            this.historyManager = historyManager;
            HistoryGrid.ItemsSource = this.historyManager.GetHistoricalPositions();

            positionManager.HistoricalPositionAddedMessage += HistoricalPositionAddedMessage;
        }

        void HistoricalPositionAddedMessage(object? sender, HistoricalPositionAddedEventArgs e)
        {
            historyManager?.AddPosition(e.Position);
        }
    }
}
