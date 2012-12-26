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
namespace Jardalu.Ratna.Administration
{

    #region using

    using System;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Store;

    #endregion

    /// <summary>
    /// Site defines domain/subdomain that stores the information
    /// in the Ratna backend.
    /// </summary>
    public sealed class Site
    {
        #region private fields

        private int id;
        private string host;
        private string title = string.Empty;

        private bool isActive;
        private bool isProvisioned;

        public const string DefaultHost = "default";

        #endregion

        #region public properties

        /// <summary>
        /// Id of the site.
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("value");
                }

                this.id = value;
            }
        }

        /// <summary>
        /// Host of the site.
        /// 
        /// Sample host names
        /// ratnazone.com
        /// jardalu.com
        /// sharepoint.jardalu.com
        /// </summary>
        public string Host
        {
            get
            {
                return this.host;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                // allow "default" host name
                if (!value.Equals( DefaultHost, StringComparison.OrdinalIgnoreCase)
                    && !Utility.IsValidDomain(value))
                {
                    throw new ArgumentException("invalid host");
                }

                this.host = value;
            }
        }

        /// <summary>
        /// Title for the Site.
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set
            {
                this.title = value;

                if (this.title == null)
                {
                    this.title = string.Empty;
                }
            }
        }

        public bool IsActive
        {
            get { return this.isActive; }
            set { this.isActive = value; }
        }

        public bool IsProvisioned
        {
            get { return this.isProvisioned; }
            set { this.isProvisioned = value; }
        }

        #endregion

        #region public methods

        public void Provision(string email, string alias, string password)
        {
            #region arguments

            if (Id <= 0)
            {
                throw new InvalidOperationException("Id is not defined");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            #endregion

            if (!IsProvisioned)
            {
                SiteStore.Instance.ProvisionSite(this, email, alias, password);
            }
        }

        #endregion

    }

}
