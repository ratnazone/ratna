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
    public class Configuration : PluginData
    {

        #region private fields

        internal const string ConfigKey = "config";
        internal const string ConfigId = "root";

        private static object syncRoot = new object();
        private static Configuration instance;

        private static string[] systemPaths = new string[] { 
            "/r-admin",
            "/scripts",
            "/templates",
            "/bin",
            "/controls",
            "/external",
            "/images",
            "/pages"
        };

        private static string[] blockedPaths = new string[] {
            "/logs",
            "/bin",
            "/App_Data"
        };

        [DataMember]
        private bool loggingEnabled;

        [DataMember]
        private LogLevel loggingLevel = LogLevel.Info;

        private DirectoryInfo uploadFolderInfo;
        private static DataContractSerializer serializer = new DataContractSerializer(typeof(Configuration));

        private static int maxUploadSize = 0;
        private static bool maxUploadInitialized;

        #endregion

        #region ctor

        public Configuration()
        {
            this.Id = ConfigId;
            this.Key = ConfigKey;
        }

        #endregion

        #region public properties

        public static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            //instance = new Configuration();
                            //load from the plugindata.
                            instance = ConfigurationPlugin.Instance.Read();
                            if (instance == null)
                            {
                                // create a new one and save it
                                instance = new Configuration();
                                instance.Update();
                            }
                        }
                    }
                }

                return instance;
            }
        }

        public HttpServerUtility Server
        {
            set;
            get;
        }

        public DirectoryInfo UploadFolderInfo
        {
            get
            {
                if (uploadFolderInfo == null)
                {
                    lock (syncRoot)
                    {
                        if (uploadFolderInfo == null)
                        {
                            //create the folder if it does not exist
                            string uploadFolder = this.Server.MapPath(SitePaths.Content);
                            if (!Directory.Exists(uploadFolder))
                            {
                                uploadFolderInfo = Directory.CreateDirectory(uploadFolder);
                            }
                            else
                            {
                                uploadFolderInfo = new DirectoryInfo(uploadFolder);
                            }
                        }
                    }
                }

                return uploadFolderInfo;
            }
        }

        public string[] SystemPaths
        {
            get
            {
                return systemPaths;
            }
        }

        public string[] BlockedPaths
        {
            get
            {
                return blockedPaths;
            }
        }

        public bool IsLoggingOn
        {
            get { return loggingEnabled; }
            set
            {
                if (this.loggingEnabled != value)
                {
                    this.loggingEnabled = value;
                    this.IsDirty = true;
                }
            }
        }

        public LogLevel LoggingLevel
        {
            get { return loggingLevel; }
            set
            {
                this.loggingLevel = value;
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
            Configuration c = serializableObject as Configuration;

            if (c != null)
            {
                this.IsLoggingOn = c.IsLoggingOn;
                this.LoggingLevel = c.LoggingLevel;
            }
        }

        #endregion

        #region public methods

        public void Update()
        {
            ConfigurationPlugin.Instance.Update(this);
        }

        public static int GetMaxUploadSize()
        {
            if (!maxUploadInitialized)
            {
                lock (syncRoot)
                {
                    if (!maxUploadInitialized)
                    {
                        System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                        System.Web.Configuration.HttpRuntimeSection section = config.GetSection("system.web/httpRuntime") as System.Web.Configuration.HttpRuntimeSection;

                        if (section != null)
                        {
                            maxUploadSize = section.MaxRequestLength;
                        }

                        maxUploadInitialized = true;
                    }
                }
            }

            return maxUploadSize;
        }

        #endregion

    }

}
