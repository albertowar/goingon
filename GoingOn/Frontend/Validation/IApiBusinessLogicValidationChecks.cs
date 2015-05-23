// ****************************************************************************
// <copyright file="IApiBusinessValidationChecks.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Business validation interface
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Validation
{
    using System;
    using System.Threading.Tasks;

    using GoingOn.Frontend.Entities;
    using GoingOn.Repository;
    using GoingOn.XStoreProxy;

    public interface IApiBusinessLogicValidationChecks
    {
        Task<bool> IsValidCreateUser(IUserRepository repository, User user);

        Task<bool> IsValidGetUser(IUserRepository repository, string nickname);

        Task<bool> IsValidUpdateUser(IUserRepository repository, User user);

        bool IsAuthorizedUser(string requesterNickname, string userNickname);

        Task<bool> IsValidDeleteUser(IUserRepository repository, string nickname);

        Task<bool> IsValidCreateNews(INewsRepository repository, News news, string city, string author, DateTime date);

        Task<bool> IsValidGetNews(INewsRepository repository, string city, DateTime date, Guid id);

        Task<bool> IsValidUpdateNews(INewsRepository repository, string city, DateTime date, Guid id, string author);

        Task<bool> IsValidDeleteNews(INewsRepository repository, string city, DateTime date, Guid id, string author);

        Task<bool> IsValidGetHotNews(IHotNewsRepository repository, string city, DateTime date);

        Task<bool> IsValidGetImageNews(INewsImageRepository repository, string city, DateTime date, Guid id);

        Task<bool> IsValidGetThumbnailImageNews(INewsImageRepository repository, string city, DateTime date, Guid id);
    }
}
