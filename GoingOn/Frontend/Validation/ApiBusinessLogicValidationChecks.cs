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
    using GoingOn.Storage;
    using GoingOn.Storage.TableStorage.Entities;

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

        public async Task<bool> IsValidCreateNews(INewsStorage storage, News news, string city, string author, DateTime date)
        {
            return !await this.IsNewsStored(storage, news, city, author, date);
        }

        public async Task<bool> IsValidGetImageNews(IImageStorage storage, string city, DateTime date, Guid id)
        {
            return !await storage.ContainsImage(city, date, id);
        }

        public Task<bool> IsValidGetThumbnailImageNews(IImageStorage storage, string city, DateTime date, Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsValidGetNews(INewsStorage storage, string city, DateTime date, Guid id)
        {
            return await storage.ContainsNews(city, date, id);
        }

        public async Task<bool> IsValidUpdateNews(INewsStorage storage, string city, DateTime date, Guid id, string author)
        {
            return await storage.IsAuthorOf(city, date, id, author);
        }

        public async Task<bool> IsValidDeleteNews(INewsStorage storage, string city, DateTime date, Guid id, string author)
        {
            return await storage.IsAuthorOf(city, date, id, author);
        }

        public async Task<bool> IsValidGetHotNews(IHotNewsStorage storage, string city, DateTime date)
        {
            return await storage.ContainsHotNews(city, date);
        }

        #region Helper methods

        private async Task<bool> IsUserStored(IUserStorage storage,  User user)
        {
            return await storage.ContainsUser(User.ToUserBll(user));
        }

        private async Task<bool> IsNewsStored(INewsStorage storage, News news, string city, string author, DateTime date)
        {
            return await storage.ContainsNewsCheckContent(NewsEntity.FromNewsBll(News.ToNewsBll(news, city, author, date)));
        }

        #endregion
    }
}