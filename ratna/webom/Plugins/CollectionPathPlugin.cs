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

    public class CollectionPathPlugin : SystemPlugin
    {
        #region private fields

        private static Dictionary<int, CollectionPathPlugin> instances = new Dictionary<int, CollectionPathPlugin>();
        private static object syncRoot = new object();

        private static Logger logger;

        #endregion

        #region ctor

        static CollectionPathPlugin()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public CollectionPathPlugin()
        {
            this.Name = "CollectionPathPlugin";
            this.Id = new Guid("8222c1f1-b140-4509-8622-64019d7a64f2");

            // this should always be registered and active.
            this.Register();
            this.Activate();
        }

        #endregion  

        #region public properties

        public static CollectionPathPlugin Instance
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
                            instances[siteId] = new CollectionPathPlugin();
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

        public CollectionPath Read(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            CollectionPath collectionPath = null;
            try
            {
                logger.Log(LogLevel.Debug, "Reading Collection Path for path : {0}", path);
                collectionPath = PluginStore.Instance.Read<CollectionPath>(Instance, CollectionPath.KeyName, path);
            }
            catch (MessageException me)
            {
                if (me.ErrorNumber != PluginErrorCodes.PluginDataNotFound)
                {
                    throw;
                }
            }

            return collectionPath;
        }

        public void Update(CollectionPath collectionPath)
        {
            if (collectionPath == null)
            {
                throw new ArgumentNullException("collectionPath");
            }

            logger.Log(LogLevel.Debug, "Saving collectionPath");

            PluginStore.Instance.Save(Instance, collectionPath);
        }

        public IList<CollectionPath> GetPaths()
        {
            return PluginStore.Instance.Read<CollectionPath>(this, CollectionPath.KeyName);
        }

        #endregion


    }
}
