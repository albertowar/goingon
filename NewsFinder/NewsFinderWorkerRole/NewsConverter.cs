// ****************************************************************************
// <copyright file="NewsConverter.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.NewsFinderWorkerRole
{
    using GoingOn.Client.Entities;
    using GoingOn.GuardianClient.API.Entities;

    public class NewsConverter
    {
        public static NewsClient ToNewsClient(GuardianSectionArticle guardianSectionArticle)
        {
            return new NewsClient
            {
                Title = guardianSectionArticle.SectionName,
                Content = guardianSectionArticle.WebUrl
            };
        }
    }
}
