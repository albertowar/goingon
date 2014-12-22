// ****************************************************************************
// <copyright file="UserCompleteEqualityComparer.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using GoingOn.Entities;

    [ExcludeFromCodeCoverage]
    public class UserCompleteEqualityComparer : IEqualityComparer<User>
    {
        public bool Equals(User user1, User user2)
        {
            return
                string.Equals(user1.Nickname, user2.Nickname) &&
                string.Equals(user1.Password, user2.Password);
        }

        public int GetHashCode(User obj)
        {
            throw new NotImplementedException();
        }
    }
}
