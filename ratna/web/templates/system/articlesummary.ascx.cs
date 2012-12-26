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

    using Jardalu.Ratna.Web.Controls;
    using Jardalu.Ratna.Core.Articles;

    #endregion

    public partial class articlesummary : System.Web.UI.UserControl, IArticleControl
    {
        public Article Article
        {
            get;
            set;
        }

        public int CommentCount
        {
            get;
            set;
        }

        public BlogArticle BlogArticle
        {
            get
            {
                return Article as BlogArticle;
            }
        }

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BlogArticle != null)
            {
                if (!string.IsNullOrEmpty(BlogArticle.SummaryImage))
                {
                    this.blogimg.Src = BlogArticle.SummaryImage;
                    this.blogimg.Visible = true;
                }
            }
        }

        #endregion

    }
}
