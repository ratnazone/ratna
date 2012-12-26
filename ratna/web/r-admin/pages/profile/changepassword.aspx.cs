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
namespace Jardalu.Ratna.Web.Admin.pages.profile
{

    #region using

    using System;

    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Web.Security;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Exceptions.ErrorCodes;

    #endregion

    public partial class changepassword : System.Web.UI.Page
    {

        #region private fields 

        private string oldpasswordHash;
        private string newpasswordHash;

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            this.r.Value = LandingUrl;
            bool attempt = WasChangeAttempt();

            //check if this is an error attempt redirect
            int error = GetErrorCode();

            if (attempt)
            {
                try
                {
                    // get the logged in user alias
                    string alias = AuthenticationUtility.Instance.GetLoggedUser().Alias;

                    UserStore.Instance.UpdateUserPassword(alias, oldpasswordHash, newpasswordHash, true);

                    this.successdiv.Visible = true;
                }
                catch (MessageException me)
                {
                    if (me.ErrorNumber == UserErrorCodes.PasswordMismatch)
                    {
                        error = ErrorCode.ChangePassword.WrongOldPassword;
                    }
                    else
                    {
                        error = -1;
                    }
                }

            }

            if (error != 0)
            {
                //set error code message
                SetErrorCodeMessage(error);
                this.errordiv.Visible = true;
            }

        }

        #endregion

        #region private methods

        private string LandingUrl
        {
            get
            {
                string url = Request[Constants.LandingUrlIdentifier];
                if (string.IsNullOrEmpty(url))
                {
                    url = Constants.Urls.DefaultLandingUrl;
                }
                return url;
            }
        }

        private bool WasChangeAttempt()
        {
            bool attempt = false;

            oldpasswordHash = Request["oldpassword"];
            newpasswordHash = Request["newpassword"];

            if (!string.IsNullOrEmpty(oldpasswordHash) || !string.IsNullOrEmpty(newpasswordHash))
            {
                attempt = true;
            }

            return attempt;
        }

        private int GetErrorCode()
        {
            int error = 0;

            string e = this.Request[Constants.ErrorCodeIdentifier];
            if (!string.IsNullOrEmpty(e))
            {
                if (!Int32.TryParse(e, out error))
                {
                    error = 0;
                }
            }

            return error;
        }

        private void SetErrorCodeMessage(int error)
        {
            switch (error)
            {
                case ErrorCode.ChangePassword.MustChange:
                    this.error.InnerHtml = ResourceManager.GetLiteral("Admin.Profile.MustChangePassword");
                    break;
                case ErrorCode.ChangePassword.WrongOldPassword:
                    this.error.InnerHtml = ResourceManager.GetLiteral("Admin.Profile.WrongOldPassword");
                    break;
                default:
                    this.error.InnerHtml = ResourceManager.GetLiteral("Admin.Profile.ChangePassword.Error");
                    break;
            }
        }

        #endregion

    }
}
