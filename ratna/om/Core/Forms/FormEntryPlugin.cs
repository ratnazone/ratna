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
namespace Jardalu.Ratna.Core.Forms
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Resource;

    #endregion

    public class FormEntryPlugin : SystemPlugin
    {
        #region private fields

        private static FormEntryPlugin instance = null;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        public FormEntryPlugin()
        {
            this.Name = "FormResponse";
            this.Id = new Guid("04c2417e-bc6e-411b-b129-e8352decd057");

            this.Register();
            this.Activate();
        }

        #endregion

        #region public properties

        public static FormEntryPlugin Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new FormEntryPlugin();
                        }
                    }
                }

                return instance;
            }
        }

        public override DataContractSerializer Serializer
        {
            get
            {
                return null;
            }
        }

        public override string Type
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        #endregion

        #region public methods

        public void Add(FormEntry response)
        {
            #region argument checking

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (!response.IsValid())
            {
                throw new InvalidOperationException("response is invalid");
            }

            #endregion

            //get the corresponding form
            Form form = FormsPlugin.Instance.Read(response.Form);

            if (form == null)
            {
                // response without form cannot be saved.
                throw new MessageException(FormsErrorCodes.NoFormFoundWithTheName, ResourceManager.GetMessage(FormMessages.FormNotFound));
            }

            // check to see that the input supplied adhere the rules of the form.
            foreach (Field field in form.Fields)
            {
                Data data = response.GetFieldData(field);

                if (field.IsRequired)
                {
                    // if the field value is required it must not be null
                    // and incase of string, must not be empty.
                    if ((data == null||data.Value == null) || 
                        (field.FieldType == FieldType.String && string.IsNullOrEmpty(data.Value.ToString())))
                    {
                        throw new MessageException(FormsErrorCodes.NotAllRequiredFieldsSupplied,
                            ResourceManager.GetMessage(FormMessages.NotAllFieldsSupplied));
                    }
                }

                bool fieldTypeMatch = true;

                if (data != null && data.Value != null)
                {
                    fieldTypeMatch = IsFieldTypeMatch(field, data.Value);
                }

                if (!fieldTypeMatch)
                {
                    //field's value does not match with the defined type.
                    throw new MessageException(FormsErrorCodes.FieldValueDoesnotMatchWithFieldType,
                        ResourceManager.GetMessage(FormMessages.FieldValueDoesnotMatchWithFieldType));
                }
            }

            // save the data
            try
            {
                // add the form
                PluginStore.Instance.Save(this, response);
            }
            catch (MessageException me)
            {
                if (me.ErrorNumber == PluginErrorCodes.IdAlreadyInUse)
                {
                    // throw with a new message
                    throw new MessageException(me.ErrorNumber, ResourceManager.GetMessage(FormMessages.ResponseIdInUse));
                }

                throw;
            }
        }

        public IList<FormEntry> GetFormResponses(string form)
        {
            #region argument checking

            if (string.IsNullOrEmpty(form))
            {
                throw new ArgumentNullException("form");
            }

            #endregion

            return PluginStore.Instance.Read<FormEntry>(this, form);
        }

        public IList<FormEntry> GetFormResponses(DateTime after)
        {
            return PluginStore.Instance.Read<FormEntry>(this, after);
        }

        public IList<FormEntry> GetFormResponses(string form, string field, object value)
        {
            if (string.IsNullOrEmpty(form))
            {
                throw new ArgumentNullException("form");
            }

            PluginDataQueryParameter parameter = new PluginDataQueryParameter();
            parameter.PropertyName = field;
            if (value == null)
            {
                parameter.PropertyValue = string.Empty;
            }
            else
            {
                parameter.PropertyValue = value.ToString();
            }

            int total;

            return PluginStore.Instance.ReadExact<FormEntry>(this, parameter, 0, 100, out total);
        }

        public IList<FormEntry> GetFormResponses(string form, int start, int count, out int total)
        {
            #region argument checking

            if ((start < 0) || (count < 0))
            {
                throw new ArgumentException("start or count");
            }

            if (string.IsNullOrEmpty(form))
            {
                throw new ArgumentNullException("form");
            }

            #endregion

            return PluginStore.Instance.Read<FormEntry>(this, form, start, count, out total);
        }

        public void Delete(IList<Guid> uids)
        {
            if (uids == null)
            {
                throw new ArgumentNullException("uids");
            }

            PluginStore.Instance.Delete(this, uids);
        }

        #endregion

        #region internal methods

        internal static bool IsFieldTypeMatch(Field field, object value)
        {
            bool fieldTypeMatch = true;

            if (value != null && value.ToString() != String.Empty)
            {
                //make sure the input data matches with the field definition
                switch (field.FieldType)
                {
                    case FieldType.String:
                        fieldTypeMatch = !string.IsNullOrEmpty(value.ToString());
                        break;
                    case FieldType.Email:
                        fieldTypeMatch = Utilities.Utility.IsValidEmail(value.ToString());
                        break;
                    case FieldType.Url:
                        fieldTypeMatch = Uri.IsWellFormedUriString(value.ToString(), UriKind.RelativeOrAbsolute);
                        break;
                    case FieldType.Integer:
                        int x;
                        fieldTypeMatch = Int32.TryParse(value.ToString(), out x);
                        break;
                    case FieldType.Float:
                        float f;
                        fieldTypeMatch = float.TryParse(value.ToString(), out f);
                        break;
                    case FieldType.Double:
                        double dd;
                        fieldTypeMatch = Double.TryParse(value.ToString(), out dd);
                        break;
                    case FieldType.DateTime:
                        DateTime d;
                        fieldTypeMatch = DateTime.TryParse(value.ToString(), out d);
                        break;
                }
            }

            return fieldTypeMatch;
        }

        internal static object CastFieldType(Field field, object value)
        {
            object casted = value;

            if (field != null && value != null)
            {
                string serialized = value as string;

                switch (field.FieldType)
                {
                    case FieldType.Integer :
                        int output;
                        if (Int32.TryParse(serialized, out output))
                        {
                            casted = output;
                        }
                        break;
                }
            }

            return casted;
        }

        #endregion

    }

}
