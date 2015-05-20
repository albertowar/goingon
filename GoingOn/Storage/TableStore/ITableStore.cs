// ****************************************************************************
// <copyright file="ITableStore.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// News Storage base interface
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy.TableStore
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GoingOn.XStoreProxy;
    using Microsoft.WindowsAzure.Storage.Table;

    public interface ITableStore
    {
        /// <summary>
        /// Adds the entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <exception cref="AzureTableStorageException">If the entity is already in the database.</exception>
        Task AddTableEntity(ITableEntity entity);

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <exception cref="AzureTableStorageException">If the entity is not in the database.</exception>
        Task UpdateTableEntity<T>(T entity) where T : ITableEntity, new();

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <exception cref="AzureTableStorageException">If the entity is not in the database.</exception>
        Task<T> GetTableEntity<T>(string partitionKey, string rowKey) where T : ITableEntity, new();

        /// <summary>
        /// List entities inside a partition (the entire partition).
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <exception cref="AzureTableStorageException">If the entity is not in the database.</exception>
        Task<IEnumerable<T>> ListTableEntity<T>(string partitionKey) where T : ITableEntity, new();

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// /// <exception cref="AzureTableStorageException">If the entity is not in the database.</exception>
        Task DeleteTableEntity<T>(string partitionKey, string rowKey) where T : ITableEntity, new();

        /// <summary>
        /// Deletes all entities in the partition.
        /// </summary>
        Task DeleteAllTableEntitiesInPartition<T>(string partitionKey) where T : ITableEntity, new();
    }
}
