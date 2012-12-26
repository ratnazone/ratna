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

    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class ArticleMedia : System.Web.UI.UserControl
    {

        #region private fields

        private const string ArticlesMediaJavascriptKey = "control.articlemedia.js";
        private const string SelectMediaToDeleteJsVariable = "L_SelectArticleMediaToDelete";
        private const string RemoveMultiMediaConfirmationJsVariable = "L_RemoveMultiMediaConfirmation";
        private const string RemoveMultiMediaSuccessJsVariable = "L_RemoveMultiMediaSuccess";

        #endregion

        #region public methods

        public string Header
        {
            get;
            set;
        }

        public bool ShowControls
        {
            get;
            set;
        }

        public string SubHeader
        {
            get;
            set;
        }

        public IList<string> Images
        {
            get;
            set;
        }

        public string UrlKey
        {
            get;
            set;
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {

            PopulateJavascriptIncludesAndVariables();

            this.header.Text = this.Header;
            this.subheader.Text = this.SubHeader;

            if (Images != null && Images.Count > 0)
            {
                imageRepeater.DataSource = Images;
                imageRepeater.DataBind();
            }

        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    ArticlesMediaJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.ArticleMediaControl)
                );
            this.clientJavaScript.RegisterClientScriptVariable(
                    SelectMediaToDeleteJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Media.Remove.SelectAtleastOne")
                );
            this.clientJavaScript.RegisterClientScriptVariable(
                    RemoveMultiMediaConfirmationJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Media.Remove.MultiImages")
                );
            this.clientJavaScript.RegisterClientScriptVariable(
                    RemoveMultiMediaSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Media.Remove.MultiImages.Success")
                );            
        }

        #endregion

    }
}
