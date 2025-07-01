using System.Collections.Generic;
using CluedIn.Core.Crawling;
using static CluedIn.ExternalSearch.Providers.Gleif.Constants;

namespace CluedIn.ExternalSearch.Providers.Gleif
{
    public class GleifExternalSearchJobData : CrawlJobData
    {
        public GleifExternalSearchJobData(IDictionary<string, object> configuration)
        {
            AcceptedEntityType = GetValue(configuration, KeyName.AcceptedEntityType, default(string));
            LeiVocabularyKey = GetValue(configuration, KeyName.LeiVocabularyKey, default(string));
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object> {
                { KeyName.AcceptedEntityType, AcceptedEntityType },
                { KeyName.LeiVocabularyKey, LeiVocabularyKey },
            };
        }

        public string AcceptedEntityType { get; set; }
        public string LeiVocabularyKey { get; set; }
    }
}
