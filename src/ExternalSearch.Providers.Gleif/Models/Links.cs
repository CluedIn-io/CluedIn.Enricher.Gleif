using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models;

public class Links
{
    [JsonProperty("first")]
    public string First { get; set; }

    [JsonProperty("last")]
    public string Last { get; set; }
}