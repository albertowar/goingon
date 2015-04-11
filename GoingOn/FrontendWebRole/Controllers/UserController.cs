// ****************************************************************************
// <copyright file="UserController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using GoingOn.Common;
    using GoingOn.Frontend.Authentication;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Links;
    using GoingOn.Frontend.Validation;
    using GoingOn.Frontend.Authentication;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Links;
    using GoingOn.Frontend.Validation;
    using GoingOn.Storage;

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

        /// <summary>
        /// Get an user by its id.
        /// </summary>
        /// <param name="userId">The id.</param>
        /// <returns>The user.</returns>
        [IdentityBasicAuthentication]
        [Authorize]
        [HttpGet]
        [Route(GOUriBuilder.GetUserTemplate)]
        public async Task<HttpResponseMessage> Get(string userId)
        {
            if (!this.inputValidation.IsValidNickName(userId))
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!await this.businessValidation.IsValidGetUser(this.storage, userId))
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "The user is not in the database");
            }
                
            var user = UserREST.FromUserBll(await this.storage.GetUser(userId), this.Request);

            var response = this.Request.CreateResponse(HttpStatusCode.OK, user);

            return response;
        }

        [HttpPost]
        [Route(GOUriBuilder.PostUserTemplate)]
        public async Task<HttpResponseMessage> Post([FromBody]User user)
        {
            if (!this.inputValidation.IsValidUser(user))
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!await this.businessValidation.IsValidCreateUser(this.storage, user))
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user is already registered");
            }

            var userToAdd = GoingOn.Frontend.Entities.User.ToUserBll(user);
            userToAdd.RegistrationDate = DateTime.Now;

            try
            {
                await this.storage.AddUser(userToAdd);
            }
            catch (Exception)
            {
                var badResponse = this.Request.CreateResponse(HttpStatusCode.InternalServerError, "Something really wrong happened");
                return badResponse;
            }

            var response = this.Request.CreateResponse(HttpStatusCode.Created, "The user was added to the database");
            response.Headers.Location = new UserLinkFactory(this.Request).Self(user.Nickname).Href;

            return response;
        }

        [IdentityBasicAuthentication]
        [Authorize]
        [HttpPatch]
        [Route(GOUriBuilder.PatchUserTemplate)]
        public async Task<HttpResponseMessage> Patch(string userId, [FromBody]User user)
        {
            if (!this.inputValidation.IsValidNickName(userId))
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.inputValidation.IsValidUser(user))
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsAuthorizedUser(this.User.Identity.Name, userId))
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "The user is not authorized to update another user");
            }

            if (!await this.businessValidation.IsValidUpdateUser(this.storage, user))
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "The user is not registered");
            }

            var userToUpdate = GoingOn.Frontend.Entities.User.ToUserBll(user);

            await this.storage.UpdateUser(userToUpdate);

            return this.Request.CreateResponse(HttpStatusCode.NoContent, "The user was updated");
        }

        [IdentityBasicAuthentication]
        [Authorize]
        [HttpDelete]
        [Route(GOUriBuilder.DeleteUserTemplate)]
        public async Task<HttpResponseMessage> Delete(string userId)
        {
            if (!this.inputValidation.IsValidNickName(userId))
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsAuthorizedUser(this.User.Identity.Name, userId))
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "The user is not authorized to delete another user");
            }

            if (!await this.businessValidation.IsValidDeleteUser(this.storage, userId))
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "The user is not registered");
            }

            await this.storage.DeleteUser(GoingOn.Frontend.Entities.User.ToUserBll(new User { Nickname = userId }));

            return this.Request.CreateResponse(HttpStatusCode.NoContent, "The user was deleted");
        }
    }
}