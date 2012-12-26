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
namespace Jardalu.Ratna.Web.Redirect
{

    #region using

    using System;
    using System.Net;
    using System.Web;
    using System.Web.UI;

    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Web.Utilities;
    using Jardalu.Ratna.Web.Applications;
    using Jardalu.Ratna.Store;

    #endregion

    public class RedirectModule : IHttpModule
    {

        #region private fields

        private static Logger logger;

        #endregion

        #region ctor

        static RedirectModule()
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
            context.BeginRequest += new EventHandler(RedirectModule_BeginRequest);
        }

        #endregion

        #region protected methods

        protected void RedirectModule_BeginRequest(object sender, EventArgs e)
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
                rawUrl = AppPathModule.SanitizeUrl(rawUrl);

                logger.Log(LogLevel.Debug, "Checking if the url [{0}] is set to be redirected", rawUrl);
                string redirectUrl = null;

                if (UrlRedirectionStore.Instance.TryGet(rawUrl, out redirectUrl))
                {
                    WebContext.Current.ResponseContent.Handled = true;

                    //redirect
                    HttpContext.Current.Response.Redirect(redirectUrl);

                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.Close();
                }

            }
        }

        #endregion

    }

}
