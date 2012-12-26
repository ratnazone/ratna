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
namespace Jardalu.Ratna.Web.Admin
{
    #region using

    using System;

    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Admin.controls.Articles;
    using Jardalu.Ratna.Web.Admin.controls.Comments;
    using Jardalu.Ratna.Web.Admin.controls.Media;
    using Jardalu.Ratna.Web.Admin.controls.Users;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class _default : System.Web.UI.Page
    {

        protected global::Jardalu.Ratna.Web.Admin.controls.Articles.ArticlesList draftArticlesList;
        protected global::Jardalu.Ratna.Web.Admin.controls.Articles.ArticlesList draftPagesList;
        protected global::Jardalu.Ratna.Web.Admin.controls.Media.MediaList mediaList;
        protected global::Jardalu.Ratna.Web.Admin.controls.Users.UsersList usersList;
        protected global::Jardalu.Ratna.Web.Admin.controls.Comments.CommentsList pendingCommentsList;
        protected global::Jardalu.Ratna.Web.Admin.controls.Forms.FormEntries formEntries;

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();
            SetArticlesParameters();
            SetMediaParameters();
            SetUsersListParameters();
            SetCommentsListParameters();
        }

        #endregion

        #region private methods

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = Constants.Navigation.Overview;
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Common.Home"), Constants.Urls.AdminUrl);
            }
        }

        private void SetArticlesParameters()
        {
            ArticleListParameters draftParameters = new ArticleListParameters();
            draftParameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Articles.NoArticles");
            draftParameters.MoreUrl = Utility.ResolveUrl(Constants.Urls.Articles.ListDraftUrl);
            draftParameters.Header = ResourceManager.GetLiteral("Admin.Articles.Draft");

            this.draftArticlesList.ArticleHandler = new BlogArticleHandler();
            this.draftArticlesList.Parameters = draftParameters;

            ArticleListParameters draftPagesParameters = new ArticleListParameters();
            draftPagesParameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Pages.NoPages");
            draftPagesParameters.MoreUrl = Utility.ResolveUrl(Constants.Urls.Pages.ListPublishedUrl);
            draftPagesParameters.Header = ResourceManager.GetLiteral("Admin.Pages.Draft");

            this.draftPagesList.ArticleHandler = new StaticArticleHandler();
            this.draftPagesList.Parameters = draftPagesParameters;
        }

        private void SetMediaParameters()
        {
            MediaListParameters mediaParameters = new MediaListParameters();
            mediaParameters.MoreUrl = Utility.ResolveUrl(Constants.Urls.Media.Url);
            mediaParameters.MediaType = Core.Media.MediaType.All;
            mediaParameters.Header = ResourceManager.GetLiteral("Admin.Media.List.AllHeader");
            mediaParameters.DisplayTableHeader = false;
            mediaParameters.HideEdit = true;
            mediaParameters.HidePreview = true;
            mediaParameters.HideUrl = true;
            mediaParameters.ShortDate = true;
            mediaParameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Media.List.All.NoFound");

            this.mediaList.Parameters = mediaParameters;
        }

        private void SetUsersListParameters()
        {
            UserListParameters parameters = new UserListParameters();
            parameters.MoreUrl = Utility.ResolveUrl(Constants.Urls.Users.Url);;

            this.usersList.Parameters = parameters;
        }

        private void SetCommentsListParameters()
        {
            CommentListParameters parameters = new CommentListParameters();
            parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Comments.List.Pending.NoComments");
            parameters.Header = ResourceManager.GetLiteral("Admin.Comments.List.Pending.Header");
            parameters.DisplayTableHeader = false;
            parameters.MoreUrl = Utility.ResolveUrl(Constants.Urls.Comments.ListPendingUrl);
            parameters.FetchPending = true;
            parameters.Count = 5;

            this.pendingCommentsList.Parameters = parameters;
        }

        #endregion

    }

}
