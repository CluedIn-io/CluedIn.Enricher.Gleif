using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models;

public class Address
{
    [JsonProperty("language")]
    public string Language { get; set; }

    [JsonProperty("addressLines")]
    public List<string> AddressLines { get; set; }

    [JsonProperty("addressNumber")]
    public string AddressNumber { get; set; }

    [JsonProperty("addressNumberWithinBuilding")]
    public string AddressNumberWithinBuilding { get; set; }

    [JsonProperty("mailRouting")]
    public string MailRouting { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("region")]
    public string Region { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }

    [JsonProperty("postalCode")]
    public string PostalCode { get; set; }
}