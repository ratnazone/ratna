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
namespace Jardalu.Ratna.Web.Admin.users
{

    #region using

    using System;
    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Resource;

    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Store;

    #endregion

    public partial class user : System.Web.UI.Page
    {
        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            string alias = GetUserAlias();

            User user = null;
            if (!string.IsNullOrEmpty(alias))
            {
                user = UserStore.Instance.LoadUser(alias);
            }

            if (user != null)
            {
                headerH1.InnerText = user.DisplayName;

                PopulateUserValues(user);

                this.usermembership.Alias = user.Alias;
            }
            else
            {
                headerH1.InnerText = "XXXXXXXXXXXXXX";
            }

            PopulateNavigationAndBreadCrumb(user);
        }

        #endregion

        #region private methods

        private string GetUserAlias()
        {
            return Request["alias"] as string;
        }

        private void PopulateNavigationAndBreadCrumb(User user)
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = Constants.Navigation.Users;
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Common.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Users"), Constants.Urls.Users.Url);

                if (user != null)
                {
                    breadcrumb.Add(user.DisplayName, Constants.Urls.Users.ViewUserUrl);
                }
            }
        }

        private void PopulateUserValues(User user)
        {

 
            this.aliasspan.InnerText = user.Alias;
            this.emailspan.InnerText = user.Email;
            this.displaynamespan.InnerText = user.DisplayName;
            this.firstnamespan.InnerText = user.FirstName;
            this.lastnamespan.InnerText = user.LastName;
            this.descriptionspan.InnerHtml = user.Description;

            this.lastsigned.InnerText = Utility.FormatDate(user.LastLoginTime);
            this.createdon.InnerText = Utility.FormatDate(user.CreatedTime);

            if (!string.IsNullOrEmpty(user.Photo))
            {
                this.userimage.Src = user.Photo;
            }
        }

        #endregion

    }
}
