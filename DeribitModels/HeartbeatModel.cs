using System.Text.Json.Serialization;

namespace Deribit_Scalper_Trainer.DeribitModels
{
    struct HeartbeatParams
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
    class HeartbeatModel : BasicReceiveModel
    {
        [JsonPropertyName("params")]
        public HeartbeatParams Params { get; set; }
    }
}
