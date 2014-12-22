// ****************************************************************************
// <copyright file="ApiBusinessValidationChecks.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Validation
{
    using GoingOn.Entities;

    public class ApiBusinessLogicValidationChecks : IApiBusinessLogicValidationChecks
    {
        public ApiBusinessLogicValidationChecks()
        {
            
        }

        public bool IsValidCreateUser(IUserStorage storage, User user)
        {
            return !this.IsUserStored(storage, user);
        }

        public bool IsValidGetUser(IUserStorage storage, string nickname)
        {
            return this.IsUserStored(storage, new User(nickname, string.Empty));
        }

        public bool IsValidUpdateUser(IUserStorage storage, User user)
        {
            return this.IsUserStored(storage, user);
        }

        public bool IsValidDeleteUser(IUserStorage storage, string nickname)
        {
            return this.IsUserStored(storage, new User(nickname, string.Empty));
        }

        #region Helper methods

        private bool IsUserStored(IUserStorage storage,  User user)
        {
            var containsUserTask = storage.ContainsUser(User.ToUserBll(user));
            containsUserTask.Wait();

            return containsUserTask.Result;
        }

        #endregion
    }
}