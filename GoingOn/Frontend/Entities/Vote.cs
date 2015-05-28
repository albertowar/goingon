// ****************************************************************************
// <copyright file="Vote.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Encapsulats the user information as the user would create it
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Entities
{
    using GoingOn.Model.EntitiesBll;

    public class Vote
    {
        /// <summary>
        /// The value of the vote.
        /// </summary>
        public int Value { get; set; }

        public static VoteBll ToVoteBll(Vote vote)
        {
            return new VoteBll
            {
                Value = vote.Value
            };
        }

        public static Vote FromVoteBll(VoteBll voteBll)
        {
            return new Vote
            {
                Value = voteBll.Value
            };
        }
    }
}
