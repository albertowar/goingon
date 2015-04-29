// ****************************************************************************
// <copyright file="NewsFinder.cs" company="Universidad de Malaga">
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
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using GoingOn.GuardianClient.API;
    using GoingOn.GuardianClient.API.Entities;
    using Newtonsoft.Json;

    public class NewsFinder
    {
        public static async Task<List<GuardianSectionArticle>> FindNews()
        {
            var guardianItems = new List<GuardianSectionArticle>();

            GuardianSectionEnumerationResponse categories = await NewsFinder.GetSections();
            GuardianSectionEnumerationResponseField sectionResponse = categories.Response;

            var queries = 0;

            foreach (GuardianSection section in sectionResponse.Results)
            {
                long start = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                guardianItems = guardianItems.Concat(await NewsFinder.GetSectionResponse(section)).ToList();

                ++queries;

                if (queries == 12)
                {
                    long end = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    var totalTime = (int) (end - start);

                    if (totalTime < 1000)
                    {
                        Thread.Sleep(1000 - totalTime + 100);
                    }

                    queries = 0;
                }
            }

            return guardianItems;
        }

        private static async Task<GuardianSectionEnumerationResponse> GetSections()
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(@"http://content.guardianapis.com/sections?api-key=zaffsfb2fpmkfrsdbygpf6hm");

                string categoriesJson = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<GuardianSectionEnumerationResponse>(categoriesJson);
            }
        }

        private static async Task<List<GuardianSectionArticle>> GetSectionResponse(GuardianSection section)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(section.ApiUrl + "?api-key=zaffsfb2fpmkfrsdbygpf6hm");

                string categoriesJson = await response.Content.ReadAsStringAsync();

                var sectionResponse = JsonConvert.DeserializeObject<GuardianSectionContainer>(categoriesJson);

                return sectionResponse.Response.Results;
            }
        }
    }
}
