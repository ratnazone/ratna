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

    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.Controls.Pagers;
using Jardalu.Ratna.Utilities;
    using System.Collections.Generic;
    using Jardalu.Ratna.Core.Articles;
    using System.Web.UI;
    using Jardalu.Ratna.Web.UI.Utilities;
    using Jardalu.Ratna.Core.Comments;
    using Jardalu.Ratna.Web.Controls;
    using System.Web.UI.HtmlControls;
    using Jardalu.Ratna.Core.Media;

    #endregion

    public abstract class CollectionPage : DynamicPage
    {
        #region private fields

        private static Logger logger;

        #endregion

        #region ctor

        static CollectionPage()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region protected properties

        protected string UrlPath
        {
            get
            {
                return Request["urlpath"] as string;
            }
        }

        public override int PageNumber
        {
            get
            {
                int page = 1;
                string pageStr = Request[Constants.PageRouteIdentifier] as string;

                if (!Int32.TryParse(pageStr, out page))
                {
                    page = 1;
                }

                if (page < 1)
                {
                    page = 1;
                }

                return page;
            }
        }

        protected string Navigation
        {
            get
            {
                return Request["nav"] as string;
            }
        }

        protected string CollectionTitle
        {

            get
            {
                return Request["title"] as string;
            }
        }

        #endregion

        #region protected methods

        protected void OnPageLoad()
        {
            //set the navigation
            if (!string.IsNullOrEmpty(Navigation))
            {
                RatnaMasterPage rmp = this.Master as RatnaMasterPage;
                if (rmp != null)
                {
                    //set the navigation      
                    rmp.SetNavigation(Navigation);
                }
            }

            this.Title = this.CollectionTitle;
        }

        protected void RenderPager(SimplePager pager)
        {

            if (pager == null)
            {
                return;
            }

            string rawUrl = Request.RawUrl;
            int index = rawUrl.IndexOf('?');
            if (index != -1)
            {
                rawUrl = rawUrl.Substring(0, index);
            }

            if (!string.IsNullOrEmpty(Query))
            {
                //set the page format
                pager.PageLinkFormat = string.Format("{0}?q={1}&p=", rawUrl, Query) + "{0}";
            }
            else
            {
                pager.PageLinkFormat = string.Format("{0}?p=", rawUrl) + "{0}";
            }

            pager.CurrentPageNumber = PageNumber;
            pager.TotalPages = this.TotalPages;

            if (pager.TotalPages <= 1)
            {
                pager.Visible = false;
            }
            else
            {
                pager.Visible = true;
            }
        }

        protected bool RenderArticles(IList<Article> articles, HtmlGenericControl articlesDiv)
        {
            logger.Log(LogLevel.Debug, "Start RenderArticles");
            bool error = true;

            Control control = ArticleBasePage.GetArticleSummaryControl(this.Master);
            if (control != null)
            {
                ArticleHandler handler = _default.GetArticleHandler(this.Master);
                if (handler != null)
                {

                    if (articles != null)
                    {

                        //get all the comments associated with the articles.
                        //and generate summary if not found
                        IList<string> commentKeys = new List<string>();
                        foreach (Article art in articles)
                        {
                            commentKeys.Add(ThreadUtility.GetKey(art));
                        }

                        //fetch all the comment count
                        IDictionary<string, int> commentsCount = CommentsPlugin.Instance.GetCommentsCount(commentKeys);

                        IArticleControl articleControl = control as IArticleControl;
                        foreach (Article article in articles)
                        {
                            BlogArticle blogArticle = article as BlogArticle;

                            if (articleControl != null)
                            {
                                articleControl.Article = article;
                                
                                if (blogArticle != null &&
                                    string.IsNullOrEmpty(blogArticle.Summary) &&
                                    !string.IsNullOrEmpty(blogArticle.Body))
                                {
                                    blogArticle.Summary = SummaryUtility.GetSummary(blogArticle.Body);
                                }

                                string commentKey = ThreadUtility.GetKey(article);

                                if (commentsCount.ContainsKey(commentKey))
                                {
                                    articleControl.CommentCount = commentsCount[commentKey];
                                }
                            }

                            articlesDiv.Controls.Add(control);
                            control = ArticleBasePage.GetArticleSummaryControl(this.Master);
                            articleControl = control as IArticleControl;

                            error = false;
                        }
                    }
                }
                else
                {
                    logger.Log(LogLevel.Warn, "Unable to locate ArticleHandler for master page - {0}", this.MasterPageFile);
                }
            }
            else
            {
                logger.Log(LogLevel.Warn, "Unable to locate ArticleControl for master page - {0}", this.MasterPageFile);
            }

            logger.Log(LogLevel.Debug, "End RenderArticles");

            return error;
        }

        protected bool RenderMedia(IList<BaseMedia> mediaList, HtmlGenericControl mediaDiv)
        {
            logger.Log(LogLevel.Debug, "Start RenderMedia");
            bool error = true;

            Control control = MediaBasePage.GetPhotoThumbnailControl(this.Master);
            if (control != null)
            {
                if (mediaList != null)
                {
                    IPhotoControl photoControl = control as IPhotoControl;
                    foreach (BaseMedia media in mediaList)
                    {
                        Photo photo = media as Photo;

                        if (photo != null)
                        {
                            photoControl.Photo = photo;
                        }

                        mediaDiv.Controls.Add(control);
                        control = MediaBasePage.GetPhotoThumbnailControl(this.Master);
                        photoControl = control as IPhotoControl;

                        error = false;
                    }
                }
            }

            logger.Log(LogLevel.Debug, "End RenderMedia");

            return error;
        }

        #endregion
    }

}
