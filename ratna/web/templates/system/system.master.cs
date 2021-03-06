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


    using Jardalu.Ratna.Core.Pages;
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Web.UI;

    #endregion

    public partial class Template : RatnaMasterPage
    {
        public Template()
        {
            PageStyle pageSytle = new PageStyle()
            {
                ArticleControl = "article.ascx",
                ArticleSummaryControl = "articlesummary.ascx",
                ThreadControl = "thread.ascx",
                PageControl = "pagecontrol.ascx",
                IsNavigationSupported = true,
                NavigationControlName = "nav",
                PhotoThumbnailControl = "photosummarycontrol.ascx"
            };

            this.PageStyle = pageSytle;

        }

        protected void Page_Load(object sender, System.EventArgs e)
        {

        }
    }
}
