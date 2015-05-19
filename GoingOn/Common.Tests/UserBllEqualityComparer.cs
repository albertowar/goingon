// ****************************************************************************
// <copyright file="UserBllEqualityComparer.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Equality comprarer for UserBll. It checks the nickname.
// </summary>
// ****************************************************************************

namespace GoingOn.Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Model.EntitiesBll;

    [ExcludeFromCodeCoverage]
    public class UserBllEqualityComparer : IEqualityComparer<UserBll>
    {
        public bool Equals(UserBll user1, UserBll user2)
        {
            return
                string.Equals(user1.Nickname, user2.Nickname);
        }

        public int GetHashCode(UserBll user)
        {
            return user.Nickname.GetHashCode();
        }
    }
}
