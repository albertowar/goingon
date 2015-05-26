// ****************************************************************************
// <copyright file="VoteRepository.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Vote repository interface
// </summary>
// ****************************************************************************

namespace GoingOn.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.XStoreProxy.Entities;
    using GoingOn.XStoreProxy.TableStore;

    public class VoteRepository : IVoteRepository
    {
        private readonly ITableStore tableTableStore;

        public VoteRepository(ITableStore tableTableStore)
        {
            this.tableTableStore = tableTableStore;
        }

        public async Task AddVote(string city, DateTime date, Guid id, string nickname, VoteBll votingBll)
        {
            await this.tableTableStore.AddTableEntity(VoteEntity.FromVoteBll(city, date, id, nickname, votingBll));
        }

        public async Task UpdateVote(string city, DateTime date, Guid id, string nickname, VoteBll votingBll)
        {
            await this.tableTableStore.UpdateTableEntity(VoteEntity.FromVoteBll(city, date, id, nickname, votingBll));
        }

        public async Task<bool> ContainsVote(string city, DateTime date, Guid id, string nickname)
        {
            List<VoteEntity> newsList = (await this.tableTableStore.ListTableEntity<VoteEntity>(VoteEntity.BuildPartitionkey(city, date))).ToList();

            return newsList.Any();
        }

        public async Task<VoteBll> GetVote(string city, DateTime date, Guid id, string nickname)
        {
            return VoteEntity.ToVoteBll(await this.tableTableStore.GetTableEntity<VoteEntity>(VoteEntity.BuildPartitionkey(city, date), VoteEntity.BuildRowKey(id, nickname)));
        }

        public async Task DeleteVote(string city, DateTime date, Guid id, string nickname)
        {
            await this.tableTableStore.DeleteTableEntity<VoteEntity>(VoteEntity.BuildPartitionkey(city, date), VoteEntity.BuildRowKey(id, nickname));
        }
    }
}