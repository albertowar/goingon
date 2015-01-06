// ****************************************************************************
// <copyright file="UserDocumentDBStorage.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Model.EntitiesBll;
using Storage.DocumentDBRepository.Entities;

namespace Storage.DocumentDBRepository
{
    public class UserDocumentDBStorage : IUserStorage
    {
        // Singleton pattern
        private static UserDocumentDBStorage instance;

        // The DocumentClient instance that allows the code to run methods that interact with the DocumentDB service
        private static DocumentClient client;
        // Retrieve the desired database id (name) from the configuration file
        private static readonly string databaseId = ConfigurationManager.AppSettings["DatabaseId"];
        // Retrieve the DocumentDB URI from the configuration file
        private static readonly string endpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
        // Retrieve the DocumentDB Authorization Key from the configuration file
        private static readonly string authorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];

        private const string CollectionID = "UserStorage";

        private UserDocumentDBStorage()
        {
            client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            RetrieveOrCreateDatabaseAsync(databaseId).Wait();
        }

        public static UserDocumentDBStorage GetInstance()
        {
            if (UserDocumentDBStorage.instance == null)
            {
                UserDocumentDBStorage.instance = new UserDocumentDBStorage();
            }

            return UserDocumentDBStorage.instance;
        }

        public async Task AddUser(UserBll userBll)
        {
            // Try to retrieve a Database if exists, else create the Database
            var database = await RetrieveOrCreateDatabaseAsync(databaseId);

            // Try to retrieve a Document Collection, else create the Document Collection
            var collection = await RetrieveOrCreateCollectionAsync(database.SelfLink, CollectionID);

            if (collection.SelfLink != null)
            {
                var document = await client.CreateDocumentAsync(collection.SelfLink, UserDocumentDB.FromUserBll(userBll));
            }
        }

        public Task<UserBll> GetUser(string nickname)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ContainsUser(UserBll userBll)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateUser(UserBll userBll)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteUser(UserBll userBll)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAllUser()
        {
            throw new System.NotImplementedException();
        }

        #region Helper methods

        private static async Task<Database> RetrieveOrCreateDatabaseAsync(string id)
        {
            // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
            var database = client.CreateDatabaseQuery().Where(db => db.Id == databaseId).AsEnumerable().FirstOrDefault();

            // If the previous call didn't return a Database, it is necessary to create it
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(new Database { Id = databaseId });
                Console.WriteLine("Created Database: id - {0} and selfLink - {1}", database.Id, database.SelfLink);
            }

            return database;
        }

        private static async Task<DocumentCollection> RetrieveOrCreateCollectionAsync(string databaseSelfLink, string id)
        {
            // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
            var collection = client.CreateDocumentCollectionQuery(databaseSelfLink).Where(c => c.Id == id).ToArray().FirstOrDefault();

            // If the previous call didn't return a Collection, it is necessary to create it
            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(databaseSelfLink, new DocumentCollection { Id = id });
            }

            return collection;
        }
 

        #endregion
    }
}
