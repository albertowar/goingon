// ****************************************************************************
// <copyright file="UserTableStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace Storage.TableStorage
{
    using System.Configuration;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    using Model.EntitiesBll;
    using Storage.TableStorage.Entities;

    using TableStorageException = Microsoft.WindowsAzure.Storage.StorageException;

    public class UserTableStorage : IUserStorage
    {
        // Configuration info
        private static readonly string TableName = ConfigurationManager.AppSettings["UserTableName"];
        private static readonly string StorageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

        // Singleton pattern
        private static UserTableStorage instance;

        // Retrieve the storage account from the connection string.
        private readonly CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

        private UserTableStorage()
        {
        }

        public static UserTableStorage GetInstance()
        {
            if (UserTableStorage.instance == null)
            {
                UserTableStorage.instance = new UserTableStorage();
            }

            return UserTableStorage.instance;
        }

        public async Task AddUser(UserBll userBll)
        {
            await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var user = UserEntity.FromUserBll(userBll);

                var insertOperation = TableOperation.Insert(user);

                table.Execute(insertOperation);
            });
        }

        public async Task<UserBll> GetUser(string nickname)
        {
            return await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var retrieveOperation = TableOperation.Retrieve<UserEntity>("World", nickname);

                var retrievedResult = table.Execute(retrieveOperation);

                if (retrievedResult.Result == null)
                {
                    throw new Storage.StorageException("The user is not in the database");
                }

                return UserEntity.ToUserBll(retrievedResult.Result as UserEntity);
            });
        }

        public async Task<bool> ContainsUser(UserBll userBll)
        {
            return await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var retrieveOperation = TableOperation.Retrieve<UserEntity>("World", userBll.Nickname);

                var retrievedResult = table.Execute(retrieveOperation);

                if (retrievedResult.Result == null)
                {
                    throw new Storage.StorageException("The user is not in the database");
                }

                return retrievedResult.Result != null;
            });
        }

        public async Task UpdateUser(UserBll userBll)
        {
            await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var retrieveOperation = TableOperation.Retrieve<UserEntity>("World", userBll.Nickname);

                var retrievedResult = table.Execute(retrieveOperation);

                var updateEntity = retrievedResult.Result as UserEntity;

                if (updateEntity != null)
                {
                    updateEntity.Password = userBll.Password;

                    var insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);

                    table.Execute(insertOrReplaceOperation);
                }
            });
        }

        public async Task DeleteUser(UserBll userBll)
        {
            await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var retrieveOperation = TableOperation.Retrieve<UserEntity>("World", userBll.Nickname);

                var retrievedResult = table.Execute(retrieveOperation);

                var deleteEntity = retrievedResult.Result as UserEntity;

                if (deleteEntity != null)
                {
                    var deleteOperation = TableOperation.Delete(deleteEntity);

                    table.Execute(deleteOperation);
                }
            });
        }

        public async Task DeleteAllUsers()
        {
            await Task.Run(() =>
            {
                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference(TableName);

                var query = new TableQuery<UserEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "World"));

                foreach (var entity in table.ExecuteQuery(query))
                {
                    var deleteOperation = TableOperation.Delete(entity);
                    table.Execute(deleteOperation);
                }
            });
        }
    }
}
