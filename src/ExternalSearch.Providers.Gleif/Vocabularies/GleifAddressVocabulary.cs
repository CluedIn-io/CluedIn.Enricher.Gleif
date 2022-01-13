// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GleifAddressVocabulary.cs" company="Clued In">
//   Copyright (c) 2018 Clued In. All rights reserved.
// </copyright>
// <summary>
//   Implements the gleif address vocabulary class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.Gleif.Vocabularies
{
    public class GleifAddressVocabulary : SimpleVocabulary
    {        
        public GleifAddressVocabulary()
        {
            this.VocabularyName = "Gleif Address";
            this.KeyPrefix      = "gleif.address";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Location;

            this.AddGroup("Gleif Address Details", group => 
            {
                this.Address                = group.Add(new VocabularyKey("address"));
                this.Number                 = group.Add(new VocabularyKey("number"));
                this.NumberWithinBuilding   = group.Add(new VocabularyKey("numberWithinBuilding"));
                this.MailRouting            = group.Add(new VocabularyKey("mailRouting"));
                this.AdditionalAddress      = group.Add(new VocabularyKey("additionalAddress"));
                this.Region                 = group.Add(new VocabularyKey("region"));
                this.City                   = group.Add(new VocabularyKey("city"));
                this.PostalCode             = group.Add(new VocabularyKey("postalCode"));
                this.CountryCode            = group.Add(new VocabularyKey("countryCode"));
            });
        }

        public VocabularyKey Address { get; internal set; }
        public VocabularyKey Number { get; internal set; }
        public VocabularyKey NumberWithinBuilding { get; internal set; }
        public VocabularyKey MailRouting { get; internal set; }
        public VocabularyKey AdditionalAddress { get; internal set; }
        public VocabularyKey Region { get; internal set; }
        public VocabularyKey City { get; internal set; }
        public VocabularyKey PostalCode { get; internal set; }
        public VocabularyKey CountryCode { get; internal set; }
    }
}
