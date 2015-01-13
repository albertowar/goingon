// ****************************************************************************
// <copyright file="NewsController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using Storage;

namespace Frontend.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Frontend.Authentication;
    using Frontend.Entities;
    using Frontend.Links;
    using Frontend.Validation;

    public class NewsController : ApiController
    {
        private readonly INewsStorage storage;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        public NewsController(INewsStorage storage, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.storage = storage;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        // GET api/news/{guid}
        public async Task<HttpResponseMessage> Get(string id)
        {
            if (!this.inputValidation.IsValidNewsId(id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The news id format is incorrect");
            }

            if (!this.businessValidation.IsValidGetNews(this.storage, id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The news is not in the database");
            }

            var news = News.FromNewsBll(await this.storage.GetNews(Guid.Parse(id)), Request);

            var response = Request.CreateResponse(HttpStatusCode.OK, news);

            return response;
        }

        // POST api/news
        [IdentityBasicAuthentication]
        [Authorize]
        public async Task<HttpResponseMessage> Post([FromBody]News news)
        {
            var nickname = User.Identity.Name;

            if (!this.inputValidation.IsValidNews(news))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The news format is incorrect");
            }

            if (!this.businessValidation.IsValidCreateNews(this.storage, news, nickname))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The news is already created");
            }

            Guid newsId = Guid.NewGuid();

            var nowTime = DateTime.UtcNow;
            await this.storage.AddNews(News.ToNewsBll(news, newsId, nickname, new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, 0, 0)));

            var response = Request.CreateResponse(HttpStatusCode.Created, "The news was added to the database");
            response.Headers.Location = new NewsLinkFactory(Request).Self(newsId.ToString()).Href;

            return response;
        }

        // PATCH api/news/{guid}
        [IdentityBasicAuthentication]
        [Authorize]
        public async Task<HttpResponseMessage> Patch(string id, [FromBody]News news)
        {
            if (!this.inputValidation.IsValidNewsId(id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The news format is incorrect");
            }

            if (!this.inputValidation.IsValidNews(news))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The news format is incorrect");
            }

            if (!this.businessValidation.IsValidUpdateNews(this.storage, id, User.Identity.Name))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The news does not exist");
            }

            await this.storage.UpdateNews(News.ToNewsBll(Guid.Parse(id), news, User.Identity.Name));

            var response = Request.CreateResponse(HttpStatusCode.OK, "The news was added to the database");

            return response;
        }

        [IdentityBasicAuthentication]
        [Authorize]
        // DELETE api/news/{guid}
        public async Task<HttpResponseMessage> Delete(string id)
        {
            if (!this.inputValidation.IsValidNewsId(id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The news format is incorrect");
            }

            if (!this.businessValidation.IsValidDeleteNews(storage, id, User.Identity.Name))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The news does not exist");
            }

            await storage.DeleteNews(Guid.Parse(id));

            return Request.CreateResponse(HttpStatusCode.NoContent, "The news was deleted");
        }
    }
}
