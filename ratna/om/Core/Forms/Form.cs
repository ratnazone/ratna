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
    using System.Text.RegularExpressions;

    using Jardalu.Ratna.Plugins;

    #endregion

    [DataContract]
    public class Form : PluginData
    {

        #region private members

        [DataMember]
        private string name;

        [DataMember]
        private string displayName;

        [DataMember]
        private IList<Field> fields = new List<Field>(3);

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(Form));

        public const string KeyName = "Form";

        #endregion

        #region ctor

        public Form()
        {
            this.Key = KeyName;
        }

        public Form(PluginData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.Key = data.Key;
            this.Id = data.Id;
            this.RawData = data.RawData;

            this.Name = data.Id;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Identifier for the form. Valid characters for name are 'azAZ09_'
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                Regex regex = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);
                if (!regex.IsMatch(value))
                {
                    throw new ArgumentException("value");
                }

                this.name = value;
                this.Id = this.name;

                this.IsDirty = true;
            }
        }

        /// <summary>
        /// Easy to understand the form name.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.displayName = value;
                this.IsDirty = true;
            }
        }

        public IList<Field> Fields
        {
            get
            {
                return this.fields;
            }
        }

        public IList<Field> RequiredFields
        {
            get
            {
                List<Field> required = new List<Field>();

                foreach (Field field in this.fields)
                {
                    if (field.IsRequired)
                    {
                        required.Add(field);
                    }
                }

                return required;
            }
        }

        public override string RawData
        {
            get
            {
                this.Prepare();
                return rawData;
            }
            set
            {
                rawData = value;
                this.Populate();
            }
        }

        public override DataContractSerializer Serializer
        {
            get
            {
                return serializer;
            }
        }

        #endregion

        #region public methods

        public override bool IsValid()
        {
            bool isValid = base.IsValid();

            if (isValid)
            {
                // make sure the required parameters are present.
                if (string.IsNullOrEmpty(Name) ||
                    string.IsNullOrEmpty(DisplayName))
                {
                    isValid = false;
                }
            }

            return isValid;
        }

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

        public void AddField(string name, FieldType type)
        {

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            AddField(new Field() { Name = name, FieldType = type } );
        }

        public void AddField(string name, FieldType type, bool required)
        {

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            AddField(new Field() { Name = name, FieldType = type, IsRequired = required });
        }

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

        public Field GetField(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            Field field = null;

            foreach (Field f in this.fields)
            {
                if (f.Name == name)
                {
                    field = f;
                    break;
                }
            }

            return field;
        }

        public override void CopySpecific(ISerializableObject serializableObject)
        {
            Form f = serializableObject as Form;

            if (f != null)
            {
                this.Name = f.Name;
                this.DisplayName = f.DisplayName;
                this.fields.Clear();
                foreach (Field field in f.fields)
                {
                    this.fields.Add(field);
                }
            }
        }

        #endregion


    }
}
