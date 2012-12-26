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

    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.UI.Snippets;

    #endregion

    public partial class UserMembershipSearchRow : SnippetControl
    {

        #region public properties

        public string Name
        {
            get;
            set;
        }

        public long PrincipalId
        {
            get;
            set;
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateControl();
        }

        #endregion

        #region public methods

        public override void PopulateControl()
        {
            this.groupid.Value = this.PrincipalId.ToString();
            this.groupname.InnerHtml = this.Name;
        }

        #endregion

    }
}
