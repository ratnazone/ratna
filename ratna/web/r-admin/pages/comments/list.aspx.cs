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
namespace Jardalu.Ratna.Web.Admin.pages.comments
{
    #region using

    using System;

    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Admin.controls.Comments;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class list : PagerSupportPage
    {

        enum View
        {
            Pending,
            Approved
        }

        #region protected properties

        protected string SearchUrl
        {
            get
            {
                return Request.Url.AbsolutePath + "?view=" + ListView + "&q={0}";
            }
        }

        #endregion

        #region private property

        private View ListView
        {
            get
            {
                View view = list.View.Approved;

                string v = Request["view"];
                if (!Enum.TryParse<View>(v, true, out view))
                {
                    view = list.View.Approved;
                }

                return view;
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {

            //set the pagesize
            this.PageSize = Pager.PageSize;

            PopulateNavigationAndBreadCrumb();
            SetCommentsParameters();

            // set the title of the page
            if (ListView == View.Pending)
            {
                this.Title = ResourceManager.GetLiteral("Admin.Breadcrumb.Comments.Pending");
                this.headerH1.InnerText = ResourceManager.GetLiteral("Admin.Breadcrumb.Comments.Pending");
            }
            else
            {
                this.Title = ResourceManager.GetLiteral("Admin.Breadcrumb.Comments.Approved");
                this.headerH1.InnerText = ResourceManager.GetLiteral("Admin.Breadcrumb.Comments.Approved");
            }

            //fetch the data
            this.comments.LoadData();
            RenderPager(this.comments.TotalRecords);

        }

        #endregion

        #region private methods

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = Constants.Navigation.Comments;
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Comments"), Constants.Urls.Comments.Url);
                if (ListView == View.Pending)
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Comments.Pending"), Constants.Urls.Comments.ListPendingUrl);
                }
                else
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Comments.Approved"), Constants.Urls.Comments.ListApprovedUrl);
                }
            }
        }

        private void SetCommentsParameters()
        {
            CommentListParameters parameters = new CommentListParameters();
            parameters.DisplayTableHeader = true;
            parameters.HideHeader = true;

            if (ListView == View.Pending)
            {
                parameters.FetchPending = true;
                parameters.ShowApprove = true;

                parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Comments.List.Pending.NoComments");
            }
            else
            {
                parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Comments.List.Approved.NoComments");
            }

            parameters.ShowDelete = true;
            parameters.ShowKey = false;
            parameters.Query = Query;
            parameters.Start = Start;
            parameters.Count = Pager.PageSize;

            this.comments.Parameters = parameters;
        }

        private void RenderPager(int total)
        {
            if (total > 0)
            {
                this.TotalPages = (total / this.Pager.PageSize) + (total % this.Pager.PageSize > 0 ? 1 : 0);
            }

            //set the page format
            this.Pager.PageLinkFormat = string.Format("{0}?view={1}&q={2}&p=", Request.Url.AbsolutePath, Request["view"], Request["q"]) + "{0}";

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
