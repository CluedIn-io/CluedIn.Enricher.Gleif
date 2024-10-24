using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.ExternalSearch.Providers.Gleif
{
    public class GleifExternalSearchJobData : CrawlJobData
    {
        public GleifExternalSearchJobData(IDictionary<string, object> configuration)
        {
            AcceptedEntityType = GetValue(configuration, nameof(AcceptedEntityType), default(string));
            LeiVocabularyKey = GetValue(configuration, nameof(LeiVocabularyKey), default(string));
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object> {
                { nameof(AcceptedEntityType), AcceptedEntityType },
                { nameof(LeiVocabularyKey), LeiVocabularyKey },
            };
        }

        public string AcceptedEntityType { get; set; }
        public string LeiVocabularyKey { get; set; }
    }
}
