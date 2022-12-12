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

            var client = new RestClient("https://api.gleif.org/api/v1/lei-records");

            var request = new RestRequest("?page[size]=1&page[number]=1&filter[lei]=" + leiCode, Method.GET);

            var response = client.ExecuteAsync(request).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                // HACK: Removes the outer array from json string
                var responseData = response.Content; //.Substring(1, response.Content.Length - 2);

                var data = JsonConvert.DeserializeObject<GleifResponse>(responseData);

                if (data?.Data != null && data.Data.Any())
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

            var code = this.GetOriginEntityCode(resultItem.Data.Data.First()?.Attributes.Lei, request);

            var clue = new Clue(code, context.Organization);

            this.PopulateMetadata(clue.Data.EntityData, resultItem, request);

            return new[] { clue };
        }

        /// <inheritdoc/>
        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<GleifResponse>();
            return this.CreateMetadata(resultItem, request);
        }

        /// <inheritdoc/>
        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            return null;
        }

        /// <summary>Creates the metadata.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The metadata.</returns>
        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<GleifResponse> resultItem, IExternalSearchRequest request)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem, request);

            return metadata;
        }

        /// <summary>Gets the origin entity code.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <param name="lei"></param>
        /// <returns>The origin entity code.</returns>
        private EntityCode GetOriginEntityCode(string lei, IExternalSearchRequest request)
        {
            return new EntityCode(request.EntityMetaData.EntityType, this.GetCodeOrigin(), request.EntityMetaData.OriginEntityCode.Value);
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
        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<GleifResponse> resultItem, IExternalSearchRequest request)
        {
            var data = resultItem.Data.Data.First();

            var code = this.GetOriginEntityCode(data.Attributes.Lei, request);

            metadata.EntityType       = request.EntityMetaData.EntityType;
            metadata.Name = request.EntityMetaData.Name; //data.Attributes.Entity.LegalName?.Name;
            metadata.OriginEntityCode = code;

            metadata.Codes.Add(code);

            if (data.Attributes.Entity.OtherNames != null)
                metadata.Aliases.AddRange(data.Attributes.Entity?.OtherNames.Select(v => v.Name));

            metadata.Properties[GleifVocabularies.Organization.LeiCode]                                     = data.Attributes.Lei;

            // Legal
            metadata.Properties[GleifVocabularies.Organization.LegalName]                                   = data.Attributes.Entity.LegalName?.Name;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.Address]                        = JoinValues(data.Attributes.Entity.LegalAddress?.AddressLines, x => x, ", ");
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.Number]                         = data.Attributes.Entity.LegalAddress?.AddressNumber;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.NumberWithinBuilding]           = data.Attributes.Entity.LegalAddress?.AddressNumberWithinBuilding;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.MailRouting]                    = data.Attributes.Entity.LegalAddress?.MailRouting;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.Region]                         = data.Attributes.Entity.LegalAddress?.Region;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.City]                           = data.Attributes.Entity.LegalAddress?.City;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.CountryCode]                    = data.Attributes.Entity.LegalAddress?.Country;
            metadata.Properties[GleifVocabularies.Organization.LegalAddress.PostalCode]                     = data.Attributes.Entity.LegalAddress?.PostalCode;
            metadata.Properties[GleifVocabularies.Organization.LegalJurisdiction]                           = data.Attributes.Entity.Jurisdiction;
            metadata.Properties[GleifVocabularies.Organization.LegalFormCode]                               = data.Attributes.Entity.LegalForm?.Id;

            // Headquarters
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.Address]                 = JoinValues(data.Attributes.Entity.HeadquartersAddress?.AddressLines, x => x, ", ");
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.Number]                  = data.Attributes.Entity.HeadquartersAddress?.AddressNumber;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.NumberWithinBuilding]    = data.Attributes.Entity.HeadquartersAddress?.AddressNumberWithinBuilding;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.MailRouting]             = data.Attributes.Entity.HeadquartersAddress?.MailRouting;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.Region]                  = data.Attributes.Entity.HeadquartersAddress?.Region;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.City]                    = data.Attributes.Entity.HeadquartersAddress?.City;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.CountryCode]             = data.Attributes.Entity.HeadquartersAddress?.Country;
            metadata.Properties[GleifVocabularies.Organization.HeadquartersAddress.PostalCode]              = data.Attributes.Entity.HeadquartersAddress?.PostalCode;

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

            metadata.Properties[GleifVocabularies.Organization.OtherEntityNames]                            = JoinValues(data.Attributes.Entity?.OtherNames, x => x?.Name);

            // Registration
            //metadata.Properties[GleifVocabularies.Organization.RegistrationAuthorityId]                     = data.Attributes.Registration?.RegistrationAuthorityId?.Value;
            //metadata.Properties[GleifVocabularies.Organization.OtherRegistrationAuthorityId]                = data.Attributes.Registration?.OtherRegistrationAuthorityId?.Value;
            //metadata.Properties[GleifVocabularies.Organization.RegistrationAuthorityEntityId]               = data.Attributes.Registration?.RegistrationAuthorityEntityId?.Value;
            metadata.Properties[GleifVocabularies.Organization.InitialRegistrationDate]                     = data.Attributes.Registration.InitialRegistrationDate;
            metadata.Properties[GleifVocabularies.Organization.LastUpdateDate]                              = data.Attributes.Registration.LastUpdateDate;
            metadata.Properties[GleifVocabularies.Organization.RegistrationStatus]                          = data.Attributes.Registration.Status;
            metadata.Properties[GleifVocabularies.Organization.NextRenewalDate]                             = data.Attributes.Registration.NextRenewalDate;
            metadata.Properties[GleifVocabularies.Organization.ManagingLOU]                                 = data.Attributes.Registration.ManagingLou;

            // Validation
            metadata.Properties[GleifVocabularies.Organization.EntityStatus]                                = data.Attributes.Entity.Status;
            //metadata.Properties[GleifVocabularies.Organization.ValidationSources]                           = data.Attributes.Registration.ValidationSources?.Value;
            //metadata.Properties[GleifVocabularies.Organization.ValidationAuthorityId]                       = data.Attributes.Registration.ValidationAuthority?.ValidationAuthorityId?.Value;
            //metadata.Properties[GleifVocabularies.Organization.OtherValidationAuthorityId]                  = data.Attributes.Registration.ValidationAuthority?.OtherValidationAuthorityId?.Value;
            //metadata.Properties[GleifVocabularies.Organization.ValidationAuthorityEntityId]                 = data.Attributes.Registration.ValidationAuthority?.ValidationAuthorityEntityId?.Value;

            metadata.Properties[GleifVocabularies.Organization.EntityCategory]                              = data.Attributes.Entity.Category;
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
