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
    using Jardalu.Ratna.Web.Admin.controls.Users;

    #endregion

    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
 
            PopulateNavigationAndBreadCrumb();

            SetUsersListParameters();

            SetActionPanels();
        }

        #region private methods

        private void SetActionPanels()
        {
            // new user
            this.actionPanel.AddAction(
                    "/images/plus.png",
                    ResourceManager.GetLiteral("Admin.Users.AddNew"),
                    Constants.Urls.Users.EditUserUrl
                );
        }

        private void PopulateNavigationAndBreadCrumb()
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
            }
        }

        private void SetUsersListParameters()
        {
            UserListParameters active = new UserListParameters();
            active.MoreUrl = Utility.ResolveUrl(Constants.Urls.Users.ListActiveUrl); ;

            this.activeUsersList.Parameters = active;

            UserListParameters pending = new UserListParameters();
            pending.MoreUrl = Utility.ResolveUrl(Constants.Urls.Users.ListPendingUrl); ;

            this.pendingUsersList.Parameters = pending;

            UserListParameters deleted = new UserListParameters();
            deleted.MoreUrl = Utility.ResolveUrl(Constants.Urls.Users.ListDeletedUrl); ;

            this.deletedUsersList.Parameters = pending;

        }

        #endregion

    }
}
