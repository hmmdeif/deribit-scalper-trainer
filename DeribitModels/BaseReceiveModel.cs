using System.Text.Json.Serialization;

namespace Deribit_Scalper_Trainer.DeribitModels
{
    public abstract class BaseReceiveModel<T>
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRPC { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("result")]
        public T Result { get; set; }

        [JsonPropertyName("usIn")]
        public long UsIn { get; set; }

        [JsonPropertyName("usOut")]
        public long UsOut { get; set; }

        [JsonPropertyName("usDiff")]
        public long UsDiff { get; set; }

        [JsonPropertyName("testnet")]
        public bool IsTestnet { get; set; }
    }
}
