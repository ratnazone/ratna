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

    public interface IPageStyle
    {
        /// <summary>
        /// Control for rendering blog article summary
        /// </summary>
        string ArticleSummaryControl { get; }

        /// <summary>
        /// Control for rendering a single blog article
        /// </summary>
        string ArticleControl { get; }

        bool IsDefaultPageSupported { get; }

        /// <summary>
        /// Control for rendering the default page
        /// </summary>
        string DefaultPageControl
        {
            get;
        }

        /// <summary>
        /// Control for rendering Thread
        /// </summary>
        string ThreadControl { get; }

        /// <summary>
        /// Control for rendering page
        /// </summary>
        string PageControl { get; }
        
        ArticleHandler ArticleHandler { get; }

        /// <summary>
        /// Tells if the active template for the page suppport navigation
        /// </summary>
        bool IsNavigationSupported { get; }
        string NavigationControlName { get; }
        bool IsBreadcrumbSupported { get; }
        string BreadcrumbControlName { get; }
        bool IsSideNavigationSupported { get; }
        string SideNavigationControlName { get; }

        /// <summary>
        /// A list of "roots" that are used for side navigation control.
        /// 
        /// For example, if the site has urls like
        /// /about/
        /// /about/contact
        /// /kb
        /// /products
        /// /products/product_1
        /// /products/product_2
        /// 
        /// In the preceeding list, '/about' and '/products' can be choosen
        /// Side navigation uses this list to create the initial navigation
        /// </summary>
        IList<string> SideNavigationRoots { get; }

        string PhotoControl { get; }
        string PhotoThumbnailControl { get; }
    }

}
