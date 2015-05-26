// ****************************************************************************
// <copyright file="VoteEntityTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Tests for Vote entity
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy.Tests.Entitites
{
    using System;
    using GoingOn.Model.EntitiesBll;
    using GoingOn.XStoreProxy.Entities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VoteEntityTests
    {
        private static readonly Guid id = Guid.NewGuid();
        private static readonly DateTime date = DateTime.Parse("2015-05-04");
        private const string City = "Malaga";
        private const string Nickname = "Alberto";

        [TestMethod]
        public void TestFromVoteBll()
        {
            var voteBll = new VoteBll
            {
                Value = 5
            };

            VoteEntity voteEntity = VoteEntity.FromVoteBll(City, date, id, Nickname, voteBll);
            Assert.AreEqual(string.Format("{0};{1}", City, date.ToString("yyyy-MM-dd")), voteEntity.PartitionKey);
            Assert.AreEqual(string.Format("VOTE;{0};{1}", id, Nickname), voteEntity.RowKey);
            Assert.AreEqual(5, voteEntity.Value);
        }

        [TestMethod]
        public void TestToVoteBll()
        {
            var voteEntity = new VoteEntity
            {
                PartitionKey = VoteEntity.BuildPartitionkey(City, date),
                RowKey = VoteEntity.BuildRowKey(id, Nickname),
                Value = 5
            };

            VoteBll voteBll = VoteEntity.ToVoteBll(voteEntity);
            Assert.AreEqual(5, voteBll.Value);
        }

        [TestMethod]
        public void TestEquals()
        {
            var voteEntity1 = new VoteEntity
            {
                PartitionKey = VoteEntity.BuildPartitionkey(City, date),
                RowKey = VoteEntity.BuildRowKey(id, Nickname)
            };

            var voteEntity2 = new VoteEntity
            {
                PartitionKey = VoteEntity.BuildPartitionkey(City, date),
                RowKey = VoteEntity.BuildRowKey(id, Nickname)
            };

            Assert.IsTrue(voteEntity1.Equals(voteEntity2));
        }

        [TestMethod]
        public void TestMerge()
        {
            var oldVoteEntity = new VoteEntity
            {
                PartitionKey = VoteEntity.BuildPartitionkey(City, date),
                RowKey = VoteEntity.BuildRowKey(id, Nickname),
                Value = 4
            };

            var newVoteEntity = new VoteEntity
            {
                PartitionKey = VoteEntity.BuildPartitionkey(City, date),
                RowKey = VoteEntity.BuildRowKey(id, Nickname),
                Value = 5
            };

            oldVoteEntity.Merge(newVoteEntity);

            Assert.AreEqual(newVoteEntity, oldVoteEntity);
        }

        [TestMethod]
        public void TestBuildPartitionKey()
        {
            Assert.AreEqual(string.Format("{0};{1}", City, date.ToString("yyyy-MM-dd")), VoteEntity.BuildPartitionkey(City, date));
        }

        [TestMethod]
        public void TestExtractFromPartitionKey()
        {
            var voteEntity = new VoteEntity
            {
                PartitionKey = VoteEntity.BuildPartitionkey(City, date),
                RowKey = VoteEntity.BuildRowKey(id, Nickname)
            };

            Tuple<string, DateTime> tuple = VoteEntity.ExtractFromPartitionKey(voteEntity.PartitionKey);
            
            Assert.AreEqual(City, tuple.Item1);
            Assert.AreEqual(date, tuple.Item2);
        }

        [TestMethod]
        public void TestBuildRoweKey()
        {
            Assert.AreEqual(string.Format("VOTE;{0};{1}", id, Nickname), VoteEntity.BuildRowKey(id, Nickname));
        }

        [TestMethod]
        public void ExtractFromRowKey()
        {
            var voteEntity = new VoteEntity
            {
                PartitionKey = VoteEntity.BuildPartitionkey(City, date),
                RowKey = VoteEntity.BuildRowKey(id, Nickname),
                Value = 5
            };

            Tuple<Guid, string> tuple = VoteEntity.ExtractFromRowKey(voteEntity.RowKey);
            Assert.AreEqual(id, tuple.Item1);
            Assert.AreEqual(Nickname, tuple.Item2);
        }
    }
}
