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
namespace Jardalu.Ratna.Web
{

    #region using

    using System;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Web;

    using Jardalu.Ratna.Utilities;

    #endregion

    public class Utility
    {
        #region private fields

        private static Logger logger;
        private const string ShortnerSuffix = "...";

        #endregion

        #region ctor

        static Utility()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region public methods

        public static string AppendQuery(string url, string query)
        {
            string modified = url;
            if (!string.IsNullOrEmpty(url))
            {
                int index = url.IndexOf('?');
                if (index == -1)
                {
                    modified = url + "?" + query;
                }
                else
                {
                    modified = url + "&" + query;
                }
            }
            return modified;
        }

        public static string ResolveUrl(string url)
        {
            return VirtualPathUtility.ToAbsolute(url);
        }

        public static string ToAbsolute(string url)
        {
            string absolute = url;

            if (!string.IsNullOrEmpty(url))
            {
                HttpContext current = HttpContext.Current;

                if (current != null)
                {
                    HttpRequest request = current.Request;

                    if (request.IsSecureConnection)
                    {
                        absolute = string.Format("https://{0}{1}", request.Url.Host, ResolveUrl(url));
                    }
                    else
                    {
                        absolute = string.Format("http://{0}{1}", request.Url.Host, ResolveUrl(url));
                    }
                }
            }

            return absolute;
        }

        public static string GetVirtualPath(string physicalPath)
        {
            string rootpath = HttpContext.Current.Server.MapPath("~/");

            physicalPath = physicalPath.Replace(rootpath, "");
            physicalPath = physicalPath.Replace("\\", "/");
            physicalPath = physicalPath.Replace(" ", "%20");
            if (!physicalPath.StartsWith("/"))
            {
                physicalPath = "/" + physicalPath;
            }

            return physicalPath;
        }

        public static string CombineVirtualPath(string path1, string path2)
        {
            string path = System.IO.Path.Combine(path1, path2);
            return GetVirtualPath(path);
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString("ddd d MMM yyyy, hh:mm");
        }

        public static string FormatShortDate(DateTime date)
        {
            return date.ToString("ddd d MMM yyyy");
        }

        public static string FormatConciseDate(DateTime date)
        {
            return date.ToString("d/MM/yy");
        }

        public static object FromString(string text, Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }

            object retVal = null;

            try
            {
                retVal = Convert.ChangeType(text, t, CultureInfo.InvariantCulture);
            }
            catch
            {
                retVal = GetDefaultValue(t);
            }

            return retVal;
        }

        public static bool IsSystemPath(string urlPath)
        {
            bool isSystemPath = false;

            if (!string.IsNullOrEmpty(urlPath))
            {
                foreach (string s in Configuration.Instance.SystemPaths)
                {
                    if (urlPath.StartsWith(s))
                    {
                        isSystemPath = true;
                        break;
                    }
                }
            }

            return isSystemPath;
        }

        public static bool IsBlockedPath(string urlPath)
        {
            bool isBlockedPath = false;

            if (!string.IsNullOrEmpty(urlPath))
            {
                foreach (string s in Configuration.Instance.BlockedPaths)
                {
                    if (urlPath.StartsWith(s))
                    {
                        isBlockedPath = true;
                        break;
                    }
                }
            }

            return isBlockedPath;
        }

        public static string SanitizeJsonHtml(string html)
        {
            string sanitized = html;

            if (!string.IsNullOrEmpty(sanitized))
            {
                // process the invalid json characters
                sanitized = sanitized.Replace(Environment.NewLine, " ");
                sanitized = sanitized.Replace("\"", "\\\"");
            }

            return sanitized;
        }

        public static Tuple<int, int> GetImageSize(string url)
        {

            #region arguments

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            #endregion

            Tuple<int, int> size = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream s = response.GetResponseStream())
                {
                    Bitmap bmp = new Bitmap(s);

                    size = new Tuple<int, int>(bmp.Width, bmp.Height);
                }
            }
            catch (Exception)
            {
                size = new Tuple<int, int>(0, 0);
            }

            return size;
        }

        public static string ShortenForDisplay(string text, int maxSize)
        {
            string shortened = text;

            if (text != null && text.Length > maxSize)
            {
                shortened = text.Substring(0, maxSize - ShortnerSuffix.Length) + ShortnerSuffix;
            }

            return shortened;
        }

        #endregion

        #region private methods

        private static T FromString<T>(string text)
        {
            try
            {
                return (T)Convert.ChangeType(text, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                return default(T);
            }
        }

        private static object GetDefaultValue(Type t)
        {
            if (!t.IsValueType)
            {
                return null;
            }

            return Activator.CreateInstance(t); ;
        }

        #endregion
    }
}
