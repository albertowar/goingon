// ****************************************************************************
// <copyright file="ApiBusinessValidationChecks.cs" company="Universidad de Malaga">
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

    public class ApiBusinessLogicValidationChecks :  IApiBusinessLogicValidationChecks
    {
        public async Task<bool> IsValidCreateUser(IUserStorage storage, User user)
        {
            return !await this.IsUserStored(storage, user);
        }

        public async Task<bool> IsValidGetUser(IUserStorage storage, string nickname)
        {
            return await this.IsUserStored(storage, new User { Nickname = nickname });
        }

        public async Task<bool> IsValidUpdateUser(IUserStorage storage, User user)
        {
            return await this.IsUserStored(storage, user);
        }

        public bool IsAuthorizedUser(string requesterNickname, string userNickname)
        {
            return string.Equals(requesterNickname, userNickname);
        }

        public async Task<bool> IsValidDeleteUser(IUserStorage storage, string nickname)
        {
            return await this.IsUserStored(storage, new User { Nickname = nickname });
        }

        public bool IsValidCreateNews(INewsStorage storage, News news, string city, string author, DateTime date)
        {
            return !this.IsNewsStored(storage, news, city, author, date);
        }

        public async Task<bool> IsValidGetNews(INewsStorage storage, string city, DateTime date, Guid id)
        {
            return await storage.Exists(city, date, id);
        }

        public async Task<bool> IsValidUpdateNews(INewsStorage storage, string city, DateTime date, Guid id, string author)
        {
            return await storage.IsAuthorOf(city, date, id, author);
        }

        public async Task<bool> IsValidDeleteNews(INewsStorage storage, string city, DateTime date, Guid id, string author)
        {
            return await storage.IsAuthorOf(city, date, id, author);
        }

        #region Helper methods

        private async Task<bool> IsUserStored(IUserStorage storage,  User user)
        {
            return await storage.ContainsUser(User.ToUserBll(user));
        }

        private bool IsNewsStored(INewsStorage storage, News news, string city, string author, DateTime date)
        {
            return storage.ContainsNews(News.ToNewsBll(news, city, author, date)).Result;
        }

        #endregion
    }
}