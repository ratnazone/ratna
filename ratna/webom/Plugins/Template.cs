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
namespace Jardalu.Ratna.Web.Templates
{

    #region using

    using System;
    using System.Runtime.Serialization;

    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Core;

    #endregion

    [DataContract]
    public class Template : PluginData
    {

        #region private fields

        [DataMember]
        private string name;
        [DataMember]
        private string templatePath;
        [DataMember]
        private string masterFileName;
        [DataMember]
        private string urlPath;
        [DataMember]
        private bool isActivated;

        public const string KeyName = "Template";

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(Template));

        #endregion

        #region ctor

        public Template()
        {
            this.Key = KeyName;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Only a unique template path defines a template.
        /// </summary>
        public override string Id
        {
            get
            {
                return this.TemplatePath;
            }
            set
            {
                // just ignore as Id cannot be set.
            }
        }

        /// <summary>
        /// TemplatePath is the Id for Template.
        /// </summary>
        public string TemplatePath
        {
            get { return this.templatePath; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.templatePath = value;
                this.IsDirty = true;
            }
        }

        public string UrlPath
        {
            get { return this.urlPath; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.urlPath = value;
                this.IsDirty = true;
            }
        }

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
                this.IsDirty = true;
            }
        }

        public string MasterFileName
        {
            get { return this.masterFileName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.masterFileName = value;
                this.IsDirty = true;
            }
        }

        public bool IsActivated
        {
            get { return this.isActivated; }
            set { this.isActivated = value; this.IsDirty = true; }
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
                    string.IsNullOrEmpty(MasterFileName) ||
                    string.IsNullOrEmpty(TemplatePath) ||
                    string.IsNullOrEmpty(UrlPath))
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public override void CopySpecific(ISerializableObject serialzableObject)
        {
            Template template = serialzableObject as Template;

            if (template != null)
            {
                this.Name = template.Name;
                this.IsActivated = template.IsActivated;
                this.MasterFileName = template.MasterFileName;
                this.TemplatePath = template.TemplatePath;
                this.UrlPath = template.UrlPath;
            }
        }

        #endregion

    }

}
