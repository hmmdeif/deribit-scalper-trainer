using System;
using System.Text.Json.Serialization;

namespace Deribit_Scalper_Trainer.DeribitModels
{
    struct IndexParamsData
    {
        [JsonPropertyName("timestamp")]
        public Int64 Timestamp { get; set; }

        [JsonPropertyName("index_name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
    struct IndexParams
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("data")]
        public IndexParamsData Data { get; set; }
    }
    class IndexModel : BasicReceiveModel
    {
        [JsonPropertyName("params")]
        public IndexParams Params { get; set; }
    }
}
