// ****************************************************************************
// <copyright file="VoteController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Vote controller class
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole.Controllers
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Http;
    using GoingOn.Common;
    using GoingOn.Frontend.Common;
    using GoingOn.Frontend.Validation;
    using GoingOn.Repository;

    /// <summary>
    /// 
    /// </summary>
    public class VoteController : GoingOnApiController
    {
        private readonly INewsRepository newsRepository;
        private readonly IVoteRepository voteRepository;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        // TODO: require authentication

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newsRepository"></param>
        /// <param name="voteRepository"></param>
        /// <param name="inputValidation"></param>
        /// <param name="businessValidation"></param>
        public VoteController(INewsRepository newsRepository, IVoteRepository voteRepository, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.newsRepository = newsRepository;
            this.voteRepository = voteRepository;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        /// <summary>
        /// Get vote
        /// </summary>
        /// <param name="city"></param>
        /// <param name="date"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        [Route(GOUriBuilder.NewsImageTemplate)]
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecuteGetAsync, city, date, newsId);
        }

        /// <summary>
        /// Create vote
        /// </summary>
        /// <param name="city"></param>
        /// <param name="date"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        [Route(GOUriBuilder.NewsImageTemplate)]
        [HttpPost]
        public async Task<HttpResponseMessage> Post(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecutePostAsync, city, date, newsId);
        }

        /// <summary>
        /// Create vote
        /// </summary>
        /// <param name="city"></param>
        /// <param name="date"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        [Route(GOUriBuilder.NewsImageTemplate)]
        [HttpPost]
        public async Task<HttpResponseMessage> Put(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecutePutAsync, city, date, newsId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <param name="date"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        [Route(GOUriBuilder.NewsImageTemplate)]
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecuteDeletetAsync, city, date, newsId);
        }

        #region Operations code

        private async Task<HttpResponseMessage> ExecuteGetAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];

            await this.ValidateGetOperation(city, date, newsId);

            // TODO: Behaviour

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        private async Task<HttpResponseMessage> ExecutePostAsync(params object[] parameters)
        {
            var city = (string) parameters[0];
            var date = (string) parameters[1];
            var newsId = (string) parameters[2];

            await this.ValidatePostOperation(city, date, newsId);

            // TODO: Behaviour

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        private async Task<HttpResponseMessage> ExecutePutAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];

            await this.ValidatePutOperation(city, date, newsId);

            // TODO: Behaviour

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        private async Task<HttpResponseMessage> ExecuteDeletetAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];

            await this.ValidateDeleteOperation(city, date, newsId);

            // TODO: Behaviour

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        #endregion

        #region Validation helpers

        private async Task ValidateGetOperation(string city, string date, string id)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!(await this.businessValidation.IsValidGetNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news is not in the database");
            }

            if (!(await this.businessValidation.IsValidGetVote(this.voteRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The user has not voted yet");
            }
        }

        private async Task ValidatePostOperation(string city, string date, string id)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!(await this.businessValidation.IsValidGetNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The vote is not in the database");
            }

            if (await this.businessValidation.IsValidGetVote(this.voteRepository, city, DateTime.Parse(date), Guid.Parse(id)))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The user has already voted");
            }
        }

        private async Task ValidatePutOperation(string city, string date, string id)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!(await this.businessValidation.IsValidGetNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The vote is not in the database");
            }

            if (await this.businessValidation.IsValidGetVote(this.voteRepository, city, DateTime.Parse(date), Guid.Parse(id)))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The user has already voted");
            }
        }

        private async Task ValidateDeleteOperation(string city, string date, string id)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!(await this.businessValidation.IsValidGetNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The vote is not in the database");
            }

            if (!(await this.businessValidation.IsValidGetVote(this.voteRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The user has not voted yet");
            }
        }

        #endregion
    }
}