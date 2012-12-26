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
namespace Jardalu.Ratna.Web.Pages
{
    #region using

    using System;
    using System.Web.UI;
    using System.Collections.Generic;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Core.Comments;    
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web.Security;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.Controls;
    using Jardalu.Ratna.Web.UI.Utilities;
    using Jardalu.Ratna.Utilities;
   
    #endregion

    public partial class ArticlePage : ArticleBasePage
    {

        #region private fields

        private static Guid BlogHandlerId = (new BlogArticleHandler()).Id;
        private BlogArticle blogArticle;

        private static Logger logger;

        #endregion

        #region ctor

        static ArticlePage()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region protected properties

        protected BlogArticle BlogArticle
        {
            get
            {
                if (blogArticle == null)
                {
                    blogArticle = new BlogArticle(Article);
                }

                return blogArticle;
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(UrlKey))
            {

                if (Article != null && Article.HandlerId == BlogHandlerId)
                {
                    // show the article.
                    this.noArticleFoundDiv.Visible = false;

                    //populate the meta-data tags
                    if (!string.IsNullOrEmpty(BlogArticle.Description))
                    {
                        this.MetaDescription = BlogArticle.Description;
                    }

                    if (!string.IsNullOrEmpty(BlogArticle.SerializedTags))
                    {
                        this.MetaKeywords = BlogArticle.SerializedTags;
                    }

                    // check if the template control can handle the article
                    Control control = GetArticleControl(this.Master);
                    IArticleControl articleControl = control as IArticleControl;
                    if (articleControl != null)
                    {
                        articleControl.Article = BlogArticle;

                        //add the control
                        this.main.Controls.AddAt(0, control);
                    }

                    //add navigation etc.
                    AddMasterPageInformation(BlogArticle);

                    if (Article.Tags != null)
                    {
                        //populate the tags
                        //this.tagRepeater.DataSource = Article.Tags;
                        //this.tagRepeater.DataBind();
                    }

                    // check if the page can handle thread
                    control = GetThreadControl(this.Master);
                    IThreadControl threadControl = control as IThreadControl;
                    if (threadControl != null)
                    {
                        
                        string key = ThreadUtility.GetKey(Article);
                        logger.Log(LogLevel.Debug, "Got threadcontrol - Key : {0}", key);
                        IList<Comment> comments = CommentsPlugin.Instance.Read(key, true /* ascending */);
                        threadControl.Comments = comments;
                        threadControl.Key = key;

                        //set the comments count
                        if (articleControl != null)
                        {
                            articleControl.CommentCount = comments.Count;
                        }

                        //read the comments
                        this.main.Controls.AddAt(this.main.Controls.Count, control);
                    }

                }
                else
                {
                    //hide the article div
                    this.articlesdiv.Visible = false;
                }

            }
        }

        #endregion

    }
}
