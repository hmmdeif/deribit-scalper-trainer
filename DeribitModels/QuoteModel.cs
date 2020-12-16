using System;
using System.Text.Json.Serialization;

namespace Deribit_Scalper_Trainer.DeribitModels
{
    struct QuoteParamsData
    {
        [JsonPropertyName("timestamp")]
        public Int64 Timestamp { get; set; }

        [JsonPropertyName("instrument_name")]
        public string Instrument { get; set; }

        [JsonPropertyName("best_bid_price")]
        public decimal BestBidPrice { get; set; }

        [JsonPropertyName("best_ask_price")]
        public decimal BestAskPrice { get; set; }

        [JsonPropertyName("best_bid_amount")]
        public decimal BestBidAmount { get; set; }

        [JsonPropertyName("best_ask_amount")]
        public decimal BestAskAmount { get; set; }
    }
    struct QuoteParams
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("data")]
        public QuoteParamsData Data { get; set; }
    }
    class QuoteModel : BasicReceiveModel
    {
        [JsonPropertyName("params")]
        public QuoteParams Params { get; set; }
    }
}
