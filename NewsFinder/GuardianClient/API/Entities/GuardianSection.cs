// ****************************************************************************
// <copyright file="GuardianSection.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.GuardianClient.API.Entities
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class GuardianSection : GuardianBaseEntity
    {
        [JsonProperty(PropertyName = "editions")]
        public List<GuardianEdition> Editions { get; set; }
    }
}
