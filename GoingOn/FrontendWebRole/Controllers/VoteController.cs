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
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using GoingOn.Common;
    using GoingOn.Frontend.Authentication;
    using GoingOn.Frontend.Common;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Validation;
    using GoingOn.Model.EntitiesBll;
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
        /// Get vote.
        /// </summary>
        /// <param name="city">The city of the news.</param>
        /// <param name="date">The date when the news was published.</param>
        /// <param name="newsId">The identifier of the news.</param>
        /// <returns></returns>
        [Route(GOUriBuilder.NewsVoteTemplate)]
        [HttpGet]
        [ResponseType(typeof(Vote))]
        public async Task<HttpResponseMessage> Get(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecuteGetAsync, city, date, newsId, this.User.Identity.Name);
        }

        /// <summary>
        /// Create vote.
        /// </summary>
        /// <param name="city">The city of the news.</param>
        /// <param name="date">The date when the news was published.</param>
        /// <param name="newsId">The identifier of the news.</param>
        /// <param name="vote">The vote to create.</param>
        /// <returns></returns>
        [Route(GOUriBuilder.NewsVoteTemplate)]
        [IdentityBasicAuthentication]
        [Authorize]
        [HttpPost]
        public async Task<HttpResponseMessage> Post(string city, string date, string newsId, [FromBody]Vote vote)
        {
            // TODO: validate if the user is the owner of the vote

            return await this.ValidateExecute(this.ExecutePostAsync, city, date, newsId, this.User.Identity.Name, vote);
        }

        /// <summary>
        /// Update vote.
        /// </summary>
        /// <param name="city">The city of the news.</param>
        /// <param name="date">The date when the news was published.</param>
        /// <param name="newsId">The identifier of the news.</param>
        /// <param name="vote">The vote to create.</param>
        /// <returns></returns>
        [Route(GOUriBuilder.NewsVoteTemplate)]
        [IdentityBasicAuthentication]
        [Authorize]
        [HttpPost]
        public async Task<HttpResponseMessage> Patch(string city, string date, string newsId, [FromBody]Vote vote)
        {
            // TODO: validate if the user is the owner of the vote

            return await this.ValidateExecute(this.ExecutePatchAsync, city, date, newsId, this.User.Identity.Name, vote);
        }

        /// <summary>
        /// Delete vote.
        /// </summary>
        /// <param name="city">The city of the news.</param>
        /// <param name="date">The date when the news was published.</param>
        /// <param name="newsId">The identifier of the news.</param>
        /// <returns></returns>
        [Route(GOUriBuilder.NewsVoteTemplate)]
        [IdentityBasicAuthentication]
        [Authorize]
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(string city, string date, string newsId)
        {
            // TODO: validate if the user is the owner of the vote

            return await this.ValidateExecute(this.ExecuteDeletetAsync, city, date, newsId, this.User.Identity.Name);
        }

        #region Operations code

        private async Task<HttpResponseMessage> ExecuteGetAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];
            var nickname = (string) parameters[3];

            await this.ValidateGetOperation(city, date, newsId);

            VoteBll vote = await this.voteRepository.GetVote(city, DateTime.Parse(date), Guid.Parse(newsId), nickname);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, Vote.FromVoteBll(vote));

            return response;
        }

        private async Task<HttpResponseMessage> ExecutePostAsync(params object[] parameters)
        {
            var city = (string) parameters[0];
            var date = (string) parameters[1];
            var newsId = (string) parameters[2];
            var nickname = (string)parameters[3];
            var vote = (Vote) parameters[4];

            await this.ValidatePostOperation(city, date, newsId);

            await this.voteRepository.AddVote(city, DateTime.Parse(date), Guid.Parse(newsId), nickname, Vote.ToVoteBll(vote));

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        private async Task<HttpResponseMessage> ExecutePatchAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];
            var nickname = (string)parameters[3];
            var vote = (Vote)parameters[4];

            await this.ValidatePatchOperation(city, date, newsId);

            await this.voteRepository.UpdateVote(city, DateTime.Parse(date), Guid.Parse(newsId), nickname, Vote.ToVoteBll(vote));

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        private async Task<HttpResponseMessage> ExecuteDeletetAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];
            var nickname = (string)parameters[3];

            await this.ValidateDeleteOperation(city, date, newsId);

            await this.voteRepository.DeleteVote(city, DateTime.Parse(date), Guid.Parse(newsId), nickname);

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
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news is not in the database");
            }

            if (await this.businessValidation.IsValidGetVote(this.voteRepository, city, DateTime.Parse(date), Guid.Parse(id)))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The user has already voted");
            }
        }

        private async Task ValidatePatchOperation(string city, string date, string id)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!(await this.businessValidation.IsValidGetNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news is not in the database");
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
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news is not in the database");
            }

            if (!(await this.businessValidation.IsValidGetVote(this.voteRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The user has not voted yet");
            }
        }

        #endregion
    }
}