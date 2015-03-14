// ****************************************************************************
// <copyright file="UriBuilder.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Common
{
    public class GOUriBuilder
    {
        private const string UserRootTemplate = "api/user";
        private const string NewsRootTemplate = "api/city/{city}/date/{date}";

        public const string GetUserTemplate = UserRootTemplate + "/{userId}";
        public const string PostUserTemplate = UserRootTemplate;
        public const string PatchUserTemplate = UserRootTemplate + "/{userId}";
        public const string DeleteUserTemplate = UserRootTemplate + "/{userId}";

        public const string GetNewsTemplate = NewsRootTemplate + "/news/{newsId}";
        public const string PostNewsTemplate = NewsRootTemplate;
        public const string PatchNewsTemplate = NewsRootTemplate + "/news/{newsId}";
        public const string DeleteNewsTemplate = NewsRootTemplate + "/news/{newsId}";

        public static string BuildUserUri(string userId)
        {
            return string.Format("{0}/{1}", UserRootTemplate, userId);
        }

        public static string BuildDiaryEntryUri(string city, string date)
        {
            var result = NewsRootTemplate.Replace("{city}", city);

            return result.Replace("{date}", date);
        }

        public static string BuildNewsUri(string city, string date, string newsId)
        {
            var result = NewsRootTemplate.Replace("{city}", city);
            result = result.Replace("{date}", date);

            return string.Format("{0}/news/{1}", result, newsId);
        }
    }
}
