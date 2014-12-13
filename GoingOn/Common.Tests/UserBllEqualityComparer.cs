// ****************************************************************************
// <copyright file="UserBllEqualityComparer.cs" company="Universidad de Malaga">
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

    using Model.EntitiesBll;

    public class UserBllEqualityComparer : IEqualityComparer<UserBll>
    {
        public bool Equals(UserBll user1, UserBll user2)
        {
            return
                string.Equals(user1.Nickname, user2.Nickname);
        }

        public int GetHashCode(UserBll obj)
        {
            throw new NotImplementedException();
        }
    }
}
