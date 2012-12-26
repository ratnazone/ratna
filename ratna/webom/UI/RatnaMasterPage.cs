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
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Pages;
    using Jardalu.Ratna.Templates;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Applications;
    using Jardalu.Ratna.Web.Templates;

    #endregion

    public class RatnaMasterPage : System.Web.UI.MasterPage
    {
        #region private properties

        private bool resolved;
        internal string templatePath;
        private string fetchUrl;

        private Logger logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static HtmlMeta generatorMeta;

        #endregion

        #region ctor

        static RatnaMasterPage()
        {
            generatorMeta = new HtmlMeta();
            generatorMeta.Name = "generator";
            generatorMeta.Content = string.Format("{0} {1}", Constants.ProductName,
                                                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        #endregion

        #region public properties

        public IPageStyle PageStyle
        {
            get;
            set;
        }

        /// <summary>
        /// The URL that was used by client to fetch the page.
        /// 
        /// For example - http://server/about/contact will result in /about/contact as the FetchUrl
        /// </summary>
        public string FetchUrl
        {
            get
            {
                string fUrl = fetchUrl;

                if (fUrl == null)
                {
                    // determine the fetchurl
                    if (HttpContext.Current != null)
                    {
                        fUrl = HttpContext.Current.Request.RawUrl;
                    }
                }

                return fUrl;
            }
            set
            {
                fetchUrl = value;
            }
        }

        /// <summary>
        /// Template path.
        /// 
        /// Returns the virtual path of the template. If the template is named "mytemplate", this would
        /// result in the templatePath returned as "/templates/mytemplate"
        /// </summary>
        private string TemplatePath
        {
            get
            {
                if (!resolved)
                {
                    logger.Log(LogLevel.Debug, "MasterPageFile : {0}", this.Page.MasterPageFile);
                    string prefix = SitePaths.Template;

                    if (!this.Page.MasterPageFile.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        // remove the ~
                        prefix = prefix.Substring(1);
                    }

                    if (prefix != null)
                    {
                        prefix = Jardalu.Ratna.Web.Utility.GetVirtualPath(prefix);
                    }

                    if (this.Page.MasterPageFile.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        int startindex = prefix.Length + 1;
                        int endindex = this.Page.MasterPageFile.IndexOf('/', startindex);

                        templatePath = this.Page.MasterPageFile.Substring(0, endindex);
                    }

                    logger.Log(LogLevel.Debug, "Setting template path : {0}", templatePath);

                    resolved = true;
                }

                return templatePath;
            }
        }

        #endregion

        #region public methods

        public string GetPathRelativeToTemplate(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            string rPath = path;

            if (TemplatePath != null)
            {

                string relativePath = Path.Combine(TemplatePath, path);

                rPath = relativePath.Replace('\\', '/');
            }

            return rPath;
        }

        public void SetNavigation(string navigation)
        {
            if (string.IsNullOrEmpty(navigation))
            {
                return;
            }

            if (PageStyle.IsNavigationSupported)
            {
                INavigationControl navigationControl = this.FindControl(PageStyle.NavigationControlName) as INavigationControl;
                if (navigationControl != null)
                {
                    navigationControl.SetSelected(navigation);
                }
            }
        }

        #endregion

        #region protected methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                // load all the apps that are PageHead apps
                HtmlHead head = this.Page.Header;
                if (head != null)
                {
                    IList<UserControl> pageHeadApps = AppEngine.GetPageHeadApps(this);
                    if (pageHeadApps != null)
                    {
                        foreach (UserControl control in pageHeadApps)
                        {
                            head.Controls.Add(control);
                        }
                    }

                    // add the meta-generator
                    head.Controls.Add(generatorMeta);

                }
            }
            catch (Exception he)
            {
                logger.Log(LogLevel.Warn, "Unable to load PageHead controls : {0}", he);
            }

        }

        #endregion

    }

}
