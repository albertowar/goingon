// ****************************************************************************
// <copyright file="NewsFinderWorkerRole.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace NewsFinderWorkerRole.Entities
{
    using Newtonsoft.Json;

    [JsonObject]
    public class Category
    {
        [JsonProperty(PropertyName = "category_id")]
        public int CategoryId { get; set; }

        [JsonProperty(PropertyName = "display_category_name")]
        public string DisplayCategoryName { get; set; }

        [JsonProperty(PropertyName = "english_category_name")]
        public string EnglishCategoryName { get; set; }

        [JsonProperty(PropertyName = "url_category_name")]
        public string UrlCategoryName { get; set; }
    }
}
