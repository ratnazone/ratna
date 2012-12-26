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
    public class NotificationConfiguration : PluginData
    {

        #region private fields

        private static Logger logger;

        internal const string ConfigKey = "notificationconfig";

        private static object syncRoot = new object();

        [DataMember]
        private string smtpAddress;

        [DataMember]
        private string fromAddress;

        [DataMember]
        private string smtpUserName;

        [DataMember]
        private string smtpPassword;

        [DataMember]
        private string notificationEmailAddress;

        [DataMember]
        private bool notifyOnComment;

        [DataMember]
        private bool notifyOnFormResponse;

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(NotificationConfiguration));

        #endregion

        #region ctor

        static NotificationConfiguration()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public NotificationConfiguration()
        {
            
        }

        public NotificationConfiguration(int siteId)
        {
            this.Id = siteId.ToString();
            this.Key = ConfigKey; 
        }

        #endregion

        #region public properties

        public string NotifyToEmail
        {
            get
            {
                return notificationEmailAddress;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && !Utility.IsValidEmail(value))
                {
                    throw new ArgumentException("invalid email");
                }

                notificationEmailAddress = value;
                IsDirty = true;
            }
        }

        public string SmtpAddress
        {
            get
            {
                return smtpAddress;
            }
            set
            {
                smtpAddress = value;
                IsDirty = true;
            }
        }

        public string FromAddress
        {
            get
            {
                return fromAddress;
            }
            set
            {

                fromAddress = value;
                IsDirty = true;
            }
        }

        public string SmtpUserName
        {
            get
            {
                return smtpUserName;
            }
            set
            {
                smtpUserName = value;
                IsDirty = true;
            }
        }

        public string SmtpPassword
        {
            get
            {
                return smtpPassword;
            }
            set
            {
                smtpPassword = value;
                IsDirty = true;
            }
        }

        public bool NotifyOnComment
        {
            get { return notifyOnComment; }
            set
            {
                if (this.notifyOnComment != value)
                {
                    this.notifyOnComment = value;
                    this.IsDirty = true;
                }
            }
        }

        public bool NotifyOnFormResponse
        {
            get { return notifyOnFormResponse; }
            set
            {
                if (this.notifyOnFormResponse != value)
                {
                    this.notifyOnFormResponse = value;
                    this.IsDirty = true;
                }
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
            NotificationConfiguration c = serializableObject as NotificationConfiguration;

            if (c != null)
            {
                this.SmtpAddress = c.SmtpAddress;
                this.SmtpUserName = c.SmtpUserName;
                this.SmtpPassword = c.SmtpPassword;
                this.FromAddress = c.FromAddress;
                this.NotifyToEmail = c.NotifyToEmail;
                this.NotifyOnComment = c.NotifyOnComment;
                this.NotifyOnFormResponse = c.NotifyOnFormResponse;
            }
        }

        #endregion

        #region public methods

        public static NotificationConfiguration Read()
        {

            int siteId = WebContext.Current.Site.Id;
            NotificationConfiguration configuration = NotificationConfigurationPlugin.Instance.Read(siteId);
            
            if (configuration == null)
            {
                logger.Log(LogLevel.Info, "NotificationConfiguration does not exist for SiteId : {0}. Saving one", siteId);
                configuration = new NotificationConfiguration(siteId);
                NotificationConfigurationPlugin.Instance.Save(configuration);
            }

            return configuration;
        }

        public void Update()
        {
            NotificationConfigurationPlugin.Update(this);
        }

        #endregion

    }

}
