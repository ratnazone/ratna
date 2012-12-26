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
    using System.Web.UI;

    using Jardalu.Ratna.Profile;

    #endregion

    public partial class UsersListRow : System.Web.UI.UserControl
    {

        #region private fields

        private bool isActive = true;
        private bool conciseView;
        private bool expandedView;

        #endregion

        #region public properties

       
        public bool IsActive
        {
            get { return this.isActive; }
            set { this.isActive = value; }
        }

        public bool IsDeleted
        {
            get;
            set;
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


        public IList<User> Users
        {
            get;
            set;
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Users == null || Users.Count == 0)
            {
                this.repeater.Visible = false;
            }
            else
            {
                this.repeater.DataSource = Users;
                this.repeater.DataBind();
            }
        }
    }

}
