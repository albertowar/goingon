// ****************************************************************************
// <copyright file="UserMemory.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.DocumentDBRepository.Entities
{
    using System;
    using Newtonsoft.Json;

    public class NewsDocumentDB
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; private set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; private set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; private set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; private set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; private set; }

        [JsonProperty(PropertyName = "rating")]
        public int Rating { get; private set; }
    }
}
