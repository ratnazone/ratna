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
    using Jardalu.Ratna.Web.UI.Snippets;
    

    #endregion

    public partial class FormFieldRowControl : SnippetControl
    {

        #region public properties

        public IList<Field> Fields
        {
            get;
            set;
        }

        public string FieldName
        {
            get;
            set;
        }

        public string FieldType
        {
            get;
            set;
        }

        public bool Required
        {
            get;
            set;
        }

        #endregion

        #region protected method

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateControl();
        }

        protected void RepeaterItemEventHandler(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Field field = e.Item.DataItem as Field;
                if (field != null)
                {
                    HtmlInputCheckBox checkbox = e.Item.FindControl("required") as HtmlInputCheckBox;
                    if (field.IsRequired)
                    {
                        checkbox.Checked = true;
                    }
                }

            }
        }

        #endregion


        #region public methods

        public override void PopulateControl()
        {
            // snippet control gets done without the field populating.
            // generate fields if needed
            GenerateFieldsIfUndefined();

            if (Fields != null)
            {
                this.repeater.DataSource = this.Fields;
                this.repeater.DataBind();
            }
        }

        #endregion

        #region private methods

        private void GenerateFieldsIfUndefined()
        {
            FieldType fieldtype = Core.FieldType.String;

            if (!Enum.TryParse<FieldType>(this.FieldType, out fieldtype))
            {
                //default to string.
                fieldtype = Core.FieldType.String;
            }

            if (!string.IsNullOrEmpty(this.FieldName))
            {
                Field field = new Field() { FieldType = fieldtype, Name = this.FieldName, IsRequired = Required };
                List<Field> fields = new List<Field>(1);
                fields.Add(field);

                this.Fields = fields;
            }
        }

        #endregion

    }
}
