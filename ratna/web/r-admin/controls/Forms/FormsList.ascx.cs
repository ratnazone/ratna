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
namespace Jardalu.Ratna.Web.Admin.controls.Forms
{
    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Core.Forms;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class FormsList : ListViewControl
    {

        #region private fields

        private const string FormsListJavascriptKey = "forms.formslist.js";
        private const string DeleteFormsConfirmJsVariable = "L_DeleteFormConfirmation";
        private const string DeleteFormsSuccessJsVariable = "L_DeleteFormSuccess";

        private IList<Form> forms = null;

        private bool loaded;
        private object syncRoot = new object();

        private ListControlParameters parameters = new ListControlParameters();

        #endregion

        #region public properties

        public ListControlParameters Parameters
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

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateJavascriptIncludesAndVariables();

            LoadData();

            if (forms != null && forms.Count > 0)
            {
                // populate the repeater
                this.repeater.DataSource = forms;
                this.repeater.DataBind();
            }
            else
            {
                // no comments
                this.repeater.Visible = false;
                this.none.Visible = true;
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

                        // load the forms
                        forms = FormsPlugin.Instance.GetForms(parameters.Query,
                                    parameters.Start, parameters.Count, out total);

                        // set the total records
                        this.TotalRecords = total;

                        loaded = true;
                    }
                }
            }
        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    FormsListJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.FormsListControl)
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                DeleteFormsConfirmJsVariable,
                ResourceManager.GetLiteral("Admin.Forms.Delete.Confirmation")
            );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteFormsSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Forms.Delete.Success")
                );
        }

        #endregion

    }
}
