using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{
    public class LegalForm
    {
        [JsonProperty("EntityLegalFormCode")]
        public DataValue EntityLegalFormCode { get; set; }

        [JsonProperty("OtherLegalForm")]
        public DataValue OtherLegalForm { get; set; }
    }
}