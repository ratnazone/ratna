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
namespace Jardalu.Ratna.Web.Admin.pages.media
{

    #region using

    using System;

    using Jardalu.Ratna.Core.Media;
    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web.Upload;

    #endregion

    public partial class editmedia : MediaBasePage
    {

        #region private fields

        private const string EditMediaLiteralJsVariable = "L_EditMediaHeader";

        #endregion

        #region protected

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();
            PopulateJavascriptIncludesAndVariables();

            switch (View)
            {
                case MediaType.Photo:
                    ShowPhotoView();
                    break;
                case MediaType.Document:
                    ShowDocumentView();
                    break;
                case MediaType.Video:
                    ShowVideoView();
                    break;
                case MediaType.Other:
                    ShowOtherView();
                    break;
            }
        }

        #endregion

        #region private methods

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = Constants.Navigation.Media;
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Media"), Constants.Urls.Media.Url);

                string crumbtext = string.Empty;
                string crumburl = string.Empty;

                switch (View)
                {
                    case MediaType.Photo:
                        if (IsEdit)
                        {
                            crumbtext = ResourceManager.GetLiteral("Admin.Media.Edit.Photo.Edit");
                            crumburl = Constants.Urls.Media.Photos.EditUrlWithKey + Url;
                        }
                        else
                        {
                            crumbtext = ResourceManager.GetLiteral("Admin.Media.Edit.Photo.New");
                            crumburl = Constants.Urls.Media.Photos.EditUrl;
                        }
                        break;
                    case MediaType.Video:
                        if (IsEdit)
                        {
                            crumbtext = ResourceManager.GetLiteral("Admin.Media.Edit.Video.Edit");
                            crumburl = Constants.Urls.Media.Videos.EditUrlWithKey + Url;
                        }
                        else
                        {
                            crumbtext = ResourceManager.GetLiteral("Admin.Media.Edit.Video.New");
                            crumburl = Constants.Urls.Media.Videos.EditUrl;
                        }
                        break;
                    case MediaType.Document:
                        if (IsEdit)
                        {
                            crumbtext = ResourceManager.GetLiteral("Admin.Media.Edit.Document.Edit");
                            crumburl = Constants.Urls.Media.Documents.EditUrlWithKey + Url;
                        }
                        else
                        {
                            crumbtext = ResourceManager.GetLiteral("Admin.Media.Edit.Document.New");
                            crumburl = Constants.Urls.Media.Documents.EditUrl;
                        }
                        break;
                    case MediaType.Other:
                        if (IsEdit)
                        {
                            crumbtext = ResourceManager.GetLiteral("Admin.Media.Edit.Other.Edit");
                            crumburl = Constants.Urls.Media.Others.EditUrlWithKey + Url;
                        }
                        else
                        {
                            crumbtext = ResourceManager.GetLiteral("Admin.Media.Edit.Other.New");
                            crumburl = Constants.Urls.Media.Others.EditUrl;
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(crumbtext) && !string.IsNullOrEmpty(crumburl))
                {
                    breadcrumb.Add(crumbtext, crumburl);
                }
            }
        }

        private void ShowPhotoView()
        {
            this.uploadtype.Value = UploadType.Photo.ToString();
            this.mediauploaderButton.Value = ResourceManager.GetLiteral("Admin.Media.Edit.Photo.Upload");

            if (IsEdit)
            {
                this.headerLabel.Text = ResourceManager.GetLiteral("Admin.Media.Edit.Photo.Edit");
                this.Title = ResourceManager.GetLiteral("Admin.Media.Edit.Photo.Edit");

                //hide upload button
                this.mediauploader.Visible = false;

                // load the photo details
                BaseMedia baseMedia;

                if (MediaStore.Instance.TryGetMedia(Url, out baseMedia))
                {
                    this.nomediafounddiv.Visible = false;

                    this.photoimage.Src = baseMedia.Url;
                    this.nameInput.Value = baseMedia.Name;
                    this.urlInput.Value = baseMedia.Url;
                    this.tagsInput.Value = baseMedia.SerializedTags;

                    //convert to photo
                    Photo photo = baseMedia as Photo;
                    
                    this.heightspan.InnerText = photo.Height.ToString();
                    this.widthspan.InnerText = photo.Width.ToString();
                }
                else
                {
                    // no photo found
                    this.nomediafounddiv.Visible = true;
                    this.nomediafounddiv.InnerText = ResourceManager.GetLiteral("Admin.Media.Edit.Photo.NoPhotoFound");
                }
            }
            else
            {
                this.headerLabel.Text = ResourceManager.GetLiteral("Admin.Media.Edit.Photo.New");
                this.Title = ResourceManager.GetLiteral("Admin.Media.Edit.Photo.New");

                this.heightspan.InnerText = ResourceManager.GetLiteral("Admin.Media.Edit.Photo.AutoGenerated");
                this.widthspan.InnerText = ResourceManager.GetLiteral("Admin.Media.Edit.Photo.AutoGenerated");
                this.urlInput.Disabled = false;
                this.deletemedia.Visible = false;

                this.nomediafounddiv.Visible = false;
            }
        }

        private void ShowDocumentView()
        {
            this.uploadtype.Value = UploadType.Document.ToString();
            this.mediauploaderButton.Value = ResourceManager.GetLiteral("Admin.Media.Edit.Document.Upload");

            if (IsEdit)
            {
                this.headerLabel.Text = ResourceManager.GetLiteral("Admin.Media.Edit.Document.Edit");
                this.Title = ResourceManager.GetLiteral("Admin.Media.Edit.Document.Edit");

                //hide upload button
                this.mediauploader.Visible = false;
                
                // load the document details
                BaseMedia baseMedia;

                if (MediaStore.Instance.TryGetMedia(Url, out baseMedia))
                {
                    this.nomediafounddiv.Visible = false;

                    //this.photoimage.Src = baseMedia.Url;
                    this.nameInput.Value = baseMedia.Name;
                    this.urlInput.Value = baseMedia.Url;
                    this.tagsInput.Value = baseMedia.SerializedTags;

                }
                else
                {
                    // no document found
                    this.nomediafounddiv.Visible = true;
                    this.nomediafounddiv.InnerText = ResourceManager.GetLiteral("Admin.Media.Edit.Document.NoDocumentFound");
                }
            }
            else
            {
                this.headerLabel.Text = ResourceManager.GetLiteral("Admin.Media.Edit.Document.New");
                this.Title = ResourceManager.GetLiteral("Admin.Media.Edit.Document.New");
                this.nomediafounddiv.Visible = false;
                this.urlInput.Disabled = false;
                this.deletemedia.Visible = false;
            }

        }

        private void ShowOtherView()
        {
            this.uploadtype.Value = UploadType.Other.ToString();
            this.mediauploaderButton.Value = ResourceManager.GetLiteral("Admin.Media.Edit.Other.Upload");

            if (IsEdit)
            {
                this.headerLabel.Text = ResourceManager.GetLiteral("Admin.Media.Edit.Other.Edit");
                this.Title = ResourceManager.GetLiteral("Admin.Media.Edit.Other.Edit");

                //hide upload button
                this.mediauploader.Visible = false;

                // load the document details
                BaseMedia baseMedia;

                if (MediaStore.Instance.TryGetMedia(Url, out baseMedia))
                {
                    this.nomediafounddiv.Visible = false;

                    //this.photoimage.Src = baseMedia.Url;
                    this.nameInput.Value = baseMedia.Name;
                    this.urlInput.Value = baseMedia.Url;
                    this.tagsInput.Value = baseMedia.SerializedTags;

                }
                else
                {
                    // no document found
                    this.nomediafounddiv.Visible = true;
                    this.nomediafounddiv.InnerText = ResourceManager.GetLiteral("Admin.Media.Edit.Other.NoOtherFound");
                }
            }
            else
            {
                this.headerLabel.Text = ResourceManager.GetLiteral("Admin.Media.Edit.Other.New");
                this.Title = ResourceManager.GetLiteral("Admin.Media.Edit.Other.New");
                this.nomediafounddiv.Visible = false;
                this.urlInput.Disabled = false;
                this.deletemedia.Visible = false;
            }

        }

        private void ShowVideoView()
        {
            this.uploadtype.Value = UploadType.Video.ToString();
            this.mediauploaderButton.Value = ResourceManager.GetLiteral("Admin.Media.Edit.Video.Upload");

            if (IsEdit)
            {
                this.headerLabel.Text = ResourceManager.GetLiteral("Admin.Media.Edit.Video.Edit");
                this.Title = ResourceManager.GetLiteral("Admin.Media.Edit.Video.Edit");

                //hide upload button
                this.mediauploader.Visible = false;
 
                // load the document details
                BaseMedia baseMedia;

                if (MediaStore.Instance.TryGetMedia(Url, out baseMedia))
                {
                    this.nomediafounddiv.Visible = false;

                    //this.photoimage.Src = baseMedia.Url;
                    this.nameInput.Value = baseMedia.Name;
                    this.urlInput.Value = baseMedia.Url;
                    this.tagsInput.Value = baseMedia.SerializedTags;

                }
                else
                {
                    // no document found
                    this.nomediafounddiv.Visible = true;
                    this.nomediafounddiv.InnerText = ResourceManager.GetLiteral("Admin.Media.Edit.Video.NoVideoFound");
                }
            }
            else
            {
                this.headerLabel.Text = ResourceManager.GetLiteral("Admin.Media.Edit.Video.New");
                this.Title = ResourceManager.GetLiteral("Admin.Media.Edit.Video.New");
 
                this.nomediafounddiv.Visible = false;
                this.urlInput.Disabled = false;
                this.deletemedia.Visible = false;
            }

        }

        private void PopulateJavascriptIncludesAndVariables()
        {
            string editMediaLiteral = string.Empty;

            switch (View)
            {
                case MediaType.Photo:
                    editMediaLiteral = ResourceManager.GetLiteral("Admin.Media.Edit.Photo.Edit");
                    break;
                case MediaType.Document:
                    editMediaLiteral = ResourceManager.GetLiteral("Admin.Media.Edit.Document.Edit");
                    break;
                case MediaType.Video:
                    editMediaLiteral = ResourceManager.GetLiteral("Admin.Media.Edit.Video.Edit");
                    break;
                case MediaType.Other:
                    editMediaLiteral = ResourceManager.GetLiteral("Admin.Media.Edit.Other.Edit");
                    break;
            }

            this.clientJavaScript.RegisterClientScriptVariable(
                    EditMediaLiteralJsVariable,
                    editMediaLiteral
                );

        }

        #endregion

    }
}
