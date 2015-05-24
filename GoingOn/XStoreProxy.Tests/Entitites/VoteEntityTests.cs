// ****************************************************************************
// <copyright file="NewsEntity.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Tests for News entity
// </summary>
// ****************************************************************************

namespace GoingOn.XStoreProxy.Tests.Entitites
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VoteEntityTests
    {
        private static readonly Guid id = Guid.NewGuid();
        private static readonly DateTime date = DateTime.Parse("2015-05-04");
        private const string city = "Malaga";
    }
}
