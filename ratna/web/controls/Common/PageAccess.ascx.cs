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
namespace Jardalu.Ratna.Web.Controls.Common
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Web;

    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web;
    using Jardalu.Ratna.Web.Security;

    #endregion

    public partial class PageAccess : System.Web.UI.UserControl
    {
        private const string LoginPage = "~/r-admin/login.aspx?r={0}&{1}={2}";
        private object syncRoot = new object();

        #region public properties

        public bool IsRestricted
        {
            get;
            set;
        }

        public string AllowGroups
        {
            get;
            set;
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsRestricted)
            {
                //find the user logged in
                User loggedUser = AuthenticationUtility.Instance.GetLoggedUser();

                // page access is restricted
                if (!string.IsNullOrEmpty(AllowGroups) && loggedUser != null)
                {
                    bool isMember = GroupAccessUtility.Instance.HasAccess(loggedUser, AllowGroups);
                    if (isMember)
                    {
                        return;
                    }
                }

                // user is not member of any group.
                // redirect the user to login page.
                this.Page.Response.Redirect(GetLoginPageUrl());
            }
        }

        #endregion

        #region private methods

        private string GetLoginPageUrl()
        {
            return string.Format(LoginPage, this.Request.RawUrl, Constants.ErrorCodeIdentifier, ErrorCode.Login.NotAuthorized);
        }

        #endregion

    }
}
