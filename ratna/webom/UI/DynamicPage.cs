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
namespace Jardalu.Ratna.Web.UI
{
    #region using

    using System;
    using System.IO;
    using System.Web.UI;

    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Templates;

    #endregion

    public class DynamicPage : Page
    {

        #region private fields

        private static Logger logger;

        #endregion

        #region ctor

        public DynamicPage()
        {

        }

        static DynamicPage()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region public properties

        /// <summary>
        /// If the page has a route that is being used, this property
        /// tells the name of the route.
        /// 
        /// for example : ~/default.aspx is mapped to /page/ and the route name is "default"
        /// 
        /// </summary>
        public string RouteName
        {
            get;
            set;
        }

        #endregion

        #region protected properties

        public string Query
        {
            get
            {
                string q = Page.RouteData.Values[Constants.SearchRouteIdentifier] as string;
                if (q == null)
                {
                    q = string.Empty;
                }
                return q;
            }
        }

        public virtual int PageNumber
        {
            get
            {
                int page = 1;

                string pageStr = Page.RouteData.Values[Constants.PageRouteIdentifier] as string;
                if (!string.IsNullOrEmpty(pageStr))
                {
                    if (!Int32.TryParse(pageStr, out page))
                    {
                        page = 1;
                    }
                }

                return page;
            }
        }

        protected int TotalPages
        {
            get;
            set;
        }

        #endregion

        #region protected methods

        protected override void OnPreInit(EventArgs e)
        {

            logger.Log(LogLevel.Debug, "Request RawUrl : {0}", Request.RawUrl);

            // don't apply if the path is within system path
            if (!Jardalu.Ratna.Web.Utility.IsSystemPath(Request.RawUrl))
            {
                logger.Log(LogLevel.Debug, "Request Url is not a systems path");

                Template template = null;

                if (UrlRewrite.UrlRewriter.IsManagedPath(Request.RawUrl))
                {
                    logger.Log(LogLevel.Debug, "Getting active template for RootUrl as the path is managed.");
                    template = TemplatePlugin.Instance.GetActiveTemplate(Constants.RootUrl);
                }
                else
                {
                    template = TemplatePlugin.Instance.GetActiveTemplate(Request.RawUrl);
                }

                if (template != null)
                {
                    
                    string masterFile = Path.Combine(TemplatePlugin.GetTemplateVirtualPath(template.TemplatePath), 
                                                     template.MasterFileName);

                    logger.Log(LogLevel.Debug, "Applying MasterFile {0}", masterFile);
                    base.MasterPageFile = masterFile;
                    base.AppRelativeTemplateSourceDirectory = Path.Combine("~/", SitePaths.TemplateVirtualPath, template.TemplatePath);
                }

                base.OnPreInit(e);
            }


        }

        #endregion

    }

}
