// ****************************************************************************
// <copyright file="UserCompleteEqualityComparer.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Equality comparer for User. It checks username and password.
// </summary>
// ****************************************************************************

namespace GoingOn.Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using GoingOn.Frontend.Entities;

    [ExcludeFromCodeCoverage]
    public class UserCompleteEqualityComparer : IEqualityComparer<User>
    {
        public bool Equals(User user1, User user2)
        {
            return
                string.Equals(user1.Nickname, user2.Nickname) &&
                string.Equals(user1.Password, user2.Password);
        }

        public int GetHashCode(User user)
        {
            return user.Nickname.GetHashCode() ^
                user.Password.GetHashCode();
        }
    }
}
