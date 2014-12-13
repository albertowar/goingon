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
        public bool IsValidUser(IUserStorage storage, User user)
        {
            return storage.ContainsUser(User.ToUserBll(user));
        }
    }
}