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
namespace Jardalu.Ratna.Utilities.Email
{

    #region using

    using System;
    using System.Net;

    #endregion

    public class EmailConfiguration
    {

        #region Private Fields

        private string smtpAddress;
        private string fromAddress;
        private NetworkCredential credential;

        #endregion

        #region public properties

        public string SmtpAddress
        {
            get
            {
                return this.smtpAddress;
            }
        }

        public string FromAddress
        {
            get
            {
                return this.fromAddress;
            }
        }

        public NetworkCredential Credentials
        {
            get
            {
                return credential;
            }
        }

        #endregion

        #region Public Method

        public void Configure(string smtpAddress, string userName, string password, string fromAddress)
        {
            #region args

            if (string.IsNullOrEmpty(smtpAddress))
            {
                throw new ArgumentNullException("smtpAddress");
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            if (string.IsNullOrEmpty(fromAddress))
            {
                throw new ArgumentNullException("fromAddress");
            }

            #endregion

            this.smtpAddress = smtpAddress;
            this.fromAddress = fromAddress;
            this.credential = new NetworkCredential(userName, password);

        }

        #endregion

    }
}
