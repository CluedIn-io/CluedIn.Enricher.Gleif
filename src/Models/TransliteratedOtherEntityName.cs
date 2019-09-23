using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{
    public class TransliteratedOtherEntityName
    {
        [JsonProperty("TransliteratedOtherEntityName")]
        public List<DataValue> Names { get; set; }
    }
}