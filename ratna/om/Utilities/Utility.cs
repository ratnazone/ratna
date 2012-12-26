/*
    Copyright (c) 2012, Jardalu LLC. (http://jardalu.com)
        
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
  
    For complete licensing, see license.txt or visit http://ratnazone.com/v0.2/license.txt

*/
namespace Jardalu.Ratna.Utilities
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    
    #endregion

    public static class Utility
    {

        #region fields

        public const string EmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                   + "@"
                                   + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

        public const string DomainPattern = @"^(([a-zA-Z]|[a-zA-Z][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z]|[A-Za-z][A-Za-z0-9\-]*[A-Za-z0-9])$";

        public const string AlphaNumberUnderscore = @"^[a-zA-Z0-9_]*$";

        public const string ImgTagInHtmlPattern = @"<\s*img [^\>]*src\s*=\s*([""\'])(.*?)\1.*?>";
        public const string AnchorTagInHtmlPattern = @"<a\s[^>]*href\s*=\s*\""([^\""]*)\""[^>]*>(.*?)</a>";

        private static char[] UniqueChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        #endregion

        #region public methods

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            return Regex.IsMatch(email, EmailPattern);
        }

        public static bool IsValidDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return false;
            }

            return Regex.IsMatch(domain, DomainPattern);
        }

        public static bool IsValidAlphaNumbericUnderscore(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return false;
            }

            return Regex.IsMatch(content, AlphaNumberUnderscore);
        }

        public static string GetUniqueString()
        {
            return GetUniqueString(8);
        }

        public static string GetUniqueString(int size)
        {
            byte[] data = new byte[1];

            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);

            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(UniqueChars[b % (UniqueChars.Length - 1)]);
            }
            return result.ToString();
        }

        public static string SanitizeUrl(string urlKey)
        {
            // UrlKey should never end with /
            string sanitized = urlKey;

            if (!string.IsNullOrEmpty(sanitized) && sanitized.Length > 1)
            {
                if (sanitized[sanitized.Length - 1] == '/')
                {
                    sanitized = sanitized.Substring(0, sanitized.Length - 1);
                }
            }

            return sanitized;
        }

        /// <summary>
        /// returns List of src for the images that are present in the text.
        /// </summary>
        /// <param name="html">text</param>
        public static IList<string> GetImagesSrc(string html)
        {
            List<string> images = new List<string>();

            if (!string.IsNullOrEmpty(html))
            {

                MatchCollection imgMatches = Regex.Matches(html, ImgTagInHtmlPattern, RegexOptions.CultureInvariant);
                if (imgMatches.Count > 0)
                {
                    foreach (Match img in imgMatches)
                    {
                        images.Add(img.Groups[2].Value);
                    }
                }
            }

            return images;
        }

        /// <summary>
        /// returns List of Href for the "anchors" that are present in the text.
        /// </summary>
        /// <param name="html">text</param>
        public static IList<string> GetAnchorsHref(string html)
        {
            List<string> links = new List<string>();

            if (!string.IsNullOrEmpty(html))
            {
                MatchCollection linkMatches = Regex.Matches(html, AnchorTagInHtmlPattern, RegexOptions.CultureInvariant);
                if (linkMatches.Count > 0)
                {
                    foreach (Match link in linkMatches)
                    {
                        links.Add(link.Groups[1].Value);
                    }
                }
            }

            return links;
        }

        /// <summary>
        /// Given an url, it returns all the url paths. For example, if the input is
        /// 
        /// /News/2011/11/12
        /// 
        /// the method will return the following
        /// 
        /// /News
        /// /News/2011
        /// /News/2011/11
        /// /News/2011/11/12
        /// 
        /// </summary>
        /// <param name="url">URL for which paths need to be extracted</param>
        /// <returns>List containing the paths</returns>
        public static List<string> GetUrlPaths(string url)
        {
            List<string> paths = new List<string>();

            if (!string.IsNullOrEmpty(url) &&
                url.StartsWith("/"))
            {
                int queryIndex = url.IndexOf('?');
                if (queryIndex != -1)
                {
                    url = url.Substring(0, queryIndex);
                }

                int start = 0;
                int index = url.IndexOf('/', start);

                while (index != -1)
                {
                    int len = index;
                    if (len == 0) { len = 1; }
                    string path = url.Substring(0, len);
                    start = index + 1;
                    index = url.IndexOf('/', start);

                    paths.Add(path);
                }

                paths.Add(url);
            }

            return paths;
        }

        #endregion

    }
}
