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
namespace Jardalu.Ratna.Core.Pages
{

    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Core.Articles;

    #endregion

    public class PageStyle : IPageStyle
    {
        #region private fields

        private string articleControl;
        private string pageControl;
        private string articleSummaryControl;
        private string threadControl;
        private string navigationControlName;
        private string breadcrumbControlName;
        private string sideNavigationControlName;
        private string photoControl;
        private string photoThumbnailControl;

        private ArticleHandler handler = new BlogArticleHandler();

        #endregion

        #region public properties

        public string ArticleSummaryControl
        {
            get
            {
                return articleSummaryControl;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.articleSummaryControl = value;
            }
        }

        public string ArticleControl
        {
            get
            {
                return articleControl;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.articleControl = value;
            }
        }

        public string PageControl
        {
            get
            {
                return pageControl;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.pageControl = value;
            }
        }

        public string PhotoControl
        {
            get
            {
                return photoControl;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.photoControl = value;
            }
        }

        public string PhotoThumbnailControl
        {
            get
            {
                return photoThumbnailControl;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.photoThumbnailControl = value;
            }
        }

        public string ThreadControl
        {
            get
            {
                return threadControl;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.threadControl = value;
            }
        }

        public string DefaultPageControl
        {
            get;
            set;
        }

        public bool IsDefaultPageSupported
        {
            get;
            set;
        }

        public string NavigationControlName
        {
            get
            {
                return navigationControlName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.navigationControlName = value;
            }
        }

        public bool IsNavigationSupported
        {
            get;
            set;
        }

        public string BreadcrumbControlName
        {
            get { return this.breadcrumbControlName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.breadcrumbControlName = value;
            }
        }

        public bool IsBreadcrumbSupported
        {
            get;
            set;
        }

        public bool IsSideNavigationSupported
        {
            get;
            set;
        }

        public string SideNavigationControlName
        {
            get { return this.sideNavigationControlName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.sideNavigationControlName = value;
            }
        }

        public IList<string> SideNavigationRoots
        {
            get;
            set;
        }

        public ArticleHandler ArticleHandler
        {
            get
            {
                return handler;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.ArticleHandler = value;
            }
        }

        #endregion

    }
}
