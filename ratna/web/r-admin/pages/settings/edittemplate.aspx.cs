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
namespace Jardalu.Ratna.Web.Admin.pages.settings
{

    #region using

    using System;

    using Jardalu.Ratna.Web.Templates;
    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class edittemplate : SettingsBasePage
    {
        #region private fields

        private const string TemplateMustBeZipJsVariable = "L_TemplateUploadMustBeZip";

        #endregion


        #region public properties

        public string Id
        {
            get
            {
                return Request["Id"];
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();

            this.menu.Selected = ResourceManager.GetLiteral("Admin.Templates.Title");

            // check if this is a new template add or
            // just an edit to an existing template.
            if (!string.IsNullOrEmpty(Id))
            {
                this.Title = ResourceManager.GetLiteral("Admin.Templates.Edit.Header");
                this.headerLiteral.Text = ResourceManager.GetLiteral("Admin.Templates.Edit.Header");
                DisplayForEditTemplate(Id);
            }
            else
            {
                this.Title = ResourceManager.GetLiteral("Admin.Templates.Upload.Header");
                this.headerLiteral.Text = ResourceManager.GetLiteral("Admin.Templates.Upload.Header");
                DisplayForNewTemplate();
            }

            foreach (Tuple<string, string> tuple in GetMenuItems())
            {
                this.menu.AddMenu(tuple.Item1, tuple.Item2);
            }
        }

        #endregion

        #region private methods

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = Constants.Navigation.Settings;
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Settings"), Constants.Urls.Settings.Url);
                if (string.IsNullOrEmpty(Id))
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.New.Template"), Constants.Urls.Settings.EditTemplateUrl);
                }
                else
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Edit.Template"), Constants.Urls.Settings.EditTemplateUrlWithKey + Id);
                }
            }
        }

        private void DisplayForNewTemplate()
        {
            this.errorDiv.Visible = false;
            this.uid.Value = Guid.Empty.ToString();

            this.template_form.Visible = false;
            this.Title = ResourceManager.GetLiteral("Admin.Breadcrumb.New.Template");

            this.clientJavaScript.RegisterClientScriptVariable(
                    TemplateMustBeZipJsVariable,
                    ResourceManager.GetLiteral("Admin.Templates.Upload.MustBeZip")
                );
        }

        private void DisplayForEditTemplate(string templateId)
        {
            this.Title = ResourceManager.GetLiteral("Admin.Breadcrumb.Edit.Template");
            template_upload_form.Visible = false;

            Template template = TemplatePlugin.Instance.GetTemplate(templateId);
            if (template != null)
            {
                this.uid.Value = template.UId.ToString();
                this.name.Value = template.Name;
                this.urlPath.Value = template.UrlPath;
                this.templatePath.Value = template.TemplatePath;
                this.templatePath.Disabled = true;
                this.masterFile.Value = template.MasterFileName;

                if (template.IsActivated)
                {
                    this.activatedTemplate.Checked = true;
                }
                else
                {
                    this.deactivatedTemplate.Checked = true;
                }

                this.errorDiv.Visible = false;
            }
            else
            {
                this.content.Visible = false;
            }
        }

        #endregion

    }
}
