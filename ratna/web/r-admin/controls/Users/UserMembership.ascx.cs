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
namespace Jardalu.Ratna.Web.Admin.controls.Users
{

    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Store;

    #endregion

    public partial class UserMembership : System.Web.UI.UserControl
    {

        #region private fields

        private int start;
        private int count = 10;

        private const string UserMembershipJavascriptKey = "users.usermembership.js";

        #endregion

        #region public properties

        public IList<Group> Groups
        {
            get;
            set;
        }

        public string Alias
        {
            get;
            set;
        }

        public int Start
        {
            get { return this.start; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("value");
                }

                this.start = value;
            }
        }

        public int Count
        {
            get { return this.count; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("value");
                }

                this.count = value;
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateJavascriptIncludesAndVariables();

            if (this.Groups == null && 
                !string.IsNullOrEmpty(Alias))
            {
                // fetch the group information
                int total;
                this.Groups = UserStore.Instance.GetMembershipGroups(this.Alias, Start, Count, out total);

                //set the groups and Alias for row
                this.memeberShipRow.Groups = this.Groups;
            }
        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    UserMembershipJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.UsersMembershipControl)
                );
        }

        #endregion
    }
}
