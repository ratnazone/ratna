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
namespace Jardalu.Ratna.Core.Apps
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Resource;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Store;

    #endregion

    /// <summary>
    /// App Definition
    /// </summary>
    [DataContract]
    [KnownType(typeof(List<object>))]
    public class App : ISerializableObject
    {
        #region private fields

        protected string rawData;
        
        [DataMember]
        private IList<Field> fields = new List<Field>();

        [DataMember]
        private IDictionary<string, object> fieldValues = new Dictionary<string, object>();

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(App));

        #endregion

        #region ctor

        public App()
        {
        }

        #endregion

        #region public properties

        /// <summary>
        /// Name of the App
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// App Publisher
        /// </summary>
        public string Publisher
        {
            get;
            set;
        }

        /// <summary>
        /// App Description
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// App Url
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Id is assigned by Ratna. Each time an app is installed, this Id
        /// will be different.
        /// </summary>
        public long Id
        {
            get;
            internal set;
        }

        /// <summary>
        /// UniqueId - Assigned by the developer
        /// </summary>
        public Guid UniqueId
        {
            get;
            set;
        }

        /// <summary>
        /// Scope of the App
        /// </summary>
        public AppScope Scope
        {
            get;
            set;
        }

        /// <summary>
        /// Version of the App
        /// </summary>
        public Version Version
        {
            get;
            set;
        }

        /// <summary>
        /// Installed location
        /// </summary>
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// Fields defined for the app.
        /// 
        /// For example, an app would want to know the zip code.
        /// One of the field will be named "Zip" with type "Integer"
        /// </summary>
        public IList<Field> Fields
        {
            get { return this.fields; }
        }

        /// <summary>
        /// App file associated for the App.
        /// </summary>
        public string File
        {
            get;
            internal set;
        }

        /// <summary>
        /// Entry point for the file
        /// </summary>
        public string FileEntry
        {
            get;
            internal set;
        }

        /// <summary>
        /// References used by the App.
        /// </summary>
        public string[] References
        {
            get;
            internal set;
        }

        /// <summary>
        /// Tells if the app has been enabled or not
        /// </summary>
        public bool IsEnabled
        {
            get;
            internal set;
        }

        /// <summary>
        /// Icon Url for App
        /// </summary>
        public string IconUrl
        {
            get;
            set;
        }

        #endregion


        #region ISerializableObject

        public virtual string RawData
        {
            get
            {
                Prepare();
                return rawData;
            }
            set
            {
                rawData = value;
                Populate();
            }
        }

        public virtual string Type
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        public virtual DataContractSerializer Serializer
        {
            get
            {
                return serializer;
            }
        }

        public bool IsDirty
        {
            get;
            set;
        }

        public virtual void Prepare()
        {
            if (IsDirty)
            {
                XmlWriterSettings ws = new XmlWriterSettings();
                ws.OmitXmlDeclaration = false;
                StringBuilder buffer = new StringBuilder();

                //real preparation
                using (XmlWriter writer = XmlWriter.Create(buffer, ws))
                {
                    Serializer.WriteObject(writer, this);
                }

                //set the rawdata finally
                rawData = buffer.ToString();

                IsDirty = false;
            }
        }

        public virtual void Populate()
        {
            if (!string.IsNullOrEmpty(this.rawData))
            {

                try
                {

                    //real preparation
                    using (XmlReader reader = XmlReader.Create(new StringReader(this.rawData)))
                    {
                        App app = Serializer.ReadObject(reader) as App;
                        this.CopySpecific(app);
                    }

                }
                catch (InvalidOperationException)
                {
                    //invalid deserialization, just ignore.
                }

                IsDirty = false;

            }
        }

        public virtual void CopySpecific(ISerializableObject serializableObject)
        {
            App app = serializableObject as App;

            if (app != null)
            {
                //fields
                this.fields.Clear();
                foreach (Field field in app.fields)
                {
                    this.fields.Add(field);
                }

                // field values
                this.fieldValues.Clear();
                foreach (string key in app.fieldValues.Keys)
                {
                    this.fieldValues[key] = app.fieldValues[key];
                }
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Activates or deactivates the App. For an app to be 
        /// activated, it must have all its required properties
        /// populated else the activation will fail.
        /// </summary>
        /// <param name="enable">true for activation</param>
        public void Activate(bool enable)
        {
            if (enable)
            {
                //check all the fields that are required have been supplied
                foreach (Field field in this.Fields)
                {
                    if (field.IsRequired &&
                        GetFieldValue(field.Name) == null)
                    {
                        // required field is missing
                        throw new MessageException(
                                AppsErrorCodes.RequiredFieldValueMissing,
                                ResourceManager.GetMessage(string.Format(AppsMessages.RequiredFieldValueMissing, field.Name))
                            );
                    }
                }
            }

            // activate/deactivate the app
            AppStore.Instance.Activate(this.Id, enable);
        }

        /// <summary>
        /// Sets the value of the field. This method checks if the correct value
        /// is being set for the fieldtype. For example, if the fieldType is an email type,
        /// this method will check that the supplied value is actually an email.
        /// </summary>
        /// <param name="field">Name of the field</param>
        /// <param name="value">Value of the field</param>
        public void SetFieldValue(string field, object value)
        {
            if (string.IsNullOrEmpty(field))
            {
                throw new ArgumentNullException("field");
            }

            Field appField = GetField(field);
            if (appField == null)
            {
                //no field found with the name
                throw new MessageException(AppsErrorCodes.PropertyNotFound,
                    ResourceManager.GetMessage(string.Format(AppsMessages.FieldNotFound, field)));
            }

            //make sure that the field type and the value matches.
            if (appField.IsRequired)
            {
                if ((value == null) ||
                    (appField.FieldType == FieldType.String && string.IsNullOrEmpty(value.ToString())))
                {
                    throw new MessageException(AppsErrorCodes.PropertyNotSupplied,
                        ResourceManager.GetMessage(string.Format(AppsMessages.FieldValueNotSupplied, appField.Name)));
                }
            }

            //check that the field and its value match
            if (!Jardalu.Ratna.Core.Forms.FormEntryPlugin.IsFieldTypeMatch(appField, value))
            {
                throw new MessageException(AppsErrorCodes.PropertyValueDoesnotMatchWithFieldType,
                        ResourceManager.GetMessage(string.Format(AppsMessages.FieldValueDoesnotMatchWithFieldType, appField.Name)));
            }

            //sanitize the url
            if (value != null &&
                appField.FieldType == FieldType.Url && 
                !string.IsNullOrEmpty(value.ToString()))
            {
                value = Utility.SanitizeUrl(value as string);
            }

            object storedValue = value;

            if (appField.IsCollection)
            {
                List<object> list = GetCollection(field);
                list.Add(value);
                storedValue = list;
            }

            //save the value
            fieldValues[field] = storedValue;
            this.IsDirty = true;
        }

        /// <summary>
        /// Returns the field value. Returns null in case the value is not set.
        /// </summary>
        /// <param name="field">Name of the field</param>
        /// <returns>Value of the field</returns>
        public object GetFieldValue(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                throw new ArgumentNullException("field");
            }

            Field appField = GetField(field);
            if (appField == null)
            {
                //no field found with the name
                throw new MessageException(AppsErrorCodes.PropertyNotFound,
                    ResourceManager.GetMessage(string.Format(AppsMessages.FieldNotFound, field)));
            }

            object returnValue = null;

            if (fieldValues.ContainsKey(field))
            {
                returnValue = fieldValues[field];
            }

            return returnValue;
        }

        /// <summary>
        /// Returns the field value. Returns null in case the value is not set.
        /// </summary>
        /// <param name="field">Name of the field</param>
        /// <returns>casted value of the field</returns>
        public object GetCastedFieldValue(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                throw new ArgumentNullException("field");
            }

            Field appField = GetField(field);
            if (appField == null)
            {
                //no field found with the name
                throw new MessageException(AppsErrorCodes.PropertyNotFound,
                    ResourceManager.GetMessage(string.Format(AppsMessages.FieldNotFound, field)));
            }

            object returnValue = null;

            if (fieldValues.ContainsKey(field))
            {
                returnValue = Jardalu.Ratna.Core.Forms.FormEntryPlugin.CastFieldType(appField, fieldValues[field]);
            }

            return returnValue;
        }

        /// <summary>
        /// Returns the Field with the field name if it exists in the App.
        /// If a field does not exist with the name, it returns null.
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <returns>The field</returns>
        public Field GetField(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            Field field = null;

            foreach (Field f in this.Fields)
            {
                if (f.Name == name)
                {
                    field = f;
                    break;
                }
            }

            return field;
        }

        /// <summary>
        /// Adds a new field to the app.
        /// </summary>
        /// <param name="field">Field</param>
        public void AddField(Field field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            if (!this.fields.Contains(field))
            {
                this.fields.Add(field);
                this.IsDirty = true;
            }
        }

        /// <summary>
        /// Removes a field from the App.
        /// </summary>
        /// <param name="name">Name of the field</param>
        public void RemoveField(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            Field field = new Field() { Name = name };
            this.fields.Remove(field);

            this.IsDirty = true;
        }

        /// <summary>
        /// Persists the app
        /// </summary>
        public void Update()
        {
            if (string.IsNullOrEmpty(this.Location))
            {
                throw new InvalidOperationException("Location not set");
            }

            AppStore.Instance.Save(this);

            if (IsDirty)
            {
                AppStore.Instance.SaveRawData(this);
            }
        }

        /// <summary>
        /// Tells if the app can be loaded as an abstract app.
        /// </summary>
        /// <returns>true if the app can be loaded as abstract app</returns>
        public bool IsAbstractApp()
        {
            return (Scope == AppScope.Comment ||
                Scope == AppScope.FormEntry ||
                Scope == AppScope.Image);
        }

        #endregion

        #region private fields

        private List<object> GetCollection(string field)
        {
            List<object> list = null;
            if (fieldValues.ContainsKey(field))
            {
                list = fieldValues[field] as List<object>;
            }
            else
            {
                list = new List<object>();
                fieldValues[field] = list;
            }

            return list;
        }

        #endregion

    }

}
