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
function _constants() {
    this.AdminUrl = "/r-admin";
    this.AdminPagesUrl = "/r-admin/pages";
    this.ServiceRoot = this.AdminUrl + "/service";

    this.PublicServiceRoot = "/service";

    this.ProfileServiceUrl = this.ServiceRoot + "/profile.asmx";
    this.ArticleServiceUrl = this.ServiceRoot + "/article.asmx";
    this.UploadServiceUrl = this.ServiceRoot + "/uploader.ashx";
    this.PermissionsServiceUrl = this.ServiceRoot + "/permissions.asmx";
    this.UsersServiceUrl = this.ServiceRoot + "/users.asmx";
    this.GroupsServiceUrl = this.ServiceRoot + "/groups.asmx";
    this.TemplatesServiceUrl = this.ServiceRoot + "/template.asmx";
    this.AutoPathsServiceUrl = this.ServiceRoot + "/autopaths.asmx";
    this.MediaServiceUrl = this.ServiceRoot + "/media.asmx";
    this.CommentsServiceUrl = this.ServiceRoot + "/comments.asmx";
    this.ConfigurationServiceUrl = this.ServiceRoot + "/configuration.asmx";
    this.SiteConfigurationServiceUrl = this.ServiceRoot + "/siteconfiguration.asmx";
    this.FormsManageServiceUrl = this.ServiceRoot + "/formsmanage.asmx";
    this.CommentsPublicServiceUrl = this.PublicServiceRoot + "/comments.asmx";
    this.AppsServiceUrl = this.ServiceRoot + "/apps.asmx";
    this.GalleryServiceUrl = this.ServiceRoot + "/gallery.asmx";

    _profileService = function (parent) {
        this.UpdateUrl = parent.ProfileServiceUrl + "/Update";
    }

    _articleService = function (parent) {
        this.ValidateKeyUrl = parent.ArticleServiceUrl + "/ValidateUrlKey";
        this.SaveUrl = parent.ArticleServiceUrl + "/Save";
        this.PageSaveUrl = parent.ArticleServiceUrl + "/SavePage";
        this.SaveMetadataUrl = parent.ArticleServiceUrl + "/SaveMetadata";
        this.SavePageMetadataUrl = parent.ArticleServiceUrl + "/SavePageMetadata";
        this.AddImagesUrl = parent.ArticleServiceUrl + "/AddImages";
        this.RemoveImageUrl = parent.ArticleServiceUrl + "/RemoveImage";
        this.DeleteUrl = parent.ArticleServiceUrl + "/Delete";
        this.DeleteVersionUrl = parent.ArticleServiceUrl + "/DeleteVersion";
        this.PublishUrl = parent.ArticleServiceUrl + "/Publish";
        this.DeleteMultipleUrl = parent.ArticleServiceUrl + "/DeleteMultiple";
        this.PublishMultipleUrl = parent.ArticleServiceUrl + "/PublishMultiple";
        this.RevertUrl = parent.ArticleServiceUrl + "/Revert";
    }

    _permissionsService = function (parent) {
        this.SearchUserOrGroupUrl = parent.PermissionsServiceUrl + "/SearchUserOrGroup";
        this.SetAclsUrl = parent.PermissionsServiceUrl + "/SetAcls";
        this.DeleteAclsUrl = parent.PermissionsServiceUrl + "/DeleteAcls";
    }

    _groupsService = function (parent) {
        this.SearchGroupUrl = parent.GroupsServiceUrl + "/SearchGroup";
    }

    _usersService = function (parent) {
        this.ActivateUserUrl = parent.UsersServiceUrl + "/ActivateUser";
        this.DeleteUserUrl = parent.UsersServiceUrl + "/DeleteUser";
    }

    _templatesService = function (parent) {
        this.SaveTemplateUrl = parent.TemplatesServiceUrl + "/SaveTemplate";
        this.DeleteUrl = parent.TemplatesServiceUrl + "/Delete";
    }

    _autoPathsService = function (parent) {
        this.SaveUrl = parent.AutoPathsServiceUrl + "/Save";
        this.DeleteUrl = parent.AutoPathsServiceUrl + "/Delete";
    }

    _mediaService = function (parent) {
        this.SaveUrl = parent.MediaServiceUrl + "/Save";
        this.DeleteUrl = parent.MediaServiceUrl + "/Delete";
        this.SearchUrl = parent.MediaServiceUrl + "/Search";
    }

    _commentsService = function (parent) {
        this.DeleteCommentUrl = parent.CommentsServiceUrl + "/Delete";
        this.ApproveCommentUrl = parent.CommentsServiceUrl + "/Approve";
    }

    _configurationService = function (parent) {
        this.UpdateUrl = parent.ConfigurationServiceUrl + "/Update";
    }

    _siteConfigurationService = function (parent) {
        this.UpdateUrl = parent.SiteConfigurationServiceUrl + "/Update";
        this.UpdateSmtpUrl = parent.SiteConfigurationServiceUrl + "/UpdateSmtp";
        this.UpdateNotificationUrl = parent.SiteConfigurationServiceUrl + "/UpdateNotification";
        this.UpdateCustomResponsesUrl = parent.SiteConfigurationServiceUrl + "/UpdateCustomResponses";
    }

    _formsManageService = function (parent) {
        this.AddFieldUrl = parent.FormsManageServiceUrl + "/AddField";
        this.DeleteFieldUrl = parent.FormsManageServiceUrl + "/DeleteField";
        this.DeleteFormUrl = parent.FormsManageServiceUrl + "/Delete";
        this.SaveFormUrl = parent.FormsManageServiceUrl + "/Save";
        this.DeleteResponsesUrl = parent.FormsManageServiceUrl + "/DeleteResponses";
        this.EditEntryUrl = parent.FormsManageServiceUrl + "/EditEntry";
    }

    _appsService = function (parent) {
        this.ActivateUrl = parent.AppsServiceUrl + "/Activate";
        this.SavePropertiesUrl = parent.AppsServiceUrl + "/SaveProperties";
        this.DeleteUrl = parent.AppsServiceUrl + "/Delete";
    }

    _galleryService = function (parent) {
        this.SaveUrl = parent.GalleryServiceUrl + "/Save";
        this.DeleteUrl = parent.GalleryServiceUrl + "/Delete";
        this.AddPhotoUrl = parent.GalleryServiceUrl + "/AddPhotos";
    }

    _commentsPublicService = function (parent) {
        this.AddCommentUrl = parent.CommentsPublicServiceUrl + "/AddComment";
    }

    _articles = function (parent) {

        this.Url = parent.AdminPagesUrl + "/articles";
        this.PreviewAppender = "__draft";
        this.EditUrl = this.Url + "/editarticle.aspx";
        this.EditUrlWithKey = this.EditUrl + "?url=";

    }

    _settings = function (parent) {
        this.Url = parent.AdminPagesUrl + "/settings";
        this.AutoPathsUrl = this.Url + "/autopaths.aspx";
    }

    _templates = function (parent) {
        this.Url = parent.AdminPagesUrl + "/settings";
        this.EditUrl = this.Url + "/edittemplate.aspx";
        this.EditUrlWithKey = this.EditUrl + "?id="
    }

    _forms = function (parent) {
        this.Url = parent.AdminPagesUrl + "/forms";
        this.EditUrl = this.Url + "/editform.aspx?form={0}";
        this.EntriesUrl = this.Url + "/responses.aspx?form={0}";
        this.EditEntryUrl = this.Url + "/editentry.aspx?form={0}&uid={1}";
    }


    _media = function (parent) {
        this.Url = parent.AdminPagesUrl + "/media";
        this.EditUrl = this.Url + "/editmedia.aspx";
        this.EditUrlWithKey = this.EditUrl + "?url=";
        this.GalleryEditUrl = this.Url + "/gallery/edit.aspx";
        this.GalleryEditUrlWithKey = this.GalleryEditUrl + "?url={0}";
    }

    _apps = function (parent) {
        this.Url = parent.AdminPagesUrl + "/apps";
        this.EditUrl = this.Url + "/edit.aspx";
        this.EditUrlWithKey = this.EditUrl + "?id="
    }

    this.ProfileService = new _profileService(this);
    this.ArticleService = new _articleService(this);
    this.PermissionsService = new _permissionsService(this);
    this.UsersService = new _usersService(this);
    this.GroupsService = new _groupsService(this);
    this.TemplatesService = new _templatesService(this);
    this.AutoPathsService = new _autoPathsService(this);
    this.MediaService = new _mediaService(this);
    this.CommentsService = new _commentsService(this);
    this.ConfigurationService = new _configurationService(this);
    this.SiteConfigurationService = new _siteConfigurationService(this);
    this.FormsManageService = new _formsManageService(this);
    this.AppsService = new _appsService(this);
    this.GalleryService = new _galleryService(this);
    this.CommentsPublicService = new _commentsPublicService(this);

    this.Articles = new _articles(this);
    this.Settings = new _settings(this);
    this.Templates = new _templates(this);
    this.Media = new _media(this);
    this.Forms = new _forms(this);
    this.Apps = new _apps(this);
}

var Constants = new _constants();
