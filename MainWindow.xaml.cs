#nullable enable
using System.Windows;

namespace Deribit_Scalper_Trainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DeribitClient Client { get; set; }
        private PositionManager PositionMan { get; set; }
        private HistoryManager HistoryMan { get; set; }
        public MainWindow()
        {
            Client = new DeribitClient();
            PositionMan = new PositionManager();
            HistoryMan = new HistoryManager();
            InitializeComponent();
            
            connectionStatus.RegisterEvents(Client);
            orderControl.RegisterEvents(Client, PositionMan);
            positionControl.RegisterEvents(Client, PositionMan);
            historyControl.RegisterEvents(PositionMan, HistoryMan);
        }
    }
}
