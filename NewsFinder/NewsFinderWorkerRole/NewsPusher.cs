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
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;
    using GoingOn.Client;
    using GoingOn.Client.Entities;
    using GoingOn.NewsFinderWorkerRole.Entities;

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

        public async Task PushNews(List<GuardianSingleItem> items)
        {
            foreach (GuardianSingleItem singleItem in items)
            {
                // TODO: improve the City
                var response = await this.client.CreateNews(
                    "Malaga",
                    DateTime.Parse(singleItem.WebPublicationDate).ToString("yyyy-MM-dd"),
                    NewsConverter.ToNewsClient(singleItem));
            }
        }
    }
}
