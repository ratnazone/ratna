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
namespace Jardalu.Ratna.Web
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Core.Comments;
    using Jardalu.Ratna.Core.Pages;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Controls;
    using Jardalu.Ratna.Web.Pages;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.AppData;
    using Jardalu.Ratna.Web.UI.Utilities;

    #endregion

    public partial class _default : DynamicPage
    {
        #region private fields

        private Logger logger;

        #endregion

        #region ctor

        public _default()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            this.RouteName = "default";
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            logger.Log( LogLevel.Debug, "Start Page_Load");

            // check if the template defines a master page control
            // if so, render the default page
            RatnaMasterPage rmp = this.Master as RatnaMasterPage;
            if (rmp != null && rmp.PageStyle.IsDefaultPageSupported  && rmp.FetchUrl == "/")
            {
                RenderTemplateDefaultPage(rmp.PageStyle);
                this.noArticleFoundDiv.Visible = false;
            }
            else
            {
                // this page will get loaded only when
                // no "/default" has been defined.
                bool error = RenderArticles();
                if (!error)
                {
                    RenderPager();
                }
                else if (rmp != null &&
                    (rmp.FetchUrl == "/" || rmp.FetchUrl == Constants.DefaultPageUrl))
                {
                    rmp.SetNavigation("home"); //default home navigation
                    RenderWelcomePage();
                }
                else
                {
                    // page not found.
                    this.noArticleFoundDiv.Visible = true;
                }
            }

            // if no title has been set, set the default title
            if (string.IsNullOrEmpty(this.Title))
            {
                this.Title = WebContext.Current.Site.Title;
            }

            logger.Log( LogLevel.Debug, "End Page_Load");
        }

        #endregion

        #region private methods

        private bool RenderArticles()
        {
            logger.Log( LogLevel.Debug, "Start RenderArticles");
            bool error = true;
            this.TotalPages = 0;

            Control control = ArticleBasePage.GetArticleSummaryControl(this.Master);
            if (control != null)
            {
                ArticleHandler handler = GetArticleHandler(this.Master);
                if (handler != null)
                {

                    int total = 0;
                    int start = 1 + (this.PageNumber - 1) * Pager.PageSize;

                    //read the articles
                    IList<Article> articles = handler.Read(this.Query, start, this.Pager.PageSize, out total);
                    if (articles != null)
                    {

                        //get all the comments associated with the articles.
                        //and generate summary if not found
                        IList<string> commentKeys = new List<string>();
                        foreach(Article art in articles)
                        {
                            commentKeys.Add(ThreadUtility.GetKey(art));
                        }

                        //fetch all the comment count
                        IDictionary<string, int> commentsCount = CommentsPlugin.Instance.GetCommentsCount(commentKeys);

                        IArticleControl articleControl = control as IArticleControl;
                        foreach (Article article in articles)
                        {
                            if (articleControl != null)
                            {
                                articleControl.Article = article;
                                BlogArticle blogArticle = article as BlogArticle;
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

                            this.articlesdiv.Controls.Add(control);
                            control = ArticleBasePage.GetArticleSummaryControl(this.Master);
                            articleControl = control as IArticleControl;
                            
                            error = false;
                        }
                    }

                    if (total > 0)
                    {
                        this.TotalPages = (total / this.Pager.PageSize) + (total % this.Pager.PageSize > 0 ? 1 : 0);
                    }
                    else
                    {
                        logger.Log(LogLevel.Info, "No articles found");
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

            if (!error)
            {
                this.noArticleFoundDiv.Visible = false;
            }

            logger.Log(LogLevel.Debug, "End RenderArticles");

            return error;
        }        

        public static ArticleHandler GetArticleHandler(MasterPage masterPage)
        {
            ArticleHandler handler = null;

            RatnaMasterPage master = masterPage as RatnaMasterPage;
            if (master != null)
            {
                IPageStyle pageStyle = master.PageStyle;

                if (pageStyle != null)
                {
                    handler = pageStyle.ArticleHandler;
                }
            }

            return handler;
        }

        private void RenderPager()
        {
            this.Pager.CurrentPageNumber = PageNumber;
            this.Pager.TotalPages = this.TotalPages;

            if (this.Pager.TotalPages <= 1)
            {
                this.Pager.Visible = false;
            }
            else
            {
                this.Pager.Visible = true;
            }
        }

        private void RenderDefaultPage()
        {
            // do nothing. url redirector
            // will automatically direct such pages.

            //hide pager
            this.Pager.Visible = false;
        }

        private void RenderWelcomePage()
        {

            XmlDocument document = new XmlDocument();
            document.LoadXml(DataReader.ReadFileContents("welcome.xml"));

            ContentPlaceHolder contentPlaceHolder = Master.FindControl("contentPlaceHolder") as ContentPlaceHolder;
            contentPlaceHolder.Controls.Clear();

            // add a new control
            LiteralControl literal = new LiteralControl(document.DocumentElement.SelectSingleNode("content").InnerXml);
            contentPlaceHolder.Controls.Add(literal);

            ContentPlaceHolder head = Master.FindControl("head") as ContentPlaceHolder;
            head.Controls.Clear();

            // add a new control
            LiteralControl headLiteral = new LiteralControl(document.DocumentElement.SelectSingleNode("head").InnerXml);
            head.Controls.Add(headLiteral);

        }

        private void RenderTemplateDefaultPage(IPageStyle pageStyle)
        {
            logger.Log(LogLevel.Debug, "Start RenderTemplateDefaultPage");

            Control control = null;
            string pageControl = pageStyle.DefaultPageControl;

            if (!string.IsNullOrEmpty(pageControl))
            {

                // check the file exists
                try
                {
                    string controlPath = string.Format("{0}/{1}", Master.AppRelativeTemplateSourceDirectory, pageControl);

                    control = Master.LoadControl(Master.ResolveUrl(pageControl));
                }
                catch (Exception exception)
                {
                    // unable to load the control.
                    logger.Log(LogLevel.Warn, "Unable to load the control at : [{0}], error message : {1}", pageControl, exception);
                }
            }
            else
            {
            }

            // control loaded
            if (control != null)
            {
                // add control to main page.
                ContentPlaceHolder contentPlaceHolder = Master.FindControl("contentPlaceHolder") as ContentPlaceHolder;
                contentPlaceHolder.Controls.Add(control);
            }

            logger.Log(LogLevel.Debug, "End RenderTemplateDefaultPage");
        }

        #endregion

    }
}
