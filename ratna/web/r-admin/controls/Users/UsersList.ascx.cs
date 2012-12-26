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
    using Jardalu.Ratna.Web.Resource;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;

    #endregion

    public partial class UsersList : System.Web.UI.UserControl
    {

        #region private fields

        private int total;
        private bool isActive = true;
        private bool isDeleted = false;
        private string title = ResourceManager.GetLiteral("Admin.Users.Title");

        private bool conciseView;
        private bool expandedView;

        private bool displayHeader = true;

        private const string UsersListJavascriptKey = "controls.users.userlist.js";
        private const string UsersListActivationSuccessJsVariable = "L_UserActivationSuccess";
        private const string UsersListActivationFailureJsVariable = "L_UserActivationFailure";
        private const string UsersListDeletionSuccessJsVariable = "L_UserDeletionSuccess";
        private const string UsersListDeletionFailureJsVariable = "L_UserDeletionFailure";

        #endregion

        #region public properties

        public string Title
        {
            get { return this.title; }
            set
            {
                this.title = value;
            }
        }

        public bool IsActive
        {
            get { return this.isActive; }
            set { this.isActive = value; }
        }

        public bool IsDeleted
        {
            get
            {
                return this.isDeleted;
            }
            set
            {
                if (value)
                {
                    IsActive = false;
                }

                this.isDeleted = value;
            }
        }

        public bool ConciseView
        {
            get
            {
                return this.conciseView;
            }
            set
            {
                if (ExpandedView && value)
                {
                    throw new InvalidOperationException("value");
                }

                this.conciseView = value;
            }
        }

        public bool ExpandedView
        {
            get
            {
                return this.expandedView;
            }
            set
            {
                if (ConciseView && value)
                {
                    throw new InvalidOperationException("value");
                }

                this.expandedView = value;
            }
        }

        public bool DisplayTableHeader
        {
            get;
            set;
        }

        public bool DisplayHeader
        {
            get { return this.displayHeader; }
            set { this.displayHeader = value; }
        }

        public int Total
        {
            get
            {
                return total;
            }
        }

        public IList<User> Users
        {
            get;
            set;
        }

        public UserListParameters Parameters
        {
            get;
            set;
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateJavascriptIncludesAndVariables();

            IList<User> users = Users;

            if (this.Parameters != null)
            {
                this.moreAnchor.HRef = this.Parameters.MoreUrl;
            }

            if (users == null && 
                this.Parameters != null)
            {
                users = UserStore.Instance.GetUsers(Parameters.Query, IsActive, IsDeleted, Parameters.Start, Parameters.Count, out total);
            }

            if (users != null && users.Count != 0)
            {
                // assign the values to UsersListRow
                this.usersListRow.ExpandedView = this.ExpandedView;
                this.usersListRow.IsActive = this.IsActive;
                this.usersListRow.IsDeleted = this.IsDeleted;
                this.usersListRow.ConciseView = this.ConciseView;

                this.usersListRow.Users = users;
            
                this.none.Visible = false;
            }
        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    UsersListJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.UsersListControl)
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    UsersListActivationSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Users.Activation.Success")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    UsersListActivationFailureJsVariable,
                    ResourceManager.GetLiteral("Admin.Users.Activation.Failure")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    UsersListDeletionSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Users.Deletion.Success")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    UsersListDeletionFailureJsVariable,
                    ResourceManager.GetLiteral("Admin.Users.Deletion.Failure")
                );
        }

        #endregion

    }
}
