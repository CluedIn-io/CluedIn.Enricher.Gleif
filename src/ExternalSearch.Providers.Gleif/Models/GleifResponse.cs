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
    public class GleifResponse
    {
        [JsonProperty("meta")]
        public Metadata Meta { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }

        [JsonProperty("data")]
        public List<Data> Data { get; set; }
    }
}
