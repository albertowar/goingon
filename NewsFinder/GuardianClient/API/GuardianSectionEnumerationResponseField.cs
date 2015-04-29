// ****************************************************************************
// <copyright file="GuardianSectionListResponse.cs" company="Universidad de Malaga">
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

    public class GuardianSectionEnumerationResponseField : GuardianBaseResponseField
    {
        [JsonProperty(PropertyName = "results")]
        public List<GuardianSection> Results { get; set; }
    }
}
