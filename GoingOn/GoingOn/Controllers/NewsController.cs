// ****************************************************************************
// <copyright file="NewsController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Controllers
{
    using GoingOn.Entities;
    using MemoryStorage;
    using Model.EntitiesBll;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class NewsController : ApiController
    {
        // GET api/news
        public IEnumerable<News> Get()
        {
            return new News[] { new News("News 1"), new News("News 2") };
        }

        // POST api/news
        public async Task<HttpResponseMessage> Post(News news)
        {
            string authorizationScheme = Request.Headers.Authorization.Scheme;
            string authorizationUserPassword = Request.Headers.Authorization.Parameter;

            // Check the scheme and return an appropiate response error in case it is not Basic
            if (authorizationScheme != "Basic" || string.IsNullOrWhiteSpace(authorizationUserPassword))
            {
                var response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.Unauthorized;

                return response;
            }

            string usernameColonPassword = Encoding.ASCII.GetString(Convert.FromBase64String(authorizationUserPassword));
            string username = usernameColonPassword.Split(new char[] { ':' }).First();
            string password = usernameColonPassword.Split(new char[] { ':' }).Last();

            UserMemoryStorage storage = UserMemoryStorage.GetInstance();

            if (storage.ContainsUser(new UserBll(username, password)))
            {
                // Add the news to the storage
            }
            else
            {
                var response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.Unauthorized;

                return response;
            }
            
            return null;
        }
    }
}
