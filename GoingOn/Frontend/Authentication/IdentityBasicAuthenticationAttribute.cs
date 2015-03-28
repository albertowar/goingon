// ****************************************************************************
// <copyright file="IdentityBasicAuthenticationAttribute.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Authentication
{
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using GoingOn.Frontend.Entities;
    using GoingOn.Storage;
    using Microsoft.Practices.Unity;

    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        [Dependency]
        public IUserStorage Storage { get; set; }

        protected override async Task<IPrincipal> AuthenticateAsync(string nickname, string password, CancellationToken cancellationToken)
        {
            bool containsUserTask = await this.Storage.ContainsUser(User.ToUserBll(new User { Nickname = nickname, Password = password }));

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