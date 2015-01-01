// ****************************************************************************
// <copyright file="ApiValidationChecks.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System;

namespace GoingOn.Validation
{
    using GoingOn.Entities;

    public class ApiInputValidationChecks : IApiInputValidationChecks
    {
        public bool IsValidUser(User user)
        {
            return 
                user != null &&
                this.IsValidNickName(user.Nickname) &&
                this.IsValidPassword(user.Password);
        }

        public bool IsValidNickName(string nickName)
        {
            return !string.IsNullOrWhiteSpace(nickName);
        }

        public bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password);
        }

        public bool IsValidNews(News news)
        {
            return
                news != null &&
                news.Title != null && IsValidTitle(news.Title) &&
                news.Content != null && this.IsValidContent(news.Content);
        }

        public bool IsValidNewsId(string id)
        {
            Guid guid;
            return Guid.TryParse(id, out guid);
        }

        public bool IsValidTitle(string title)
        {
            return !string.IsNullOrWhiteSpace(title);
        }

        public bool IsValidContent(string content)
        {
            return !string.IsNullOrWhiteSpace(content);
        }
    }
}