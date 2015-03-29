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
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(@"http://api.feedzilla.com/v1/categories.json");

                var categoriesJson = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Category>>(categoriesJson);
            }
        }

        private static async Task<List<Article>> GetArticles(Category category)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(string.Format(@"http://api.feedzilla.com/v1/categories/{0}/articles.json", category.CategoryId));

                var categoriesJson = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ArticleEnumeration>(categoriesJson).Articles;
            }
        }
    }
}
