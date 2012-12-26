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
namespace Jardalu.Ratna.Web.Pages
{

    #region using

    using System;
    
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.UI;
    using System.Web.UI;
    using Jardalu.Ratna.Core.Pages;
    using System.Web.UI.HtmlControls;
    using Jardalu.Ratna.Templates;
    using System.Collections.Generic;

    #endregion

    public class SingleItemPage : DynamicPage
    {

        enum ControlType
        {
            Thread,
            Page
        }

        #region private fields

        private static Logger logger;

        #endregion

        #region ctor

        static SingleItemPage()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region protected methods

        public static Control GetPageControl(MasterPage masterPage)
        {
            Control control = GetControl(masterPage, ControlType.Page);
            
            // if there is no control defined, select the default one
            if (control == null)
            {
                HtmlGenericControl div = new HtmlGenericControl("div");
                control = div;
            }

            return control;
        }

        public static Control GetThreadControl(MasterPage masterPage)
        {
            return GetControl(masterPage, ControlType.Thread);
        }

        private static Control GetControl(MasterPage masterPage, ControlType type)
        {
            Control control = null;

            RatnaMasterPage master = masterPage as RatnaMasterPage;
            if (master != null)
            {
                IPageStyle pageStyle = master.PageStyle;

                if (pageStyle != null)
                {

                    // get the control type.
                    string controlName = null;

                    switch (type)
                    {
                        case ControlType.Page :
                            controlName = pageStyle.PageControl;
                            break;

                        case ControlType.Thread:
                            controlName = pageStyle.ThreadControl;
                            break;
                    }


                    if (!string.IsNullOrEmpty(controlName))
                    {
                        // check the file exists
                        try
                        {
                            string controlPath = string.Format("{0}/{1}", master.AppRelativeTemplateSourceDirectory, controlName);

                            control = master.LoadControl(master.ResolveUrl(controlName));
                        }
                        catch (Exception exception)
                        {
                            // unable to load the control.
                            logger.Log(LogLevel.Warn, "Unable to load the control at : [{0}], error message : {1}", controlName, exception);
                        }
                    }
                }
            }

            return control;
        }

        protected void BuildBreadcrumbData(IBreadcrumbControl breadcrumb, string url)
        {
            if (breadcrumb != null &&
                !string.IsNullOrEmpty(url))
            {

                IList<Tuple<string, string>> data = NavigationDataUtility.GetNavigtionData(Constants.DefaultPageUrl, url);

                foreach (Tuple<string, string> tuple in data)
                {
                    breadcrumb.AddBreadcrumb(tuple.Item1, tuple.Item2);
                }
            }
        }


        /// <summary>
        /// Populates the Side Navigation Control with the navigation data.
        /// </summary>
        /// <param name="sideNavigation">SideNavigation control</param>
        /// <param name="url">UrlKey for the page</param>
        /// <param name="roots">All the roots</param>
        protected void BuildSideNavigationData(ISideNavigationControl sideNavigation, string url, IList<string> roots)
        {

            if (sideNavigation != null &&
                !string.IsNullOrEmpty(url))
            {

                // if the url does not belong to one of the roots
                // don't fetch any navigation data.
                bool fetch = false;
                if (roots != null)
                {
                    foreach (string root in roots)
                    {
                        if (url.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                        {
                            fetch = true;
                            break;
                        }
                    }
                }

                if (fetch)
                {
                    // get the pages for the root level
                    IList<Tuple<string, string>> data = NavigationDataUtility.GetNavigtionData(Constants.DefaultPageUrl, roots);
                    foreach (Tuple<string, string> tuple in data)
                    {
                        sideNavigation.AddNavigationData(tuple.Item1, tuple.Item2);
                    }

                    if (!roots.Contains(url))
                    {
                        data = NavigationDataUtility.GetNavigtionData(Constants.DefaultPageUrl, url);
                        foreach (Tuple<string, string> tuple in data)
                        {
                            sideNavigation.AddNavigationData(tuple.Item1, tuple.Item2);
                        }
                    }

                    // get all the childs till the page.
                    data = NavigationDataUtility.GetChildNavigationData(Constants.DefaultPageUrl, url);
                    foreach (Tuple<string, string> tuple in data)
                    {
                        sideNavigation.AddNavigationData(tuple.Item1, tuple.Item2);
                    }
                }
            }
        }

        #endregion
    }

}
