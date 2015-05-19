// ****************************************************************************
// <copyright file="HttpAuthenticationChallengeContextExtensions.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// HttpAuthenticationChallengeContextExtensions class
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Authentication
{
    using System;
    using System.Net.Http.Headers;
    using System.Web.Http.Filters;

    public static class HttpAuthenticationChallengeContextExtensions
    {
        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme)
        {
            HttpAuthenticationChallengeContextExtensions.ChallengeWith(context, new AuthenticationHeaderValue(scheme));
        }

        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme, string parameter)
        {
            HttpAuthenticationChallengeContextExtensions.ChallengeWith(context, new AuthenticationHeaderValue(scheme, parameter));
        }

        private static void ChallengeWith(this HttpAuthenticationChallengeContext context, AuthenticationHeaderValue challenge)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
        }
    }
}
