using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{

    public class ValidatedAt
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("other")] public string Other { get; set; }
    }
}
