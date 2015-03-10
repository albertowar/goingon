// ****************************************************************************
// <copyright file="ApiBusinessValidationChecks.cs" company="Universidad de Malaga">
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

    public class ApiBusinessLogicValidationChecks :  IApiBusinessLogicValidationChecks
    {
        public bool IsValidCreateUser(IUserStorage storage, User user)
        {
            return !this.IsUserStored(storage, user);
        }

        public bool IsValidGetUser(IUserStorage storage, string nickname)
        {
            return this.IsUserStored(storage, new User { Nickname = nickname });
        }

        public bool IsValidUpdateUser(IUserStorage storage, User user)
        {
            return this.IsUserStored(storage, user);
        }

        public bool IsAuthorizedUser(string requesterNickname, string userNickname)
        {
            return string.Equals(requesterNickname, userNickname);
        }

        public bool IsValidDeleteUser(IUserStorage storage, string nickname)
        {
            return
                this.IsUserStored(storage, new User { Nickname = nickname });
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

        private bool IsUserStored(IUserStorage storage,  User user)
        {
            return storage.ContainsUser(User.ToUserBll(user)).Result;
        }

        private bool IsNewsStored(INewsStorage storage, News news, string city, string author, DateTime date)
        {
            return storage.ContainsNews(News.ToNewsBll(news, city, author, date)).Result;
        }

        #endregion
    }
}