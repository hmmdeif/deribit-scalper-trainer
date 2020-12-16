#nullable enable
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deribit_Scalper_Trainer.DeribitModels
{
    public class SubscribeSendModel
    {
        public SubscribeSendModel(int? id, string method, Dictionary<string, List<string>?>? p, string? accesstoken)
        {
            Id = id;
            Method = method;
            Params = p;
            AccessToken = accesstoken;
            JsonRPC = "2.0";
        }

        [JsonPropertyName("jsonrpc")]
        private string JsonRPC { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("params")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, List<string>?>?  Params {get; set;}

        [JsonPropertyName("access_token")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AccessToken { get; set; }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
