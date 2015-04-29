// ****************************************************************************
// <copyright file="GuardianSection.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.GuardianClient.API
{
    using Newtonsoft.Json;

    public class GuardianSectionEnumerationResponse
    {
        [JsonProperty(PropertyName = "response")]
        public GuardianSectionEnumerationResponseField Response { get; set; }
    }
}
