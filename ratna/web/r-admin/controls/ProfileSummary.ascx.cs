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


namespace Jardalu.Ratna.Web.Admin.controls
{
    #region using

    using System;

    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Web.Security;

    #endregion

    public partial class ProfileSummary : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            User user = AuthenticationUtility.Instance.GetLoggedUser();
            this.userName.Text = user.DisplayName;
            this.lastSignedInTime.Text = Utility.FormatDate(user.LastLoginTime);
            if (!string.IsNullOrEmpty(user.Photo))
            {
                this.profilePhoto.Src = user.Photo;
            }

            //set the profile url
            this.profileanchor.HRef = Utility.ResolveUrl(Constants.Urls.Profile.Url);
        }
    }
}
