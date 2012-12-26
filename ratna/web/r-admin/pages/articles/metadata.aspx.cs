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
namespace Jardalu.Ratna.Web.Admin.pages.articles
{
    #region using

    using System;

    using Jardalu.Ratna.Web.Admin.articles;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Core.Navigation;

    #endregion

    public partial class metadata : ArticleBasePage
    {

        #region private fields

        private const string ArticleMetadataJavascriptKey = "articles.metadata.js";
        private const string CancelUrlJsVariable = "CancelUrl";

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateJavascriptIncludesAndVariables();

            this.menu.Selected = ResourceManager.GetLiteral("Admin.Articles.Metadata");
            this.Title = ResourceManager.GetLiteral("Admin.Articles.Metadata");

            foreach (Tuple<string, string> tuple in GetMenuItems())
            {
                this.menu.AddMenu(tuple.Item1, tuple.Item2);
            }

            //load the article for editing
            Article article = null;

            if (!string.IsNullOrEmpty(UrlKey))
            {
                bool success = ArticleStore.Instance.TryGetArticle(UrlKey, PublishingStage.Draft, out article);
                this.urlkey.Value = UrlKey;

                if (success)
                {
                    this.content.Visible = true;
                    this.errorDiv.Visible = false;

                    //navigation
                    PopulateNavigationAndBreadCrumb(article);
                }
                else
                {
                    this.content.Visible = false;
                    this.errorDiv.Visible = true;
                }
            }
            else
            {
                this.content.Visible = false;
                this.errorDiv.Visible = true;
            }

            if (IsPageView)
            {
                RenderForPage(article == null?null:new StaticArticle(article));
                this.articleview.Value = "page";
            }
            else 
            {
                RenderForBlogArticle(article == null ? null : new BlogArticle(article));
            }
        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    ArticleMetadataJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.ArticleMetadataPage)
                );

            string cancelUrl = Constants.Urls.Articles.Url;

            if (IsPageView)
            {
                cancelUrl = Constants.Urls.Pages.Url;
            }

            this.clientJavaScript.RegisterClientScriptVariable(
                    CancelUrlJsVariable,
                    Utility.ResolveUrl(cancelUrl)
            );
        }

        private void PopulateNavigationAndBreadCrumb(Article article)
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
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Common.Home"), Constants.Urls.AdminUrl);

                if (IsPageView)
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Pages"), Constants.Urls.Pages.Url);
                    breadcrumb.Add(article.Title, Constants.Urls.Pages.EditUrlWithKey + article.UrlKey);
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Articles.Metadata"), Constants.Urls.Pages.MetadataUrlWithKey + article.UrlKey);
                }
                else
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Articles"), Constants.Urls.Articles.Url);
                    breadcrumb.Add(article.Title, Constants.Urls.Articles.EditUrlWithKey + article.UrlKey);
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Articles.Metadata"), Constants.Urls.Articles.MetadataUrlWithKey + article.UrlKey);
                }
                
            }
        }

        private void RenderForPage(StaticArticle article)
        {
            // pages don't have summary
            this.summarydiv.Visible = false;

            if (article != null)
            {
                this.headtext.Value = article.Head;
                this.tags.Value = article.SerializedTags;
                this.navigationtab.Value = ((INavigationTag)article).Name;
                this.description.Value = article.Description;
            }
        }

        private void RenderForBlogArticle(BlogArticle article)
        {
            this.headdiv.Visible = false;

            if (article != null)
            {
                this.summary.Value = article.Summary;
                this.tags.Value = article.SerializedTags;
                this.navigationtab.Value = ((INavigationTag)article).Name;
                this.description.Value = article.Description;
            }
        }

        #endregion

    }
}
