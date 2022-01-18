using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models;

public class Attributes
{
    [JsonProperty("lei")]
    public string Lei { get; set; }

    [JsonProperty("entity")]
    public Entity Entity { get; set; }

    [JsonProperty("registration")]
    public Registration Registration { get; set; }
}