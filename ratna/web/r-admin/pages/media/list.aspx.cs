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
    using Jardalu.Ratna.Web.Admin.controls.Media;
    using Jardalu.Ratna.Web.Resource;
        
    #endregion

    public partial class list : MediaBasePage
    {

        #region fields

        protected global::Jardalu.Ratna.Web.Admin.controls.ActionPanel actionPanel;
        protected global::Jardalu.Ratna.Web.Admin.controls.ActionPanel listPanel;

        #endregion

        #region protected properties

        protected string SearchUrl
        {
            get
            {
                return Request.Url.AbsolutePath + "?view=" + View.ToString() + "&q={0}";
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageSize = this.Pager.PageSize;

            PopulateNavigationAndBreadCrumb();
            SetMediaListParameters();
            SetActionPanels();

            switch (View)
            {
                case MediaType.Video:
                    this.headerH1.InnerText = ResourceManager.GetLiteral("Admin.Media.List.VideoHeader");
                    this.Title = ResourceManager.GetLiteral("Admin.Media.List.VideoHeader");
                    break;
                case MediaType.Photo:
                    this.headerH1.InnerText = ResourceManager.GetLiteral("Admin.Media.List.PhotoHeader");
                    this.Title = ResourceManager.GetLiteral("Admin.Media.List.PhotoHeader");
                    break;
                case MediaType.Document:
                    this.headerH1.InnerText = ResourceManager.GetLiteral("Admin.Media.List.DocumentHeader");
                    this.Title = ResourceManager.GetLiteral("Admin.Media.List.DocumentHeader");
                    break;
                case MediaType.Other:
                    this.headerH1.InnerText = ResourceManager.GetLiteral("Admin.Media.List.OtherHeader");
                    this.Title = ResourceManager.GetLiteral("Admin.Media.List.OtherHeader");
                    break;

            }

            if (View == MediaType.Photo)
            {
                imagesList.LoadData();
                this.RenderPager(this.imagesList.TotalRecords);
            }
            else
            {
                this.mediaList.LoadData();
                this.RenderPager(this.mediaList.TotalRecords);
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
                            crumbtext = ResourceManager.GetLiteral("Admin.Media.List.PhotoHeader");
                            crumburl = Constants.Urls.Media.Photos.ListUrl;
                        break;
                    case MediaType.Video:
                            crumbtext = ResourceManager.GetLiteral("Admin.Media.List.VideoHeader");
                            crumburl = Constants.Urls.Media.Videos.ListUrl;
                        break;
                    case MediaType.Document:
                            crumbtext = ResourceManager.GetLiteral("Admin.Media.List.DocumentHeader");
                            crumburl = Constants.Urls.Media.Documents.ListUrl;
                        break;
                    case MediaType.Other:
                            crumbtext = ResourceManager.GetLiteral("Admin.Media.List.OtherHeader");
                            crumburl = Constants.Urls.Media.Others.ListUrl;
                        break;
                }

                if (!string.IsNullOrEmpty(crumbtext) && !string.IsNullOrEmpty(crumburl))
                {
                    breadcrumb.Add(crumbtext, crumburl);
                }
            }
        }

        private void SetActionPanels()
        {
            if (View == MediaType.Photo)
            {
                // new photo
                this.actionPanel.AddAction(
                        "/images/picture.png",
                        ResourceManager.GetLiteral("Admin.Media.List.NewPhoto"),
                        Constants.Urls.Media.Photos.EditUrl
                    );
            }

            if (View == MediaType.Video)
            {
                // new video
                this.actionPanel.AddAction(
                        "/images/video.png",
                        ResourceManager.GetLiteral("Admin.Media.List.NewVideo"),
                        Constants.Urls.Media.Videos.EditUrl
                    );
            }

            if (View == MediaType.Document)
            {
                // new document
                this.actionPanel.AddAction(
                        "/images/document-new.png",
                        ResourceManager.GetLiteral("Admin.Media.List.NewDocument"),
                        Constants.Urls.Media.Documents.EditUrl
                    );
            }

            if (View == MediaType.Other)
            {
                // new other
                this.actionPanel.AddAction(
                        "/images/star.png",
                        ResourceManager.GetLiteral("Admin.Media.List.NewOther"),
                        Constants.Urls.Media.Others.EditUrl
                    );
            }


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

        private void SetMediaListParameters()
        {
            MediaListParameters parameters = new MediaListParameters();
            parameters.DisplayTableHeader = true;
            parameters.HideHeader = true;
            parameters.Count = this.Pager.PageSize;
            parameters.Query = this.Query;
            parameters.Start = this.Start;

            switch (View)
            {
                case MediaType.Photo:
                        parameters.MediaType = MediaType.Photo;            
                        parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Media.List.NoPhotosFound");
                    break;
                case MediaType.Document:
                        parameters.MediaType = MediaType.Document;            
                        parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Media.List.NoDocumentsFound");
                    break;
                case MediaType.Video:
                        parameters.MediaType = MediaType.Video;            
                        parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Media.List.NoVideosFound");
                    break;
                case MediaType.Other:
                        parameters.MediaType = MediaType.Other;            
                        parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Media.List.NoOthersFound");
                    break;
            }

            if (View == MediaType.Photo)
            {
                this.imagesList.Parameters = parameters;
                this.mediaList.Visible = false;
            }
            else
            {
                this.mediaList.Parameters = parameters;
                this.imagesList.Visible = false;
            }
        }

        private void RenderPager(int total)
        {
            if (total > 0)
            {
                this.TotalPages = (total / this.Pager.PageSize) + (total % this.Pager.PageSize > 0 ? 1 : 0);
            }

            //set the page format
            this.Pager.PageLinkFormat = string.Format("{0}?view={1}&q={2}&p=", Request.Url.AbsolutePath, View, Query) + "{0}";

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
