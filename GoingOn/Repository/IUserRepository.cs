// ****************************************************************************
// <copyright file="IUserRepository.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// User Storage interface
// </summary>
// ****************************************************************************

namespace GoingOn.Repository
{
    using System.Threading.Tasks;
    using GoingOn.Model.EntitiesBll;

    public interface IUserRepository
    {
        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="userBll">The user BLL.</param>
        /// <exception cref="AzureTableStorageException">If the user is already in the database.</exception>
        Task AddUser(UserBll userBll);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="city"></param>
        /// <param name="nickname">The nickname.</param>
        /// /// <exception cref="AzureTableStorageException">If the user is not in the database.</exception>
        Task<UserBll> GetUser(string city, string nickname);

        /// <summary>
        /// Determines whether the specified user BLL contains user.
        /// </summary>
        /// <param name="userBll">The user BLL.</param>
        Task<bool> ContainsUser(UserBll userBll);

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="userBll">The user BLL.</param>
        Task UpdateUser(UserBll userBll);

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userBll">The user BLL.</param>
        /// /// <exception cref="AzureTableStorageException">If the entity is not in the database.</exception>
        Task DeleteUser(UserBll userBll);

        /// <summary>
        /// Deletes all users.
        /// </summary>
        Task DeleteAllUsers(string city);
    }
}