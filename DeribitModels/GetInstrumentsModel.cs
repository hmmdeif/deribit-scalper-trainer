using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Deribit_Scalper_Trainer.DeribitModels
{
    public class GetInstrumentResultsModel
    {
        [JsonPropertyName("instrument_name")]
        public string Name { get; set; }

        [JsonPropertyName("tick_size")]
        public decimal TickSize { get; set; }

        [JsonPropertyName("contract_size")]
        public decimal ContractSize { get; set; }

        [JsonPropertyName("taker_commission")]
        public decimal TakerFee { get; set; }

        [JsonPropertyName("maker_commission")]
        public decimal MakerFee { get; set; }

        [JsonPropertyName("expiration_timestamp")]
        public long Expiration { get; set; }
    }

    public class GetInstrumentsModel : BaseReceiveModel<List<GetInstrumentResultsModel>>
    {

    }
}
