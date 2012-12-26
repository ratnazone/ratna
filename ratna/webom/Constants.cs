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
namespace Jardalu.Ratna.Web
{
    #region using

    using System;

    #endregion

    public static partial class Constants
    {
        public const string PageRouteIdentifier = "p";
        public const string SearchRouteIdentifier = "q";
        public const string ErrorCodeIdentifier = "e";
        public const string LandingUrlIdentifier = "r";
        public const string StageIdentifier = "stage";
        public const string VersionIdentifier = "version";
        public const string ViewIdentifier = "view";
        public const string UrlIdentifier = "url";
        public const string QueryIdentifier = "q";
        public const string DraftStageIdentifier = "/__draft";
        public const string FetchVersionIdentifier = "/__version__";

        public const string RootUrl = "/";
        public const string DefaultPageUrl = "/default";

        public const string DefaultTemplateName = "template";
        public const int LatestVersion = -1;                        // use for getting the article with version

        public const string ProductName = "Ratna";

        public static class Snippet
        {
            public const string SnippetIdentifier = "_s";
            public const string SnippetNameIdentifier = "_sn";
            public const string SnippetKeysIdentifier = "_sk";
            public static char[] SnippetKeysToken = new char[1]{','};
        }

        public static class Urls
        {
            public const string DefaultLandingUrl = "/r-admin/";
            public const string RelativeAdminUrl = "/r-admin/";
            public const string AdminUrl = "~/r-admin";
            public const string AdminPagesUrl = "~/r-admin/pages";

            public const string MainUrl = "~";

            public static class Users
            {
                public const string Url = AdminPagesUrl + "/users";
                public const string ListUrl = Url + "/list.aspx";
                public const string ListActiveUrl = Url + "/list.aspx?type=active";
                public const string ListPendingUrl = Url + "/list.aspx?type=pending";
                public const string ListDeletedUrl = Url + "/list.aspx?type=deleted";
                public const string ViewUserUrl = Url + "/user.aspx";
                public const string ViewUserUrlWithKey = ViewUserUrl + "?alias=";
                public const string EditUserUrl = Url + "/edituser.aspx";
            }

            public static class Profile
            {
                public const string Url = AdminPagesUrl + "/profile";
                public const string EditProfileUrl = Url + "/editprofile.aspx";
            }

            public static class Service
            {
                public const string Url = AdminUrl + "/service";
                public const string UploadUrl = Url + "/uploader.ashx";
            }

            public static class Articles
            {
                public const string Url = AdminPagesUrl + "/articles";
                public const string EditUrl = Url + "/editarticle.aspx";
                public const string EditUrlWithKey = EditUrl + "?url=";
                public const string ListUrl = Url + "/list.aspx";
                public const string ListDraftUrl = ListUrl + "?stage=draft";
                public const string ListPublishedUrl = ListUrl;
                public const string VersionsUrlWithKey = Url + "/versions.aspx?url=";
                public const string MetadataUrlWithKey = Url + "/metadata.aspx?url=";
                public const string MediaUrlWithKey = Url + "/articlemedia.aspx?url=";
            }

            public static class Pages
            {
                public const string Url = AdminPagesUrl + "/articles/default.aspx?view=page";
                public const string BaseUrl = AdminPagesUrl + "/articles";
                public const string EditUrl = BaseUrl + "/editarticle.aspx?view=page";
                public const string EditUrlWithKey = EditUrl + "&url=";
                public const string ListUrl = BaseUrl + "/list.aspx?view=page";
                public const string ListDraftUrl = BaseUrl + "/list.aspx?view=page&stage=draft";
                public const string ListPublishedUrl = BaseUrl + "/list.aspx?view=page";
                public const string VersionsUrlWithKey = BaseUrl + "/versions.aspx?view=page&url=";
                public const string MetadataUrlWithKey = BaseUrl + "/metadata.aspx?view=page&url=";
                public const string MediaUrlWithKey = BaseUrl + "/articlemedia.aspx?view=page&url=";
            }

            public static class Media
            {
                public const string Url = AdminPagesUrl + "/media/default.aspx";
                public const string BaseUrl = AdminPagesUrl + "/media";
                
                public static class Photos
                {
                    public const string EditUrl = BaseUrl + "/editmedia.aspx?view=photo";
                    public const string EditUrlWithKey = EditUrl + "&url=";
                    public const string ListUrl = BaseUrl + "/list.aspx?view=photo";
                }

                public static class Videos
                {
                    public const string EditUrl = BaseUrl + "/editmedia.aspx?view=video";
                    public const string EditUrlWithKey = EditUrl + "&url=";
                    public const string ListUrl = BaseUrl + "/list.aspx?view=video";
                }

                public static class Documents
                {
                    public const string EditUrl = BaseUrl + "/editmedia.aspx?view=document";
                    public const string EditUrlWithKey = EditUrl + "&url=";
                    public const string ListUrl = BaseUrl + "/list.aspx?view=document";
                }

                public static class Others
                {
                    public const string EditUrl = BaseUrl + "/editmedia.aspx?view=other";
                    public const string EditUrlWithKey = EditUrl + "&url=";
                    public const string ListUrl = BaseUrl + "/list.aspx?view=other";
                }

                public static class Gallery
                {
                    public const string Url = BaseUrl + "/gallery";
                    public const string EditUrl = Url + "/edit.aspx";
                    public const string EditUrlWithKey = EditUrl + "?url=";
                }
            }

            public static class Settings
            {
                public const string Url = AdminPagesUrl + "/settings";
                public const string TemplatesUrl = Url + "/templates.aspx";
                public const string EditTemplateUrl = Url + "/edittemplate.aspx";
                public const string EditTemplateUrlWithKey = EditTemplateUrl + "?id=";
                public const string NotificationUrl = Url + "/notification.aspx";
                public const string CustomErrorsUrl = Url + "/customerrors.aspx";
                public const string SmtpUrl = Url + "/smtp.aspx";
                public const string AutoPathsUrl = Url + "/autopaths.aspx";
                public const string EditAutoPathUrl = Url + "/editautopath.aspx";
                public const string EditAutoPathUrlWithKey = EditAutoPathUrl + "?path=";
            }

            public static class Forms
            {
                public const string Url = AdminPagesUrl + "/forms";
                public const string ResponsesUrl = Url + "/responses.aspx?form=";
                public const string EditUrl = Url + "/editform.aspx";
                public const string EditUrlWithKey = Url + "/editform.aspx?form=";
                public const string EntryUrl = Url + "/editentry.aspx";
                public const string EntryUrlWithKey = Url + "/editentry.aspx?form={0}&uid={1}";
                public const string RecentUrl = Url + "/recent.aspx";
            }

            public static class Apps
            {
                public const string Url = AdminPagesUrl + "/apps";
                public const string AddNewUrl = Url + "/add.aspx";
                public const string EditUrl = Url + "/edit.aspx?id={0}";
            }

            public static class Comments
            {
                public const string Url = AdminPagesUrl + "/comments";
                public const string ListPendingUrl = Url + "/list.aspx?view=pending";
                public const string ListApprovedUrl = Url + "/list.aspx";
            }

            public static class Scripts
            {
                public const string Url = AdminUrl + "/scripts";
                public const string ArticleListControl = Url + "/articles/control.articlelist.js";
                public const string ArticleEditPage = Url + "/articles/articles.editarticle.js";
                public const string ArticleMetadataPage = Url + "/articles/articles.metadata.js";
                public const string ArticleMediaControl = Url + "/articles/control.articlemedia.js";
                public const string CommentListControl = Url + "/comments/comments.commentlist.js";
                public const string UsersListControl = Url + "/users/users.userlist.js";
                public const string UsersMembershipControl = Url + "/users/users.usermembership.js";
                public const string TemplateListControl = Url + "/settings/control.templatelist.js";
                public const string AutoPathListControl = Url + "/settings/control.autopathslist.js";
                public const string ConfigurationControl = Url + "/settings/control.configuration.js";
                public const string SiteConfigurationControl = Url + "/settings/control.siteconfiguration.js";
                public const string FormsListControl = Url + "/forms/forms.formslist.js";
                public const string FormResponsesControl = Url + "/forms/forms.responseslist.js";

                public static class External
                {
                    public const string Url = MainUrl + "/external";
                    public const string Confirm = Url + "/simplemodal/confirm.js";
                }
            }

        }

        public static class Navigation
        {
            public const string Overview = "overview";
            public const string Users = "users";
            public const string Profile = "profile";
            public const string Pages = "pages";
            public const string Media = "media";
            public const string Articles = "articles";
            public const string Comments = "comments";
            public const string Settings = "settings";
            public const string Forms = "forms";
            public const string Apps = "Apps";
        }

        public const string CookieName = "session";

        public static class Json
        {
            public const string Success = "success";
            public const string Message = "message";
            public const string Error = "error";
            public const string Html = "html";
        }

        public static class ControlPaths
        {
            public const string UserMembershipSearchRowControl = "~/r-admin/controls/Users/UserMembershipSearchRow.ascx";
        }

    }

}
