// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GleifResponse.cs" company="Clued In">
//   Copyright (c) 2018 Clued In. All rights reserved.
// </copyright>
// <summary>
//   Implements the gleif response class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Gleif.Models
{
    public class GleifResponse
    {
        [JsonProperty("LEI")]
        public DataValue Lei { get; set; }

        [JsonProperty("Entity")]
        public Entity Entity { get; set; }

        [JsonProperty("Registration")]
        public Registration Registration { get; set; }
    }
}
