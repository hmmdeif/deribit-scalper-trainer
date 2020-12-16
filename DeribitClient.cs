#nullable enable
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Deribit_Scalper_Trainer.DeribitModels;
using System.Diagnostics;
using System.Text.Json;

namespace Deribit_Scalper_Trainer
{
    public class DeribitClient
    {
        private ClientWebSocket Client { get; set; }
        private int Id { get; set; }
        private readonly Dictionary<int, Tuple<string, Dictionary<string, dynamic?>?>> Requests = new Dictionary<int, Tuple<string, Dictionary<string, dynamic?>?>>();
        private CancellationTokenSource Cts { get; set; }
        private readonly int PingInterval = 20;
        private CancellationTokenSource HeartbeatCts { get; set; }

        public DeribitClient()
        {
            Client = new ClientWebSocket();
            Cts = new CancellationTokenSource();
            HeartbeatCts = new CancellationTokenSource();

            _ = Connect();
        }

        private async Task Connect()
        {
            try
            {
                await Client.ConnectAsync(new Uri("wss://www.deribit.com/ws/api/v2"), Cts.Token);
                Debug.WriteLine("Deribit client connect attempt: {0}", Client.State);
                OnClientConnected(new ClientConnectionEventArgs(true));
                var task = Task.Run(() => StartReceiving(), Cts.Token);
                await SendMessage("public/hello", new Dictionary<string, dynamic?>() {
                    { "client_name", "hmmdeif deribit scalper trainer <deif@pm.me> (https://github.com/hmmdeif/deribit-scalper-trainer)" },
                    { "client_version", "1.0.0" }
                });
                await SendMessage("public/get_instruments", new Dictionary<string, dynamic?>() {
                    { "currency", "BTC" },
                    { "expired", false },
                    { "kind", "future" }
                });
                await task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: {0}", ex);
            } 
            finally
            {
                OnClientConnected(new ClientConnectionEventArgs(false));
                if (Client != null)
                {
                    Client.Dispose();
                }

                await Task.Delay(10000);
                Client = new ClientWebSocket();
                Cts = new CancellationTokenSource();
                _ = Connect();
            }
        }

        private async Task SendMessage(string method, Dictionary<string, dynamic?>? p = null, string? accesstoken = null)
        {
            ++Id;
            Requests.Add(Id, new Tuple<string, Dictionary<string, dynamic?>?>(method, p));
            string message = new BaseSendModel(Id, method, p, accesstoken).Serialize();
            var buff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await Client.SendAsync(buff, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: Cts.Token);
            Debug.WriteLine("Deribit message sent: {0}", message, null);
        }

        public async Task Subscribe(string method, Dictionary<string, List<string>?>? p, string? accesstoken = null)
        {
            ++Id;
            string message = new SubscribeSendModel(Id, method, p, accesstoken).Serialize();
            var buff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await Client.SendAsync(buff, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: Cts.Token);
            Debug.WriteLine("Deribit message sent: {0}", message, null);
        }

        public event EventHandler<ClientConnectionEventArgs>? ClientConnected;
        protected virtual void OnClientConnected(ClientConnectionEventArgs e)
        {
            EventHandler<ClientConnectionEventArgs>? handler = ClientConnected;
            handler?.Invoke(this, e);
        }

        private void DoHeartbeat()
        {
            if (!HeartbeatCts.IsCancellationRequested)
            {
                HeartbeatCts.Cancel();
            }
            
            HeartbeatCts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                var token = HeartbeatCts.Token;
                await Task.Delay((PingInterval * 1000) + 10000);
                if (!token.IsCancellationRequested)
                {
                    Cts.Cancel();
                }
            }, HeartbeatCts.Token);
        }

        private async Task StartHeartbeat()
        {
            await SendMessage("public/set_heartbeat", new Dictionary<string, dynamic?>() { { "interval", PingInterval.ToString() } });
        }

        private async Task RespondToHeartbeat(HeartbeatModel? model)
        {
            if (model != null)
            {
                if (model.Params.Type == "heartbeat")
                {
                    DoHeartbeat();
                } else if (model.Params.Type == "test_request")
                {
                    await SendMessage("public/test");
                }
            }
        }

        public event EventHandler<SubscriptionEventArgs>? ClientSubscriptionMessage;
        protected virtual void OnClientSubscriptionMessage(SubscriptionEventArgs e)
        {
            EventHandler<SubscriptionEventArgs>? handler = ClientSubscriptionMessage;
            handler?.Invoke(this, e);
        }

        public event EventHandler<GetInstrumentsEventArgs>? GetInstrumentsResponseMessage;
        protected virtual void OnGetInstrumentsResponse(GetInstrumentsEventArgs e)
        {
            EventHandler<GetInstrumentsEventArgs>? handler = GetInstrumentsResponseMessage;
            handler?.Invoke(this, e);
        }

        private async Task ProcessGetInstrumentsResponse(GetInstrumentsModel? model)
        {
            var subscriptions = new List<string>() { "deribit_price_index.btc_usd" };
            if (model != null)
            {
                foreach (var result in model.Result)
                {
                    subscriptions.Add("quote." + result.Name);
                }

                OnGetInstrumentsResponse(new GetInstrumentsEventArgs(model.Result));
            }
 
            await Subscribe("public/subscribe", new Dictionary<string, List<string>?>()
            {
                { "channels", subscriptions }
            });
        }

        private async Task StartReceiving()
        {
            while (true)
            {
                var b = new byte[2048];
                var buff = new ArraySegment<byte>(b);
                WebSocketReceiveResult res = await Client.ReceiveAsync(buff, Cts.Token);
                byte[] messageBytes = buff.Skip(buff.Offset).Take(res.Count).ToArray();
                string message = Encoding.UTF8.GetString(messageBytes);
                Debug.WriteLine("Deribit message received: {0}", message, null);

                var basicResult = JsonSerializer.Deserialize<BasicReceiveModel>(message);
                if (basicResult != null)
                {
                    if (basicResult.Id.HasValue && Requests.ContainsKey(basicResult.Id.Value))
                    {
                        DoHeartbeat();
                        var originalRequest = Requests[basicResult.Id.Value];
                        switch (originalRequest.Item1)
                        {
                            case "public/hello":
                                await StartHeartbeat();
                                break;
                            case "public/get_instruments":
                                await ProcessGetInstrumentsResponse(JsonSerializer.Deserialize<GetInstrumentsModel>(message));
                                break;
                        }
                    } else
                    {
                        switch(basicResult.Method)
                        {
                            case "heartbeat":
                                await RespondToHeartbeat(JsonSerializer.Deserialize<HeartbeatModel>(message));
                                break;
                            case "subscription":
                                OnClientSubscriptionMessage(new SubscriptionEventArgs(message));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
