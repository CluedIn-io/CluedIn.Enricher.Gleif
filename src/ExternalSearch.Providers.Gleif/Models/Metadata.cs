using System;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{

    public class Metadata
    {
        [JsonProperty("goldenCopy")]
        public GoldenCopy GoldenCopy { get; set; }

        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
    }

    public class GoldenCopy
    {
        [JsonProperty("publishDate")]
        public DateTimeOffset? PublishDate { get; set; }
    }

    public class Pagination
    {
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("perPage")]
        public int PerPage { get; set; }

        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("to")]
        public int To { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("lastPage")]
        public int LastPage { get; set; }
    }
}
