// ****************************************************************************
// <copyright file="IApiBusinessValidationChecks.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Validation
{
    using System;
    using System.Threading.Tasks;

    using GoingOn.Frontend.Entities;
    using GoingOn.Storage;

    public interface IApiBusinessLogicValidationChecks
    {
        Task<bool> IsValidCreateUser(IUserStorage storage, User user);

        Task<bool> IsValidGetUser(IUserStorage storage, string nickname);

        Task<bool> IsValidUpdateUser(IUserStorage storage, User user);

        bool IsAuthorizedUser(string requesterNickname, string userNickname);

        Task<bool> IsValidDeleteUser(IUserStorage storage, string nickname);

        bool IsValidCreateNews(INewsStorage storage, News news, string city, string author, DateTime date);

        Task<bool> IsValidGetNews(INewsStorage storage, string city, DateTime date, Guid id);

        Task<bool> IsValidUpdateNews(INewsStorage storage, string city, DateTime date, Guid id, string author);

        Task<bool> IsValidDeleteNews(INewsStorage storage, string city, DateTime date, Guid id, string author);
    }
}
