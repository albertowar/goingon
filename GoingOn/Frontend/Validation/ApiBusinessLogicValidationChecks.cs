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

    using Frontend.Entities;
    using Storage;

    public class ApiBusinessLogicValidationChecks : IApiBusinessLogicValidationChecks
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

        public bool IsValidCreateNews(INewsStorage storage, News news, string author)
        {
            return !this.IsNewsStored(storage, news, author);
        }

        public bool IsValidGetNews(INewsStorage storage, string id)
        {
            return storage.ContainsNews(Guid.Parse(id)).Result;
        }

        public bool IsValidUpdateNews(INewsStorage storage, string city, DateTime date, string id, string author)
        {
            return storage.ContainsNews(Guid.Parse(id), author).Result;
        }

        public bool IsValidDeleteNews(INewsStorage storage, string id, string author)
        {
            return storage.ContainsNews(Guid.Parse(id), author).Result;
        }

        #region Helper methods

        private bool IsUserStored(IUserStorage storage,  User user)
        {
            return storage.ContainsUser(User.ToUserBll(user)).Result;
        }

        private bool IsNewsStored(INewsStorage storage, News news, string author)
        {
            return storage.ContainsNews(News.ToNewsBll(news, author)).Result;
        }

        #endregion
    }
}