// ****************************************************************************
// <copyright file="UserRepository.cs" company="Universidad de Malaga">
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
    using GoingOn.XStoreProxy;
    using GoingOn.XStoreProxy.Entities;
    using GoingOn.XStoreProxy.TableStore;

    public class UserRepository : IUserRepository
    {
        private readonly ITableStore tableTableStore;

        public UserRepository(ITableStore tableTableStore)
        {
            this.tableTableStore = tableTableStore;
        }

        public async Task AddUser(UserBll userBll)
        {
            await this.tableTableStore.AddTableEntity(UserEntity.FromUserBll(userBll));
        }

        public async Task<UserBll> GetUser(string nickname)
        {
            return UserEntity.ToUserBll(await this.tableTableStore.GetTableEntityByPartitionKey<UserEntity>(nickname));
        }

        public async Task<bool> ContainsUser(UserBll userBll)
        {
            try
            {
                await this.tableTableStore.GetTableEntity<UserEntity>(userBll.City, userBll.Nickname);

                return true;
            }
            catch (AzureXStoreException)
            {
                return false;
            }
        }

        public async Task<UserBll> GetUserByNickname(string nickname)
        {
            return UserEntity.ToUserBll(await this.tableTableStore.GetTableEntityByPartitionKey<UserEntity>(nickname));
        }

        public async Task UpdateUser(UserBll userBll)
        {
            await this.tableTableStore.UpdateTableEntity(UserEntity.FromUserBll(userBll));
        }

        public async Task DeleteUser(UserBll userBll)
        {
            await this.tableTableStore.DeleteTableEntity<UserEntity>(userBll.City, userBll.Nickname);
        }

        public async Task DeleteAllUsers(string city)
        {
            await this.tableTableStore.DeleteAllTableEntitiesInPartition<UserEntity>(city);
        }
    }
}
