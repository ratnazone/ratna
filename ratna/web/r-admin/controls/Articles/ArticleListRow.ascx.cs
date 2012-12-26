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
namespace Jardalu.Ratna.Web.Admin.controls.Articles
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Web.UI.Snippets;
    using Jardalu.Ratna.Store;

    #endregion

    /// <summary>
    /// To generate snippet successfully, the following parameters must be provided by
    /// the caller.
    /// 
    /// urlkey - the key for the article
    /// 
    /// </summary>
    public partial class ArticleListRow : SnippetControl
    {

        #region private fields

        private ArticleListParameters parameters = new ArticleListParameters();
        private bool parametersSet;

        #endregion

        #region public properties

        public string urlkey
        {
            get;
            set;
        }

        public IList<Article> Articles
        {
            get;
            set;
        }

        public ArticleListParameters Parameters
        {
            get { return this.parameters; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.parameters = value;
                parametersSet = true;
            }
        }


        public PublishingStage Stage
        {
            get;
            set;
        }

        public string PreviewAppender
        {
            get
            {
                string appender = string.Empty;
                if (Stage == PublishingStage.Draft)
                {
                    appender = Constants.DraftStageIdentifier;
                }

                return appender;
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateControl();
        }

        protected void RepeaterItemEventHandler(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (Parameters != null)
                {
                    Article article = e.Item.DataItem as Article;
                    if (article != null && !string.IsNullOrEmpty(Parameters.EditUrlPrefix))
                    {
                        HtmlAnchor editurlanchor = e.Item.FindControl("editurlanchor") as HtmlAnchor;
                        editurlanchor.HRef = Parameters.EditUrlPrefix + article.UrlKey;
                    }
                }
            }
        }

        #endregion

        #region public methods

        public override void PopulateControl()
        {

            // snippet control gets done without the parameter populating.
            // generate parameters if needed
            GenerateParametersIfUndefined();

            if (Articles == null || Articles.Count == 0)
            {
                this.repeater.Visible = false;
            }
            else
            {
                this.repeater.DataSource = Articles;
                this.repeater.DataBind();
            }
        }

        #endregion

        #region private methods

        private void GenerateParametersIfUndefined()
        {
            if (!parametersSet)
            {
                // snippet generation is supported for published stage.
                // it gets used when the 

                this.Stage = PublishingStage.Published;

                ArticleListParameters p = new ArticleListParameters();
                p.ShowDelete = true;
                p.ShowPublish = false;
                p.EditUrlPrefix = Constants.Urls.Pages.EditUrlWithKey;
                this.Parameters = p;

                if (!string.IsNullOrEmpty(urlkey))
                {
                    // get the articles.
                    Article article = ArticleStore.Instance.GetArticle(urlkey, this.Stage);
                    List<Article> articles = new List<Article>(1);
                    articles.Add(article);

                    this.Articles = articles;
                }
            }
        }

        #endregion

    }
}
