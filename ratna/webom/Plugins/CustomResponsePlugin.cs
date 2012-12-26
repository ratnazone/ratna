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

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Utilities;

    #endregion

    internal class CustomResponsePlugin : SystemPlugin
    {
        #region private fields

        private static Dictionary<int, CustomResponsePlugin> instances = new Dictionary<int, CustomResponsePlugin>();
        private static object syncRoot = new object();

        private static Logger logger;

        #endregion

        #region ctor

        static CustomResponsePlugin()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public CustomResponsePlugin()
        {
            this.Name = "CustomResponsePlugin";
            this.Id = new Guid("d69acfc2-c7b4-4c2f-b36d-eb7b258b5899");

            // this should always be registered and active.
            this.Register();
            this.Activate();
        }

        #endregion  

        #region public properties

        public static CustomResponsePlugin Instance
        {
            get
            {
                int siteId = WebContext.Current.Site.Id;

                if (!instances.ContainsKey(siteId))
                {
                    lock (syncRoot)
                    {
                        if (!instances.ContainsKey(siteId))
                        {
                            instances[siteId] = new CustomResponsePlugin();
                        }
                    }
                }

                return instances[siteId];
            }
        }

        public override DataContractSerializer Serializer
        {
            get
            {
                return null;
            }
        }

        public override string Type
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        #endregion

        #region public methods

        public static CustomResponse Read()
        {
            return Read(WebContext.Current.Site.Id);
        }

        public static CustomResponse Read(int siteId)
        {
            CustomResponse configuration = null;
            try
            {
                logger.Log(LogLevel.Debug, "Reading Site Configuration for siteId : {0}", siteId);
                configuration = PluginStore.Instance.Read<CustomResponse>(Instance, CustomResponse.ConfigKey, siteId.ToString());
            }
            catch (MessageException me)
            {
                if (me.ErrorNumber != PluginErrorCodes.PluginDataNotFound)
                {
                    throw;
                }
            }

            return configuration;
        }

        public static void Update(CustomResponse configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            logger.Log(LogLevel.Debug, "Saving custom response");

            PluginStore.Instance.Save(Instance, configuration);
        }

        #endregion


    }
}
