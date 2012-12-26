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

    #endregion

    public partial class articlemedia : ArticleBasePage
    {

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = ResourceManager.GetLiteral("Admin.Articles.Media");
            this.menu.Selected = ResourceManager.GetLiteral("Admin.Articles.Media");
            foreach (Tuple<string, string> tuple in GetMenuItems())
            {
                this.menu.AddMenu(tuple.Item1, tuple.Item2);
            }

            /* article gallery */
            this.articleGallery.Header = ResourceManager.GetLiteral("Admin.Articles.Media.Images");
            this.articleGallery.SubHeader = ResourceManager.GetLiteral("Admin.Articles.Media.Images.Help");
            this.articleGallery.ShowControls = true;

            /* article referenced images */
            this.articleReferencedImages.SubHeader = ResourceManager.GetLiteral("Admin.Articles.Media.Images.Referenced.Help");
            this.articleReferencedImages.Header = ResourceManager.GetLiteral("Admin.Articles.Media.Images.Referenced");

            if (!string.IsNullOrEmpty(UrlKey))
            {
                //load the article for editing
                Article article = null;
                bool success = ArticleStore.Instance.TryGetArticle(UrlKey, PublishingStage.Draft, out article);

                if (success)
                {
                    this.content.Visible = true;
                    this.errorDiv.Visible = false;

                    //navigation
                    PopulateNavigationAndBreadCrumb(article);

                    RenderGalleryImages(article);
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
        }

        #endregion

        #region private methods

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
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Articles.Media"), Constants.Urls.Pages.MediaUrlWithKey + article.UrlKey);
                }
                else
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Articles"), Constants.Urls.Articles.Url);
                    breadcrumb.Add(article.Title, Constants.Urls.Articles.EditUrlWithKey + article.UrlKey);
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Articles.Media"), Constants.Urls.Articles.MediaUrlWithKey + article.UrlKey);
                }

            }
        }

        private void RenderGalleryImages(Article article) 
        {
            if (article != null)
            {
                this.articleGallery.UrlKey = article.UrlKey;

                BlogArticle blogArticle = new BlogArticle(article);
                if (blogArticle != null &&
                    blogArticle.Images != null &&
                    blogArticle.Images.Length != 0)
                {
                    // populate the image gallery
                    this.articleGallery.Images = blogArticle.Images;
                }

                if (blogArticle != null && !string.IsNullOrEmpty(blogArticle.Body))
                {
                    //get the referenced images
                    this.articleReferencedImages.Images = Jardalu.Ratna.Utilities.Utility.GetImagesSrc(blogArticle.Body);
                }
            }

        }

        #endregion
    }
}
