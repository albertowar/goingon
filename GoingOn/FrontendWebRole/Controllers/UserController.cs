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
    using System.Web.Http.Description;
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
        /// Get an user by its nickname.
        /// </summary>
        /// <param name="userId">The nickname of the user.</param>
        /// <returns>The user.</returns>
        [IdentityBasicAuthentication]
        [Authorize]
        [HttpGet]
        [Route(GOUriBuilder.GetUserTemplate)]
        [ResponseType(typeof(UserREST))]
        public async Task<HttpResponseMessage> Get(string userId)
        {
            return await this.ValidateExecute(this.ExecuteGetAsync, userId, this.User.Identity.Name);
        }

        /// <summary>
        /// Creates an user.
        /// </summary>
        /// <param name="user">The user information.</param>
        /// <returns></returns>
        [HttpPost]
        [Route(GOUriBuilder.PostUserTemplate)]
        public async Task<HttpResponseMessage> Post([FromBody]User user)
        {
            return await this.ValidateExecute(this.ExecutePostAsync, user, this.User.Identity.Name);
        }

        /// <summary>
        /// Updates the user information.
        /// </summary>
        /// <param name="userId">The nickname of the user.</param>
        /// <param name="user">The user information.</param>
        /// <returns></returns>
        [IdentityBasicAuthentication]
        [Authorize]
        [HttpPatch]
        [Route(GOUriBuilder.PatchUserTemplate)]
        public async Task<HttpResponseMessage> Patch(string userId, [FromBody]User user)
        {
            return await this.ValidateExecute(this.ExecutePatchAsync, userId, user, this.User.Identity.Name);
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userId">The nickname of the user.</param>
        /// <returns></returns>
        [IdentityBasicAuthentication]
        [Authorize]
        [HttpDelete]
        [Route(GOUriBuilder.DeleteUserTemplate)]
        public async Task<HttpResponseMessage> Delete(string userId)
        {
            return await this.ValidateExecute(this.ExecuteDeleteAsync, userId, this.User.Identity.Name);
        }

        #region Operations code

        private async Task<HttpResponseMessage> ExecuteGetAsync(params object[] parameters)
        {
            var userId = (string)parameters[0];
            var authenticatedUser = (string)parameters[1];

            await this.ValidateGetOperation(userId, authenticatedUser);

            UserREST user = UserREST.FromUserBll(await this.repository.GetUser(userId), this.Request);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, user);

            return response;
        }

        private async Task<HttpResponseMessage> ExecutePostAsync(params object[] parameters)
        {
            var user = (User) parameters[0];

            await this.ValidatePostNewsOperation(user);

            UserBll userToAdd = Frontend.Entities.User.ToUserBll(user);
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
            var authenticatedUser = (string)parameters[2];

            await this.ValidatePatchNewsOperation(userId, user, authenticatedUser);

            UserBll userToUpdate = Frontend.Entities.User.ToUserBll(user);

            await this.repository.UpdateUser(userToUpdate);

            return this.Request.CreateResponse(HttpStatusCode.NoContent, "The user was updated");
        }

        private async Task<HttpResponseMessage> ExecuteDeleteAsync(params object[] parameters)
        {
            var userId = (string)parameters[0];
            var authenticatedUser = (string)parameters[1];

            await this.ValidateDeleteNewsOperation(userId, authenticatedUser);

            await this.repository.DeleteUser(Frontend.Entities.User.ToUserBll(new User { Nickname = userId }));

            return this.Request.CreateResponse(HttpStatusCode.NoContent, "The user was deleted");
        }

        #endregion

        #region Validation code

        public async Task ValidateGetOperation(string userId, string authenticatedUser)
        {
            if (!this.inputValidation.IsValidNickName(userId))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The nickname format is incorrect");
            }

            if (!this.businessValidation.IsAuthorizedUser(authenticatedUser, userId))
            {
                throw new BusinessValidationException(HttpStatusCode.Unauthorized, "The user is not allow to retrieve the resource");
            }

            if (!await this.businessValidation.IsUserCreated(this.repository, userId))
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

        public async Task ValidatePatchNewsOperation(string userId, User user, string authenticatedUser)
        {
            if (!this.inputValidation.IsValidNickName(userId))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsAuthorizedUser(authenticatedUser, userId))
            {
                throw new BusinessValidationException(HttpStatusCode.Unauthorized, "The user is not allow to retrieve the resource");
            }

            if (!this.inputValidation.IsValidUser(user))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!await this.businessValidation.IsUserCreated(this.repository, user))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The user is not registered");
            }
        }

        public async Task ValidateDeleteNewsOperation(string id, string userId)
        {
            if (!this.inputValidation.IsValidNickName(userId))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, "The user format is incorrect");
            }

            if (!this.businessValidation.IsAuthorizedUser(this.User.Identity.Name, userId))
            {
                throw new BusinessValidationException(HttpStatusCode.Unauthorized, "The user is not authorized to delete another user");
            }

            if (!await this.businessValidation.IsUserCreated(this.repository, userId))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The user is not registered");
            }
        }

        #endregion
    }
}