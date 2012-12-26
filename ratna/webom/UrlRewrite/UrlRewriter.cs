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
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.UI;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Administration;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web.Utilities;
    using Jardalu.Ratna.Web.Plugins;

    #endregion

    public class UrlRewriter : IHttpModule
    {

        #region private fields

        private Logger logger;

        #endregion

        #region ctor

        public UrlRewriter()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region IHttpModule Members

        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(RewriteModule_BeginRequest);
        }

        protected void RewriteModule_BeginRequest(object sender, EventArgs e)
        {
            //first assign site properties
            bool sitePresent = SiteUtility.AssignSitePropeties();

            if (sitePresent)
            {
                string path = HttpContext.Current.Request.Path;
                if (path.Length == 0) return;

                // if the response is already handled, just do nothing
                if (WebContext.Current.ResponseContent.Handled)
                {
                    return;
                }

                string rawUrl = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath;
                if (rawUrl != null && rawUrl[0] == '~')
                {
                    // remove the ~
                    rawUrl = rawUrl.Substring(1);
                }

                logger.Log(LogLevel.Debug, "RawUrl to check for rewrite : [{0}]", rawUrl);

                // if the path is a system path or managed path ( reserved path ) ,
                // it cannot be supported via rewrite
                bool isReservedPath = IsReservedPath(rawUrl);

                logger.Log(LogLevel.Debug, "Is RawUrl [{0}] reserved path : {1} ", rawUrl, isReservedPath);

                if (!isReservedPath)
                {
                    #region article/page path

                    string rewriteUrl = null;
                    bool match =
                        UrlRewriterManager.Instance.FindRuleMatch(rawUrl, out rewriteUrl);

                    if (match)
                    {
                        logger.Log(LogLevel.Debug, "UrlRewriterManager found rewriteUrl [{0}]", rewriteUrl);
                    }

                    if (!match)
                    {
                        //check for site url mappings
                        match = SiteUrlMapper.FindRuleMatch(rawUrl, out rewriteUrl);

                        if (match)
                        {
                            logger.Log(LogLevel.Debug, "SiteUrlMapper found rewriteUrl [{0}]", rewriteUrl);
                        }

                    }

                    if (match)
                    {
                        //there was a match. Since the original path may come with query string,
                        //append the query string.
                        string originalQuery = HttpContext.Current.Request.QueryString.ToString();
                        if (!string.IsNullOrEmpty(originalQuery))
                        {
                            //set the new uri with complete query
                            rewriteUrl = Jardalu.Ratna.Web.Utility.AppendQuery(rewriteUrl, originalQuery);
                        }

                        HttpContext.Current.RewritePath(rewriteUrl);
                    }


                    #endregion
                }
            }

        }

        #endregion

        #region private methods

        private bool IsReservedPath(string rawUrl)
        {
            return IsSystemPath(rawUrl) || IsManagedPath(rawUrl);
        }

        private bool IsSystemPath(string rawUrl)
        {
            return Jardalu.Ratna.Web.Utility.IsSystemPath(rawUrl);
        }

        public static bool IsManagedPath(string rawUrl)
        {
            ManagedPath mPath = null;

            return IsManagedPath(rawUrl, out mPath);
        }

        public static bool IsManagedPath(string rawUrl, out ManagedPath managedPath)
        {

            if (rawUrl == null)
            {
                throw new ArgumentNullException("rawUrl");
            }

            managedPath = null;

            bool isManagedPath = false;

            IList<ManagedPath> managedPaths = ManagedPathPlugin.Instance.GetPaths();
            
            foreach(ManagedPath path in managedPaths)
            {
                //path contains the syntax like /search/{type}/{q}
                string pathExtended = path.Path;
                string pathPrefix = pathExtended;
                int index = pathExtended.IndexOf('{');
                if (index != -1)
                {
                    pathPrefix = pathExtended.Substring(0, index);
                }

                if (rawUrl.StartsWith(pathPrefix)) 
                {
                    managedPath = path;
                    isManagedPath = true;
                    break;
                }
            }

            return isManagedPath;
        }
        
        #endregion

    }
}
