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
namespace Jardalu.Ratna.Web.Admin.pages.settings
{

    #region using

    using System;

    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.Admin.controls;

    #endregion

    public partial class customerrors : SettingsBasePage
    {

        #region private fields

        private const string CancelUrlJsVariable = "CancelUrl";
        private const string L_SavedSucceesJsVariable = "L_SavedSuccees";

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();
            PopulateJavascriptIncludesAndVariables();

            this.menu.Selected = ResourceManager.GetLiteral("Admin.Settings.CustomErrors.Title");
            this.Title = ResourceManager.GetLiteral("Admin.Settings.CustomErrors.Title");

            foreach (Tuple<string, string> tuple in GetMenuItems())
            {
                this.menu.AddMenu(tuple.Item1, tuple.Item2);
            }

            CustomResponse config = CustomResponse.Read();
            if (config != null)
            {
                RenderCustomResponseValues(config);
            }

        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptVariable(
                    CancelUrlJsVariable,
                    Utility.ResolveUrl(Constants.Urls.Settings.Url)
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    L_SavedSucceesJsVariable,
                    ResourceManager.GetLiteral("Admin.Common.Saved")
                );
        }

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = Constants.Navigation.Settings;
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Settings"), Constants.Urls.Settings.Url);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Settings.CustomErrors.Title"), Constants.Urls.Settings.CustomErrorsUrl);
            }
        }

        private void RenderCustomResponseValues(CustomResponse config)
        {
            this.error404.Value = config.PageNotFound;
            this.error500.Value = config.InteralServerError;
            this.errorothers.Value = config.OtherErrors;
        }

        #endregion

    }
}
