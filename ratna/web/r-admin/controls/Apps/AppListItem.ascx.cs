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
namespace Jardalu.Ratna.Web.Admin.controls.Apps
{

    #region using

    using System;

    using Jardalu.Ratna.Core.Apps;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class AppListItem : System.Web.UI.UserControl
    {

        #region public properties

        public App App
        {
            get;
            set;
        }

        public string IconUrl
        {
            get
            {
                return GetAppIconUrl(this.App);
            }
        }

        public string Name
        {
            get
            {
                return (App == null ? string.Empty : App.Name);
            }
        }

        public string Description
        {
            get
            {                
                return (App == null ? string.Empty : App.Description);
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (App != null)
            {
                this.editLink.HRef = string.Format(Constants.Urls.Apps.EditUrl, App.Id);
            }
        }

        #endregion

        #region public methods

        public static string GetAppIconUrl(App app)
        {
            string iconUrl = "";
            if (app != null && 
                !string.IsNullOrEmpty(app.IconUrl))
            {                
                iconUrl = app.IconUrl;

                // resolve the icon location, if the icon was uploaded with the package itself.
                if (iconUrl.IndexOf('/') == -1)
                {
                    iconUrl = Utility.ResolveUrl(string.Format("{0}/{1}/{2}", SitePaths.App, app.Location, iconUrl));
                }
            }

            if (string.IsNullOrEmpty(iconUrl))
            {
                iconUrl = "/images/app.png";
            }

            return iconUrl;
        }

        #endregion

    }
}
