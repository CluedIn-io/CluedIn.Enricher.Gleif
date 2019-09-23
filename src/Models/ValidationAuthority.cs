using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{
    public class ValidationAuthority
    {
        [JsonProperty("ValidationAuthorityID")]
        public DataValue ValidationAuthorityId { get; set; }

        [JsonProperty("OtherValidationAuthorityID")]
        public DataValue OtherValidationAuthorityId { get; set; }

        [JsonProperty("ValidationAuthorityEntityID")]
        public DataValue ValidationAuthorityEntityId { get; set; }
    }
}