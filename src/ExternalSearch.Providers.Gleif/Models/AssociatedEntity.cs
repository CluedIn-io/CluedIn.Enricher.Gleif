using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{

    public class AssociatedEntity
    {
        [JsonProperty("lei")] public string Lei { get; set; }

        [JsonProperty("name")] public string Name { get; set; }
    }
}
