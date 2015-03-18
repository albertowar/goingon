// ****************************************************************************
// <copyright file="NewsClient.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Client
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using GoingOn.Client.Entities;
    using GoingOn.Common;

    public class GOClient
    {
        private string rootUri;

        private string username;

        private string password;

        public GOClient(string rootUri, string username, string password)
        {
            this.rootUri = rootUri;
            this.username = username;
            this.password = password;
        }

        #region User methods
        public async Task<HttpResponseMessage> GetUser(string usernameToGet)
        {
            string authenticationCredentials = GOClient.EncodeTo64(string.Format("{0}:{1}", this.username, this.password));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.rootUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authenticationCredentials);

                return await client.GetAsync(GOUriBuilder.BuildUserUri(usernameToGet));
            }
        }

        public async Task<HttpResponseMessage> CreateUser(UserClient user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.rootUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return await client.PostAsJsonAsync(GOUriBuilder.PostUserTemplate, user);
            }
        }

        public async Task<HttpResponseMessage> UpdateUser(UserClient userToUpdate)
        {
            string authenticationCredentials = GOClient.EncodeTo64(string.Format("{0}:{1}", this.username, this.password));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.rootUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authenticationCredentials);

                return await client.PatchAsync(GOUriBuilder.BuildUserUri(userToUpdate.Nickname), userToUpdate, new JsonMediaTypeFormatter());
            }
        }

        public async Task<HttpResponseMessage> DeleteUser(string usernameToDelete)
        {
            string authenticationCredentials = GOClient.EncodeTo64(string.Format("{0}:{1}", this.username, this.password));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.rootUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authenticationCredentials);

                return await client.DeleteAsync(GOUriBuilder.BuildUserUri(usernameToDelete));
            }
        }

        #endregion

        #region News methods

        public async Task<HttpResponseMessage> CreateNews(string city, string date, NewsClient news)
        {
            var authorizationString = GOClient.EncodeTo64(string.Format("{0}:{1}", this.username, this.password));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.rootUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationString);

                return await client.PostAsJsonAsync(GOUriBuilder.BuildDiaryEntryUri(city, date), news);
            }
        }

        public async Task<HttpResponseMessage> UpdateNews(string city, string date, string id, NewsClient news)
        {
            string authorizationString = GOClient.EncodeTo64(string.Format("{0}:{1}", this.username, this.password));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.rootUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationString);

                return await client.PatchAsJsonAsync(GOUriBuilder.BuildNewsUri(city, date, id), news);
            }
        }

        public async Task<HttpResponseMessage> GetNews(string city, string date, string guid)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.rootUri);

                return await client.GetAsync(GOUriBuilder.BuildNewsUri(city, date, guid));
            }
        }

        public async Task<HttpResponseMessage> DeleteNews(string city, string date, string guid)
        {
            string authorizationString = GOClient.EncodeTo64(string.Format("{0}:{1}", this.username, this.password));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.rootUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationString);

                return await client.DeleteAsync(GOUriBuilder.BuildNewsUri(city, date, guid));
            }
        }

        #endregion

        private static string EncodeTo64(string stringToEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(stringToEncode);

            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;
        }
    }
}
