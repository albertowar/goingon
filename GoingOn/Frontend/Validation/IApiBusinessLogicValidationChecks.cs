// ****************************************************************************
// <copyright file="IApiBusinessValidationChecks.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Frontend.Validation
{
    using System;
    using System.Threading.Tasks;

    using Frontend.Entities;
    using Storage;

    public interface IApiBusinessLogicValidationChecks
    {
        bool IsValidCreateUser(IUserStorage storage, User user);

        bool IsValidGetUser(IUserStorage storage, string nickname);

        bool IsValidUpdateUser(IUserStorage storage, User user);

        bool IsAuthorizedUser(string requesterNickname, string userNickname);

        bool IsValidDeleteUser(IUserStorage storage, string nickname);

        bool IsValidCreateNews(INewsStorage storage, News news, string author);

        Task<bool> IsValidGetNews(INewsStorage storage, string city, DateTime date, Guid id);

        Task<bool> IsValidUpdateNews(INewsStorage storage, string city, DateTime date, Guid id, string author);

        Task<bool> IsValidDeleteNews(INewsStorage storage, string city, DateTime date, Guid id, string author);
    }
}
