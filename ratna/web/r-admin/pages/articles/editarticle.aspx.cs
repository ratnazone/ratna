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

    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Web.Admin.controls;

    #endregion

    public partial class editarticle : ArticleBasePage
    {

        #region private fields

        private const string EditArticlePageJavascriptKey = "articles.editarticle.js";
        private const string CancelUrlJsVariable = "CancelUrl";
        private const string ValidateUrlSuccessJsVariable = "L_ValidateUrlSuccess";
        private const string ValidateUrlErrorJsVariable = "L_ValidateUrlError";
        private const string AddImageTitleJsVariable = "L_AddImageTitle";
        private const string PublishArticleConfirmJsVariable = "L_PublishArticleConfirmation";
        private const string PublishArticleSuccessJsVariable = "L_PublishArticleSuccess";

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateJavascriptIncludesAndVariables();

            if (!string.IsNullOrEmpty(UrlKey))
            {
                DisplayForEditArticle(UrlKey);
            }
            else
            {
                DisplayForNewArticle();
            }

            if (IsPageView)
            {
                this.menu.Selected = ResourceManager.GetLiteral("Admin.Pages.Page");
            }
            else
            {
                this.menu.Selected = ResourceManager.GetLiteral("Admin.Articles.Post");
            }

            foreach (Tuple<string, string> tuple in GetMenuItems())
            {
                this.menu.AddMenu(tuple.Item1, tuple.Item2);
            }
        }
        
        #endregion

        #region private methods

        private bool IsBlogArticle(Article article)
        {
            bool isBlog = false;

            if (article != null &&
                article.HandlerId.ToString() == BlogArticleHandler.HandlerId )
            {
                isBlog = true;
            }

            return isBlog;
        }

        private bool IsPageArticle(Article article)
        {
            bool isPage = false;

            if (article != null &&
                article.HandlerId.ToString() == StaticArticleHandler.HandlerId)
            {
                isPage = true;
            }

            return isPage;
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
                    if (article != null)
                    {
                        breadcrumb.Add(article.Title, Constants.Urls.Pages.EditUrlWithKey + article.UrlKey);
                    }
                    else
                    {
                        breadcrumb.Add(ResourceManager.GetLiteral("Admin.Pages.Edit"), string.Empty);
                    }
                }
                else
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Articles"), Constants.Urls.Articles.Url);
                    if (article != null)
                    {
                        breadcrumb.Add(article.Title, Constants.Urls.Articles.EditUrlWithKey + article.UrlKey);
                    }
                    else
                    {
                        breadcrumb.Add(ResourceManager.GetLiteral("Admin.Articles.Edit"), string.Empty);
                    }
                }
                
            }
        }

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    EditArticlePageJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.ArticleEditPage)
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

            this.clientJavaScript.RegisterClientScriptVariable(
                    ValidateUrlSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Edit.Url.Validate.Success")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    ValidateUrlErrorJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Edit.Url.Validate.Error")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                 AddImageTitleJsVariable,
                 ResourceManager.GetLiteral("Admin.Articles.Edit.Images.AddImageTitle")
             );

            this.clientJavaScript.RegisterClientScriptVariable(
                PublishArticleConfirmJsVariable,
                ResourceManager.GetLiteral("Admin.Articles.Publish.Confirmation")
            );

            this.clientJavaScript.RegisterClientScriptVariable(
                    PublishArticleSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Publish.Success")
                );
        }

        private void DisplayForNewArticle()
        {

            if (IsPageView)
            {
                this.Title = ResourceManager.GetLiteral("Admin.Pages.New");
                this.headerLiteral.Text = ResourceManager.GetLiteral("Admin.Pages.Edit");
                this.articleview.Value = "page";
                
            }
            else
            {
                this.Title = ResourceManager.GetLiteral("Admin.Articles.New");
            }

            //adding new article should not show error
            this.errorDiv.Visible = false;

            // need to populate the url to the editing of the page
            PopulateNavigationAndBreadCrumb(null);

        }

        private void DisplayForEditArticle(string urlKey)
        {
            if (IsPageView)
            {
                this.Title = ResourceManager.GetLiteral("Admin.Pages.Edit");
                this.headerLiteral.Text = ResourceManager.GetLiteral("Admin.Pages.Edit");
            }
            else
            {
                this.Title = ResourceManager.GetLiteral("Admin.Articles.Edit");
            }

            //load the article for editing
            Article article = null;
            bool success = ArticleStore.Instance.TryGetArticle(urlKey, PublishingStage.Draft, out article);

            //make sure article can be handled by the page
            if (success)
            {
                if (IsPageView)
                {
                    success = IsPageArticle(article);
                }
                else
                {
                    success = IsBlogArticle(article);
                }               
            }

            if (!success)
            {
                this.errorDiv.Visible = true;
                this.content.Visible = false;
            }
            else
            {
                //populate the fields
                this.errorDiv.Visible = false;

                // need to populate the url to the editing of the page
                PopulateNavigationAndBreadCrumb(article);

                this.articletitle.Value = article.Title;
                this.urlkey.Value = article.UrlKey;
                this.urlkey.Disabled = true;
                this.validatebutton.Disabled = true;


                if (IsPageView)
                {
                    StaticArticle staticArticle = new StaticArticle(article);
                    this.articlebody.Value = staticArticle.Body;

                    this.articleview.Value = "page";

                }
                else
                {
                    BlogArticle blogArticle = new BlogArticle(article);
                    this.articlebody.Value = blogArticle.Body;
                }
            }
        }

        #endregion

    }
}
