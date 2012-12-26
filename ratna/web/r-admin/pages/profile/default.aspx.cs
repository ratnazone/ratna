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
namespace Jardalu.Ratna.Web.Admin.Profile
{
    #region using

    using System;

    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.Security;

    #endregion

    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();
            PopulateProfileValues();
        }

        #region private methods

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = "profile";
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Profile"), Constants.Urls.Profile.Url);
            }
        }

        private void PopulateProfileValues()
        {

            //populate the values
            Ratna.Profile.User user = AuthenticationUtility.Instance.GetLoggedUser();

            this.aliasspan.InnerText = user.Alias;
            this.emailspan.InnerText = user.Email;
            this.displaynamespan.InnerText = user.DisplayName;
            this.firstnamespan.InnerText = user.FirstName;
            this.lastnamespan.InnerText = user.LastName;
            this.descriptionspan.InnerHtml = user.Description;

            if (!string.IsNullOrEmpty(user.Photo))
            {
                this.profileimage.Src = user.Photo;
            }
        }

        #endregion
    }
}
