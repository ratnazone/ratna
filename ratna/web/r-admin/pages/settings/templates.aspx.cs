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

    public partial class templates : SettingsBasePage
    {
        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            this.menu.Selected = ResourceManager.GetLiteral("Admin.Templates.Title");
            this.Title = ResourceManager.GetLiteral("Admin.Templates.Title");

            foreach (Tuple<string, string> tuple in GetMenuItems())
            {
                this.menu.AddMenu(tuple.Item1, tuple.Item2);
            }
        }

        #endregion


        #region private methods

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
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Templates.Title"), Constants.Urls.Settings.TemplatesUrl);
            }
        }

        #endregion
    }
}
