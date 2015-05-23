// ****************************************************************************
// <copyright file="ApiBusinessValidationChecks.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Business validation
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Validation
{
    using System;
    using System.Threading.Tasks;
    using GoingOn.Frontend.Entities;
    using GoingOn.Repository;

    public class ApiBusinessLogicValidationChecks : IApiBusinessLogicValidationChecks
    {
        public async Task<bool> IsValidCreateUser(IUserRepository repository, User user)
        {
            return !await this.IsUserStored(repository, user);
        }

        public async Task<bool> IsValidGetUser(IUserRepository repository, string nickname)
        {
            return await this.IsUserStored(repository, new User { Nickname = nickname });
        }

        public async Task<bool> IsValidUpdateUser(IUserRepository repository, User user)
        {
            return await this.IsUserStored(repository, user);
        }

        public bool IsAuthorizedUser(string requesterNickname, string userNickname)
        {
            return string.Equals(requesterNickname, userNickname);
        }

        public async Task<bool> IsValidDeleteUser(IUserRepository repository, string nickname)
        {
            return await this.IsUserStored(repository, new User { Nickname = nickname });
        }

        public async Task<bool> IsValidCreateNews(INewsRepository repository, News news, string city, string author, DateTime date)
        {
            return !await this.IsNewsStored(repository, news, city, author, date);
        }

        public async Task<bool> IsValidGetImageNews(INewsImageRepository repository, string city, DateTime date, Guid id)
        {
            return !await repository.ContainsImage(city, date, id);
        }

        public async Task<bool> IsValidGetThumbnailImageNews(INewsImageRepository repository, string city, DateTime date, Guid id)
        {
            return !await repository.ContainsImageThumbnail(city, date, id);
        }

        public async Task<bool> IsValidGetNews(INewsRepository repository, string city, DateTime date, Guid id)
        {
            return await repository.ContainsNews(city, date, id);
        }

        public async Task<bool> IsValidUpdateNews(INewsRepository repository, string city, DateTime date, Guid id, string author)
        {
            return await repository.IsAuthorOf(city, date, id, author);
        }

        public async Task<bool> IsValidDeleteNews(INewsRepository repository, string city, DateTime date, Guid id, string author)
        {
            return await repository.IsAuthorOf(city, date, id, author);
        }

        public async Task<bool> IsValidGetHotNews(IHotNewsRepository repository, string city, DateTime date)
        {
            return await repository.ContainsAnyHotNews(city, date);
        }

        #region Helper methods

        private async Task<bool> IsUserStored(IUserRepository repository, User user)
        {
            return await repository.ContainsUser(User.ToUserBll(user));
        }

        private async Task<bool> IsNewsStored(INewsRepository repository, News news, string city, string author, DateTime date)
        {
            return await repository.ContainsNewsCheckContent((News.ToNewsBll(news, city, author, date)));
        }

        #endregion
    }
}