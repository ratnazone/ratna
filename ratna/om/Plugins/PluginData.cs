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
namespace Jardalu.Ratna.Plugins
{
    #region using

    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    using Jardalu.Ratna.Core;

    #endregion

    /// <summary>
    /// For a PluginData, combination of both Key and Id is unique identifier.
    /// </summary>
    [DataContract]
    public class PluginData : ISerializableObject
    {

        #region private fields

        protected string key;
        protected string id;
        protected Guid uid;
        protected string rawData;

        protected DateTime createdTime;
        protected DateTime updatedTime;

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(PluginData));

        #endregion

        #region public properties

        /// <summary>
        /// Each PluginData must have a Key defined. If the Key is not defined, save will fail.
        /// 
        /// Key is used to group PluginData.
        /// </summary>
        public virtual string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.key = value;
            }
        }

        /// <summary>
        /// Each PluginData must have an Id defined. If the Id is not defined, save will fail.
        /// 
        /// Id is the idenfitier for PluginData.
        /// </summary>
        public virtual string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.id = value;
            }
        }

        /// <summary>
        /// An identifier to uniquely identify plugin data.
        /// </summary>
        public Guid UId
        {
            get
            {
                return uid;
            }
            set
            {
                this.uid = value;
            }
        }

        public DateTime CreatedTime
        {
            get
            {
                return this.createdTime;
            }
            set
            {
                this.createdTime = value;
            }
        }

        public DateTime UpdatedTime
        {
            get
            {
                return this.updatedTime;
            }
            set
            {
                this.updatedTime = value;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Tells if the plugin data valid
        /// </summary>
        /// <returns>true if valid</returns>
        public virtual bool IsValid()
        {
            bool isValid = false;

            if (!string.IsNullOrEmpty(Id) && 
                !string.IsNullOrEmpty(Key))
            {
                isValid = true;
            }

            return isValid;
        }

        #endregion

        #region ISerializableObject

        public virtual string RawData
        {
            get
            {
                return rawData;
            }
            set
            {
                rawData = value;
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
                        PluginData pluginData = Serializer.ReadObject(reader) as PluginData;
                        this.CopySpecific(pluginData);
                    }

                }
                catch (InvalidOperationException)
                {
                    //invalid deserialization, just ignore.
                }

                IsDirty = false;

            }
        }

        public virtual void CopySpecific(ISerializableObject serialzableObject)
        {
        }

        #endregion

    }

}
