// ****************************************************************************
// <copyright file="VoteEntity.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Encapsulates the vote information that will be stored
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy.Entities
{
    using System;
    using GoingOn.Model.EntitiesBll;
    using Microsoft.WindowsAzure.Storage.Table;

    public class VoteEntity : TableEntity
    {
        /// <summary>
        /// The value of the vote
        /// </summary>
        public int Value { get; set; }

        public static VoteEntity FromVoteBll(string city, DateTime date, Guid id, string username, VoteBll voteBll)
        {
            return new VoteEntity
            {
                PartitionKey = VoteEntity.BuildPartitionkey(city, date),
                RowKey = VoteEntity.BuildRowKey(id, username),
                Value = voteBll.Value
            };
        }

        public static VoteBll ToVoteBll(VoteEntity voteEntity)
        {
            return new VoteBll
            {
                Value = voteEntity.Value
            };
        }

        public override bool Equals(object anotherVoteObject)
        {
            var anotherVote = anotherVoteObject as VoteEntity;

            return
                anotherVote != null
                && string.Equals(this.PartitionKey, anotherVote.PartitionKey)
                && this.RowKey.Equals(anotherVote.RowKey);
        }

        public override int GetHashCode()
        {
            return
                this.PartitionKey.GetHashCode()
                ^ this.RowKey.GetHashCode();
        }

        public void Merge(VoteEntity voteEntity)
        {
            if (this.Equals(voteEntity))
            {
                this.Value = voteEntity.Value;
            }
        }

        public static string BuildPartitionkey(string city, DateTime date)
        {
            return string.Format("{0};{1}", city, date.ToString("yyyy-MM-dd"));
        }

        public static Tuple<string, DateTime> ExtractFromPartitionKey(string partitionKey)
        {
            string[] values = partitionKey.Split(';');

            return new Tuple<string, DateTime>(values[0], DateTime.Parse(values[1]));
        }

        public static string BuildRowKey(Guid id, string username)
        {
            return string.Format("VOTE;{0};{1}", id, username);
        }

        public static Tuple<Guid, string> ExtractFromRowKey(string rowKey)
        {
            string[] values = rowKey.Split(';');

            return new Tuple<Guid, string>(Guid.Parse(values[1]), values[2]);
        }
    }
}
