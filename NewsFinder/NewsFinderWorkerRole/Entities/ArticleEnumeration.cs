// ****************************************************************************
// <copyright file="Articles.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace NewsFinderWorkerRole.Entities
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ArticleEnumeration
    {
        [JsonProperty(PropertyName = "articles")]
        public List<Article> Articles { get; set; } 
    }
}
