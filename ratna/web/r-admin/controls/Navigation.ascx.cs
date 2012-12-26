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


namespace Jardalu.Ratna.Web.Admin.controls
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Data;    
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.Security;
    using Jardalu.Ratna.Profile;

    #endregion

    public partial class Navigation : System.Web.UI.UserControl
    {

        #region private fields

        private static DataTable navtable;

        #endregion

        #region ctor

        static Navigation()
        {
            navtable = new DataTable();
            navtable.Columns.Add("Name");
            navtable.Columns.Add("Url");
            navtable.Columns.Add("Label");
            navtable.Columns.Add("Groups");

            navtable.Rows.Add("overview", Utility.ResolveUrl(Constants.Urls.AdminUrl), ResourceManager.GetLiteral("Admin.Navigation.Overview"));
            navtable.Rows.Add("articles", Utility.ResolveUrl(Constants.Urls.Articles.Url), ResourceManager.GetLiteral("Admin.Navigation.Articles"));
            navtable.Rows.Add("pages", Utility.ResolveUrl(Constants.Urls.Pages.Url), ResourceManager.GetLiteral("Admin.Navigation.Pages"));
            navtable.Rows.Add("media", Utility.ResolveUrl(Constants.Urls.Media.Url), ResourceManager.GetLiteral("Admin.Navigation.Media"));
            navtable.Rows.Add("forms", Utility.ResolveUrl(Constants.Urls.Forms.Url), ResourceManager.GetLiteral("Admin.Navigation.Forms"), "Administrator");
            //navtable.Rows.Add("users", Utility.ResolveUrl(Constants.Urls.Users.Url), ResourceManager.GetLiteral("Admin.Navigation.Users"), "Administrator");
            //navtable.Rows.Add("apps", Utility.ResolveUrl(Constants.Urls.Apps.Url), ResourceManager.GetLiteral("Admin.Navigation.Apps"), "Administrator");
            navtable.Rows.Add("comments", Utility.ResolveUrl(Constants.Urls.Comments.Url), ResourceManager.GetLiteral("Admin.Navigation.Comments"), "Administrator");
            navtable.Rows.Add("profile", Utility.ResolveUrl(Constants.Urls.Profile.Url), ResourceManager.GetLiteral("Admin.Navigation.Profile"));
            navtable.Rows.Add("settings", Utility.ResolveUrl(Constants.Urls.Settings.Url), ResourceManager.GetLiteral("Admin.Navigation.Settings"), "Administrator");

        }

        #endregion

        #region public properties

        public string Selected
        {
            get;
            set;
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {

            this.repeater.DataSource = GetAccesibleNavTable();
            this.repeater.DataBind();

        }

        protected void RepeaterItemEventHandler(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // check if the user has access to the navigaton item.
                DataRowView row = e.Item.DataItem as DataRowView;
                if ( row != null)
                {
                   
                    // selected item
                    if (!string.IsNullOrEmpty(Selected))
                    {

                        if (row["Name"].ToString().Equals(Selected, StringComparison.OrdinalIgnoreCase))
                        {
                            // selected row
                            HtmlAnchor anchor = e.Item.FindControl("navlink") as HtmlAnchor;

                            HtmlGenericControl span = new HtmlGenericControl("span");
                            span.Attributes["class"] = "active";

                            foreach (Control c in anchor.Controls)
                            {
                                span.Controls.Add(c);
                            }

                            Control parent = anchor.Parent;

                            parent.Controls.Clear();
                            parent.Controls.Add(span);
                        }
                    }
                }
            }
        }

        #endregion

        #region private methods

        private DataTable GetAccesibleNavTable()
        {
            DataTable accesibleNavTable = new DataTable();

            accesibleNavTable.Columns.Add("Name");
            accesibleNavTable.Columns.Add("Url");
            accesibleNavTable.Columns.Add("Label");

            foreach(DataRow row in navtable.Rows)
            {
                string groups = row["Groups"] as string;
                bool hasAccess = true;

                if (!string.IsNullOrEmpty(groups))
                {
                    // check the user access
                    User loggedUser = AuthenticationUtility.Instance.GetLoggedUser();
                    hasAccess = GroupAccessUtility.Instance.HasAccess(loggedUser, groups);
                }

                if (hasAccess)
                {
                    DataRow newrow = accesibleNavTable.NewRow();
                    foreach (DataColumn column in accesibleNavTable.Columns)
                    {
                        newrow[column.ColumnName] = row[column.ColumnName];
                    }

                    // add to the accesible
                    accesibleNavTable.Rows.Add(newrow);
                }

            }

            return accesibleNavTable;
        }

        #endregion

    }
}
