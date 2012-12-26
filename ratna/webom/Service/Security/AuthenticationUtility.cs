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
namespace Jardalu.Ratna.Web.Security
{

    #region using
    
    using System;
    using System.Web;
    using System.Web.Security;

    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Store;

    #endregion

    public class AuthenticationUtility
    {
        
        #region private fields

        private static object syncRoot = new object();
        private static AuthenticationUtility utility;

        #endregion

        #region ctor

        private AuthenticationUtility()
        {
        }

        #endregion

        #region public properties

        public static AuthenticationUtility Instance
        {
            get
            {
                if (utility == null)
                {
                    lock (syncRoot)
                    {
                        if (utility == null)
                        {
                            utility = new AuthenticationUtility();
                        }
                    }
                }

                return utility;
            }
        }

        #endregion

        #region public methods

        public User GetLoggedUser()
        {

            User user = null;

            // request received
            HttpRequest request = HttpContext.Current.Request;
            HttpCookie cookie = request.Cookies[Constants.CookieName];
            if (cookie != null)
            {
                try
                {
                    FormsAuthenticationTicket decrytedTicket = FormsAuthentication.Decrypt(cookie.Value);
                    string issuedCookie = decrytedTicket.UserData;
                    string username = decrytedTicket.Name;

                    // check the validity of the cookie
                    bool valid = UserStore.Instance.ValidateUserCookie(username, issuedCookie);
                    if (valid)
                    {
                        user = UserStore.Instance.LoadUser(username);
                    }
                }
                catch
                {
                    //ignore exception
                }

            }

            return user;
        }

        public HttpCookie CreateFormsCookie(string username, string cookie, DateTime expiry, bool isPersistent)
        {
            // create the cookie
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                            username,
                            DateTime.Now,
                            expiry,
                            isPersistent,
                            cookie,
                            FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            HttpCookie httpCookie = new HttpCookie(Constants.CookieName, encTicket);
            httpCookie.Expires = expiry;
            httpCookie.HttpOnly = !isPersistent;

            return httpCookie;
        }

        public HttpCookie CreateLogoutCookie()
        {
            HttpCookie cookie = new HttpCookie(Constants.CookieName, "");
            cookie.Expires = DateTime.Now.AddDays(-7);
            return cookie;
        }

        public void Logout(string alias)
        {
            UserStore.Instance.Signout(alias);
        }

        #endregion

    }
}
