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


namespace Jardalu.Ratna.Web.Admin.controls.gallery
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using Jardalu.Ratna.Web.Plugins;
    using Jardalu.Ratna.Web.Resource;


    #endregion

    public partial class GalleryList : ListViewControl
    {

        #region private fields

        private const int UrlDisplayLength = 50;

        private IList<Gallery> list = null;

        private bool loaded;
        private object syncRoot = new object();

        private const string DeleteGalleryConfirmJsVariable = "L_DeleteGalleryConfirmation";
        private const string DeleteGallerySuccessJsVariable = "L_DeleteGallerySuccess";

        #endregion

        #region public properties

        public ListControlParameters Parameters
        {
            get;
            set;
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
                        int total;

                        // fetch the galleries
                        list = GalleryPlugin.Instance.Read(Parameters.Start, Parameters.Count, out total);

                        // set the total records
                        this.TotalRecords = total;

                        loaded = true;
                    }
                }
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateJavascriptIncludesAndVariables();

            if (Parameters != null)
            {
                // load the data
                LoadData();

                if (list.Count == 0)
                {
                    //hide the repeater
                    this.repeater.Visible = false;
                    this.none.Visible = true;
                }
                else
                {
                    //hide the none with display.
                    this.none.Style["display"] = "none";

                    this.repeater.DataSource = list;
                    this.repeater.DataBind();
                }

            }
        }

        protected void RepeaterItemEventHandler(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
            }
        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptVariable(
                   DeleteGalleryConfirmJsVariable,
                   ResourceManager.GetLiteral("Admin.Media.Gallery.Delete.Confirmation")
               );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteGallerySuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Media.Gallery.Delete.Success")
                );
        }

        #endregion
    }
}
