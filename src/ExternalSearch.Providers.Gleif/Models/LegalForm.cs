using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models;

public class LegalForm
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("other")]
    public object Other { get; set; }
}