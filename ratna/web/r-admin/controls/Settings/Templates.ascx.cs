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


namespace Jardalu.Ratna.Web.Admin.controls.Settings
{
    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Web.Templates;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class Templates : System.Web.UI.UserControl
    {
        #region private fields

        private string TemplatesListJavascriptKey = "controls.templatelist.js";
        private string DeleteTemplateConfirmJsVariable = "L_DeleteTemplateConfirmation";
        private string DeleteTemplateSuccessJsVariable = "L_DeleteTemplateSuccess";

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateJavascriptIncludesAndVariables();

            this.addnew.HRef = Utility.ResolveUrl(Constants.Urls.Settings.EditTemplateUrl);

            IList<Template> templates = TemplatePlugin.Instance.GetTemplates();

            if (templates == null || templates.Count == 0)
            {
                this.repeater.Visible = false;
            }
            else
            {
                this.repeater.DataSource = templates;
                this.repeater.DataBind();
                this.none.Visible = false;
            }
        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    TemplatesListJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.TemplateListControl)
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                DeleteTemplateConfirmJsVariable,
                ResourceManager.GetLiteral("Admin.Templates.Delete.Confirmation")
            );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteTemplateSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Templates.Delete.Success")
                );
        }

        #endregion
    }
}
