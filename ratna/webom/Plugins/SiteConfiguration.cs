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
    using Jardalu.Ratna.Web.Plugins;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Utilities;

    #endregion

    [DataContract]
    public class SiteConfiguration : PluginData
    {

        #region private fields

        private static Logger logger;

        internal const string ConfigKey = "siteconfig";

        private static object syncRoot = new object();

        [DataMember]
        private string locale = "en-us";

        [DataMember]
        private bool defaultPageStatic = true;

        [DataMember]
        private bool commentModeration;

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(SiteConfiguration));

        #endregion

        #region ctor

        static SiteConfiguration()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public SiteConfiguration()
        {
            
        }

        public SiteConfiguration(int siteId)
        {
            this.Id = siteId.ToString();
            this.Key = ConfigKey; 
            
        }

        #endregion

        #region public properties

        public string Locale
        {
            get
            {
                return this.locale;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                // check if the value has really changed
                if (!value.Equals(this.locale))
                {
                    this.locale = value;
                    this.IsDirty = true;

                    //reset the resource manager
                    ResourceManager.SetSiteLocale(this.locale);
                }
            }
        }

        public bool IsCommentModerationOn
        {
            get { return commentModeration; }
            set
            {
                if (this.commentModeration != value)
                {
                    this.commentModeration = value;
                    this.IsDirty = true;
                }
            }
        }

        public bool IsDefaultPageStatic
        {
            get
            {
                return defaultPageStatic;
            }
            set
            {
                this.defaultPageStatic = value;
                this.IsDirty = true;
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
            SiteConfiguration c = serializableObject as SiteConfiguration;

            if (c != null)
            {
                this.Locale = c.Locale;
                this.IsDefaultPageStatic = c.IsDefaultPageStatic;
                this.IsCommentModerationOn = c.IsCommentModerationOn;
            }
        }

        #endregion

        #region public methods

        public static SiteConfiguration Read()
        {

            int siteId = WebContext.Current.Site.Id;
            SiteConfiguration configuration = SiteConfigurationPlugin.Read(siteId);

            if (configuration == null)
            {
                logger.Log(LogLevel.Info, "SiteConfiguraiton does not exist for SiteId : {0}. Saving one", siteId);
                configuration = new SiteConfiguration(siteId);
                SiteConfigurationPlugin.Instance.Save(configuration);
            }

            return configuration;
        }

        public void Update()
        {
            SiteConfigurationPlugin.Update(this);
        }

        #endregion

    }

}
