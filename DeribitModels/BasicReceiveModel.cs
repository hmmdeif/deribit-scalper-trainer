using System.Text.Json.Serialization;

namespace Deribit_Scalper_Trainer.DeribitModels
{
    class BasicReceiveModel
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRPC { get; set; }

        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }
    }
}
