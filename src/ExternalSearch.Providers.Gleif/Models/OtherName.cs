using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{

    public class OtherName
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("language")] public string Language { get; set; }

        [JsonProperty("type")] public string Type { get; set; }
    }
}
