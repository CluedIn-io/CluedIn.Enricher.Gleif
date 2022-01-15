// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GleifExternalSearchProvider.cs" company="Clued In">
//   Copyright (c) 2018 Clued In. All rights reserved.
// </copyright>
// <summary>
//   Implements the gleif external search provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.ExternalSearch;
using CluedIn.Core.Providers;
using CluedIn.ExternalSearch.Providers.Gleif.Models;
using CluedIn.ExternalSearch.Providers.Gleif.Vocabularies;
using EasyNetQ.Events;
using RestSharp;
using Newtonsoft.Json;
using EntityType = CluedIn.Core.Data.EntityType;

namespace CluedIn.ExternalSearch.Providers.Gleif
{
    /// <summary>The gleif graph external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    public class GleifExternalSearchProvider : ExternalSearchProviderBase, IExtendedEnricherMetadata, IConfigurableExternalSearchProvider
    {
        private static readonly EntityType[] AcceptedEntityTypes = { EntityType.Organization };

        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/

        public GleifExternalSearchProvider()
            : base(Constants.ProviderId, AcceptedEntityTypes)
        {
        }

        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        /// <inheritdoc/>
        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request)
        {
            if (!this.Accepts(request.EntityMetaData.EntityType))
                yield break;

            var entityType       = request.EntityMetaData.EntityType;
            var leiCodes         = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.CodesLeiCode, new HashSet<string>());

            if (leiCodes != null && leiCodes.Any())
            {
                var validLEICodes = leiCodes.Where(LEICode.IsValidCode);

                foreach (var value in validLEICodes)
                    yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Identifier, value);
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query)
        {
            var leiCode = query.QueryParameters[ExternalSearchQueryParameter.Identifier].FirstOrDefault();

            if (string.IsNullOrEmpty(leiCode))
                yield break;

            var client = new RestClient("https://leilookup.gleif.org/api/v2/leirecords");

            var request = new RestRequest("?lei=" + leiCode, Method.GET);

            var response = client.ExecuteAsync(request).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                // HACK: Removes the outer array from json string
                var responseData = response.Content.Substring(1, response.Content.Length - 2);

                var data = JsonConvert.DeserializeObject<GleifResponse>(responseData);

                if (data != null)
                    yield return new ExternalSearchQueryResult<GleifResponse>(query, data);
            }
            else if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
                yield break;
            else if (response.ErrorException != null)
                throw new AggregateException(response.ErrorException.Message, response.ErrorException);
            else
                throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode + "; Content: " + response.Content);
        }

        /// <inheritdoc/>
        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<GleifResponse>();

            var code = this.GetOriginEntityCode(resultItem);

            var clue = new Clue(code, context.Organization);

            this.PopulateMetadata(clue.Data.EntityData, resultItem);

            return new[] { clue };
        }

        /// <inheritdoc/>
        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<GleifResponse>();
            return this.CreateMetadata(resultItem);
        }

        /// <inheritdoc/>
        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            return null;
        }

        /// <summary>Creates the metadata.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The metadata.</returns>
        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<GleifResponse> resultItem)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem);

            return metadata;
        }

        /// <summary>Gets the origin entity code.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The origin entity code.</returns>
        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<GleifResponse> resultItem)
        {
            return new EntityCode(EntityType.Organization, this.GetCodeOrigin(), resultItem.Data.Lei.Value);
        }

        /// <summary>Gets the code origin.</summary>
        /// <returns>The code origin</returns>
        private CodeOrigin GetCodeOrigin()
        {
            return CodeOrigin.CluedIn.CreateSpecific("gleif");
        }

        /// <summary>Populates the metadata.</summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="resultItem">The result item.</param>
        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<GleifResponse> resultItem)
        {
            var code = this.GetOriginEntityCode(resultItem);
            var data = resultItem.Data;

            metadata.EntityType       = EntityType.Organization;
            metadata.Name             = data.Entity.LegalName?.Value;
            metadata.OriginEntityCode = code;

            metadata.Codes.Add(code);

            if (data.Entity.OtherEntityNames?.Names != null)
                metadata.Aliases.AddRange(data.Entity.OtherEntityNames?.Names.Select(v => v.Value));

            metadata.Properties[GleifVocabularies.Organization.LeiCode]                                     = data.Lei.Value;

            // Legal
            metadata.Properties[GleifVocabularies.Organization.LegalName]                                   = data.Entity.LegalName?.Value;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.Address]                        = data.Entity.LegalAddress?.FirstAddressLine?.Value;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.Number]                         = data.Entity.LegalAddress?.AddressNumber?.Value;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.NumberWithinBuilding]           = data.Entity.LegalAddress?.AddressNumberWithinBuilding?.Value;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.MailRouting]                    = data.Entity.LegalAddress?.MailRouting?.Value;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.AdditionalAddress]              = JoinValues(data.Entity.LegalAddress?.AdditionalAddressLine, x => x?.Value);
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.Region]                         = data.Entity.LegalAddress?.Region?.Value;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.City]                           = data.Entity.LegalAddress?.City?.Value;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.CountryCode]                    = data.Entity.LegalAddress?.Country?.Value;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.PostalCode]                     = data.Entity.LegalAddress?.PostalCode?.Value;
            metadata.Properties[GleifVocabularies.Organization.LegalJurisdiction]                           = data.Entity.LegalJurisdiction?.Value;
            metadata.Properties[GleifVocabularies.Organization.LegalFormCode]                               = data.Entity.LegalForm?.EntityLegalFormCode?.Value;
            metadata.Properties[GleifVocabularies.Organization.LegalFormType]                               = data.Entity.LegalForm?.OtherLegalForm?.Value;

            // Headquarters
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.Address]                 = data.Entity.HeadquartersAddress?.FirstAddressLine?.Value;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.Number]                  = data.Entity.HeadquartersAddress?.AddressNumber?.Value;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.NumberWithinBuilding]    = data.Entity.HeadquartersAddress?.AddressNumberWithinBuilding?.Value;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.MailRouting]             = data.Entity.HeadquartersAddress?.MailRouting?.Value;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.AdditionalAddress]       = JoinValues(data.Entity.HeadquartersAddress?.AdditionalAddressLine, x => x?.Value);
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.Region]                  = data.Entity.HeadquartersAddress?.Region?.Value;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.City]                    = data.Entity.HeadquartersAddress?.City?.Value;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.CountryCode]             = data.Entity.HeadquartersAddress?.Country?.Value;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.PostalCode]              = data.Entity.HeadquartersAddress?.PostalCode?.Value;

            // TODO: Fix this

            //// Transliterated
            //metadata.Properties[GleifVocabularies.Organization.TransliteratedOtherEntityNames]           = JoinValues(data.Entity.TransliteratedOtherEntityNames?.Names, x => x.Value);
            //metadata.Properties[GleifVocabularies.Organization.TransliteratedOtherAddresses]             = JoinValues(data.Entity.TransliteratedOtherAddresses?.Addresses, x => x?.FirstAddressLine?.Value);
            //metadata.Properties[GleifVocabularies.Organization.TransliteratedOtherAddressNumbers]        = JoinValues(data.Entity.TransliteratedOtherAddresses?.Addresses, x => x?.AddressNumber?.Value);
            //metadata.Properties[GleifVocabularies.Organization.TransliteratedOtherAdditionalAddresses]   = JoinValues(data.Entity.TransliteratedOtherAddresses?.Addresses, x => JoinValues(x.AdditionalAddressLine, y => y.Value, separator: ","));
            //metadata.Properties[GleifVocabularies.Organization.TransliteratedOtherAddressRegions]        = JoinValues(data.Entity.TransliteratedOtherAddresses?.Addresses, x => x?.Region?.Value);
            //metadata.Properties[GleifVocabularies.Organization.TransliteratedOtherAddressCities]         = JoinValues(data.Entity.TransliteratedOtherAddresses?.Addresses, x => x?.City?.Value);
            //metadata.Properties[GleifVocabularies.Organization.TransliteratedOtherAddressCountryCodes]   = JoinValues(data.Entity.TransliteratedOtherAddresses?.Addresses, x => x?.Country.Value);
            //metadata.Properties[GleifVocabularies.Organization.TransliteratedOtherAddressPostalCodes]    = JoinValues(data.Entity.TransliteratedOtherAddresses?.Addresses, x => x?.PostalCode?.Value);

            //// Other Addresses
            //metadata.Properties[GleifVocabularies.Organization.OtherAddresses]                           = JoinValues(data.Entity.OtherAddresses?.Addresses, x => x?.FirstAddressLine?.Value);
            //metadata.Properties[GleifVocabularies.Organization.OtherAddressNumbers]                      = JoinValues(data.Entity.OtherAddresses?.Addresses, x => x?.AddressNumber?.Value);
            //metadata.Properties[GleifVocabularies.Organization.OtherAdditionalAddresses]                 = JoinValues(data.Entity.OtherAddresses?.Addresses, x => JoinValues(x?.AdditionalAddressLine, y => y?.Value, separator: ","));
            //metadata.Properties[GleifVocabularies.Organization.OtherAddressRegions]                      = JoinValues(data.Entity.OtherAddresses?.Addresses, x => x?.Region?.Value);
            //metadata.Properties[GleifVocabularies.Organization.OtherAddressCities]                       = JoinValues(data.Entity.OtherAddresses?.Addresses, x => x?.City?.Value);
            //metadata.Properties[GleifVocabularies.Organization.OtherAddressCountryCodes]                 = JoinValues(data.Entity.OtherAddresses?.Addresses, x => x?.Country.Value);
            //metadata.Properties[GleifVocabularies.Organization.OtherAddressPostalCodes]                  = JoinValues(data.Entity.OtherAddresses?.Addresses, x => x?.PostalCode?.Value);

            metadata.Properties[GleifVocabularies.Organization.OtherEntityNames]                            = JoinValues(data.Entity.OtherEntityNames?.Names, x => x?.Value);

            // Registration
            metadata.Properties[GleifVocabularies.Organization.RegistrationAuthorityId]                     = data.Entity.RegistrationAuthority?.RegistrationAuthorityId?.Value;
            metadata.Properties[GleifVocabularies.Organization.OtherRegistrationAuthorityId]                = data.Entity.RegistrationAuthority?.OtherRegistrationAuthorityId?.Value;
            metadata.Properties[GleifVocabularies.Organization.RegistrationAuthorityEntityId]               = data.Entity.RegistrationAuthority?.RegistrationAuthorityEntityId?.Value;
            metadata.Properties[GleifVocabularies.Organization.InitialRegistrationDate]                     = data.Registration.InitialRegistrationDate?.Value;
            metadata.Properties[GleifVocabularies.Organization.LastUpdateDate]                              = data.Registration.LastUpdateDate?.Value;
            metadata.Properties[GleifVocabularies.Organization.RegistrationStatus]                          = data.Registration.RegistrationStatus?.Value;
            metadata.Properties[GleifVocabularies.Organization.NextRenewalDate]                             = data.Registration.NextRenewalDate?.Value;
            metadata.Properties[GleifVocabularies.Organization.ManagingLOU]                                 = data.Registration.ManagingLou?.Value;

            // Validation
            metadata.Properties[GleifVocabularies.Organization.EntityStatus]                                = data.Entity.EntityStatus?.Value;
            metadata.Properties[GleifVocabularies.Organization.ValidationSources]                           = data.Registration.ValidationSources?.Value;
            metadata.Properties[GleifVocabularies.Organization.ValidationAuthorityId]                       = data.Registration.ValidationAuthority?.ValidationAuthorityId?.Value;
            metadata.Properties[GleifVocabularies.Organization.OtherValidationAuthorityId]                  = data.Registration.ValidationAuthority?.OtherValidationAuthorityId?.Value;
            metadata.Properties[GleifVocabularies.Organization.ValidationAuthorityEntityId]                 = data.Registration.ValidationAuthority?.ValidationAuthorityEntityId?.Value;

            metadata.Properties[GleifVocabularies.Organization.EntityCategory]                              = data.Entity.EntityCategory?.Value;
        }

        /// <summary>
        /// Joins the properties of a list into a string (comma separated)
        /// </summary>
        /// <typeparam name="T">The object</typeparam>
        /// <param name="items">The list of objects to be joined</param>
        /// <param name="property">The property that should be joined</param>
        /// <returns>A comma separated string containing the properties</returns>
        private static string JoinValues<T>(List<T> items, Func<T, string> property, string separator = ";")
        {
            if (items != null && items.Any())
            {
                return string.Join(separator, items.Where(x => !string.IsNullOrEmpty(property(x))).ToList().ConvertAll(x => property(x)));
            }

            return null;
        }

        public IEnumerable<EntityType> Accepts(IDictionary<string, object> config, IProvider provider)
        {
            return AcceptedEntityTypes;
        }

        public IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return BuildQueries(context, request);
        }

        public IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query, IDictionary<string, object> config, IProvider provider)
        {
            return ExecuteSearch(context, query);
        }

        public IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return BuildClues(context, query, result, request);
        }

        public IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return GetPrimaryEntityMetadata(context, result, request);
        }

        public IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return GetPrimaryEntityPreviewImage(context, result, request);
        }

        public string Icon { get; } = Constants.Icon;
        public string Domain { get; } = Constants.Domain;
        public string About { get; } = Constants.About;
        public AuthMethods AuthMethods { get; } = Constants.AuthMethods;
        public IEnumerable<Control> Properties { get; } = Constants.Properties;
        public Guide Guide { get; } = Constants.Guide;
        public IntegrationType Type { get; } = Constants.IntegrationType;
    }
}
