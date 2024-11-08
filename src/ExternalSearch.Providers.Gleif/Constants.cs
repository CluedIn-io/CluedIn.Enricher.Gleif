using System;
using System.Collections.Generic;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Providers;

namespace CluedIn.ExternalSearch.Providers.Gleif
{
    public static class Constants
    {
        public const string ComponentName = "Gleif";
        public const string ProviderName = "Gleif";
        public static readonly Guid ProviderId = Guid.Parse("6d47d335-2bf3-4249-88c4-0f08d322c24c");

        public static string About { get; set; } = "Gleif is an enricher which provides information using the Legal Entity Identifier (LEI) of an organization";
        public static string Icon { get; set; } = "Resources.gleif.png";
        public static string Domain { get; set; } = "https://www.gleif.org/en";

        public static AuthMethods AuthMethods { get; set; } = new AuthMethods
        {
            token = new List<Control>()
            {
                new()
                {
                    displayName = "Accepted Entity Type",
                    type = "input",
                    isRequired = true,
                    name = nameof(GleifExternalSearchJobData.AcceptedEntityType)
                },
                new()
                {
                    displayName = "Lei Code Vocabulary Key",
                    type = "input",
                    isRequired = false,
                    name = nameof(GleifExternalSearchJobData.LeiVocabularyKey)
                },
                new()
                {
                    displayName = "Skip Entity Code Creation (Lei Code)",
                    type = "checkbox",
                    isRequired = false,
                    name =  nameof(GleifExternalSearchJobData.SkipEntityCodeCreation),
                }
            }
        };

        public static IEnumerable<Control> Properties { get; set; } = new List<Control>()
        {
            // NOTE: Leaving this commented as an example - BF
            //new()
            //{
            //    displayName = "Some Data",
            //    type = "input",
            //    isRequired = true,
            //    name = "someData"
            //}
        };

        public static Guide Guide { get; set; } = null;
        public static IntegrationType IntegrationType { get; set; } = IntegrationType.Enrichment;
    }
}
