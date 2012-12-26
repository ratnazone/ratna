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
    using System.Web.UI.HtmlControls;

    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Core.Navigation;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Templates;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.Controls;

    #endregion

    public partial class _static : ArticleBasePage
    {
        #region private fields

        private static Guid StaticHandlerId = (new StaticArticleHandler()).Id;
        protected System.Web.UI.WebControls.Literal pageheadLiteral;

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(UrlKey))
            {
                if (Article != null && Article.HandlerId == StaticHandlerId)
                {
                    StaticArticle staticArticle = new StaticArticle(Article);

                    this.Title = staticArticle.Title;

                    if (!string.IsNullOrEmpty(staticArticle.Head))
                    {
                        this.pageheadLiteral.Text = staticArticle.Head;
                    }

                    //populate the meta-data tags
                    if (!string.IsNullOrEmpty(staticArticle.Description))
                    {
                        this.MetaDescription = staticArticle.Description;
                    }

                    if (!string.IsNullOrEmpty(staticArticle.SerializedTags))
                    {
                        this.MetaKeywords = staticArticle.SerializedTags;
                    }

                    // show the article.
                    this.noArticleFoundDiv.Visible = false;

                    Control control = GetPageControl(this.Master);
                    IArticleControl articleControl = control as IArticleControl;
                    if (articleControl != null)
                    {
                        articleControl.Article = staticArticle;
                    }
                    else
                    {
                        // try as html container control
                        HtmlContainerControl htmlControl = control as HtmlContainerControl;
                        if (htmlControl != null)
                        {
                            htmlControl.InnerHtml = staticArticle.Body;
                        }
                    }

                    // add the control to div
                    this.articlesDiv.Controls.Add(control);

                    //add navigation etc.
                    AddMasterPageInformation(staticArticle);

                }
                else
                {
                    //hide the article div
                    this.articlesDiv.Visible = false;
                }

            }
        }

        #endregion


    }
}
