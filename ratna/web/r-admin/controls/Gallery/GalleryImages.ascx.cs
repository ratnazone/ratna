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
namespace Jardalu.Ratna.Web.Admin.controls.gallery
{

    #region using

    using System;

    #endregion

    public partial class GalleryImages : System.Web.UI.UserControl
    {

        #region public properties

        public Gallery Gallery
        {
            get;
            set;
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Gallery != null && Gallery.Photos != null)
            {
                // add the photos
                this.repeater.DataSource = Gallery.Photos;
                this.repeater.DataBind();
            }
        }

        #endregion

    }
}
