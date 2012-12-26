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

namespace Jardalu.Ratna.Core.Media
{
    #region using
    
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    using Jardalu.Ratna.Core.Tags;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Resource;

    #endregion

    [DataContract]
    public abstract class BaseMedia : TagResource, ISerializableObject
    {
        #region protected fields

        protected long id;
        private string url;
        private string name;

        [DataMember]
        private string description;

        protected string rawData;

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(BaseMedia));

        public const string MediaTagKey = "0a763c47-4dfa-4525-9817-64c2141a8489";

        #endregion

        #region ctor

        public BaseMedia()
        {
            //make sure the tag is registered
            RegisterTagKey(new Guid(TagKey));
        }

        #endregion

        #region public properties

        public virtual string TagKey
        {
            get
            {
                return MediaTagKey;
            }
        }

        public MediaType MediaType
        {
            get;
            set;
        }

        public long Id 
        {
            get { return this.id; }
            set { if (value <= 0) throw new ArgumentException("Id"); this.id = value; }
        }

        public string Url 
        {
            get { return this.url; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                // url must be a valid url
                if (!Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute))
                {
                    throw new MessageException(ResourceManager.GetMessage(CommonMessages.InvalidUrl));
                }

                this.url = value;
            }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Description
        {
            get { return this.description; }
            set { this.description = value; this.IsDirty = true; }
        }

        public DateTime CreatedDate
        {
            get;
            set;
        }

        public DateTime LastModifiedDate
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
                        BaseMedia ba = Serializer.ReadObject(reader) as BaseMedia;
                        this.CopySpecific(ba);
                    }

                }
                catch (InvalidOperationException)
                {
                    //invalid deserialization, just ignore.
                }

                IsDirty = false;

            }
        }

        public virtual void CopySpecific(ISerializableObject media)
        {
        }

        public void Copy(BaseMedia media)
        {
            if (media == null)
            {
                throw new ArgumentNullException("media");
            }

            this.Id = media.Id;
            this.MediaType = media.MediaType;
            this.Name = media.Name;
            this.Description = media.Description;
            this.Owner = media.Owner;
            this.Url = media.Url;

            this.InitializeTags(media.Tags);
        }

        #endregion

        #region public methods

        public void Update()
        {
            if (string.IsNullOrEmpty(this.url))
            {
                throw new InvalidOperationException("url not specified");
            }

            if (string.IsNullOrEmpty(this.Name))
            {
                throw new InvalidOperationException("Empty name");
            }

            BaseMedia updated = MediaStore.Instance.Save(this);
            if (this.Id == 0)
            {
                this.Id = updated.Id;
            }
            if (Tags.Count > 0)
            {
                TagStore.Instance.AddTags(new Guid(TagKey), this.ResourceId, this.Tags);
            }
        }

        #endregion

    }

}
