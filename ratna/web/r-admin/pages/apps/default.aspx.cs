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
namespace Jardalu.Ratna.Web.Admin.pages.apps
{
    #region using

    using System;

    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class _default : System.Web.UI.Page
    {
        #region private fields

        private static Logger logger;
        private const string AppMustBeZipJsVariable = "L_AppUploadMustBeZip";
        private const string SelectAnAppToUploadJsVariable = "L_SelectAnAppToUpload";

        #endregion

        #region ctor

        static _default()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();
            SetActionPanels();

            this.clientJavaScript.RegisterClientScriptVariable(
                AppMustBeZipJsVariable,
                ResourceManager.GetLiteral("Admin.Apps.UploadMustBeZip")
            );

            this.clientJavaScript.RegisterClientScriptVariable(
                SelectAnAppToUploadJsVariable,
                ResourceManager.GetLiteral("Admin.Apps.SelectAnAppToUpload")
            );

            // set the parameters for list
            enabledAppsList.Parameters.ShowEnabled = true;
            enabledAppsList.Parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Apps.NoAppsActivated");
            enabledAppsList.Parameters.Header = ResourceManager.GetLiteral("Admin.Apps.Activated");

            installedAppsList.Parameters.ShowEnabled = false;
            installedAppsList.Parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Apps.NoAppsInstalled");
            installedAppsList.Parameters.Header = ResourceManager.GetLiteral("Admin.Apps.Installed");
        }

        #endregion

        #region private methods

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = Constants.Navigation.Apps;
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Apps"), Constants.Urls.Apps.Url);
            }
        }

        private void SetActionPanels()
        {

            /* list panel */
            this.listPanel.AddAction(
                    "/images/start.png",
                    ResourceManager.GetLiteral("Admin.Apps.Installed"),
                    Constants.Urls.Media.Photos.ListUrl
                );

            this.listPanel.AddAction(
                    "/images/start.png",
                    ResourceManager.GetLiteral("Admin.Apps.Activated"),
                    Constants.Urls.Media.Photos.ListUrl
                );
            
        }

        #endregion
    }
}
