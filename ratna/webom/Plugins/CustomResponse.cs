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
    public class CustomResponse : PluginData
    {

        #region private fields

        private static Logger logger;

        internal const string ConfigKey = "customresponse";

        private static object syncRoot = new object();

        [DataMember]
        private string pageNotFound = string.Empty;

        [DataMember]
        private string internalServerError = string.Empty;

        [DataMember]
        private string otherErrors = string.Empty;

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(CustomResponse));

        #endregion

        #region ctor

        static CustomResponse()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public CustomResponse()
        {
            
        }

        public CustomResponse(int siteId)
        {
            this.Id = siteId.ToString();
            this.Key = ConfigKey; 
        }

        #endregion

        #region public properties

        public string PageNotFound
        {
            get
            {
                return this.pageNotFound;
            }
            set
            {
                this.pageNotFound = value;
                this.IsDirty = true;
            }
        }

        public string InteralServerError
        {
            get
            {
                return this.internalServerError;
            }
            set
            {
                this.internalServerError = value;
                this.IsDirty = true;
            }
        }

        public string OtherErrors
        {
            get
            {
                return this.otherErrors;
            }
            set
            {
                this.otherErrors = value;
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
            CustomResponse c = serializableObject as CustomResponse;

            if (c != null)
            {
                this.PageNotFound = c.PageNotFound;
                this.InteralServerError = c.InteralServerError;
                this.OtherErrors = c.OtherErrors;
            }
        }

        #endregion

        #region public methods

        public static CustomResponse Read()
        {

            int siteId = WebContext.Current.Site.Id;
            CustomResponse configuration = CustomResponsePlugin.Read(siteId);

            if (configuration == null)
            {
                logger.Log(LogLevel.Info, "CustomResponse does not exist for SiteId : {0}. Saving one", siteId);
                configuration = new CustomResponse(siteId);
                CustomResponsePlugin.Instance.Save(configuration);
            }

            return configuration;
        }

        public void Update()
        {
            CustomResponsePlugin.Update(this);
        }

        #endregion

    }

}
