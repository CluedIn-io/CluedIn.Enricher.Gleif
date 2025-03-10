using System;
using System.Collections.Generic;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Providers;

namespace CluedIn.ExternalSearch.Providers.Gleif
{
    public static class Constants
    {
        public struct KeyName
        {
            public const string AcceptedEntityType = "acceptedEntityType";
            public const string LeiVocabularyKey = "leiVocabularyKey";
            public const string SkipEntityCodeCreation = "skipEntityCodeCreation";
        }

        public const string ComponentName = "Gleif";
        public const string ProviderName = "Gleif";
        public static readonly Guid ProviderId = Guid.Parse("6d47d335-2bf3-4249-88c4-0f08d322c24c");
        public static readonly string Instruction = $$"""
            [
              {
                "type": "bulleted-list",
                "children": [
                  {
                    "type": "list-item",
                    "children": [
                      {
                        "text": "Add the {{EntityTypeLabel.ToLower()}} to specify the golden records you want to enrich. Only golden records belonging to that {{EntityTypeLabel.ToLower()}} will be enriched."
                      }
                    ]
                  },
                  {
                    "type": "list-item",
                    "children": [
                      {
                        "text": "Add the vocabulary keys to provide the input for the enricher to search for additional information. For example, if you provide the website vocabulary key for the Web enricher, it will use specific websites to look for information about companies. In some cases, vocabulary keys are not required. If you don't add them, the enricher will use default vocabulary keys."
                      }
                    ]
                  }
                ]
              }
            ]
            """;
        public static string About { get; set; } = "Gleif is an enricher which provides information using the Legal Entity Identifier (LEI) of an organization";
        public static string Icon { get; set; } = "Resources.gleif.png";
        public static string Domain { get; set; } = "https://www.gleif.org/en";

        private static Version _cluedInVersion;
        public static Version CluedInVersion => _cluedInVersion ??= typeof(Core.Constants).Assembly.GetName().Version;
        public static string EntityTypeLabel => CluedInVersion < new Version(4, 5, 0) ? "Entity Type" : "Business Domain";
        public static string EntityCodeLabel => CluedInVersion < new Version(4, 5, 0) ? "Entity Code" : "Identifier";
        public static string EntityCodesLabel => CluedInVersion < new Version(4, 5, 0) ? "Entity Codes" : "Identifiers";
        public static AuthMethods AuthMethods { get; set; } = new AuthMethods
        {
            Token = new List<Control>()
            {
                new()
                {
                    DisplayName = $"Accepted {EntityTypeLabel}",
                    Type = "entityTypeSelector",
                    IsRequired = true,
                    Name = KeyName.AcceptedEntityType,
                    Help = $"The {EntityTypeLabel.ToLower()} that defines the golden records you want to enrich (e.g., /Organization)."
                },
                new()
                {
                    DisplayName = "Lei Code Vocabulary Key",
                    Type = "vocabularyKeySelector",
                    IsRequired = false,
                    Name = KeyName.LeiVocabularyKey,
                    Help = "The vocabulary key that contains the LEI codes of companies you want to enrich (e.g., organization.leicodes)."
                },
                new()
                {
                    DisplayName = $"Skip Entity {EntityCodeLabel} Creation (LEI Code)",
                    Type = "checkbox",
                    IsRequired = false,
                    Name =  KeyName.SkipEntityCodeCreation,
                    Help = $"Toggle to control the creation of new entity {EntityCodesLabel.ToLower()} using the LEI code."
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

        public static Guide Guide { get; set; } = new Guide
        {
            Instructions = Instruction
        };
        public static IntegrationType IntegrationType { get; set; } = IntegrationType.Enrichment;
    }
}
