// ****************************************************************************
// <copyright file="GuardianBaseEntity.cs" company="Universidad de Malaga">
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

    public class GuardianBaseEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "webUrl")]
        public string WebUrl { get; set; }

        [JsonProperty(PropertyName = "apiUrl")]
        public string ApiUrl { get; set; }

        [JsonProperty(PropertyName = "webTitle")]
        public string WebTitle { get; set; }
    }
}
