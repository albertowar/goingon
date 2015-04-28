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
    using System.Threading.Tasks;
    using GoingOn.NewsFinderWorkerRole.Entities;
    using Newtonsoft.Json;

    public class NewsFinder
    {
        public static async Task<List<GuardianSingleItem>> FindNews()
        {
            var guardianItems = new List<GuardianSingleItem>();

            GuardianSectionsContainer categories = await NewsFinder.GetSections();
            GuardianSectionsListResponse sectionResponse = categories.Response;

            List<GuardianSection> results = sectionResponse.Results;

            for (int i = 0; i < 10; ++i)
            {
                guardianItems = guardianItems.Concat(await NewsFinder.GetSectionResponse(results.ElementAt(i))).ToList();
            }

            return guardianItems;
        }

        private static async Task<GuardianSectionsContainer> GetSections()
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(@"http://content.guardianapis.com/sections?api-key=zaffsfb2fpmkfrsdbygpf6hm");

                string categoriesJson = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<GuardianSectionsContainer>(categoriesJson);
            }
        }

        private static async Task<List<GuardianSingleItem>> GetSectionResponse(GuardianSection section)
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
