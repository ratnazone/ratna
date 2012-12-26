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
namespace Jardalu.Ratna.Web.Admin.posts
{
    #region using

    using System;

    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Web.Admin.controls.Articles;
    using Jardalu.Ratna.Web.Admin.controls;

    #endregion

    public partial class _default : System.Web.UI.Page
    {

        #region public properties

        public bool IsPageView
        {
            get
            {
                bool viewPage = false;

                string style = this.Request["view"];
                if (!string.IsNullOrEmpty(style) &&
                    style.Equals("page", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    viewPage = true;
                }

                return viewPage;
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();
            SetActionPanels();

            if (IsPageView)
            {
                SetPageView();
            }
            else
            {
                SetArticleView();
            }
        }

        #endregion

        #region private methods

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
                }
                else
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Articles"), Constants.Urls.Articles.Url);
                }
            }
        }

        private void SetActionPanels()
        {
            if (IsPageView)
            {
                // new page
                this.actionPanel.AddAction(
                        "/images/plus.png",
                        ResourceManager.GetLiteral("Admin.Pages.List.AddNew"),
                        Constants.Urls.Pages.EditUrl
                    );

            }
            else
            {
                // new article
                this.actionPanel.AddAction(
                        "/images/plus.png",
                        ResourceManager.GetLiteral("Admin.Articles.List.AddNew"),
                        Constants.Urls.Articles.EditUrl
                    );
            }

        }

        private void SetPageView()
        {
            this.Title = ResourceManager.GetLiteral("Admin.Pages.List.PageTitle");


            //get a static handler
            StaticArticleHandler handler = new StaticArticleHandler();

            //fetch only pages
            this.draftArticlesList.ArticleHandler = handler;
            this.publishedArticlesList.ArticleHandler = handler;

            ArticleListParameters draftParameters = new ArticleListParameters();
            draftParameters.EditUrlPrefix = Constants.Urls.Pages.EditUrlWithKey;
            draftParameters.MoreUrl = Constants.Urls.Pages.ListDraftUrl;
            draftParameters.ShowDelete = true;
            draftParameters.ShowPublish = true;
            draftParameters.Header = ResourceManager.GetLiteral("Admin.Pages.Draft");
            draftParameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Pages.NoPages");

            ArticleListParameters publishedParameters = new ArticleListParameters();
            publishedParameters.EditUrlPrefix = Constants.Urls.Pages.EditUrlWithKey;
            publishedParameters.MoreUrl = Constants.Urls.Pages.ListPublishedUrl;
            publishedParameters.ShowDelete = true;
            publishedParameters.ShowPublish = false;
            publishedParameters.Header = ResourceManager.GetLiteral("Admin.Pages.Published");
            publishedParameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Pages.NoPages");
            
            this.draftArticlesList.Parameters = draftParameters;
            this.publishedArticlesList.Parameters = publishedParameters;
        }

        private void SetArticleView()
        {
            this.Title = ResourceManager.GetLiteral("Admin.Articles.List.PageTitle");

            //blog article handler
            BlogArticleHandler handler = new BlogArticleHandler();

            //fetch only blog articles
            this.draftArticlesList.ArticleHandler = handler;
            this.publishedArticlesList.ArticleHandler = handler;

            ArticleListParameters draftParameters = new ArticleListParameters();
            draftParameters.EditUrlPrefix = Constants.Urls.Articles.EditUrlWithKey;
            draftParameters.MoreUrl = Constants.Urls.Articles.ListDraftUrl;
            draftParameters.ShowDelete = true;
            draftParameters.ShowPublish = true;
            draftParameters.Header = ResourceManager.GetLiteral("Admin.Articles.Draft");
            draftParameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Articles.NoArticles");

            ArticleListParameters publishedParameters = new ArticleListParameters();
            publishedParameters.EditUrlPrefix = Constants.Urls.Articles.EditUrlWithKey;
            publishedParameters.MoreUrl = Constants.Urls.Articles.ListPublishedUrl;
            publishedParameters.ShowDelete = true;
            publishedParameters.ShowPublish = false;
            publishedParameters.Header = ResourceManager.GetLiteral("Admin.Articles.Published");
            publishedParameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Articles.NoArticles");

            this.draftArticlesList.Parameters = draftParameters;
            this.publishedArticlesList.Parameters = publishedParameters;
        }

        #endregion

    }
}
