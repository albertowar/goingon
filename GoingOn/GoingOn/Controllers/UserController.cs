// ****************************************************************************
// <copyright file="UserController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Threading.Tasks;

    using GoingOn.Authentication;
    using GoingOn.Links;
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
        // GET api/user/{id}
        public async Task<HttpResponseMessage> Get(string id)
        {
            if (!this.inputValidation.IsValidNickName(id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsValidGetUser(storage, id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The user is not in the database");
            }
                
            var user = FrontendEntities.User.FromUserBll(await storage.GetUser(id), Request);

            var response = Request.CreateResponse(HttpStatusCode.OK, user);

            return response;
        }

        // POST api/user
        public async Task<HttpResponseMessage> Post([FromBody]FrontendEntities.User user)
        {
            if (!this.inputValidation.IsValidUser(user))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsValidCreateUser(storage, user))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user is already registered");
            }

            await storage.AddUser(FrontendEntities.User.ToUserBll(user));

            var response = Request.CreateResponse(HttpStatusCode.Created, "The user was added to the database");
            response.Headers.Location = new UserLinkFactory(Request).Self(user.Nickname).Href;

            return response;
        }

        [IdentityBasicAuthentication]
        [Authorize]
        // PUT api/user/{id}
        public async Task<HttpResponseMessage> Put(string id, [FromBody]FrontendEntities.User user)
        {
            if (!this.inputValidation.IsValidNickName(id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsValidUpdateUser(storage, user))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The user is not registered");
            }

            await storage.UpdateUser(FrontendEntities.User.ToUserBll(user));

            return Request.CreateResponse(HttpStatusCode.NoContent, "The user was updated");
        }

        [IdentityBasicAuthentication]
        [Authorize]
        // PUT api/user/{id}
        public async Task<HttpResponseMessage> Delete(string id)
        {
            if (!this.inputValidation.IsValidNickName(id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsValidDeleteUser(storage, id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The user is not registered");
            }

            await storage.DeleteUser(FrontendEntities.User.ToUserBll(new FrontendEntities.User(id, string.Empty)));

            return Request.CreateResponse(HttpStatusCode.NoContent, "The user was updated");
        }
    }
}