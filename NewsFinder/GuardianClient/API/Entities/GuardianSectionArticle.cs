// ****************************************************************************
// <copyright file="GuardianSectionArticle.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.GuardianClient.API.Entities
{
    using Newtonsoft.Json;

    public class GuardianSectionArticle : GuardianBaseEntity
    {
        [JsonProperty(PropertyName = "sectionId")]
        public string SectionId { get; set; }

        [JsonProperty(PropertyName = "webPublicationDate")]
        public string WebPublicationDate { get; set; }

        [JsonProperty(PropertyName = "sectionName")]
        public string SectionName { get; set; }
    }
}
