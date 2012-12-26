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
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;

    #endregion

    public partial class Menu : System.Web.UI.UserControl
    {
        #region ActionItem

        class MenuItem
        {
            public string Title { get; set; }
            public string HRef { get; set; }
        }

        #endregion

        #region private fields

        private List<MenuItem> menuItems = new List<MenuItem>();

        #endregion

        #region public properties

        public string Selected
        {
            get;
            set;
        }

        #endregion

        #region public methods

        public void AddMenu(string title, string href)
        {

            #region arguments

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException("title");
            }
            if (string.IsNullOrEmpty(href))
            {
                throw new ArgumentNullException("href");
            }

            #endregion

            MenuItem menuItem = new MenuItem() { Title = title, HRef = href };
            this.menuItems.Add(menuItem);

        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            this.repeater.DataSource = this.menuItems;
            this.repeater.DataBind();
        }

        protected void RepeaterItemEventHandler(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (!string.IsNullOrEmpty(Selected))
                {
                    MenuItem item = e.Item.DataItem as MenuItem;
                    if (item != null && item.Title == Selected)
                    {
                        HtmlAnchor anchor = e.Item.FindControl("anchor") as HtmlAnchor;
                        anchor.Attributes["class"] = "active";
                    }
                }
            }
        }

        #endregion

    }
}
