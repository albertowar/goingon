// ****************************************************************************
// <copyright file="NewsConverter.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace NewsFinderWorkerRole
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;
    using Entities;
    using GoingOn.Client;
    using GoingOn.Client.Entities;

    public class NewsPusher
    {
        private GOClient client;

        public NewsPusher()
        {
            string username = ConfigurationManager.AppSettings["NewsFinderWorkerRoleUsername"];
            string password = ConfigurationManager.AppSettings["NewsFinderWorkerRolePassword"];

            this.client = new GOClient(
                ConfigurationManager.AppSettings["GoingOnEndpoint"], 
                username, 
                password);

            this.client.CreateUser(new UserClient { Nickname = username, Password = password, City = "Malaga", Name = "Fedzilla", Email = "alberto@gmail.com" }).Wait();
        }

        public async Task PushNews(List<Article> articles)
        {
            foreach (var article in articles)
            {
                // TODO: improve the City
                var response = await this.client.CreateNews(
                    "Malaga", 
                    article.PublishDate.ToString("yyyy-MM-dd"),
                    NewsConverter.ToNewsClient(article));
            }
        }
    }
}
