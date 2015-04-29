// ****************************************************************************
// <copyright file="GuardianBaseResponseField.cs" company="Universidad de Malaga">
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

    public class GuardianBaseResponseField
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "userTier")]
        public string UserTier { get; set; }

        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
    }
}
