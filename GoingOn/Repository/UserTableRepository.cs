// ****************************************************************************
// <copyright file="UserTableRepository.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Manages the repository of user data
// </summary>
// ****************************************************************************

namespace GoingOn.Repository
{
    using System.Threading.Tasks;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.Repository.Entities;
    using GoingOn.XStoreProxy;
    using GoingOn.XStoreProxy.TableStore;

    public class UserTableRepository : IUserRepository
    {
        private readonly ITableStore tableStore;

        public UserTableRepository(string connectionString, string tableName)
        {
            this.tableStore = new TableStore(connectionString, tableName);
        }

        public async Task AddUser(UserBll userBll)
        {
            await this.tableStore.AddTableEntity(UserEntity.FromUserBll(userBll));
        }

        public async Task<UserBll> GetUser(string city, string nickname)
        {
            return UserEntity.ToUserBll(await this.tableStore.GetTableEntity<UserEntity>(city, nickname));
        }

        public async Task<bool> ContainsUser(UserBll userBll)
        {
            try
            {
                await this.tableStore.GetTableEntity<UserEntity>(userBll.City, userBll.Nickname);

                return true;
            }
            catch (AzureTableStorageException)
            {
                return false;
            }
        }

        public async Task UpdateUser(UserBll userBll)
        {
            await this.tableStore.UpdateTableEntity(UserEntity.FromUserBll(userBll));
        }

        public async Task DeleteUser(UserBll userBll)
        {
            await this.tableStore.DeleteTableEntity<UserEntity>(userBll.City, userBll.Nickname);
        }

        public async Task DeleteAllUsers(string city)
        {
            await this.tableStore.DeleteAllTableEntitiesInPartition<UserEntity>(city);
        }
    }
}
