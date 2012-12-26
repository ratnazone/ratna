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

    using Jardalu.Ratna.Core.Forms;
    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Admin.controls.Forms;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class responses : PagerSupportPage
    {
        #region private fields

        private Form form;
        private bool loaded;

        #endregion

        #region private properties

        private string FormName
        {
            get
            {
                return Request["form"];
            }
        }

        private new Form Form
        {
            get
            {
                if (!loaded)
                {
                    lock (this)
                    {
                        if (!loaded)
                        {
                            if (!string.IsNullOrEmpty(this.FormName))
                            {
                               FormsPlugin.Instance.TryRead(this.FormName, out form);
                            }

                            loaded = true;
                        }
                    }
                }

                return form;
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            //set the pagesize
            this.PageSize = Pager.PageSize;

            PopulateNavigationAndBreadCrumb();
            SetResponsesParameters();
            SetActionPanels();

            if (this.Form != null)
            {
                this.entriesList.FormName = this.Form.Name;
                this.Title = this.Form.DisplayName;

                //fetch the data
                this.entriesList.LoadData();
                RenderPager(this.entriesList.TotalRecords);
            }
            else
            {
                this.entriesList.Visible = false;
            }

        }

        #endregion

        #region private methods

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = Constants.Navigation.Forms;
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null && this.Form != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Forms"), Constants.Urls.Forms.Url);
                breadcrumb.Add(this.Form.DisplayName, Constants.Urls.Forms.ResponsesUrl + this.Form.Name);
            }
        }

        private void SetResponsesParameters()
        {
            ListControlParameters parameters = new ListControlParameters();
            
            parameters.Query = Query;
            parameters.Start = Start;
            parameters.Count = Pager.PageSize;

            this.entriesList.Parameters = parameters;
        }

        private void SetActionPanels()
        {
            // new entries
            this.actionPanel.AddAction(
                    "/images/plus.png",
                    ResourceManager.GetLiteral("Admin.Forms.Entry.AddNew"),
                    string.Format(Constants.Urls.Forms.EntryUrlWithKey, FormName, Guid.Empty)
                );
        }

        private void RenderPager(int total)
        {
            if (total > 0)
            {
                this.TotalPages = (total / this.Pager.PageSize) + (total % this.Pager.PageSize > 0 ? 1 : 0);
            }

            //set the page format
            this.Pager.PageLinkFormat = string.Format("{0}?form={1}&p=", Request.Url.AbsolutePath, Request["form"]) + "{0}";

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
