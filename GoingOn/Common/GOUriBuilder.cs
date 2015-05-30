// ****************************************************************************
// <copyright file="UriBuilder.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Utility class to generate URIs
// </summary>
// ****************************************************************************

namespace GoingOn.Common
{
    public class GOUriBuilder
    {
        private const string UserRootTemplate = "api/user";
        private const string CityRootTemplate = "api/city/{city}";
        private const string NewsRootTemplate = CityRootTemplate + "/date/{date}";
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

        public const string NewsImageTemplate = GetNewsTemplate + "/image";

        public const string NewsImageThumbnailTemplate = GetNewsTemplate + "/thumbnail";

        public const string NewsVoteTemplate = GetNewsTemplate + "/vote";

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

        public static string BuildAbsoluteNewsImageUri(string scheme, string host, int port, string city, string date, string newsId)
        {
            return string.Format("{0}/image", GOUriBuilder.BuildAbsoluteNewsUri(scheme, host, port, city, date, newsId));
        }

        public static string BuildAbsoluteNewsThumbnailImageUri(string scheme, string host, int port, string city, string date, string newsId)
        {
            return string.Format("{0}/thumbnail", GOUriBuilder.BuildAbsoluteNewsUri(scheme, host, port, city, date, newsId));
        }
    }
}
