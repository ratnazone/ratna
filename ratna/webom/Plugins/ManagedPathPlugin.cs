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

    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Store;
    using System.Runtime.Serialization;

    #endregion

    public class ManagedPathPlugin : SystemPlugin
    {

        #region private fields

        private static ManagedPathPlugin instance;
        private static object syncRoot = new object();

        private static Logger logger;
        private bool refresh = true;

        private List<ManagedPath> paths = new List<ManagedPath>();

        #endregion

        #region ctor

        static ManagedPathPlugin()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public ManagedPathPlugin()
        {
            this.Name = "ManagedPathPlugin";
            this.Id = new Guid("392af3d9-1641-4b70-95bf-ff57d8a94a69");

            // this should always be registered and active.
            this.Register();
            this.Activate();
        }

        #endregion

        #region public properties

        public static ManagedPathPlugin Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new ManagedPathPlugin();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region public methods

        public override string Type
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        public override DataContractSerializer Serializer
        {
            get
            {
                return null;
            }
        }

        public IList<ManagedPath> GetPaths()
        {

            if (refresh)
            {
                lock (syncRoot)
                {
                    if (refresh)
                    {
                        paths.Clear();
                        paths.AddRange(GetWellDefinedPaths());

                        IList<ManagedPath> mPaths = this.Read();
                        if (mPaths != null && mPaths.Count > 0)
                        {
                            paths.AddRange(mPaths);
                        }

                        refresh = false;
                    }
                }
            }

            //return a cloned one
            //return new List<ManagedPath>(paths);
            return paths;
          
        }

        public void Save(ManagedPath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            logger.Log(LogLevel.Debug, "Saving managed path - {0}", path.Name);

            PluginStore.Instance.Save(this, path);

            refresh = true;
        }

        public IList<ManagedPath> Read()
        {
            return PluginStore.Instance.Read<ManagedPath>(this, ManagedPath.KeyName);
        }

        #endregion

        #region private methods

        private static IList<ManagedPath> GetWellDefinedPaths()
        {

            List<ManagedPath> paths = new List<ManagedPath>();

            ManagedPath path = new ManagedPath()
            {
                Name = "default",
                Path = "page/{p}/{q}",
                ResolvedPath = "~/default.aspx"
            };

            path.AddDefault("q", string.Empty);
            path.AddDefault("p", "1");

            paths.Add(path);

            // search path
            path = new ManagedPath()
            {
                Name = "search",
                Path = "search/{q}/{t}/{p}",
                ResolvedPath = "~/pages/search.aspx"
            };

            path.AddDefault("q", string.Empty);
            path.AddDefault("t", "all");  // type
            path.AddDefault("p", "1");

            paths.Add(path);

            return paths;

        }

        #endregion

    }
}
