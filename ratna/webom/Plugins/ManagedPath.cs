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
namespace Jardalu.Ratna.Web.Plugins
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Core;

    #endregion

    /// <summary>
    /// 
    /// ManagedPath defines how a URL can be resolved.
    /// 
    /// For example : when the user fetches a url like ( http://server/gallery/dog ) 
    /// this may be internally resolved as http://server/mycode/gallery.aspx?galleryname=dog
    /// 
    /// For ASP.NET programmers --> ManagedPath internally uses "Page Routes"
    /// 
    /// </summary>
    [DataContract]
    public class ManagedPath : PluginData
    {

        #region private fields

        [DataMember]
        private string name;

        [DataMember]
        private string path;

        [DataMember]
        private string resolvedPath;

        [DataMember]
        private IDictionary<string, string> dictionary;

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(ManagedPath));

        public const string KeyName = "ManagedPath";

        #endregion

        #region ctor

        public ManagedPath()
        {
            this.Key = KeyName;
        }

        public ManagedPath(string name, string path, string resolvedPath): this()
        {
            
            #region arguments

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            if (string.IsNullOrEmpty(resolvedPath))
            {
                throw new ArgumentNullException("resolvedPath");
            }
            #endregion

            this.Name = name;
            this.Path = path;
            this.ResolvedPath = resolvedPath;
        }

        #endregion

        #region public properties

        public string Name
        {
            get { return this.name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.name = value;
            }
        }

        public string Path
        {
            get { return this.path; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.path = value;
                this.Id = Path;
            }
        }

        public string ResolvedPath
        {
            get { return this.resolvedPath; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.resolvedPath = value;
            }
        }

        public IDictionary<string, string> Defaults
        {
            get
            {
                if (this.dictionary == null)
                {
                    lock (this)
                    {
                        if (this.dictionary == null)
                        {
                            this.dictionary = new Dictionary<string, string>(1);
                        }
                    }
                }

                return this.dictionary;
            }
        }

        #endregion

        #region public methods

        public void AddDefault(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (value == null)
            {
                value = string.Empty;
            }

            Defaults.Add(name, value);
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
            ManagedPath path = serializableObject as ManagedPath;

            if (path != null)
            {
                this.Name = path.Name;
                this.Path = path.Path;
                this.ResolvedPath = path.ResolvedPath;

                foreach(string key in path.Defaults.Keys)
                {
                    this.AddDefault(key, path.Defaults[key]);
                }
            }
        }

        #endregion

    }

}
