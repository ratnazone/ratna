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

    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class ActionPanel : System.Web.UI.UserControl
    {
        #region ActionItem

        class ActionItem
        {
            public string Image { get; set; }
            public string Title { get; set; }
            public string HRef { get; set; }
        }

        #endregion

        #region private fields

        private string title = ResourceManager.GetLiteral("Admin.Common.Actions");
        private List<ActionItem> actionItems = new List<ActionItem>();

        #endregion

        #region public properties

        public string Title
        {
            get { return title; }
            set { this.title = value; }
        }

        #endregion

        #region public methods

        public void AddAction(string title, string href)
        {
            AddAction(null, title, href);
        }

        public void AddAction(string image, string title, string href)
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

            ActionItem actionItem = new ActionItem() { Image = image, Title = title, HRef = href };
            this.actionItems.Add(actionItem);

        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            this.repeater.DataSource = this.actionItems;
            this.repeater.DataBind();
        }

        #endregion
    }
}
