// ****************************************************************************
// <copyright file="Article.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace NewsFinderWorkerRole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Entities;
    using Newtonsoft.Json;

    public class NewsFinder
    {
        public static async Task<List<Article>> FindNews()
        {
            var articles = new List<Article>();

            var categories = await NewsFinder.GetCategories();

            foreach (var category in categories)
            {
                articles = articles.Concat(await NewsFinder.GetArticles(category)).ToList();
            }

            return articles;
        }

        private static async Task<List<Category>> GetCategories()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 0, 1, 0);

                    HttpResponseMessage response = await client.GetAsync(@"http://api.feedzilla.com/v1/categories.json");

                    string categoriesJson = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<Category>>(categoriesJson);
                }
                catch (Exception)
                {
                }

                return new List<Category>();
            }
        }

        private static async Task<List<Article>> GetArticles(Category category)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 0, 1, 0);

                    HttpResponseMessage response = await client.GetAsync(string.Format(@"http://api.feedzilla.com/v1/categories/{0}/articles.json", category.CategoryId));

                    string categoriesJson = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<ArticleEnumeration>(categoriesJson).Articles;
                }
                catch (Exception)
                {
                }

                return new List<Article>();
            }
        }
    }
}
