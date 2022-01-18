using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models;

public class SuccessorEntity
{
    [JsonProperty("lei")]
    public object Lei { get; set; }

    [JsonProperty("name")]
    public object Name { get; set; }
}