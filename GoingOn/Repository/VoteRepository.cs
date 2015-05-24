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
    using System.Threading.Tasks;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.XStoreProxy.TableStore;

    public class VoteRepository : IVoteRepository
    {
        private readonly ITableStore tableTableStore;

        public VoteRepository(ITableStore tableTableStore)
        {
            this.tableTableStore = tableTableStore;
        }

        public Task AddVote(string city, DateTime date, Guid id, string nickname, VoteBll votingBll)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVote(string city, DateTime date, Guid id, string nickname, VoteBll votingBll)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsVote(string city, DateTime date, Guid id, string nickname)
        {
            throw new NotImplementedException();
        }

        public Task<VoteBll> GetVote(string city, DateTime date, Guid id, string nickname)
        {
            throw new NotImplementedException();
        }

        public Task DeleteVote(string city, DateTime date, Guid id, string nickname)
        {
            throw new NotImplementedException();
        }
    }
}
