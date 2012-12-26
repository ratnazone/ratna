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
namespace Jardalu.Ratna.Web.Admin.controls.Media
{

    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Core.Media;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web.Security;

    #endregion

    public partial class ImageViewList : ListViewControl
    {
        #region private fields

        private IList<BaseMedia> list = null;

        private bool loaded;
        private object syncRoot = new object();

        #endregion

        #region public properties

        public bool ShortMode
        {
            get;
            set;
        }

        public MediaListParameters Parameters
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

                        // fetch the media.
                        list = MediaStore.Instance.Read(
                                Parameters.Query,
                                MediaType.Photo,
                                AuthenticationUtility.Instance.GetLoggedUser().Id,
                                Parameters.Start,
                                Parameters.Count,
                                out total
                            );

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
            if (Parameters != null)
            {
                if (Parameters.HideHeader)
                {
                    this.headerspan.Visible = false;
                }
                else
                {
                    //set the header
                    this.header.Text = Parameters.Header;
                    if (!string.IsNullOrEmpty(Parameters.MoreUrl))
                    {
                        this.more.HRef = Utility.ResolveUrl(Parameters.MoreUrl);
                    }
                }

                //load the data
                LoadData();

                if (list.Count == 0)
                {
                    //hide the repeater
                    this.repeater.Visible = false;
                    this.none.Visible = true;
                }
                else
                {
                    //hide the none
                    this.none.Visible = false;
                    this.repeater.DataSource = list;
                    this.repeater.DataBind();
                }
            }
        }

        #endregion

    }
}
