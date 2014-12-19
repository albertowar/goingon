// ****************************************************************************
// <copyright file="UserController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System.Threading.Tasks;
using GoingOn.Links;
using Model.EntitiesBll;

namespace GoingOn.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using GoingOn.Authentication;
    using GoingOn.Validation;
    using FrontendEntities = GoingOn.Entities;

    public class UserController : ApiController
    {
        private readonly IUserStorage storage;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        public UserController(IUserStorage storage, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.storage = storage;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        [IdentityBasicAuthentication]
        [Authorize]
        public async Task<HttpResponseMessage> Get(string id)
        {
            if (await storage.ContainsUser(new UserBll(id, string.Empty)))
            {
                var user = FrontendEntities.User.FromUserBll(await storage.GetUser(id));

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The user is not in the database");
        }

        // POST api/user
        public HttpResponseMessage Post([FromBody]FrontendEntities.User user)
        {
            if (!this.inputValidation.IsValidUser(user))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsValidCreateUser(storage, user))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user is already registered");
            }

            storage.AddUser(FrontendEntities.User.ToUserBll(user));

            var response = Request.CreateResponse(HttpStatusCode.Created, "The user was added to the database");
            response.Headers.Location = new UserLinkFactory(Request).Self(user.Nickname).Href;

            return response;
        }
    }
}