﻿// ****************************************************************************
// <copyright file="ApiValidationChecks.cs" company="Universidad de Malaga">
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

    public class ApiInputValidationChecks : IApiInputValidationChecks
    {
        public ApiInputValidationChecks()
        {
            
        }

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
    }
}