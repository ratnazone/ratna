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
    using Jardalu.Ratna.Store;
    using System.Collections.Generic;
    using Jardalu.Ratna.Profile;

    #endregion

    public partial class list : System.Web.UI.Page
    {

        protected enum UserType
        {
            Active,
            Pending,
            Deleted
        }

        #region protected properties

        public string Query
        {
            get
            {
                string q = Request[Constants.SearchRouteIdentifier] as string;
                if (q == null)
                {
                    q = string.Empty;
                }
                return q;
            }
        }

        public int PageNumber
        {
            get
            {
                int page = 1;

                string pageStr = Request[Constants.PageRouteIdentifier] as string;
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

        protected UserType RequestUserType
        {
            get
            {
                UserType userType = UserType.Active;

                if ("pending".Equals(Request["type"],StringComparison.OrdinalIgnoreCase))
                {
                    userType = UserType.Pending;
                }
                else if ("deleted".Equals(Request["type"], StringComparison.OrdinalIgnoreCase))
                {
                    userType = UserType.Deleted;
                }

                return userType;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

            // check the type of users
            // active, pending, deleted
            // by default, active users
            UserType userType = RequestUserType;

            PopulateNavigationAndBreadCrumb(userType);

            // fetch the data, header based on usertype
            switch (userType)
            {
                case UserType.Active:
                    headerH1.InnerText = ResourceManager.GetLiteral("Admin.Users.Active");
                    break;
                case UserType.Pending:
                    headerH1.InnerText = ResourceManager.GetLiteral("Admin.Users.Pending");
                    usersList.IsActive = false;
                    break;
                case UserType.Deleted:
                    headerH1.InnerText = ResourceManager.GetLiteral("Admin.Users.Deleted");
                    usersList.IsDeleted = true;
                    break;
            }

            this.TotalPages = 0;

            int start = (this.PageNumber - 1) * Pager.PageSize;
            int total;

            IList<User> users = UserStore.Instance.GetUsers(Query, usersList.IsActive, usersList.IsDeleted, start, Pager.PageSize, out total);
            usersList.Users = users;

            if (total > 0)
            {
                this.TotalPages = (total / this.Pager.PageSize) + (total % this.Pager.PageSize > 0 ? 1 : 0);
            }


            RenderPager();

        }

        #region private methods

        private void PopulateNavigationAndBreadCrumb(UserType userType)
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

                // get the header based on usertype
                string header = ResourceManager.GetLiteral("Admin.Users.Active");
                if (userType == UserType.Pending)
                {
                    header = ResourceManager.GetLiteral("Admin.Users.Pending");
                }
                else if (userType == UserType.Deleted)
                {
                    header = ResourceManager.GetLiteral("Admin.Users.Deleted");
                }

                breadcrumb.Add(header , Constants.Urls.Users.ListUrl);
            }
        }
       
        private void RenderPager()
        {

            //set the page format
            this.Pager.PageLinkFormat = string.Format("{0}?type={1}&q={2}&p=", Request.Url.AbsolutePath, Request["type"], Request["q"]) + "{0}";

            this.Pager.CurrentPageNumber = PageNumber;
            this.Pager.TotalPages = this.TotalPages;

            if (this.Pager.TotalPages <= 1)
            {
                this.Pager.Visible = false;
            }
            else
            {
                this.Pager.Visible = true;
            }
        }

        #endregion

    }
}
