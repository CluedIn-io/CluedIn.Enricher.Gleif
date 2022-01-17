using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{
    public class Address
    {
        [JsonProperty("FirstAddressLine")]
        public DataValue FirstAddressLine { get; set; }

        [JsonProperty("AddressNumber")]
        public DataValue AddressNumber { get; set; }

        [JsonProperty("AddressNumberWithinBuilding")]
        public DataValue AddressNumberWithinBuilding { get; set; }

        [JsonProperty("MailRouting")]
        public DataValue MailRouting { get; set; }

        [JsonProperty("AdditionalAddressLine")]
        public List<DataValue> AdditionalAddressLine { get; set; }

        [JsonProperty("Region")]
        public DataValue Region { get; set; }

        [JsonProperty("City")]
        public DataValue City { get; set; }

        [JsonProperty("Country")]
        public DataValue Country { get; set; }

        [JsonProperty("PostalCode")]
        public DataValue PostalCode { get; set; }
    }
}