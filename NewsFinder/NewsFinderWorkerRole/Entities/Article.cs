// ****************************************************************************
// <copyright file="Article.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace NewsFinderWorkerRole.Entities
{
    using System;
    using Newtonsoft.Json;

    public class Article
    {
        [JsonProperty(PropertyName = "publish_date")]
        public DateTime PublishDate { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "source_url")]
        public string SourceUrl { get; set; }

        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}
