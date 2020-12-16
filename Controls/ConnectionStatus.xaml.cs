#nullable enable
using Deribit_Scalper_Trainer.DeribitModels;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace Deribit_Scalper_Trainer
{
    /// <summary>
    /// Interaction logic for ConnectionStatus.xaml
    /// </summary>
    public partial class ConnectionStatus : UserControl
    {
        public ConnectionStatus()
        {
            InitializeComponent();            
        }

        private DeribitClient? client;

        void ClientConnected(object? sender, ClientConnectionEventArgs e)
        {
            if (client != null)
            {
                Debug.WriteLine("Client connected");

                Dispatcher.Invoke(() =>
                {
                    if (e.Connected)
                    {
                        ConnectionStatusEllipse.Fill = new SolidColorBrush(Colors.SpringGreen);
                    }
                    else
                    {
                        ConnectionStatusEllipse.Fill = new SolidColorBrush(Colors.Red);
                    }
                });
            }
        }

        public void RegisterEvents(DeribitClient client)
        {
            this.client = client;
            client.ClientConnected += ClientConnected;
        }
    }
}
