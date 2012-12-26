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
namespace Jardalu.Ratna.Web.Admin.articles
{
    #region using

    using System;

    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.Admin.controls.Articles;
    using Jardalu.Ratna.Web.Admin.controls;

    #endregion

    public partial class list : ArticleBasePage
    {

        #region protected properties

        protected string SearchUrl
        {
            get
            {
                return Request.Url.AbsolutePath + "?view=" + View + "&stage=" + Stage.ToString() + "&q={0}";
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();

            if (IsPageView)
            {
                DisplayPageView();
            }
            else
            {
                DisplayArticleView();
            }

            this.PageSize = this.Pager.PageSize;
            this.articlesList.Parameters.Count = this.Pager.PageSize;
            this.articlesList.Parameters.Query = Query;
            this.articlesList.Parameters.Start = Start;
            this.articlesList.LoadData();

            this.RenderPager(this.articlesList.TotalRecords);

            this.Title = this.headerH1.InnerText;
        }

        #endregion

        #region private methods

        private void DisplayPageView()
        {
            ArticleListParameters parameters = new ArticleListParameters();
            parameters.HideHeader = true;
            parameters.DisplayTableHeader = true;
            parameters.ShowDelete = true;
            parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Pages.NoPages");
            parameters.AllowMultiSelect = true;
            parameters.EditUrlPrefix = Constants.Urls.Pages.EditUrlWithKey;

            if (Stage == PublishingStage.Draft)
            {
                this.headerH1.InnerText = ResourceManager.GetLiteral("Admin.Pages.Draft");
                parameters.ShowPublish = true;
            }
            else
            {
                this.headerH1.InnerText = ResourceManager.GetLiteral("Admin.Pages.Published");
            }

            this.articlesList.ArticleHandler = new StaticArticleHandler();
            this.articlesList.Stage = Stage;
            this.articlesList.Parameters = parameters;
        }

        private void DisplayArticleView()
        {
            ArticleListParameters parameters = new ArticleListParameters();
            parameters.HideHeader = true;
            parameters.DisplayTableHeader = true;
            parameters.ShowDelete = true;
            parameters.AllowMultiSelect = true;

            parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Articles.NoArticles");

            if (Stage == PublishingStage.Draft)
            {
                this.headerH1.InnerText = ResourceManager.GetLiteral("Admin.Articles.Draft");
                parameters.ShowPublish = true;
            }
            else
            {
                this.headerH1.InnerText = ResourceManager.GetLiteral("Admin.Articles.Published");
                parameters.ShowPublish = false;
            }

            this.articlesList.Stage = Stage;
            this.articlesList.Parameters = parameters;
        }

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                if (IsPageView)
                {
                    navigation.Selected = Constants.Navigation.Pages;
                }
                else
                {
                    navigation.Selected = Constants.Navigation.Articles;
                }
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                if (IsPageView)
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Pages"), Constants.Urls.Pages.Url);
                    if (Stage == PublishingStage.Draft)
                    {
                        breadcrumb.Add(ResourceManager.GetLiteral("Admin.Pages.Draft"), Constants.Urls.Pages.ListDraftUrl);
                    }
                    else
                    {
                        breadcrumb.Add(ResourceManager.GetLiteral("Admin.Pages.Published"), Constants.Urls.Pages.ListPublishedUrl);
                    }
                }
                else
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Articles"), Constants.Urls.Articles.Url);
                    if (Stage == PublishingStage.Draft)
                    {
                        breadcrumb.Add(ResourceManager.GetLiteral("Admin.Articles.Draft"), Constants.Urls.Articles.ListDraftUrl);
                    }
                    else
                    {
                        breadcrumb.Add(ResourceManager.GetLiteral("Admin.Articles.Published"), Constants.Urls.Articles.ListPublishedUrl);
                    }
                }
            }
        }

        private void RenderPager(int total)
        {
            if (total > 0)
            {
                this.TotalPages = (total / this.Pager.PageSize) + (total % this.Pager.PageSize > 0 ? 1 : 0);
            }

            //set the page format
            this.Pager.PageLinkFormat = string.Format("{0}?view={1}&stage={2}&q={3}&p=", Request.Url.AbsolutePath, View, Stage, Query) + "{0}";

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
