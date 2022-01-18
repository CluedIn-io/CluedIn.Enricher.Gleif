// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GleifResponse.cs" company="Clued In">
//   Copyright (c) 2018 Clued In. All rights reserved.
// </copyright>
// <summary>
//   Implements the gleif response class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{
    //public class GleifResponse
    //{
    //    [JsonProperty("LEI")]
    //    public DataValue Lei { get; set; }

    //    [JsonProperty("Entity")]
    //    public Entity Entity { get; set; }

    //    [JsonProperty("Registration")]
    //    public Registration Registration { get; set; }
    //}

    public class GleifResponse
    {
        [JsonProperty("meta")]
        public Metadata Meta { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }

        [JsonProperty("data")]
        public List<Data> Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("attributes")]
        public Attributes Attributes { get; set; }
    }

    public class Attributes
    {
        [JsonProperty("lei")]
        public string Lei { get; set; }

        [JsonProperty("entity")]
        public Entity Entity { get; set; }

        [JsonProperty("registration")]
        public Registration Registration { get; set; }
    }

    public class ValidatedAt
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("other")]
        public object Other { get; set; }
    }

    public class Registration
    {
        [JsonProperty("initialRegistrationDate")]
        public string InitialRegistrationDate { get; set; }

        [JsonProperty("lastUpdateDate")]
        public string LastUpdateDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("nextRenewalDate")]
        public string NextRenewalDate { get; set; }

        [JsonProperty("managingLou")]
        public string ManagingLou { get; set; }

        [JsonProperty("corroborationLevel")]
        public string CorroborationLevel { get; set; }

        [JsonProperty("validatedAt")]
        public ValidatedAt ValidatedAt { get; set; }

        [JsonProperty("validatedAs")]
        public string ValidatedAs { get; set; }

        [JsonProperty("otherValidationAuthorities")]
        public List<object> OtherValidationAuthorities { get; set; }
    }

    public class LegalName
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
    }

    public class OtherName
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class Address
    {
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("addressLines")]
        public List<string> AddressLines { get; set; }

        [JsonProperty("addressNumber")]
        public string AddressNumber { get; set; }

        [JsonProperty("addressNumberWithinBuilding")]
        public string AddressNumberWithinBuilding { get; set; }

        [JsonProperty("mailRouting")]
        public string MailRouting { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
    }


    public class RegisteredAt
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("other")]
        public object Other { get; set; }
    }

    public class LegalForm
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("other")]
        public object Other { get; set; }
    }

    public class AssociatedEntity
    {
        [JsonProperty("lei")]
        public object Lei { get; set; }

        [JsonProperty("name")]
        public object Name { get; set; }
    }

    public class Expiration
    {
        [JsonProperty("date")]
        public object Date { get; set; }

        [JsonProperty("reason")]
        public object Reason { get; set; }
    }

    public class SuccessorEntity
    {
        [JsonProperty("lei")]
        public object Lei { get; set; }

        [JsonProperty("name")]
        public object Name { get; set; }
    }

    public class Entity
    {
        [JsonProperty("legalName")]
        public LegalName LegalName { get; set; }

        [JsonProperty("otherNames")]
        public List<OtherName> OtherNames { get; set; }

        [JsonProperty("transliteratedOtherNames")]
        public List<object> TransliteratedOtherNames { get; set; }

        [JsonProperty("legalAddress")]
        public Address LegalAddress { get; set; }

        [JsonProperty("headquartersAddress")]
        public Address HeadquartersAddress { get; set; }

        [JsonProperty("registeredAt")]
        public RegisteredAt RegisteredAt { get; set; }

        [JsonProperty("registeredAs")]
        public string RegisteredAs { get; set; }

        [JsonProperty("jurisdiction")]
        public string Jurisdiction { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("legalForm")]
        public LegalForm LegalForm { get; set; }

        [JsonProperty("associatedEntity")]
        public AssociatedEntity AssociatedEntity { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("expiration")]
        public Expiration Expiration { get; set; }

        [JsonProperty("successorEntity")]
        public SuccessorEntity SuccessorEntity { get; set; }

        [JsonProperty("otherAddresses")]
        public List<object> OtherAddresses { get; set; }
    }


    public class Links
    {
        [JsonProperty("first")]
        public string First { get; set; }

        [JsonProperty("last")]
        public string Last { get; set; }
    }

    public class Metadata
    {
    }
}
