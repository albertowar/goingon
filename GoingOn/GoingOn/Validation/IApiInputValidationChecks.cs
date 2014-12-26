// ****************************************************************************
// <copyright file="IApiInputValidationChecks.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Validation
{
    using GoingOn.Entities;

    public interface IApiInputValidationChecks
    {
        bool IsValidUser(User user);

        bool IsValidNickName(string nickName);

        bool IsValidPassword(string password);

        bool IsValidNews(News news);

        bool IsValidNewsId(string id);
    }
}