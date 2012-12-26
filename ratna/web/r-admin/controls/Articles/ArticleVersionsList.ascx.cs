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
    using System.Data;

    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class ArticleVersionsList : System.Web.UI.UserControl
    {
        #region private fields

        private const string DeleteArticleVersionConfirmJsVariable = "L_DeleteArticleVersionConfirmation";
        private const string RevertArticleVersionConfirmJsVariable = "L_RevertArticleVersionConfirmation";
        private const string DeleteArticleVersionSuccessJsVariable = "L_DeleteArticleVersionSuccess";

        #endregion

        #region public properties

        public DataTable VersionsTable
        {
            get;
            set;
        }

        protected string VersionAppender
        {
            get
            {
                return Constants.FetchVersionIdentifier;
            }
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

            if (VersionsTable != null)
            {
                versionsRepeater.DataSource = VersionsTable;
                versionsRepeater.DataBind();
            }
        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteArticleVersionConfirmJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.DeleteVersion.Confirmation")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    RevertArticleVersionConfirmJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.RevertVersion.Confirmation")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteArticleVersionSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Articles.Delete.Success")
                );
            

        }

        #endregion

    }
}
