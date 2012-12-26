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
    using System.Collections.Generic;
    using System.Web.UI;

    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Core.Comments;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.UI.Utilities;
    using Jardalu.Ratna.Web.Controls;
    using Jardalu.Ratna.Web.UI;

    #endregion

    public partial class articlecollection : CollectionPage
    {
        #region private fields

        private static Logger logger;

        #endregion

        #region ctor

        static articlecollection()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion


        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            base.OnPageLoad();

            string urlPath = UrlPath;
            int page = PageNumber;
            bool found = false;

            if (!string.IsNullOrEmpty(urlPath))
            {
                int total;

                // page number starts from 1 in UI
                int start = 1 + ((page-1) * Pager.PageSize);

                IList<Article> articles = BlogArticleHandler.Instance.ReadInPath(urlPath, start, Pager.PageSize, out total);
                if (articles != null && articles.Count > 0)
                {
                    found = true;
                    this.TotalPages = (total / this.Pager.PageSize) + (total % this.Pager.PageSize > 0 ? 1 : 0);
                    RenderArticles(articles);
                    RenderPager(this.Pager);
                }
            }

            if (!found)
            {
                noArticleFoundDiv.Visible = true;
            }

        }

        protected void RenderArticles(IList<Article> articles)
        {
            if (!base.RenderArticles(articles, this.articlesdiv))
            {
                this.noArticleFoundDiv.Visible = false;
            }            
        }

        #endregion

    }
}
