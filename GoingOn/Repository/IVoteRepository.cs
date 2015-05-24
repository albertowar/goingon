// ****************************************************************************
// <copyright file="IVoteRepository.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Vote repository
// </summary>
// ****************************************************************************

namespace GoingOn.Repository
{
    using System;
    using System.Threading.Tasks;
    using GoingOn.Model.EntitiesBll;

    public interface IVoteRepository
    {
        Task AddVote(string city, DateTime date, Guid id, string nickname, VoteBll votingBll);

        Task UpdateVote(string city, DateTime date, Guid id, string nickname, VoteBll votingBll);

        Task<bool> ContainsVote(string city, DateTime date, Guid id, string nickname);

        Task<VoteBll> GetVote(string city, DateTime date, Guid id, string nickname);

        Task DeleteVote(string city, DateTime date, Guid id, string nickname);
    }
}
