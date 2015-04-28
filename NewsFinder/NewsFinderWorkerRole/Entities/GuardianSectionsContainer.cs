﻿// ****************************************************************************
// <copyright file="GuardianSection.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.NewsFinderWorkerRole.Entities
{
    using Newtonsoft.Json;

    public class GuardianSectionsContainer
    {
        [JsonProperty(PropertyName = "response")]
        public GuardianSectionsListResponse Response { get; set; }
    }
}
