// ****************************************************************************
// <copyright file="UriBuilderTests.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Common.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GOUriBuilderTests
    {
        private const string Scheme = "scheme";
        private const string Host = "host";
        private const int Port = 123;

        [TestMethod]
        public void BuildAbsoluteUserUriTest()
        {
            Assert.AreEqual("scheme://host:123/api/user/alberto", GOUriBuilder.BuildAbsoluteUserUri(Scheme, Host, Port, "alberto"));
        }

        [TestMethod]
        public void BuildAbsoluteDiaryEntryUri()
        {
            Assert.AreEqual("scheme://host:123/api/city/Malaga/date/2015-12-05", GOUriBuilder.BuildAbsoluteDiaryEntryUri(Scheme, Host, Port, "Malaga", "2015-12-05"));
        }

        [TestMethod]
        public void BuildAbsoluteNewsUri()
        {
            string guid = Guid.NewGuid().ToString();

            Assert.AreEqual("scheme://host:123/api/city/Malaga/date/2015-12-05/news/" + guid, GOUriBuilder.BuildAbsoluteNewsUri(Scheme, Host, Port, "Malaga", "2015-12-05", guid));
        }

        [TestMethod]
        public void BuildCreateAbsoluteUseruri()
        {
            Assert.AreEqual("scheme://host:123/api/user", GOUriBuilder.BuildCreateAbsoluteUserUri(Scheme, Host, Port));
        }
    }
}
