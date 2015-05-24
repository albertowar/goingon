// ****************************************************************************
// <copyright file="IApiInputValidationChecks.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Input validation interface
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Validation
{
    using System;
    using System.Net.Http.Headers;
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

        void ValidateDiaryEntryParameters(string city, string date);

        void ValidateNewsParameters(string city, string date, string newsId);

        void ValidateImage(byte[] imageBytes, MediaTypeHeaderValue contentType);
    }
}