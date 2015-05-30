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

    public interface IApiBusinessLogicValidationChecks
    {
        Task<bool> IsValidCreateUser(IUserRepository repository, User user);

        Task<bool> IsValidCreateNews(INewsRepository repository, News news, string city, string author, DateTime date);

        Task<bool> IsValidGetHotNews(IHotNewsRepository repository, string city, DateTime date);

        Task<bool> IsValidGetVote(IVoteRepository repository, string city, DateTime parse, Guid id, string author);

        bool IsAuthorizedUser(string requesterNickname, string userNickname);

        Task<bool> IsUserCreated(IUserRepository repository, string nickname);

        Task<bool> IsUserCreated(IUserRepository repository, User user);

        Task<bool> IsValidGetNews(INewsRepository repository, string city, DateTime date, Guid id);

        Task<bool> IsValidModifyNews(INewsRepository repository, string city, DateTime date, Guid id, string author);
        
        Task<bool> IsValidGetImageNews(IImageRepository repository, string city, DateTime date, Guid id);

        Task<bool> IsValidGetThumbnailImageNews(IImageRepository repository, string city, DateTime date, Guid id);
    }
}
