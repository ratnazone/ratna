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
namespace Jardalu.Ratna.Web.Applications
{

    #region using

    using System;
    using System.Net;
    using System.Web;
    using System.Web.UI;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Apps;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Utilities;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.UI.Apps;

    #endregion

    /// <summary>
    /// The AppPathModule checks if any app is registered to serve the path/url.
    /// If there is an app registered to serve the path/url. this module will call
    /// the app to serve the page.
    /// </summary>
    public class AppPathModule : IHttpModule
    {

        #region private fields

        private static Logger logger;
        internal const string AppExtension = ".app";

        #endregion

        #region ctor

        static AppPathModule()
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
            context.BeginRequest += new EventHandler(AppPathModule_BeginRequest);
        }

        #endregion

        #region protected methods

        protected void AppPathModule_BeginRequest(object sender, EventArgs e)
        {

            //first assign site properties
            bool sitePresent = SiteUtility.AssignSitePropeties();

            if (sitePresent)
            {

                if (WebContext.Current.ResponseContent.Handled)
                {
                    return;
                }

                // sanitize the url
                string rawUrl = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath;
                rawUrl = SanitizeUrl(rawUrl);

                logger.Log(LogLevel.Debug, "Checking if the url [{0}] is served by an app", rawUrl);
                Page page = null;
                App app = AppEngine.FindAppRegisteredForPath(rawUrl, out page);

                if (page != null && app != null)
                {
                    logger.Log(LogLevel.Debug, "Found app associated with [{0}] - [{1}]. Invoking ProcessRequest", rawUrl, page);
                    long executionId = AppEngine.RecordAppExecutionStart(app);
                    try
                    {
                        // set the App information
                        AppDynamicPage dynamicPage = page as AppDynamicPage;
                        if (dynamicPage != null)
                        {
                            dynamicPage.InvokedApp = app;
                            dynamicPage.FetchUrl = rawUrl;
                        }

                        page.ProcessRequest(HttpContext.Current);
                        HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.OK;
                        AppEngine.RecordAppExecutionEnd(executionId, true, string.Empty);
                    }
                    catch (Exception exception)
                    {
                        HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        AppEngine.RecordAppExecutionEnd(executionId, false, exception.Message);
                        logger.Log(LogLevel.Warn, "Error executing app [{0}]. Error - {1}", app, exception);
                    }

                    WebContext.Current.ResponseContent.Handled = true;
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.Close();
                }

            }
        }

        #endregion

        #region private methods

        internal static string SanitizeUrl(string rawUrl)
        {

            if (!string.IsNullOrEmpty(rawUrl))
            {
                if (rawUrl[0] == '~')
                {
                    // remove the ~
                    rawUrl = rawUrl.Substring(1);
                }
            }

            return rawUrl;
        }


        #endregion

    }

}
