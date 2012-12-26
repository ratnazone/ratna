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
    using Jardalu.Ratna.Web.Admin.controls.Media;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.Admin.controls;


    #endregion

    public partial class _default : System.Web.UI.Page
    {

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();

            SetActionPanels();

            SetPhotosListParameters();
            SetVideosListParameters();
            SetDocumentsListParameters();

        }

        #endregion

        #region private methods

        private void SetActionPanels()
        {
            // new photo
            this.actionPanel.AddAction(
                    "/images/picture.png",
                    ResourceManager.GetLiteral("Admin.Media.List.NewPhoto"), 
                    Constants.Urls.Media.Photos.EditUrl
                );

            // new video
            this.actionPanel.AddAction(
                    "/images/video.png",
                    ResourceManager.GetLiteral("Admin.Media.List.NewVideo"),
                    Constants.Urls.Media.Videos.EditUrl
                );

            // new document
            this.actionPanel.AddAction(
                    "/images/document-new.png",
                    ResourceManager.GetLiteral("Admin.Media.List.NewDocument"),
                    Constants.Urls.Media.Documents.EditUrl
                );

            // new other
            this.actionPanel.AddAction(
                    "/images/star.png",
                    ResourceManager.GetLiteral("Admin.Media.List.NewOther"),
                    Constants.Urls.Media.Others.EditUrl
                );


            /* list panel */
            this.listPanel.AddAction(
                    "/images/start.png",
                    ResourceManager.GetLiteral("Admin.Media.List.PhotoHeader"),
                    Constants.Urls.Media.Photos.ListUrl
                );

            this.listPanel.AddAction(
                    "/images/start.png",
                    ResourceManager.GetLiteral("Admin.Media.List.VideoHeader"),
                    Constants.Urls.Media.Videos.ListUrl
                );

            this.listPanel.AddAction(
                    "/images/start.png",
                    ResourceManager.GetLiteral("Admin.Media.List.DocumentHeader"),
                    Constants.Urls.Media.Documents.ListUrl
                );

            this.listPanel.AddAction(
                    "/images/start.png",
                    ResourceManager.GetLiteral("Admin.Media.List.OtherHeader"),
                    Constants.Urls.Media.Others.ListUrl
                );


            this.listPanel.AddAction(
                    "/images/start.png",
                    ResourceManager.GetLiteral("Admin.Media.Gallery.List.Header"),
                    Constants.Urls.Media.Gallery.Url
                );
        }

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
            }
        }

        private void SetPhotosListParameters()
        {
            MediaListParameters parameters = new MediaListParameters();
            parameters.MediaType = MediaType.Photo;
            parameters.DisplayTableHeader = true;
            parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Media.List.NoPhotosFound");
            parameters.Header = ResourceManager.GetLiteral("Admin.Media.List.PhotoHeader");
            parameters.DisplayTableHeader = false;
            parameters.MoreUrl = Utility.ResolveUrl(Constants.Urls.Media.Photos.ListUrl);
            parameters.Count = 8;

            this.photosMediaList.Parameters = parameters;
        }

        private void SetDocumentsListParameters()
        {
            MediaListParameters parameters = new MediaListParameters();
            parameters.MediaType = MediaType.Document;
            parameters.DisplayTableHeader = true;
            parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Media.List.NoDocumentsFound");
            parameters.Header = ResourceManager.GetLiteral("Admin.Media.List.DocumentHeader");
            parameters.DisplayTableHeader = false;
            parameters.MoreUrl = Utility.ResolveUrl(Constants.Urls.Media.Documents.ListUrl);

            this.documentsMediaList.Parameters = parameters;
        }

        private void SetVideosListParameters()
        {
            MediaListParameters parameters = new MediaListParameters();
            parameters.MediaType = MediaType.Video;
            parameters.DisplayTableHeader = true;
            parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Media.List.NoVideosFound");
            parameters.Header = ResourceManager.GetLiteral("Admin.Media.List.VideoHeader");
            parameters.DisplayTableHeader = false;
            parameters.MoreUrl = Utility.ResolveUrl(Constants.Urls.Media.Videos.ListUrl);

            this.videosMediaList.Parameters = parameters;
        }

        #endregion

    }
}
