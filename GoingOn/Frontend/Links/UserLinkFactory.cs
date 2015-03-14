// ****************************************************************************
// <copyright file="UserLinkFactory.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Links
{
    using System.Net.Http;
    using Frontend.Controllers;

    public class UserLinkFactory : LinkFactory<UserController>
    {
        public UserLinkFactory(HttpRequestMessage message) : base(message)
        {
        }
    }
}