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
    using System.Text;
    using System.Web;
    using System.Web.UI.HtmlControls;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Forms;
    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Store;

    #endregion

    public partial class editentry : System.Web.UI.Page
    {
        #region private fields

        private Form form;
        private bool loaded;

        public const string IdPrefix = "___";

        #endregion

        #region public properties

        public string FormName
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

        public bool IsEdit
        {
            get
            {
                bool isEdit = false;

                if (UId != Guid.Empty)
                {
                    isEdit = true;
                }

                return isEdit;
            }
        }

        public Guid UId
        {
            get
            {
                Guid uid = Guid.Empty;
                if (!Guid.TryParse(Request["uid"], out uid))
                {
                    uid = Guid.Empty;
                }

                return uid;
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();
            SetActionPanels();

            //reset the uid and formname
            this.formname.Value = FormName;
            this.uid.Value = HttpUtility.HtmlEncode(UId.ToString());

            FormEntry entry = null;

            if (IsEdit)
            {
                this.Title = ResourceManager.GetLiteral("Admin.Forms.Entry.Edit");
                this.headerLiteral.Text = string.Format("{0} ({1})", Form.DisplayName, ResourceManager.GetLiteral("Admin.Forms.Entry.Edit"));

                //read the entry
                if (!PluginStore.Instance.TryRead<FormEntry>(FormEntryPlugin.Instance, UId, out entry))
                {
                    //not found.
                }
            }
            else
            {
                this.Title = ResourceManager.GetLiteral("Admin.Forms.Entry.New");
                this.headerLiteral.Text = string.Format("{0} ({1})", Form.DisplayName, ResourceManager.GetLiteral("Admin.Forms.Entry.New"));
            }

            PopulateEntryFields(entry);

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
            if (breadcrumb != null && 
                !string.IsNullOrEmpty(this.FormName))
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Forms"), Constants.Urls.Forms.Url);
                breadcrumb.Add(this.Form.DisplayName, Constants.Urls.Forms.ResponsesUrl + this.FormName);

                if (IsEdit)
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Forms.Entry.Edit"), Request.Url.ToString());
                }
                else
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Forms.Entry.New"), Request.Url.ToString());
                }
            }
        }

        private void SetActionPanels()
        {
            // new entries
            this.actionPanel.AddAction(
                    "/images/plus.png",
                    ResourceManager.GetLiteral("Admin.Forms.Entry.AddNew"),
                    string.Format(Constants.Urls.Forms.EntryUrlWithKey, FormName, Guid.Empty)
                );
        }

        private void PopulateEntryFields(FormEntry entry)
        {
            if (Form != null)
            {
                // get all the fields
                foreach (Field field in Form.Fields)
                {                   
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell namecell = new HtmlTableCell();
                    namecell.Style.Add("width","80px");
                    namecell.Style.Add("vertical-align", "top");
                    namecell.InnerText = field.Name;
                    HtmlTableCell entrycell = new HtmlTableCell();
                    entrycell.InnerHtml = GetFieldHtml(field, entry);
                    row.Cells.Add(namecell);
                    row.Cells.Add(entrycell);

                    this.entrybody.Controls.Add(row);
                }
            }
        }


        private string GetFieldHtml(Field field, FormEntry entry)
        {
            StringBuilder builder = new StringBuilder();
            string fieldValue = string.Empty;
            if (entry != null)
            {
                Data data = entry.GetFieldData(field);
                if (data != null)
                {
                    fieldValue = entry.GetFieldData(field).Value as string;
                }
            }

            if (field.FieldType == FieldType.MultiLine)
            {
                builder.Append(@"<textarea style=""width:600px;""");
            }
            else if (field.FieldType == FieldType.Html)
            {
                builder.Append(@"<textarea style=""width:600px;"" rows='8'");
            }
            else
            {                
                builder.Append("<input type=\"text\"");

                if (field.FieldType == FieldType.String || field.FieldType == FieldType.Url)
                {
                    builder.Append(@" style=""width:600px;""");
                }
                else if (field.FieldType == FieldType.DateTime)
                {
                    builder.Append(@" style=""width:100px;""");
                }
                else
                {
                    builder.Append(@" style=""width:300px;""");
                }

            }

            builder.AppendFormat(@" id=""{0}{1}"" ", IdPrefix, field.Name);


            if (field.FieldType == FieldType.MultiLine ||
                field.FieldType == FieldType.Html)
            {
                builder.AppendFormat(">{0}</textarea>", fieldValue);
            }
            else
            {

                string validate = GetValidateString(field);
                if (!string.IsNullOrEmpty(validate))
                {
                    builder.Append(validate);
                }

                builder.AppendFormat(@"value=""{0}""", fieldValue);
                builder.Append("/>");
            }

            return builder.ToString();
        }


        private string GetValidateString(Field field)
        {
            string validate = string.Empty;
            string vclasses = string.Empty;

            if (field.IsRequired)
            {
                vclasses = " required";
            }

            switch (field.FieldType)
            {
                case FieldType.Url:
                    vclasses += " url";
                    break;
                case FieldType.Email:
                    vclasses += " email";
                    break;
                case FieldType.Integer:
                    vclasses += " number";
                    break;
                case FieldType.DateTime:
                    vclasses += " date-pick";
                    break;

            }

            if (!string.IsNullOrEmpty(vclasses))
            {
                validate = string.Format(@"class=""{0}""", vclasses);
            }

            return validate;
        }

        #endregion

    }
}
