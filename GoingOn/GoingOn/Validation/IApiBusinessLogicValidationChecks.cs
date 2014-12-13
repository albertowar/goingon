// ****************************************************************************
// <copyright file="IApiBusinessValidationChecks.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using GoingOn.Entities;

namespace GoingOn.Validation
{
    public interface IApiBusinessLogicValidationChecks
    {
        bool IsValidUser(IUserStorage storage, User user);
    }
}
