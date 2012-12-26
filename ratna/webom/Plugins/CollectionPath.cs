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
    using System.IO;
    using System.Runtime.Serialization;
    using System.Web;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Plugins;

    #endregion

    public enum CollectionType
    {
        BlogArticle,
        Photo
    }

    /// <summary>
    /// Represents a collection path. For a collection path, Ratna can auto generate a "collection page".
    /// 
    /// Collection pages can be of the following types
    ///     a) BlogArticles
    ///     b) Media
    ///     
    /// </summary>
    [DataContract]
    public class CollectionPath : PluginData
    {
        #region private fields

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(CollectionPath));

        [DataMember]
        private string title = string.Empty;

        [DataMember]
        private string navigation = string.Empty;

        [DataMember]
        private string path = string.Empty;

        [DataMember]
        private CollectionType collectionType = CollectionType.BlogArticle;

        [DataMember]
        private int pageSize = 4;

        public const string KeyName = "CollectionPath";

        #endregion

        #region ctor

        public CollectionPath()
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
                return this.Path;
            }
            set
            {
                // just ignore as Id cannot be set.
            }
        }

        public string Path
        {
            get { return this.path; }
            set { this.path = value; this.IsDirty = true; }
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

        public CollectionType CollectionType
        {
            get { return collectionType; }
            set { collectionType = value; this.IsDirty = true; }
        }

        public int PageSize
        {
            get { return this.pageSize; }
            set { this.pageSize = value; this.IsDirty = true; }
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
            CollectionPath c = serializableObject as CollectionPath;

            if (c != null)
            {
                this.Path = c.Path;
                this.Title = c.Title;
                this.Navigation = c.Navigation;
                this.CollectionType = c.CollectionType;
                this.PageSize = c.PageSize;
            }
        }

        public override bool IsValid()
        {
            bool isValid = base.IsValid();

            if (isValid)
            {
                // make sure the required parameters are present.
                if (string.IsNullOrEmpty(Path))
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        #endregion
    }

}
