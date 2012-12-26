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
namespace Jardalu.Ratna.Web.Admin.pages.apps
{

    #region using

    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Apps;    
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web.Admin.controls.Apps;
    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Resource;


    #endregion

    public partial class edit : System.Web.UI.Page
    {

        #region private fields


        private const string DeletingAppJsVariable = "L_DeletingApp";
        private const string DeletedAppJsVariable = "L_DeletedApp";
        private const string DeletionAppFailedJsVariable = "L_DeletionAppFailed";

        private const string ActivatingAppJsVariable = "L_ActivatingApp";
        private const string ActivatedAppJsVariable = "L_ActivatedApp";
        private const string ActivationAppFailedJsVariable = "L_ActivationAppFailed";


        private const string FieldWrapperFormat = @"<div>
                <span>{0}</span>
                <input type=""text"" name=""{0}"" id=""{1}"" value=""{2}""/>
            </div>";

        public const string CollectionFieldWrapperFormat = @"<div>
                <span>{0}</span>
                <input type=""text"" name=""{0}"" id=""{1}""/>
                <input type=""button"" value=""+"" id=""collection_{0}""/>
            </div>";

        #endregion


        #region public properties

        public long Id
        {
            get
            {
                long id = -1;

                if (Request["id"] != null)
                {
                    if (!Int64.TryParse(Request["id"], out id))
                    {
                        id = -1;
                    }
                }

                return id;
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            
            PopulateNavigationAndBreadCrumb();
            PopulateJavascriptIncludesAndVariables();

            App app = null;

            if (Id > 0)
            {
                app = AppStore.Instance.GetApp(Id);
                if (app != null)
                {
                    this.appname.Text = app.Name;
                    this.description.InnerText = app.Description;
                    this.applogo.Src = AppListItem.GetAppIconUrl(app);
                    this.appid.Value = app.Id.ToString();

                    //activate-deactivate button
                    if (app.IsEnabled)
                    {
                        deactivatebtn.Visible = true;
                        deactivatebtn.Value = ResourceManager.GetLiteral("Admin.Apps.Deactivate");
                    }
                    else
                    {
                        activatebtn.Visible = true;
                        activatebtn.Value = ResourceManager.GetLiteral("Admin.Apps.Activate");
                    }

                    if (app.Fields == null || app.Fields.Count == 0)
                    {
                        this.fieldsDiv.Visible = false;
                    }
                    else
                    {
                        // set the help display
                        this.propertiesLiteral.Text = ResourceManager.GetLiteral("Admin.Apps.AppProperties");
                        this.propertieshelp.InnerText = string.Format(ResourceManager.GetLiteral("Admin.Apps.AppPropertiesHelp"), app.Name);

                        // app fields display
                        foreach (Field field in app.Fields)
                        {
                            Control control = GetFieldControl(app, field);
                            fieldsInnerDiv.Controls.Add(control);
                        }
                    }
                }
            }

            if (app == null)
            {
                //error case
                this.error.Visible = true;
                this.content.Visible = false;
            }
        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeletingAppJsVariable,
                    ResourceManager.GetLiteral("Admin.Apps.Deleting")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeletedAppJsVariable,
                    ResourceManager.GetLiteral("Admin.Apps.Deleted")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeletionAppFailedJsVariable,
                    ResourceManager.GetLiteral("Admin.Apps.DeletionFailure")
                );
            
            this.clientJavaScript.RegisterClientScriptVariable(
                    ActivatingAppJsVariable,
                    ResourceManager.GetLiteral("Admin.Apps.Activating")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                   ActivatedAppJsVariable,
                   ResourceManager.GetLiteral("Admin.Apps.Activated")
               );

            this.clientJavaScript.RegisterClientScriptVariable(
                   ActivationAppFailedJsVariable,
                   ResourceManager.GetLiteral("Admin.Apps.ActivationFailure")
               );
        }

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = Constants.Navigation.Apps;
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Apps"), Constants.Urls.Apps.Url);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Apps.Edit"), string.Format(Constants.Urls.Apps.EditUrl, this.Id));
            }
        }

        private Control GetFieldControl(App app, Field field)
        {
            string fieldHtml = null;

            string fieldName = field.Name;
            string id = field.Name;

            if (field.IsRequired)
            {
                fieldName = string.Format("{0} (*)", fieldName);
            }

            if (field.IsCollection)
            {
                fieldHtml = string.Format(CollectionFieldWrapperFormat, fieldName, id);
            }
            else
            {
                string value = app.GetFieldValue(field.Name) as string;
                fieldHtml = string.Format(FieldWrapperFormat, fieldName, id, value);
            }
            
            return this.Page.ParseControl(fieldHtml);
        }

        #endregion

    }
}
