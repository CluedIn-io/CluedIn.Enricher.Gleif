using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.ExternalSearch.Providers.Gleif
{
    public class GleifExternalSearchJobData : CrawlJobData
    {
        public GleifExternalSearchJobData(IDictionary<string, object> configuration)
        {
            AcceptedEntityType = GetValue<string>(configuration, Constants.KeyName.AcceptedEntityType);
            LeiCodeKey = GetValue<string>(configuration, Constants.KeyName.LeiCodeKey);
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object> {
                { Constants.KeyName.AcceptedEntityType, AcceptedEntityType },
                { Constants.KeyName.LeiCodeKey, LeiCodeKey }
            };
        }
        public string AcceptedEntityType { get; set; }
        public string LeiCodeKey { get; set; }

    }
}
