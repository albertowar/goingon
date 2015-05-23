// ****************************************************************************
// <copyright file="UserController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// User controller class
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
    using GoingOn.Frontend.Common;
    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Links;
    using GoingOn.Frontend.Validation;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Repository;

    public class UserController : GoingOnApiController
    {
        private readonly IUserRepository repository;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        public UserController(IUserRepository repository, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.repository = repository;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        /// <summary>
        /// Get an user by its id.
        /// </summary>
        /// <param name="city"></param>
        /// <param name="userId">The id.</param>
        /// <returns>The user.</returns>
        [IdentityBasicAuthentication]
        [Authorize]
        [HttpGet]
        [Route(GOUriBuilder.GetUserTemplate)]
        public async Task<HttpResponseMessage> Get(string city, string userId)
        {
            return await this.ValidateExecute(this.ExecuteGetAsync, userId);
        }

        [HttpPost]
        [Route(GOUriBuilder.PostUserTemplate)]
        public async Task<HttpResponseMessage> Post(string city, [FromBody]User user)
        {
            return await this.ValidateExecute(this.ExecutePostAsync, user);
        }

        [IdentityBasicAuthentication]
        [Authorize]
        [HttpPatch]
        [Route(GOUriBuilder.PatchUserTemplate)]
        public async Task<HttpResponseMessage> Patch(string city, string userId, [FromBody]User user)
        {
            return await this.ValidateExecute(this.ExecutePatchAsync, userId, user);
        }

        [IdentityBasicAuthentication]
        [Authorize]
        [HttpDelete]
        [Route(GOUriBuilder.DeleteUserTemplate)]
        public async Task<HttpResponseMessage> Delete(string city, string userId)
        {
            return await this.ValidateExecute(this.ExecuteDeleteAsync, userId);
        }

        #region Operations code

        private async Task<HttpResponseMessage> ExecuteGetAsync(params object[] parameters)
        {
            // TODO: city will return null reference exception
            var city = (string) parameters[0];
            var userId = (string)parameters[1];

            await this.ValidateGetOperation(city, userId);

            UserREST user = UserREST.FromUserBll(await this.repository.GetUser(city, userId), this.Request);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, user);

            return response;
        }

        private async Task<HttpResponseMessage> ExecutePostAsync(params object[] parameters)
        {
            User user = (User) parameters[0];

            await this.ValidatePostNewsOperation(user);

            UserBll userToAdd = GoingOn.Frontend.Entities.User.ToUserBll(user);
            userToAdd.RegistrationDate = DateTime.Now;

            await this.repository.AddUser(userToAdd);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.Created, "The user was added to the database");
            response.Headers.Location = new UserLinkFactory(this.Request).Self(user.Nickname).Href;

            return response;
        }

        private async Task<HttpResponseMessage> ExecutePatchAsync(params object[] parameters)
        {
            var userId = (string) parameters[0];
            var user = (User) parameters[1];

            await this.ValidatePatchNewsOperation(userId, user);

            UserBll userToUpdate = GoingOn.Frontend.Entities.User.ToUserBll(user);

            await this.repository.UpdateUser(userToUpdate);

            return this.Request.CreateResponse(HttpStatusCode.NoContent, "The user was updated");
        }

        private async Task<HttpResponseMessage> ExecuteDeleteAsync(params object[] parameters)
        {
            string userId = (string)parameters[0];

            await this.ValidateDeleteNewsOperation(userId);

            await this.repository.DeleteUser(GoingOn.Frontend.Entities.User.ToUserBll(new User { Nickname = userId }));

            return this.Request.CreateResponse(HttpStatusCode.NoContent, "The user was deleted");
        }

        #endregion

        #region Validation code

        public async Task ValidateGetOperation(string city, string userId)
        {
            if (!this.inputValidation.IsValidCity(city))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The city is not supported");
            }

            if (!this.inputValidation.IsValidNickName(userId))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The nickname format is incorrect");
            }

            if (!await this.businessValidation.IsValidGetUser(this.repository, userId))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The user is not in the database");
            }
        }

        public async Task ValidatePostNewsOperation(User user)
        {
            if (!this.inputValidation.IsValidUser(user))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!await this.businessValidation.IsValidCreateUser(this.repository, user))
            {
                throw new BusinessValidationException(HttpStatusCode.BadRequest, "The user is already registered");
            }
        }

        public async Task ValidatePatchNewsOperation(string userId, User user)
        {
            if (!this.inputValidation.IsValidNickName(userId))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.inputValidation.IsValidUser(user))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsAuthorizedUser(this.User.Identity.Name, userId))
            {
                throw new BusinessValidationException(HttpStatusCode.Unauthorized, "The user is not authorized to update another user");
            }

            if (!await this.businessValidation.IsValidUpdateUser(this.repository, user))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The user is not registered");
            }
        }

        public async Task ValidateDeleteNewsOperation(string userId)
        {
            if (!this.inputValidation.IsValidNickName(userId))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsAuthorizedUser(this.User.Identity.Name, userId))
            {
                throw new BusinessValidationException(HttpStatusCode.Unauthorized, "The user is not authorized to delete another user");
            }

            if (!await this.businessValidation.IsValidDeleteUser(this.repository, userId))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The user is not registered");
            }
        }

        #endregion
    }
}