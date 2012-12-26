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
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web.Security;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.Resource;
    using System.Web.UI.HtmlControls;


    #endregion

    public partial class ArticlesList : ListViewControl
    {

        #region private fields

        private const string ArticlesListJavascriptKey = "controls.articlelist.js";
        private const string DeleteArticleConfirmJsVariable = "L_DeleteArticleConfirmation";
        private const string DeleteArticleSuccessJsVariable = "L_DeleteArticleSuccess";
        private const string PublishArticleConfirmJsVariable = "L_PublishArticleConfirmation";
        private const string PublishArticleSuccessJsVariable = "L_PublishArticleSuccess";
        private const string SelectArticlesToDeleteJsVariable = "L_SelectArticlesToDelete";
        private const string SelectArticlesToPublishJsVariable = "L_SelectArticlesToPublish";
        private const string DeleteMultiArticlesConfirmationJsVariable = "L_DeleteMultiArticlesConfirmation";
        private const string PublishMultiArticlesConfirmationJsVariable = "L_PublishMultiArticlesConfirmation";

        private ArticleListParameters parameters = new ArticleListParameters();
        
        private IList<Article> articles = null;

        private bool loaded;
        private object syncRoot = new object();

        #endregion

        #region ctor

        public ArticlesList()
        {           
        }

        #endregion

        #region public properties

        public ArticleHandler ArticleHandler
        {
            get;
            set;
        }

        public PublishingStage Stage
        {
            get;
            set;
        }

        public string MoreUrl
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
            }
        }

        #endregion

        #region public methods

        public override void LoadData()
        {
            if (!loaded)
            {
                lock (syncRoot)
                {
                    if (!loaded)
                    {
                        int total;
                        long ownerId = AuthenticationUtility.Instance.GetLoggedUser().Id;

                        // load the articles
                        if (ArticleHandler == null)
                        {
                            articles = ArticleStore.Instance.Read(
                                                parameters.Query, 
                                                ownerId, 
                                                Stage, 
                                                parameters.Start, 
                                                parameters.Count, 
                                                out total);
                        }
                        else
                        {
                            articles = ArticleStore.Instance.Read(
                                                this.ArticleHandler, 
                                                parameters.Query, 
                                                ownerId, 
                                                Stage, 
                                                parameters.Start, 
                                                parameters.Count, 
                                                out total);
                        }

                        // set the total records
                        this.TotalRecords = total;

                        loaded = true;
                    }
                }
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {

            PopulateJavascriptIncludesAndVariables();

            this.more.HRef = MoreUrl;

            if (Parameters != null)
            {

                //set the header
                this.header.Text = Parameters.Header;

                if (Parameters.HideHeader)
                {
                    this.headerspan.Visible = false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(Parameters.MoreUrl))
                    {
                        this.more.HRef = Parameters.MoreUrl;
                    }
                }
            }

           
            // set the Id
            if (!string.IsNullOrEmpty(this.ID))
            {
                this.articleListDiv.ID = this.ID;
            }

            //load data
            LoadData();

            //set the articles
            this.articleListRow.Stage = this.Stage;
            this.articleListRow.Parameters = Parameters;
            this.articleListRow.Articles = articles;

            if (articles != null && articles.Count != 0)
            {
                this.none.Style["display"] = "none";
            }
            else
            {
                this.actioncontrols.Visible = false;
            }

        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    ArticlesListJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.ArticleListControl)
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteArticleConfirmJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Delete.Confirmation")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteArticleSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Delete.Success")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    PublishArticleConfirmJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Publish.Confirmation")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    PublishArticleSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Publish.Success")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    SelectArticlesToDeleteJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Delete.SelectAtleastOne")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    SelectArticlesToPublishJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Publish.SelectAtleastOne")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                DeleteMultiArticlesConfirmationJsVariable,
                ResourceManager.GetLiteral("Admin.Articles.Delete.MultiArticles")
            );

            this.clientJavaScript.RegisterClientScriptVariable(
                PublishMultiArticlesConfirmationJsVariable,
                ResourceManager.GetLiteral("Admin.Articles.Publish.MultiArticles")
            );
        }

        #endregion

    }
}
