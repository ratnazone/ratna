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
namespace Jardalu.Ratna.Web.Admin.pages.forms
{
    #region using

    using System;

    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class _default : PagerSupportPage
    {

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();
            SetActionPanels();

            this.formsList.Parameters.HideHeader = false;
            this.formsList.Parameters.DisplayTableHeader = true;
            this.formsList.Parameters.ShowDelete = true;
            this.Title = ResourceManager.GetLiteral("Admin.Forms.PageTitle");

            this.PageSize = this.Pager.PageSize;
            this.formsList.Parameters.Count = this.Pager.PageSize;
            this.formsList.Parameters.Start = Start;
            this.formsList.LoadData();

            this.RenderPager(this.formsList.TotalRecords);
        }

        #endregion

        #region private methods

        private void SetActionPanels()
        {
            // new forms
            this.actionPanel.AddAction(
                    "/images/plus.png",
                    ResourceManager.GetLiteral("Admin.Forms.AddNew"),
                    Constants.Urls.Forms.EditUrl
                );

            /* list panel */
            this.listPanel.AddAction(
                    "/images/start.png",
                    ResourceManager.GetLiteral("Admin.Forms.Entry.Recent"),
                    Constants.Urls.Forms.RecentUrl
                );
        }

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = Constants.Navigation.Forms;
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Forms"), Constants.Urls.Forms.Url);
            }
        }

        private void RenderPager(int total)
        {
            if (total > 0)
            {
                this.TotalPages = (total / this.Pager.PageSize) + (total % this.Pager.PageSize > 0 ? 1 : 0);
            }

            //set the page format
            this.Pager.PageLinkFormat = string.Format("{0}?p=", Request.Url.AbsolutePath) + "{0}";

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
