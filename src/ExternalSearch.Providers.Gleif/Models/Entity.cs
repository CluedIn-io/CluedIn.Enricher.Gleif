using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{

    public class Entity
    {
        [JsonProperty("legalName")] public LegalName LegalName { get; set; }

        [JsonProperty("otherNames")] public List<OtherName> OtherNames { get; set; }

        [JsonProperty("transliteratedOtherNames")]
        public List<object> TransliteratedOtherNames { get; set; }

        [JsonProperty("legalAddress")] public Address LegalAddress { get; set; }

        [JsonProperty("headquartersAddress")] public Address HeadquartersAddress { get; set; }

        [JsonProperty("registeredAt")] public RegisteredAt RegisteredAt { get; set; }

        [JsonProperty("registeredAs")] public string RegisteredAs { get; set; }

        [JsonProperty("jurisdiction")] public string Jurisdiction { get; set; }

        [JsonProperty("category")] public string Category { get; set; }

        [JsonProperty("legalForm")] public LegalForm LegalForm { get; set; }

        [JsonProperty("associatedEntity")] public AssociatedEntity AssociatedEntity { get; set; }

        [JsonProperty("status")] public string Status { get; set; }

        [JsonProperty("expiration")] public Expiration Expiration { get; set; }

        [JsonProperty("successorEntity")] public SuccessorEntity SuccessorEntity { get; set; }

        [JsonProperty("otherAddresses")] public List<object> OtherAddresses { get; set; }
    }
}
