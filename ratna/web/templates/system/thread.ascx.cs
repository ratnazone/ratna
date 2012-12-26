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
namespace Jardalu.Ratna.Web.templates.system
{

    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Core.Comments;
    using Jardalu.Ratna.Web.Controls;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.UI.Snippets;
    using System.Web.UI.WebControls;

    #endregion

    public partial class thread : System.Web.UI.UserControl, IThreadControl
    {


        #region public methods

        public IList<Comment> Comments
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }

        #endregion

        #region public method

        public string GetMessageRow(string imageurl, string name, string url, string time, string message)
        {

            #region argument checking

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            #endregion

            // message row control path
            string messageRowControlPath = string.Format("{0}/{1}", this.AppRelativeTemplateSourceDirectory, "messagerow.ascx");

            messagerow mrow = FormlessPage.GetControl(messageRowControlPath) as messagerow;
            string html = string.Empty;

            if (mrow != null)
            {
                mrow.Name = name;
                mrow.Url = url;
                mrow.ImageUrl = imageurl;
                mrow.Message = message;
                mrow.Time = time;

                html = mrow.GetHtml();
            }

            return html;
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            //set the snippet to be used
            RatnaMasterPage rmp = this.Page.Master as RatnaMasterPage;
            if (rmp != null &&
                !string.IsNullOrEmpty(rmp.PageStyle.ThreadControl))
            {
                this.threadrenderer.Value = rmp.PageStyle.ThreadControl;
                this.permalink.Value = rmp.FetchUrl;

                //thread control path
                string threadControlPath = string.Format("{0}/{1}", this.AppRelativeTemplateSourceDirectory, rmp.PageStyle.ThreadControl);

                SnippetManager.Instance.RegisterControl(rmp.PageStyle.ThreadControl, threadControlPath);

            }

            if (Comments != null)
            {
                //render
                this.repeater.DataSource = this.Comments;
                this.repeater.DataBind();
            }
        }

        protected void RepeaterItemEventHandler(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {

                messagerow mrow = e.Item.FindControl("messageRow") as messagerow;
                if (mrow != null)
                {
                    mrow.IsAlt = true;
                }

            }
        }

        #endregion


    }
}
