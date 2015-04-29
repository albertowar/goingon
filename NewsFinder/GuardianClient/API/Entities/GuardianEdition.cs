// ****************************************************************************
// <copyright file="GuardianEdition.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Reprents the edition of the newspaper in an specific location
// </summary>
// ****************************************************************************

namespace GoingOn.GuardianClient.API.Entities
{
    using Newtonsoft.Json;

    public class GuardianEdition : GuardianBaseEntity
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
    }
}
