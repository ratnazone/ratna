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

    #endregion

    public partial class _default : System.Web.UI.Page
    {
        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();
            SetActionPanels();

            SetGalleryListParameters();
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
            }
        }

        private void SetActionPanels()
        {
            
            // new gallery
            this.actionPanel.AddAction(
                    "/images/picture.png",
                    ResourceManager.GetLiteral("Admin.Media.Gallery.New"),
                    Constants.Urls.Media.Gallery.EditUrl
                );
                       

        }

        private void SetGalleryListParameters()
        {
            ListControlParameters parameters = new ListControlParameters();
            parameters.DisplayTableHeader = true;
            parameters.NothingFoundText = ResourceManager.GetLiteral("Admin.Media.Gallery.List.NoGalleryFound");
            parameters.Header = ResourceManager.GetLiteral("Admin.Media.Gallery.List.Header");
            parameters.Count = 8;

            this.galleryList.Parameters = parameters;
        }

        #endregion
    }
}
