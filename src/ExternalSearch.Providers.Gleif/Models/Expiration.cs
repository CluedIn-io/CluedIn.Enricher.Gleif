using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models;

public class Expiration
{
    [JsonProperty("date")]
    public object Date { get; set; }

    [JsonProperty("reason")]
    public object Reason { get; set; }
}