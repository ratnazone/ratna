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

    using Jardalu.Ratna.Web.Plugins;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class autopathscontrol : System.Web.UI.UserControl
    {
        #region private fields

        private string AutoPathListJavascriptKey = "controls.autopathslist.js";
        private string DeleteAutoPathConfirmJsVariable = "L_DeleteAutoPathConfirmation";
        private string DeleteAutoPathSuccessJsVariable = "L_DeleteAutoPathSuccess";

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateJavascriptIncludesAndVariables();

            this.addnew.HRef = Utility.ResolveUrl(Constants.Urls.Settings.EditAutoPathUrl);

            IList<CollectionPath> paths = CollectionPathPlugin.Instance.GetPaths();

            if (paths != null && paths.Count > 0)
            {
                this.repeater.DataSource = paths;
                this.repeater.DataBind();
                this.none.Visible = false;
            }
            else
            {
                this.repeater.Visible = false;
            }
        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    AutoPathListJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.AutoPathListControl)
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                DeleteAutoPathConfirmJsVariable,
                ResourceManager.GetLiteral("Admin.Templates.Delete.Confirmation")
            );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteAutoPathSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Templates.Delete.Success")
                );
        }

        #endregion

    }
}
