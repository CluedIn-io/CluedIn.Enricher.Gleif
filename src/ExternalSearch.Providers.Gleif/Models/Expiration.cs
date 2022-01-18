using System;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{

    public class Expiration
    {
        [JsonProperty("date")] public DateTimeOffset Date { get; set; }

        [JsonProperty("reason")] public string Reason { get; set; }
    }
}
