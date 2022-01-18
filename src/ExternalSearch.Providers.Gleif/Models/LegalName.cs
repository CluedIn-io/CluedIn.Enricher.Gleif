using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{

    public class LegalName
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("language")] public string Language { get; set; }
    }
}
