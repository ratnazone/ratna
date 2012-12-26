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
    using System.Configuration;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Web;
    using System.Web.Routing;
    using System.Web.Security;

    using Jardalu.Ratna.Administration;
    using Jardalu.Ratna.Database;
    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.AppData;
    using Jardalu.Ratna.Web.Utilities;
    using Jardalu.Ratna.Web.Plugins;

    #endregion

    public class Global : System.Web.HttpApplication
    {

        #region private fields

        private Logger logger;

        #endregion

        #region ctor

        public Global()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region protected methods

        protected void Application_Start(object sender, EventArgs e)
        {
            // set the db connection string
            ConnectionInformation.Instance.ConnectionString = ConfigurationManager.AppSettings["RatnaDbConnectionString"];
            RegisterRoutes(System.Web.Routing.RouteTable.Routes);

            //load the configuration
            Configuration.Instance.Server = this.Server;

            //logging
            Jardalu.Ratna.Utilities.Logger.IsEnabled = Configuration.Instance.IsLoggingOn;
            Jardalu.Ratna.Utilities.Logger.EnabledLevel = Configuration.Instance.LoggingLevel;

#if DEBUG
            Jardalu.Ratna.Utilities.Logger.IsEnabled = true;
            Jardalu.Ratna.Utilities.Logger.EnabledLevel = LogLevel.Debug;
#endif
            logger.Log(LogLevel.Info, "Application_Start done");
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

            // when the request begins, this request may have already been assigned
            // stickyid and/or site.

            if (!HttpContext.Current.Items.Contains(Jardalu.Ratna.Core.PublicConstants.StickyId))
            {
                // generate a new sticky id
                Guid guid = Guid.NewGuid();
                HttpContext.Current.Items[Jardalu.Ratna.Core.PublicConstants.StickyId] = guid;
            }

            if (!HttpContext.Current.Items.Contains(Jardalu.Ratna.Core.PublicConstants.Site))
            {
                SiteUtility.AssignSitePropeties();
            }

            // if there is no site associated with the HttpContext, this request cannot served.
            if (!HttpContext.Current.Items.Contains(Jardalu.Ratna.Core.PublicConstants.Site))
            {
                // this request cannot be served with this particular server
                this.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                this.Response.Output.Write(DataReader.ReadFileContents("urlnotserved.html"));

                this.Response.Flush();
                this.Response.End();
            }

        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            Jardalu.Ratna.Web.Controls.Common.ClientJavaScript.Clean();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            logger.Log(LogLevel.Info, "Application_End done");
        }

        #endregion

        #region private methods

        private void RegisterRoutes(System.Web.Routing.RouteCollection routes)
        {
            IList<ManagedPath> paths = ManagedPathPlugin.Instance.GetPaths();
            foreach (ManagedPath path in paths)
            {
                RouteValueDictionary defaults = null;

                if (path.Defaults.Count > 0)
                {
                    defaults = new RouteValueDictionary();
                    foreach (string key in path.Defaults.Keys)
                    {
                        defaults.Add(key, path.Defaults[key]);
                    }
                }

                if (defaults == null)
                {
                    routes.MapPageRoute(path.Name, path.Path, path.ResolvedPath);
                }
                else
                {
                    routes.MapPageRoute(path.Name, path.Path, path.ResolvedPath, false, defaults);
                }

            }
        }

        #endregion
    }
}
