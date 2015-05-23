// ****************************************************************************
// <copyright file="IdentityBasicAuthenticationAttribute.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Implementation of the basic authentication attribute using user repository
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Authentication
{
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using GoingOn.Repository;
    using Microsoft.Practices.Unity;

    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        [Dependency]
        public IUserRepository Repository { get; set; }

        protected override async Task<IPrincipal> AuthenticateAsync(string nickname, string password, CancellationToken cancellationToken)
        {
            // TODO: improve the authentication mechanism to avoid full table scans
            bool containsUserTask = await this.Repository.ContainsUserInAnyCity(nickname, password);

            if (containsUserTask)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return new GenericPrincipal(new GenericIdentity(nickname), null);
            }

            // No user with userName/password exists.
            return null;
        }
    }
}