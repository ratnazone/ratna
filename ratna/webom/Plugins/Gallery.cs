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
namespace Jardalu.Ratna.Web
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Web;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Plugins;

    #endregion


    /// <summary>
    /// 
    /// Represents a gallery ( collection of photos ). Gallery is identified by its path.
    /// 
    /// /gallery/world_cup_2011
    /// 
    /// Gallery's name, description etc are part of CollectionPath.
    /// </summary>
    [DataContract]
    public class Gallery : PluginData
    {
        #region private fields

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(Gallery));

        [DataMember]
        private string title = string.Empty;

        [DataMember]
        private string description = string.Empty;

        [DataMember]
        private string navigation = string.Empty;

        [DataMember]
        private string url = string.Empty;

        [DataMember]
        private IList<string> photos = new List<string>();

        public const string KeyName = "Gallery";

        #endregion

        #region ctor

        public Gallery()
        {
            this.Key = KeyName;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Only a unique path defines a collection path.
        /// </summary>
        public override string Id
        {
            get
            {
                return this.Url;
            }
            set
            {
                // just ignore as Id cannot be set.
            }
        }

        public string Url
        {
            get { return this.url; }
            set { this.url = value; this.IsDirty = true; }
        }

        public string Title
        {
            get { return this.title; }
            set { this.title = value; this.IsDirty = true; }
        }

        public string Navigation
        {
            get { return this.navigation; }
            set { this.navigation = value; this.IsDirty = true; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; this.IsDirty = true; }
        }

        public IList<string> Photos
        {
            get
            {
                return this.photos;
            }
        }

        #endregion

        #region plugindata specific

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

        public override void CopySpecific(ISerializableObject serializableObject)
        {
            Gallery c = serializableObject as Gallery;

            if (c != null)
            {
                this.Url = c.Url;
                this.Title = c.Title;
                this.Navigation = c.Navigation;
                this.Description = c.Description;

                if (c.photos != null)
                {
                    // add photos
                    foreach (string photo in c.photos)
                    {
                        this.photos.Add(photo);
                    }
                }
            }
        }

        public override bool IsValid()
        {
            bool isValid = base.IsValid();

            if (isValid)
            {
                // make sure the required parameters are present.
                if (string.IsNullOrEmpty(Url))
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public void Add(string photo)
        {
            if (!string.IsNullOrEmpty(photo) && 
                Uri.IsWellFormedUriString(photo, UriKind.RelativeOrAbsolute) &&
                !photos.Contains(photo))
            {
                photos.Add(photo);
                this.IsDirty = true;
            }
        }

        public void Delete(string photo)
        {
            if (!string.IsNullOrEmpty(photo) &&
                photos.Contains(photo))
            {
                photos.Remove(photo);
                this.IsDirty = true;
            }
        }

        #endregion
    }

}
