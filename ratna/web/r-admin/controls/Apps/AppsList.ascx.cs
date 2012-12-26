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
namespace Jardalu.Ratna.Web.Admin.controls.Apps
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using Jardalu.Ratna.Core.Apps;
    using Jardalu.Ratna.Store;

    #endregion

    public partial class AppsList : ListViewControl
    {
        #region private fields

        private AppListParameters parameters = new AppListParameters();
        private IList<App> appsList = null;

        private bool loaded;
        private object syncRoot = new object();

        #endregion

        #region public properties

        public AppListParameters Parameters
        {
            get { return this.parameters; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.parameters = value;
            }
        }

        #endregion

        #region protected

        protected void Page_Load(object sender, EventArgs e)
        {
            //load data
            LoadData();

            //set the ui


            if (appsList != null && appsList.Count > 0)
            {
                this.repeater.DataSource = appsList;
                this.repeater.DataBind();
            }
            else
            {
                // no apps found.
                this.noapps.Visible = true;
            }

        }

        protected void RepeaterItemEventHandler(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (Parameters != null)
                {
                    App app = e.Item.DataItem as App;
                    if (app != null)
                    {
                        AppListItem appListItem = e.Item.FindControl("appListItem") as AppListItem;
                        if (appListItem != null)
                        {
                            appListItem.App = app;
                        }
                    }
                }
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
                        int total = 0;

                        if (Parameters.ShowEnabled)
                        {
                            // fetch the enabled apps
                            appsList = AppStore.Instance.GetAppList(true);
                            total = appsList.Count;
                        }
                        else
                        {
                            appsList = AppStore.Instance.GetAppList(false);
                            total = appsList.Count;
                        }

                        // set the total records
                        this.TotalRecords = total;

                        loaded = true;
                    }
                }
            }
        }

        #endregion


    }
}
