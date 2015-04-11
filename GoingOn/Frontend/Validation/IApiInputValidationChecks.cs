// ****************************************************************************
// <copyright file="IApiInputValidationChecks.cs" company="Universidad de Malaga">
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

    using GoingOn.Frontend.Entities;
    using GoingOn.Frontend.Entities;

    public interface IApiInputValidationChecks
    {
        bool IsValidUser(User user);

        bool IsValidNickName(string nickName);

        bool IsValidPassword(string password);

        bool IsValidCity(string city);

        bool IsValidName(string name);

        bool IsValidEmail(string email);

        bool IsValidBirthDate(DateTime birthDate);

        bool IsValidNews(News news);

        bool IsValidNewsId(string id);

        bool IsValidNewsDate(string date);
    }
}