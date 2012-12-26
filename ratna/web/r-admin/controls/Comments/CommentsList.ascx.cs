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
namespace Jardalu.Ratna.Web.Admin.controls.Comments
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using Jardalu.Ratna.Core.Comments;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class CommentsList : ListViewControl
    {
        #region private fields

        private const string CommentsListJavascriptKey = "comments.commentlist.js";
        private const string DeleteCommentConfirmJsVariable = "L_DeleteCommentConfirmation";
        private const string DeleteCommentSuccessJsVariable = "L_CommentDeletionSuccess";
        private const string DeleteCommentFailureJsVariable = "L_CommentDeletionFailure";
        private const string ApproveCommentSuccessJsVariable = "L_CommentApprovalSuccess";
        private const string ApproveCommentFailureJsVariable = "L_CommentApprovalFailure";

        private CommentListParameters parameters = new CommentListParameters();
        private IList<Comment> comments = null;

        private bool loaded;
        private object syncRoot = new object();

        #endregion

        #region public properties

        public CommentListParameters Parameters
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

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateJavascriptIncludesAndVariables();

            if (Parameters.HideHeader)
            {
                this.headerspan.Visible = false;
            }
            else
            {
                this.header.Text = Parameters.Header;
                this.more.HRef = Parameters.MoreUrl;

            }

            LoadData();

            if (comments != null && comments.Count > 0)
            {
                // populate the repeater
                this.repeater.DataSource = comments;
                this.repeater.DataBind();
            }
            else
            {
                // no comments
                this.repeater.Visible = false;
                this.none.Visible = true;
            }
        }

        protected void OnItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {


                Comment comment = e.Item.DataItem as Comment;
                if (comment != null)
                {
                    Label dateLabel = e.Item.FindControl("dateLabel") as Label;
                    dateLabel.Text = Utility.FormatConciseDate(comment.UpdatedTime);
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
                        int total;

                        // load the comments
                        comments = CommentsPlugin.Instance.GetComments(parameters.Query, 
                                parameters.FetchPending, parameters.Start, parameters.Count, out total);

                        // set the total records
                        this.TotalRecords = total;

                        loaded = true;
                    }
                }
            }
        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    CommentsListJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.CommentListControl)
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteCommentConfirmJsVariable,
                    ResourceManager.GetLiteral("Admin.Comments.Delete.Confirmation")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteCommentSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Comments.Delete.Success")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteCommentFailureJsVariable,
                    ResourceManager.GetLiteral("Admin.Comments.Delete.Failure")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    ApproveCommentSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Comments.Approve.Success")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    ApproveCommentFailureJsVariable,
                    ResourceManager.GetLiteral("Admin.Comments.Approve.Failure")
                );
        }

        #endregion

    }
}
