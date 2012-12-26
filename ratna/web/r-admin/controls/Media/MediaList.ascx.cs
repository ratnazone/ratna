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
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using Jardalu.Ratna.Core.Media;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web.Security;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class MediaList : ListViewControl
    {
        #region fields

        private const int UrlDisplayLength = 50;
        private const int NameDisplayLength = 30;
        private IList<BaseMedia> list = null;

        private bool loaded;
        private object syncRoot = new object();

        #endregion

        #region public properties

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
                                    Parameters.MediaType,
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
            PopulateJavascriptIncludesAndVariables();

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
                    //hide the none
                    this.none.Visible = false;
                    this.repeater.DataSource = list;
                    this.repeater.DataBind();
                }

            }

        }

        protected void RepeaterItemEventHandler(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                if (Parameters != null)
                {
                    BaseMedia media = e.Item.DataItem as BaseMedia;
                    if (media != null)
                    {
                        Label createdDateLabel = e.Item.FindControl("createdDateLabel") as Label;
                        if (Parameters.ShortDate)
                        {
                            createdDateLabel.Text = Utility.FormatConciseDate(media.CreatedDate);
                        }
                        else
                        {
                            createdDateLabel.Text = Utility.FormatShortDate(media.CreatedDate);
                        }

                        //set the edit url
                        HtmlAnchor editAnchor = e.Item.FindControl("editurlanchor") as HtmlAnchor ;
                        if (media.MediaType == MediaType.Video)
                        {
                            editAnchor.HRef = Constants.Urls.Media.Videos.EditUrlWithKey + media.Url;
                        }
                        else if (media.MediaType == MediaType.Document)
                        {
                            editAnchor.HRef = Constants.Urls.Media.Documents.EditUrlWithKey + media.Url;
                        }

                        //incase the url is taking too much space, restrict it.
                        if (media.Url.Length > UrlDisplayLength)
                        {
                            Label urlLabel = e.Item.FindControl("urlLabel") as Label;
                            urlLabel.Text = Utility.ShortenForDisplay(media.Url, UrlDisplayLength);
                        }

                        //incase the name is too long, restrict it as well
                        if (media.Name.Length > NameDisplayLength)
                        {
                            Label nameLabel = e.Item.FindControl("nameLabel") as Label;
                            nameLabel.Text = Utility.ShortenForDisplay(media.Name, NameDisplayLength);
                        }

                    }
                }
            }
        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {

        }

        #endregion

    }
}
