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
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Resource;
    using Jardalu.Ratna.Store;
    
    #endregion

    [DataContract]
    public abstract class Plugin : ISerializableObject
    {

        #region private fields

        protected string rawData;
        public const int MaxNameLength = 24;

        #endregion

        #region Public Properties

        public string Name
        {
            get;
            set;
        }

        public Guid Id
        {
            get;
            set;
        }

        public bool Active
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
                return this.rawData;
            }
            set
            {
                this.rawData = value;
            }
        }

        public abstract string Type
        {
            get;
        }

        public abstract DataContractSerializer Serializer
        {
            get;
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
                if (Serializer != null)
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
                }

                IsDirty = false;
            }
        }

        public virtual void Populate()
        {
            if (!string.IsNullOrEmpty(this.rawData))
            {
                if (Serializer != null)
                {
                    try
                    {

                        //real preparation
                        using (XmlReader reader = XmlReader.Create(new StringReader(this.rawData)))
                        {
                            Plugin plugin = Serializer.ReadObject(reader) as Plugin;
                            this.CopySpecific(plugin);
                        }

                    }
                    catch (InvalidOperationException)
                    {
                        //invalid deserialization, just ignore.
                    }
                }

                IsDirty = false;

            }
        }

        public virtual void CopySpecific(ISerializableObject plugin)
        {
        }

        #endregion

        #region Public Methods

        public void Register()
        {
            PluginStore.Instance.Register(this);
        }

        public void Activate()
        {
            PluginStore.Instance.Activate(this);
            this.Active = true;
        }

        public void Deactivate()
        {
            PluginStore.Instance.Deactivate(this);
            this.Active = false;
        }

        public void Save(PluginData data)
        {
            #region argument checking

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (!data.IsValid())
            {
                throw new InvalidOperationException("data is invalid");
            }

            #endregion

            EnsurePluginCorrectness();

            PluginStore.Instance.Save(this, data);
        }

        public void Delete(string key, string id)
        {

            #region argument checking

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            #endregion

            PluginStore.Instance.Delete(this, key, id);
        }

        #endregion

        #region Private Methods

        private void EnsurePluginCorrectness()
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new MessageException(ResourceManager.GetMessage(PluginMessages.NameNotSpecified));
            }

            if (Id == Guid.Empty)
            {
                throw new MessageException(ResourceManager.GetMessage(PluginMessages.IdNotSpecified));
            }
        }

        #endregion

    }

}
