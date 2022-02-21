using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.ExternalSearch.Providers.Gleif
{
    public class GleifExternalSearchJobData : CrawlJobData
    {
        public GleifExternalSearchJobData(IDictionary<string, object> configuration)
        {
           
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object> {
                
            };
        }
    }
}
