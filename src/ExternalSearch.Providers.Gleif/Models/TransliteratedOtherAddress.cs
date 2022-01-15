using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{
    public class TransliteratedOtherAddress
    {
        [JsonProperty("TransliteratedOtherAddress")]
        public List<Address> Addresses { get; set; }
    }
}