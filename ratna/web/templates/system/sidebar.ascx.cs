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
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Core.Comments;
    using Jardalu.Ratna.Core.Forms;
        
    #endregion

    public partial class sidebar : System.Web.UI.UserControl
    {

        private const int Size = 5;
        private int PostSize;
        private int CommentsSize;
        private int LinksSize;

        protected void Page_Load(object sender, EventArgs e)
        {

            int total;

            //read the last 5 articles
            IList<Article> articles = BlogArticleHandler.Instance.Read(string.Empty, 1, Size, out total);
            if (articles != null && articles.Count > 0)
            {
                PostSize = articles.Count;

                //databind
                this.postsrepeater.DataSource = articles;
                this.postsrepeater.DataBind();

            }
            else
            {
                // hide latest posts
                this.recent.Visible = false;
            }

            //read the last 5 comments
            IList<Comment> comments = CommentsPlugin.Instance.GetComments(string.Empty, 1, Size, out total);
            if (comments != null && comments.Count > 0)
            {
                CommentsSize = comments.Count;

                this.commentsrepeater.DataSource = comments;
                this.commentsrepeater.DataBind();
            }
            else
            {
                // hide recent comments
                this.latestcomments.Visible = false;
            }

            // read fav links
            Form form;
            bool formsDisplay = false;
            if (FormsPlugin.Instance.TryRead("links", out form))
            {
                //read the form entries
                IList<FormEntry> entries = FormEntryPlugin.Instance.GetFormResponses(form.Name);
                if (entries != null && entries.Count > 0)
                {
                    formsDisplay = true;
                    LinksSize = entries.Count;

                    linksRepeater.DataSource = entries;
                    linksRepeater.DataBind();
                }
            }

            if (!formsDisplay)
            {
                favlinks.Visible = false;
            }
        }

        protected void Posts_OnItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Article article = e.Item.DataItem as Article;

                if (article != null)
                {
                    HtmlAnchor postanchor = e.Item.FindControl("postanchor") as HtmlAnchor;
                    if (postanchor != null)
                    {
                        postanchor.HRef = Utility.ResolveUrl(article.UrlKey);
                        postanchor.InnerText = article.Title;
                    }

                    Literal postedLiteral = e.Item.FindControl("postedLiteral") as Literal;
                    postedLiteral.Text = article.PublishedDate.ToString("MMM dd yyyy");

                    if (e.Item.ItemIndex == PostSize - 1)
                    {
                        //last item
                        HtmlGenericControl postli = e.Item.FindControl("postli") as HtmlGenericControl;
                        if (postli != null)
                        {
                            string classText = postli.Attributes["class"];

                            if (string.IsNullOrEmpty(classText))
                            {
                                postli.Attributes["class"] = "last";
                            }
                            else
                            {
                                postli.Attributes["class"] = classText + " last";
                            }
                        }
                    }
                }
            }
        }

        protected void Comments_OnItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Comment comment = e.Item.DataItem as Comment;

                if (comment != null)
                {
                    string link = comment.PermaLink == null ? "#" : string.Format("{0}#{1}", comment.PermaLink, comment.Id);

                    HtmlAnchor commentsanchor = e.Item.FindControl("commentsanchor") as HtmlAnchor;
                    if (commentsanchor != null)
                    {
                        commentsanchor.HRef = string.Format("{0}#{1}", comment.PermaLink, comment.Id);
                        commentsanchor.InnerText = comment.Body;
                    }

                    HtmlGenericControl namespan = e.Item.FindControl("namespan") as HtmlGenericControl;
                    if (namespan != null)
                    {
                        namespan.InnerText = comment.Name;
                    }

                    if (e.Item.ItemIndex == CommentsSize - 1)
                    {
                        //last item
                        HtmlGenericControl postli = e.Item.FindControl("postli") as HtmlGenericControl;
                        if (postli != null)
                        {
                            string classText = postli.Attributes["class"];

                            if (string.IsNullOrEmpty(classText))
                            {
                                postli.Attributes["class"] = "last";
                            }
                            else
                            {
                                postli.Attributes["class"] = classText + " last";
                            }
                        }
                    }
                }

            }
        }

        protected void Links_OnItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                FormEntry entry = e.Item.DataItem as FormEntry;

                if (entry != null)
                {
                    
                    HtmlAnchor linksanchor = e.Item.FindControl("linksanchor") as HtmlAnchor;
                    if (linksanchor != null)
                    {
                        string href = entry.GetFieldData("Href").Value as string;
                        string title = entry.GetFieldData("Title").Value as string;

                        linksanchor.HRef = href;
                        linksanchor.InnerText = title;
                    }

                   

                    if (e.Item.ItemIndex == LinksSize - 1)
                    {
                        //last item
                        HtmlGenericControl postli = e.Item.FindControl("postli") as HtmlGenericControl;
                        if (postli != null)
                        {
                            string classText = postli.Attributes["class"];

                            if (string.IsNullOrEmpty(classText))
                            {
                                postli.Attributes["class"] = "last";
                            }
                            else
                            {
                                postli.Attributes["class"] = classText + " last";
                            }
                        }
                    }
                }

            }
        }
    }
}
