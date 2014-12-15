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
    using GoingOn.Authentication;
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
        [IdentityBasicAuthentication]
        [Authorize]
        public HttpResponseMessage Post(News news)
        {
            // Add the news to the storage
            
            return null;
        }
    }
}
