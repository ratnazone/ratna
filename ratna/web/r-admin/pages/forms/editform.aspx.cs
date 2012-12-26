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
namespace Jardalu.Ratna.Web.Admin.pages.forms
{
    #region using

    using System;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Forms;
    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class editform : System.Web.UI.Page
    {
        #region private fields

        private Form form;
        private bool loaded;

        private const string DeleteFieldConfirmJsVariable = "L_DeleteFieldConfirmation";
        private const string DeleteFieldSuccessJsVariable = "L_DeleteFieldSuccess";

        #endregion

        #region private properties

        private string FormName
        {
            get
            {
                return Request["form"];
            }
        }

        private new Form Form
        {
            get
            {
                if (!loaded)
                {
                    lock (this)
                    {
                        if (!loaded)
                        {
                            if (!string.IsNullOrEmpty(this.FormName))
                            {
                                FormsPlugin.Instance.TryRead(this.FormName, out form);
                            }

                            loaded = true;
                        }
                    }
                }

                return form;
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();
            PopulateJavascriptIncludesAndVariables();

            //set the header
            if (this.Form == null)
            {
                //new form
                this.headerLiteral.Text = ResourceManager.GetLiteral("Admin.Forms.AddNew");
                this.Title = this.headerLiteral.Text;
            }
            else
            {
                RenderForEditForm();
                this.Title = this.Form.DisplayName;
            }

            PopulateFieldTypes();

        }

        #endregion

        #region private methods

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = Constants.Navigation.Forms;
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Forms"), Constants.Urls.Forms.Url);

                if (Form == null)
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Forms.AddNew"), Constants.Urls.Forms.EditUrl);
                }
                else
                {
                    breadcrumb.Add(Form.DisplayName, Constants.Urls.Forms.EditUrlWithKey + Form.Name);
                }
            }
        }

        private void PopulateJavascriptIncludesAndVariables()
        {

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteFieldConfirmJsVariable,
                    ResourceManager.GetLiteral("Admin.Forms.Field.Delete.Confirmation")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteFieldSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Forms.Field.Delete.Success")
                );
        }

        private void RenderForEditForm()
        {
            this.headerLiteral.Text = this.Form.DisplayName;

            this.name.Value = this.Form.Name;
            this.name.Disabled = true;
            this.uid.Value = this.Form.UId.ToString();

            this.displayname.Value = this.Form.DisplayName;

            this.formFieldRowControl.Fields = this.Form.Fields;
        }

        private void PopulateFieldTypes()
        {
            int index = 0;
            int selectedIndex = 0;

            foreach (string fieldtype in Enum.GetNames(typeof(FieldType)))
            {
                if (fieldtype == FieldType.String.ToString())
                {
                    selectedIndex = index;
                }

                this.newfieldselect.Items.Add(fieldtype);
                index++;
            }

            this.newfieldselect.SelectedIndex = selectedIndex;
        }

        #endregion
    }
}
