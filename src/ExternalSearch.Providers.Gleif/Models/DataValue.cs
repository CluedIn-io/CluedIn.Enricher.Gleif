using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{
    public class DataValue
    {
        [JsonProperty("$")]
        public string Value { get; set; }
    }
}