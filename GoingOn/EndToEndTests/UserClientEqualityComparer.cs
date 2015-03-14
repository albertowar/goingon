// ****************************************************************************
// <copyright file="UserClientEqualityComparer.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.EndToEndTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using GoingOn.Client.Entities;

    [ExcludeFromCodeCoverage]
    public class UserClientEqualityComparer : IEqualityComparer<UserClient>
    {
        public bool Equals(UserClient user1, UserClient user2)
        {
            return
                string.Equals(user1.Nickname, user2.Nickname) &&
                string.Equals(user1.Password, user2.Password);
        }

        public int GetHashCode(UserClient obj)
        {
            throw new NotImplementedException();
        }
    }
}
