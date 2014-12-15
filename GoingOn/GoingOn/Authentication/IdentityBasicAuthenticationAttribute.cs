// ****************************************************************************
// <copyright file="IdentityBasicAuthenticationAttribute.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Authentication
{
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using GoingOn.Entities;
    using MemoryStorage;

    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        protected override async Task<IPrincipal> AuthenticateAsync(string nickname, string password, CancellationToken cancellationToken)
        {
            IUserStorage storage = UserMemoryStorage.GetInstance();

            if (storage.ContainsUser(User.ToUserBll(new User(nickname, password))))
            {
                cancellationToken.ThrowIfCancellationRequested();

                ClaimsIdentity identity = new ClaimsIdentity(null, "Basic");
                return new ClaimsPrincipal(identity);
            }

            // No user with userName/password exists.
            return null;
        }
    }
}