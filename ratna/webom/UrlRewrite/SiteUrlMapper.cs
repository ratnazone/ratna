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
namespace Jardalu.Ratna.Web.UrlRewrite
{
    #region using

    using System;
    using System.IO;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Utilities;

    #endregion

    internal class SiteUrlMapper
    {
        #region fields

        private static Logger logger;
        private const string UploadContentPath = "/" + SitePaths.ContentFolderName + "/";

        #endregion

        #region ctor

        static SiteUrlMapper()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        public static bool FindRuleMatch(string originalUrl, out string rewriteUrl)
        {
            bool success = false;
            rewriteUrl = originalUrl;

            if (originalUrl != null &&
                originalUrl.StartsWith(UploadContentPath, StringComparison.OrdinalIgnoreCase))
            {

                // the following path 
                // /r-content/[xyz]
                // should translate to
                // /sites/<site id>/content/[xyz]

                string suffix = originalUrl.Substring(UploadContentPath.Length - 1); // grab the '/' as well
                rewriteUrl = string.Format("{0}{1}", SitePaths.Content, suffix);

                logger.Log(LogLevel.Debug, "UploadContentPath - [{0}] Suffix - [{1}] RewriteUrl - [{2}]",  
                        UploadContentPath, suffix, rewriteUrl);

                success = true;
            }

            return success;
        }
    }

}
