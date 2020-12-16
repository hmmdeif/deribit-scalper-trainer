using Deribit_Scalper_Trainer.Controls.Models;
using System.Collections.ObjectModel;

namespace Deribit_Scalper_Trainer
{
    public class HistoryManager
    {
        public HistoryManager()
        {
            HistoricalPositions = new ObservableCollection<HistoricalPosition>();
        }

        protected ObservableCollection<HistoricalPosition> HistoricalPositions { get; set; }

        public ObservableCollection<HistoricalPosition> GetHistoricalPositions()
        {
            return HistoricalPositions;
        }

        public void AddPosition(HistoricalPosition position)
        {
            HistoricalPositions.Add(position);
        }
    }
}
