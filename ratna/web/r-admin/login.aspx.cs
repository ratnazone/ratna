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
namespace Jardalu.Ratna.Web.Admin
{
    #region using

    using System;
    using System.Text;
    using System.Web;
    using Jardalu.Ratna.Web.Resource;
    using System.Web.Security;

    using Jardalu.Ratna.Store;
    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Web.Security;

    #endregion

    public partial class Login : System.Web.UI.Page
    {
        #region private fields

        private string username;
        private string passwordHash;
        private bool isRememberMe;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //default error message
            this.error.InnerHtml = ResourceManager.GetLiteral("Login.WrongUserNamePassword");

            this.errordiv.Visible = false;
            this.r.Value = LandingUrl;
            bool attempt = WasLoginAttempt();

            //check if this is an error attempt redirect
            int error = GetErrorCode();
            if (error != 0)
            {
                //set error code message
                SetErrorCodeMessage(error);
                this.errordiv.Visible = true;
            }

            if (attempt)
            {
                HttpCookie cookie = null;
                bool success = LoginUser(out cookie);

                if (!success)
                {
                    this.errordiv.Visible = true;
                }
                else
                {
                    Response.Cookies.Add(cookie);

                    //redirect to the location.
                    Response.Redirect(LandingUrl);
                }
            }
        }

        #region private methods

        private bool IsRememberMe
        {
            get
            {
                return isRememberMe;
            }
        }

        private bool WasLoginAttempt()
        {
            bool attempt = false;

            username = Request["username"];
            passwordHash = Request["password"];

            if (Request["remember"] == "on")
            {
                isRememberMe = true;
            }

            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(passwordHash))
            {
                attempt = true;
            }

            return attempt;
        }

        private bool LoginUser(out HttpCookie httpCookie)
        {

            bool success = false;
            httpCookie = null;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(passwordHash))
            {
                string cookie;
                DateTime expiry;
                bool isPersistent = IsRememberMe;

                success = UserStore.Instance.SignIn(this.username, this.passwordHash, true, out cookie, out expiry);
                if (success)
                {
                    // make sure the user is activated
                    RatnaUser user = UserStore.Instance.LoadUser(this.username);
                    if (!user.IsActivated)
                    {
                        this.error.InnerHtml = ResourceManager.GetLiteral("Login.UserNotActivated");
                        success = false;
                    }
                    else
                    {
                        // create the cookie
                        httpCookie = AuthenticationUtility.Instance.CreateFormsCookie(username, cookie, expiry, isPersistent);
                    }
                }
            }
            return success;
        }

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
                case ErrorCode.Login.NotAuthorized:
                    this.error.InnerHtml =
                         ResourceManager.GetLiteral("Login.UserNotAuthorized");
                    break;
                case ErrorCode.Login.NotActivated:
                    this.error.InnerHtml =
                         ResourceManager.GetLiteral("Login.UserNotActivated");
                    break;
            }
        }

        #endregion

    }
}
