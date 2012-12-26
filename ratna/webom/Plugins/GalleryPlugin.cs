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

    public class GalleryPlugin : SystemPlugin
    {
        #region private fields

        private static Dictionary<int, GalleryPlugin> instances = new Dictionary<int, GalleryPlugin>();
        private static object syncRoot = new object();

        private static Logger logger;

        #endregion

        #region ctor

        static GalleryPlugin()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public GalleryPlugin()
        {
            this.Name = "GalleryPlugin";
            this.Id = new Guid("cc3c6daf-362b-4ec0-ac6c-c4889e904a83");

            // this should always be registered and active.
            this.Register();
            this.Activate();
        }

        #endregion  

        #region public properties

        public static GalleryPlugin Instance
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
                            instances[siteId] = new GalleryPlugin();
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

        public Gallery Read(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            Gallery gallery = null;
            try
            {
                logger.Log(LogLevel.Debug, "Reading Gallery for url : {0}", url);
                gallery = PluginStore.Instance.Read<Gallery>(Instance, Gallery.KeyName, url);
            }
            catch (MessageException me)
            {
                if (me.ErrorNumber != PluginErrorCodes.PluginDataNotFound)
                {
                    throw;
                }
            }

            return gallery;
        }

        public IList<Gallery> Read(int start, int count, out int total)
        {
            return PluginStore.Instance.Read<Gallery>(this, Gallery.KeyName, start, count, out total);
        }

        public void Update(Gallery gallery)
        {
            if (gallery == null)
            {
                throw new ArgumentNullException("gallery");
            }

            logger.Log(LogLevel.Debug, "Saving gallery");

            PluginStore.Instance.Save(Instance, gallery);
        }

        #endregion

    }
}
