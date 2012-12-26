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
namespace Jardalu.Ratna.Web.Admin.pages.media.gallery
{

    #region using

    using System;

    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.Plugins;

    #endregion

    public partial class edit : System.Web.UI.Page
    {
        #region private fields

        private string url;
        private bool isInitialized = false;
        private object syncRoot = new object();
        private Gallery gallery;

        private const string EditGalleryHeaderJsVariable = "L_EditGalleryHeader";

        #endregion

        #region public properties

        public string Url
        {
            get
            {
                this.Initialize();
                return this.url;
            }
        }

        #endregion

        #region private properties

        private Gallery Gallery
        {
            get
            {
                this.Initialize();
                return gallery;
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();
            PopulateJavascriptIncludesAndVariables();

            if (Gallery != null)
            {
                this.headerLabel.Text = ResourceManager.GetLiteral("Admin.Media.Gallery.Edit");
                this.Title = ResourceManager.GetLiteral("Admin.Media.Gallery.Edit");

                RenderForEditGallery();

                galleryImages.Gallery = Gallery;
            }
            else
            {
                this.headerLabel.Text = ResourceManager.GetLiteral("Admin.Media.Gallery.New");
                this.Title = ResourceManager.GetLiteral("Admin.Media.Gallery.New");
                this.uid.Value = Guid.Empty.ToString();
            }
        }

        protected void Initialize()
        {
            if (!isInitialized)
            {
                lock (syncRoot)
                {
                    if (!isInitialized)
                    {
                        
                        url = Request.QueryString[Constants.UrlIdentifier] as string;

                        if (!string.IsNullOrEmpty(url))
                        {
                            gallery = GalleryPlugin.Instance.Read(url);
                        }

                        isInitialized = true;
                    }
                }
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
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Media.Gallery"), Constants.Urls.Media.Gallery.Url);

                string crumbtext = string.Empty;
                string crumburl = string.Empty;

                if (Gallery != null)
                {
                    crumbtext = ResourceManager.GetLiteral("Admin.Media.Gallery.Edit");
                    crumburl = Constants.Urls.Media.Gallery.EditUrlWithKey + Url;
                }
                else
                {
                    crumbtext = ResourceManager.GetLiteral("Admin.Media.Gallery.New");
                    crumburl = Constants.Urls.Media.Gallery.EditUrl;
                }

                if (!string.IsNullOrEmpty(crumbtext) && !string.IsNullOrEmpty(crumburl))
                {
                    breadcrumb.Add(crumbtext, crumburl);
                }
            }
        }

        private void PopulateJavascriptIncludesAndVariables()
        {

            this.clientJavaScript.RegisterClientScriptVariable(
                    EditGalleryHeaderJsVariable,
                    ResourceManager.GetLiteral("Admin.Media.Gallery.Edit")
                );
        }

        private void RenderForEditGallery()
        {
            this.name.Value = this.Gallery.Title;
            this.urlInput.Value = this.Gallery.Url;
            this.urlInput.Disabled = true;
            this.uid.Value = this.Gallery.UId.ToString();
            this.description.Value = this.Gallery.Description;
            this.nav.Value = this.Gallery.Navigation;
        }

        #endregion

    }
}
