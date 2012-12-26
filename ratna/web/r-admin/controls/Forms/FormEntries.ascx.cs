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
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Forms;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class FormEntries : ListViewControl
    {

        #region private fields

        private const string FormResponsesJavascriptKey = "forms.responseslist.js";
        private const string DeleteResponsesConfirmJsVariable = "L_DeleteFormResponsesConfirmation";
        private const string DeleteResponsesSuccessJsVariable = "L_DeleteFormResponsesSuccess";
        private const string SelectResponseToDeleteJsVairable = "L_SelectResponseToDelete";

        private const int displayColumnsCount = 5;


        private string formName;
        private Form form;
        private bool formLoaded;

        private ListControlParameters parameters = new ListControlParameters();

        private IList<FormEntry> responses = null;

        private bool loaded;
        private object syncRoot = new object();

        #endregion

        #region public properties

        public string FormName
        {
            get
            {
                return this.formName;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.formName = value;
            }
        }

        public ListControlParameters Parameters
        {
            get
            {
                return this.parameters;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                parameters = value;
            }
        }

        #endregion

        #region private properties

        private Form Form
        {
            get
            {
                if (!formLoaded && !string.IsNullOrEmpty(FormName))
                {
                    lock (this)
                    {
                        if (!formLoaded)
                        {
                            FormsPlugin.Instance.TryRead(FormName, out form);
                            formLoaded = true;
                        }
                    }
                }

                return form;
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


                        // load the responses
                        responses = FormEntryPlugin.Instance.GetFormResponses(Form.Name,
                                    parameters.Start, parameters.Count, out total);


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
            LoadData();

            if (Form != null)
            {
                this.header.Text = string.Format("{0} ({1})", Form.DisplayName, Form.Name);
            }

            RenderHeaders();
            RenderEntries();

        }

        protected void RepeaterItemEventHandler(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                FormEntry entry = e.Item.DataItem as FormEntry;

                if (entry != null)
                {
                    if (Form != null)
                    {
                        //edit image link
                        HtmlAnchor editAnchor = e.Item.FindControl("editentryanchor") as HtmlAnchor;
                        if (editAnchor != null)
                        {
                            editAnchor.HRef = string.Format(Constants.Urls.Forms.EntryUrlWithKey, form.Name, entry.UId);
                        }

                        //find the tr
                        HtmlTableRow responsetr = e.Item.FindControl("responsetr") as HtmlTableRow;

                        int count = 0;
                        foreach (Field field in Form.Fields)
                        {

                            if (count >= displayColumnsCount)
                            {
                                break;
                            }

                            Data data = entry.GetFieldData(field);
                            if (data != null && data.Value != null)
                            {
                                HtmlTableCell cell = new HtmlTableCell();
                                cell.InnerText = data.Value.ToString();
                                responsetr.Cells.Add(cell);
                            }
                            else
                            {
                                // data is not found.
                                HtmlTableCell cell = new HtmlTableCell();
                                cell.InnerText = string.Empty;
                                responsetr.Cells.Add(cell);
                            }

                            count++;
                        }

                        //add the date column
                        HtmlTableCell dateCell = new HtmlTableCell();
                        dateCell.InnerText = Utility.FormatConciseDate(entry.CreatedTime);
                        responsetr.Cells.Add(dateCell);
                    }
                }
            }
        }

        #endregion

        #region private methods


        private void RenderHeaders()
        {
            RenderFormHead(Form);

            // add a created date column
            HtmlTableCell createdDateCell = new HtmlTableCell("th");
            createdDateCell.InnerText = ResourceManager.GetLiteral("Admin.Common.CreatedDate");
            createdDateCell.Style.Add("width", "80px");
            headtr.Cells.Add(createdDateCell);
        }

        private void RenderFormHead(Form form)
        {
            IList<Field> fields = form.Fields;
            if (fields != null && fields.Count > 0)
            {
                int count = 0;

                foreach (Field field in fields)
                {

                    if (count >= displayColumnsCount)
                    {
                        break;
                    }

                    HtmlTableCell cell = new HtmlTableCell("th");
                    cell.InnerText = field.Name;
                    headtr.Cells.Add(cell);

                    count++;
                }


            }
        }

        private void RenderEntries()
        {

            if (responses == null || responses.Count == 0)
            {
                this.none.Visible = true;
                this.controlsTR.Visible = false;
            }
            else
            {
                // display the values
                this.repeater.DataSource = responses;
                this.repeater.DataBind();
            }
        }

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    FormResponsesJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.FormResponsesControl)
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                DeleteResponsesConfirmJsVariable,
                ResourceManager.GetLiteral("Admin.Forms.Responses.Delete.Confirmation")
            );

            this.clientJavaScript.RegisterClientScriptVariable(
                    DeleteResponsesSuccessJsVariable,
                    ResourceManager.GetLiteral("Admin.Forms.Responses.Delete.Success")
                );

            this.clientJavaScript.RegisterClientScriptVariable(
                    SelectResponseToDeleteJsVairable,
                    ResourceManager.GetLiteral("Admin.Forms.Responses.Delete.SelectAtleastOne")
                );

        }

        #endregion

    }
}
