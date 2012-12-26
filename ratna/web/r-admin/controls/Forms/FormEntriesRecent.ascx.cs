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
namespace Jardalu.Ratna.Web.Admin.controls.Forms
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using Jardalu.Ratna.Core.Forms;
    using System.Web.UI.HtmlControls;
   

    #endregion

    public partial class FormEntriesRecent : ListViewControl
    {
        #region private fields

        private IList<FormEntry> responses = null;
        private ListControlParameters parameters = new ListControlParameters();

        private bool loaded;
        private object syncRoot = new object();

        #endregion

        #region public properties

        public ListControlParameters Parameters
        {
            get
            {
                return this.parameters;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                parameters = value;
            }
        }

        #endregion

        #region public methods

        public override void LoadData()
        {
            if (!loaded)
            {
                lock (syncRoot)
                {
                    if (!loaded)
                    {

                        // load the responses
                        responses = FormEntryPlugin.Instance.GetFormResponses(DateTime.Today.AddDays(-1));

                        // set the total records
                        this.TotalRecords = responses.Count;

                        loaded = true;
                    }
                }
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();

            RenderEntries();

            if (Parameters.HideHeader)
            {
                this.headerspan.Visible = false;
            }
            else
            {
                this.more.HRef = Utility.ResolveUrl(Constants.Urls.Forms.RecentUrl);
            }
        }

        protected void RepeaterItemEventHandler(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                FormEntry entry = e.Item.DataItem as FormEntry;

                if (entry != null)
                {
                    HtmlGenericControl pubDateSpan = e.Item.FindControl("updatedTime") as HtmlGenericControl;
                    if (pubDateSpan != null)
                    {
                        pubDateSpan.InnerText = entry.UpdatedTime.ToString();
                    }

                    HtmlAnchor editAnchor = e.Item.FindControl("editentryanchor") as HtmlAnchor;
                    if (editAnchor != null)
                    {
                        editAnchor.HRef = string.Format(Constants.Urls.Forms.EntryUrlWithKey, entry.Form, entry.UId);
                    }

                    HtmlAnchor formentriesanchor = e.Item.FindControl("formentriesanchor") as HtmlAnchor;
                    if (formentriesanchor != null)
                    {
                        formentriesanchor.HRef = Constants.Urls.Forms.ResponsesUrl + entry.Form;
                    }
                }
            }
        }

        #endregion

        #region private methods

        private void RenderEntries()
        {

            if (responses == null || responses.Count == 0)
            {
                this.none.Visible = true;
            }
            else
            {
                // display the values
                this.repeater.DataSource = responses;
                this.repeater.DataBind();
            }
        }

        #endregion

    }
}
