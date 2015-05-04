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
        private const string HotNewsRootTemplate = "api/hot/{city}";

        public const string GetUserTemplate = UserRootTemplate + "/{userId}";
        public const string PostUserTemplate = UserRootTemplate;
        public const string PatchUserTemplate = UserRootTemplate + "/{userId}";
        public const string DeleteUserTemplate = UserRootTemplate + "/{userId}";

        public const string GetNewsTemplate = NewsRootTemplate + "/news/{newsId}";
        public const string PostNewsTemplate = NewsRootTemplate;
        public const string PatchNewsTemplate = NewsRootTemplate + "/news/{newsId}";
        public const string DeleteNewsTemplate = NewsRootTemplate + "/news/{newsId}";

        public const string GetHotNewsTemplate = HotNewsRootTemplate;

        public const string PostNewsImageTemplate = GetNewsTemplate + "/image";

        private static string BuildUserUri(string userId)
        {
            return string.Format("{0}/{1}", UserRootTemplate, userId);
        }

        public static string BuildAbsoluteUserUri(string scheme, string host, int port, string userId)
        {
            return string.Format("{0}://{1}:{2}/{3}", scheme, host, port, GOUriBuilder.BuildUserUri(userId));
        }

        private static string BuildCreateUserUri()
        {
            return UserRootTemplate;
        }

        public static string BuildCreateAbsoluteUserUri(string scheme, string host, int port)
        {
            return string.Format("{0}://{1}:{2}/{3}", scheme, host, port, GOUriBuilder.BuildCreateUserUri());
        }

        private static string BuildDiaryEntryUri(string city, string date)
        {
            string result = NewsRootTemplate.Replace("{city}", city);

            return result.Replace("{date}", date);
        }

        public static string BuildAbsoluteDiaryEntryUri(string scheme, string host, int port, string city, string date)
        {
            return string.Format("{0}://{1}:{2}/{3}", scheme, host, port, GOUriBuilder.BuildDiaryEntryUri(city, date));
        }

        private static string BuildNewsUri(string city, string date, string newsId)
        {
            string result = NewsRootTemplate.Replace("{city}", city);
            result = result.Replace("{date}", date);

            return string.Format("{0}/news/{1}", result, newsId);
        }

        public static string BuildAbsoluteNewsUri(string scheme, string host, int port, string city, string date, string newsId)
        {
            return string.Format("{0}://{1}:{2}/{3}", scheme, host, port, GOUriBuilder.BuildNewsUri(city, date, newsId));
        }
    }
}
