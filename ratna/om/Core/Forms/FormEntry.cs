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

    #endregion

    [DataContract]
    public class FormEntry : PluginData
    {
        #region private fields

        [DataMember]
        private string form;

        [DataMember]
        private IList<Data> datum = new List<Data>(3);

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(FormEntry));

        #endregion

        #region ctor

        public FormEntry()
        {
        }

        public FormEntry(PluginData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.Key = data.Key;
            this.Id = data.Id;
            this.RawData = data.RawData;

            //Key and Form are same
            this.Form = data.Key;
        }

        #endregion

        #region public properties

        public string Form
        {
            get
            {
                return this.form;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                this.form = value;
                this.Key = this.Form;

                this.IsDirty = true;
            }
        }

        public string DisplayValue
        {
            get
            {
                string display = string.Empty;

                // returns the first item
                if (datum != null && datum.Count > 0)
                {
                    Data data = datum[0];
                    display = data.Value.ToString();
                }

                return display;
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
                if (string.IsNullOrEmpty(Form))
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public void Add(Data data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            Clean(data);

            datum.Add(data);
            this.IsDirty = true;
        }

        public Data GetFieldData(string fieldName)
        {
            #region argument checking
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }
            #endregion

            Data returnData =null;

            foreach (Data data in datum)
            {
                if (data.Name == fieldName)
                {
                    returnData = data;
                    break;
                }
            }

            return returnData;
        }

        public Data GetFieldData(Field field)
        {
            #region argument checking
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            #endregion

            return GetFieldData(field.Name);
        }

        public override void CopySpecific(ISerializableObject serializableObject)
        {
            FormEntry r = serializableObject as FormEntry;

            if (r != null)
            {
                this.Form = r.Form;
                this.datum.Clear();
                foreach (Data data in r.datum)
                {
                    this.datum.Add(data);
                }
            }
        }

        #endregion

        #region private methods

        private void Clean(Data data)
        {
            if (data == null)
            {
                return;
            }

            for(int i=0;i<datum.Count;i++)
            {
                Data d = datum[i];
                if (d.Name == data.Name)
                {
                    datum.RemoveAt(i);
                }
            }
        }

        #endregion

    }

}
