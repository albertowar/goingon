// ****************************************************************************
// <copyright file="IUserStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage
{
    using System.Threading.Tasks;

    using Model.EntitiesBll;

    public interface IUserStorage
    {
        Task AddUser(UserBll userBll);
        Task<UserBll> GetUser(string nickname);
        Task<bool> ContainsUser(UserBll userBll);
        Task UpdateUser(UserBll userBll);
        Task DeleteUser(UserBll userBll);
        Task DeleteAllUsers();
    }
}