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
namespace Jardalu.Ratna.Web.pages
{

    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.Pages;
    using Jardalu.Ratna.Core.Media;
    
    #endregion

    public partial class search : CollectionPage
    {

        enum SearchType
        {
            All,
            Posts,
            Photos
        }


        #region private fields

        private Logger logger;
        private const int PhotosPagerSize = 6;
        private const int PostsPagerSize = 4;

        #endregion

        #region ctor

        public search()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region public properties

        private SearchType SearchEntity
        {
            get
            {
                SearchType sType = SearchType.All;

                string s = Page.RouteData.Values["t"] as string;
                if (!Enum.TryParse<SearchType>(s, true, out sType))
                {
                    sType = SearchType.All;
                }

                return sType;
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = ResourceManager.GetLiteral("Common.Search");

            if (!string.IsNullOrEmpty(Query))
            {
                // search for pages
                this.__ratna_search_q.Value = Query;

                // set the href for articles link
                articlesAllAnchor.HRef = string.Format("/search/{0}/{1}", Query, "posts");
                photosAllAnchor.HRef = string.Format("/search/{0}/{1}", Query, "photos");

                bool renderPager = false;
                bool foundAnyData = false;

                switch (SearchEntity)
                {
                    case SearchType.All:
                        foundAnyData = SearchPosts() || foundAnyData;
                        foundAnyData = SearchPhotos() || foundAnyData;
                        break;
                    case SearchType.Posts:
                        renderPager = true;
                        foundAnyData = SearchPosts();
                        this.photosdiv.Visible = false;
                        break;
                    case SearchType.Photos:
                        renderPager = true;
                        foundAnyData = SearchPhotos();
                        this.articlesdiv.Visible = false;
                        break;
                }

                if (renderPager)
                {
                    RenderPager(this.Pager);
                }

                if (!foundAnyData)
                {
                    this.nosearchfound.Visible = true;
                }
            }
            else
            {
                this.nosearchfound.Visible = true;
            }
        }

        #endregion

        #region private methods

        private bool SearchPosts()
        {
            bool found = true;
            int total;
            int page = PageNumber;

            Pager.PageSize = PostsPagerSize;

            // page number starts from 1 in UI
            int start = 1 + ((page - 1) * Pager.PageSize);

            IList<Article> articles = BlogArticleHandler.Instance.Read(Query, start, Pager.PageSize, out total);

            if (articles.Count > 0)
            {
                RenderArticles(articles, this.articlesdiv);
                this.articlescount.Text = total.ToString();

                this.TotalPages = (total / this.Pager.PageSize) + (total % this.Pager.PageSize > 0 ? 1 : 0);
            }
            else
            {
                found = false;
                this.articlesdiv.Visible = false;
            }

            return found;
        }

        private bool SearchPhotos()
        {
            bool found = true;
            int total;
            int page = PageNumber;

            Pager.PageSize = PhotosPagerSize;

            // page number starts from 1 in UI
            int start = 1 + ((page - 1) * Pager.PageSize);

            IList<BaseMedia> mediaList = MediaStore.Instance.Read(Query, MediaType.Photo, start, Pager.PageSize, out total);

            if (mediaList.Count > 0)
            {
                RenderMedia(mediaList, this.photosdiv);
                this.photosCount.Text = total.ToString();

                this.TotalPages = (total / this.Pager.PageSize) + (total % this.Pager.PageSize > 0 ? 1 : 0);
            }
            else
            {
                found = false;
                this.photosdiv.Visible = false;
            }

            return found;
        }

        #endregion

    }

}
