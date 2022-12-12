// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GleifOrganizationVocabulary.cs" company="Clued In">
//   Copyright (c) 2018 Clued In. All rights reserved.
// </copyright>
// <summary>
//   Implements the gleif organization vocabulary class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.Gleif.Vocabularies
{
    public class GleifOrganizationVocabulary : SimpleVocabulary
    {        
        public GleifOrganizationVocabulary()
        {
            this.VocabularyName = "Gleif Organization";
            this.KeyPrefix      = "gleif.organization";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Organization;

            this.AddGroup("Gleif Organization Details", group => 
            {
                this.LeiCode                                    = group.Add(new VocabularyKey("leiCode"));
                this.LegalName                                  = group.Add(new VocabularyKey("legalName"));
                this.LegalAddress                               = group.Add(new GleifAddressVocabulary().AsCompositeKey("legalAddress"));
                this.LegalJurisdiction                          = group.Add(new VocabularyKey("legalJurisdiction"));
                this.LegalFormCode                              = group.Add(new VocabularyKey("legalFormCode"));
                this.LegalFormType                              = group.Add(new VocabularyKey("legalFormType"));

                this.HeadquartersAddress                        = group.Add(new GleifAddressVocabulary().AsCompositeKey("headquartersAddress"));

                this.TransliteratedOtherEntityNames             = group.Add(new VocabularyKey("transliteratedOtherEntityNames"));
                this.TransliteratedOtherAddresses               = group.Add(new VocabularyKey("transliteratedOtherAddresses"));
                this.TransliteratedOtherAddressNumbers          = group.Add(new VocabularyKey("transliteratedOtherAddressNumbers"));
                this.TransliteratedOtherAdditionalAddresses     = group.Add(new VocabularyKey("transliteratedOtherAdditionalAddresses"));
                this.TransliteratedOtherAddressCities           = group.Add(new VocabularyKey("transliteratedOtherAddressCities"));
                this.TransliteratedOtherAddressRegions          = group.Add(new VocabularyKey("transliteratedOtherAddressRegions"));
                this.TransliteratedOtherAddressPostalCodes      = group.Add(new VocabularyKey("transliteratedOtherAddressPostalCode"));
                this.TransliteratedOtherAddressCountryCodes     = group.Add(new VocabularyKey("transliteratedOtherAddressCountryCode"));

                this.OtherEntityNames                           = group.Add(new VocabularyKey("otherEntityNames"));
                this.OtherAddresses                             = group.Add(new VocabularyKey("otherAddresses"));
                this.OtherAddressNumbers                        = group.Add(new VocabularyKey("otherAddressNumbers"));
                this.OtherAdditionalAddresses                   = group.Add(new VocabularyKey("otherAdditionalAddresses"));
                this.OtherAddressCities                         = group.Add(new VocabularyKey("otherAddressCities"));
                this.OtherAddressRegions                        = group.Add(new VocabularyKey("otherAddressRegions"));
                this.OtherAddressPostalCodes                    = group.Add(new VocabularyKey("otherAddressPostalCode"));
                this.OtherAddressCountryCodes                   = group.Add(new VocabularyKey("otherAddressCountryCode"));

                this.RegistrationAuthorityId                    = group.Add(new VocabularyKey("registrationAuthorityId"));
                this.OtherRegistrationAuthorityId               = group.Add(new VocabularyKey("otherRegistrationAuthorityId"));
                this.RegistrationAuthorityEntityId              = group.Add(new VocabularyKey("registrationAuthorityEntityId"));
                this.EntityStatus                               = group.Add(new VocabularyKey("entityStatus"));
                this.InitialRegistrationDate                    = group.Add(new VocabularyKey("initialRegistrationDate"));
                this.LastUpdateDate                             = group.Add(new VocabularyKey("lastUpdateDate"));
                this.RegistrationStatus                         = group.Add(new VocabularyKey("registrationStatus"));
                this.NextRenewalDate                            = group.Add(new VocabularyKey("nextRenewalDate"));
                this.ManagingLOU                                = group.Add(new VocabularyKey("managingLOU"));
                this.ValidationSources                          = group.Add(new VocabularyKey("validationSources"));
                this.ValidationAuthorityId                      = group.Add(new VocabularyKey("validationAuthorityId"));
                this.OtherValidationAuthorityId                 = group.Add(new VocabularyKey("ptherValidationAuthorityId"));
                this.ValidationAuthorityEntityId                = group.Add(new VocabularyKey("validationAuthorityEntityId"));
                this.EntityCategory                             = group.Add(new VocabularyKey("entityCategory"));
            });

        }

        public VocabularyKey LeiCode { get; internal set; }
        public VocabularyKey LegalName { get; internal set; }
        public GleifAddressVocabulary LegalAddress { get; internal set; }
        public VocabularyKey LegalJurisdiction { get; internal set; }
        public VocabularyKey LegalFormCode { get; internal set; }
        public VocabularyKey LegalFormType { get; internal set; }

        public GleifAddressVocabulary HeadquartersAddress { get; internal set; }

        public VocabularyKey TransliteratedOtherEntityNames { get; internal set; }
        public VocabularyKey TransliteratedOtherAddresses { get; internal set; }
        public VocabularyKey TransliteratedOtherAddressNumbers { get; internal set; }
        public VocabularyKey TransliteratedOtherAdditionalAddresses { get; internal set; }
        public VocabularyKey TransliteratedOtherAddressCities { get; internal set; }
        public VocabularyKey TransliteratedOtherAddressRegions { get; internal set; }
        public VocabularyKey TransliteratedOtherAddressPostalCodes { get; internal set; }
        public VocabularyKey TransliteratedOtherAddressCountryCodes { get; internal set; }

        public VocabularyKey OtherEntityNames { get; internal set; }
        public VocabularyKey OtherAddresses { get; internal set; }
        public VocabularyKey OtherAddressNumbers { get; internal set; }
        public VocabularyKey OtherAdditionalAddresses { get; internal set; }
        public VocabularyKey OtherAddressCities { get; internal set; }
        public VocabularyKey OtherAddressRegions { get; internal set; }
        public VocabularyKey OtherAddressPostalCodes { get; internal set; }
        public VocabularyKey OtherAddressCountryCodes { get; internal set; }

        public VocabularyKey RegistrationAuthorityId { get; internal set; }
        public VocabularyKey OtherRegistrationAuthorityId { get; internal set; }
        public VocabularyKey RegistrationAuthorityEntityId { get; internal set; }
        public VocabularyKey EntityStatus { get; internal set; }
        public VocabularyKey InitialRegistrationDate { get; internal set; }
        public VocabularyKey LastUpdateDate { get; internal set; }
        public VocabularyKey RegistrationStatus { get; internal set; }
        public VocabularyKey NextRenewalDate { get; internal set; }
        public VocabularyKey ManagingLOU { get; internal set; }
        public VocabularyKey ValidationSources { get; internal set; }
        public VocabularyKey ValidationAuthorityId { get; internal set; }
        public VocabularyKey OtherValidationAuthorityId { get; internal set; }
        public VocabularyKey ValidationAuthorityEntityId { get; internal set; }

        public VocabularyKey EntityCategory { get; internal set; }
    }
}
