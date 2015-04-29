// ****************************************************************************
// <copyright file="GuardianSectionResponse.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.GuardianClient.API
{
    using System.Collections.Generic;
    using GoingOn.GuardianClient.API.Entities;
    using Newtonsoft.Json;

    public class GuardianSectionResponseField : GuardianBaseResponseField
    {
        [JsonProperty(PropertyName = "startIndex")]
        public int StartIndex { get; set; }

        [JsonProperty(PropertyName = "pageSize")]
        public int PageSize { get; set; }

        [JsonProperty(PropertyName = "currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty(PropertyName = "pages")]
        public int Pages { get; set; }

        [JsonProperty(PropertyName = "edition")]
        public GuardianEdition Edition { get; set; }

        [JsonProperty(PropertyName = "section")]
        public GuardianSection Section { get; set; }

        [JsonProperty(PropertyName = "results")]
        public List<GuardianSectionArticle> Results { get; set; }
    }
}
