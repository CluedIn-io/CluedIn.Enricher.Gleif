using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models;

public class Registration
{
    [JsonProperty("initialRegistrationDate")]
    public string InitialRegistrationDate { get; set; }

    [JsonProperty("lastUpdateDate")]
    public string LastUpdateDate { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("nextRenewalDate")]
    public string NextRenewalDate { get; set; }

    [JsonProperty("managingLou")]
    public string ManagingLou { get; set; }

    [JsonProperty("corroborationLevel")]
    public string CorroborationLevel { get; set; }

    [JsonProperty("validatedAt")]
    public ValidatedAt ValidatedAt { get; set; }

    [JsonProperty("validatedAs")]
    public string ValidatedAs { get; set; }

    [JsonProperty("otherValidationAuthorities")]
    public List<object> OtherValidationAuthorities { get; set; }
}