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
namespace Jardalu.Ratna.Store
{

    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Database;
    using Jardalu.Ratna.Plugins;

    #endregion

    public class PluginStore
    {

        #region private fields

        private static PluginStore store;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private PluginStore()
        {
        }

        #endregion

        #region public properties

        public static PluginStore Instance
        {
            get
            {
                if (store == null)
                {
                    lock (syncRoot)
                    {
                        if (store == null)
                        {
                            store = new PluginStore();
                        }
                    }
                }

                return store;
            }
        }
       
        #endregion

        #region public Methods

        public void Register(Plugin plugin)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            PluginDbInteractor.Instance.Register(plugin.Name, plugin.Id, plugin.Type, plugin.RawData);
        }

        public void Activate(Plugin plugin)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            PluginDbInteractor.Instance.ChangeActivationState(plugin.Id, true);
        }

        public void Deactivate(Plugin plugin)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            PluginDbInteractor.Instance.ChangeActivationState(plugin.Id, false);
        }

        public void Save(Plugin plugin, PluginData data)
        {
            #region argument checking

            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (string.IsNullOrEmpty(data.Key))
            {
                throw new ArgumentException("data");
            }

            if (string.IsNullOrEmpty(data.Id))
            {
                throw new ArgumentException("data");
            }

            #endregion

            PluginDbInteractor.Instance.SavePluginData(
                                plugin.Id,
                                data.Key,
                                data.Id,
                                data.UId,
                                data.RawData
                            );

            // refresh the plugin data to get the UId
            if (data.UId == Guid.Empty)
            {
                PluginData pluginData = Read<PluginData>(plugin, data.Key, data.Id);
                data.UId = pluginData.UId;
            }

        }

        public void Delete(Plugin plugin, Guid uid)
        {
            #region using

            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            #endregion

            PluginDbInteractor.Instance.DeletePluginData(
                    plugin.Id,
                    uid
                );
        }

        public void Delete(Plugin plugin, IList<Guid> uids)
        {
            #region using

            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            if (uids == null)
            {
                throw new ArgumentNullException("uids");
            }

            #endregion

            PluginDbInteractor.Instance.DeletePluginData(
                    plugin.Id,
                    uids
                );
        }

        public void Delete(Plugin plugin, string key, string id)
        {
            #region argument checking

            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            #endregion

            PluginDbInteractor.Instance.DeletePluginData(plugin.Id, key, id);
        }

        public T Read<T>(Plugin plugin, Guid uid)
        {
            #region argument checking

            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            #endregion

            return PluginDbInteractor.Instance.GetPluginData<T>(plugin.Id, uid);
        }

        public bool TryRead<T>(Plugin plugin, Guid uid, out T t)
        {
            #region argument checking

            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            #endregion

            bool success = false;
            t = default(T);

            try
            {
                t = PluginDbInteractor.Instance.GetPluginData<T>(plugin.Id, uid);
                success = true;
            }
            catch(Jardalu.Ratna.Exceptions.MessageException)
            {
            }

            return success;
        }

        public T Read<T>(Plugin plugin, string key, string id)
        {
            #region argument checking

            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            #endregion

            return PluginDbInteractor.Instance.GetPluginData<T>(plugin.Id, key, id);
        }

        public IList<T> Read<T>(Plugin plugin, string key)
        {

            #region argument checking

            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            #endregion

            return PluginDbInteractor.Instance.GetPluginData<T>(plugin.Id, key);

        }

        public IList<T> Read<T>(Plugin plugin, DateTime after)
        {

            #region argument checking

            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            #endregion

            return PluginDbInteractor.Instance.GetPluginData<T>(plugin.Id, after);

        }

        public IDictionary<string, int> GetCount<T>(Plugin plugin, IList<string> keys)
        {
            #region argument checking

            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            #endregion

            return PluginDbInteractor.Instance.GetPluginDataCount<T>(plugin.Id, keys);
        }

        public IList<T> Read<T>(Plugin plugin,string key, int start, int count, out int total)
        {
            #region argument checking

            if (plugin == null)
            {
                throw new ArgumentNullException("plugin");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            #endregion

            return PluginDbInteractor.Instance.GetPluginData<T>(plugin.Id, key, start, count, out total);
        }

        public IList<T> Read<T>(Plugin plugin, PluginDataQueryParameter query, int start, int count, out int total)
        {
            PluginDataQueryParameter[] queries = null;

            if (query != null)
            {
                queries = new PluginDataQueryParameter[1] { query };
            }

            return Read<T>(plugin, queries, start, count, false, out total);
        }

        public IList<T> ReadExact<T>(Plugin plugin, PluginDataQueryParameter query, int start, int count, out int total)
        {
            PluginDataQueryParameter[] queries = null;

            if (query != null)
            {
                queries = new PluginDataQueryParameter[1] { query };
            }

            return Read<T>(plugin, queries, start, count, true, out total);
        }

        public IList<T> Read<T>(Plugin plugin, PluginDataQueryParameter[] queries, int start, int count, bool exact, out int total)
        {

            #region argument checking

            if (start < 0)
            {
                throw new ArgumentException("start");
            }

            if (count < 1)
            {
                throw new ArgumentException("count");
            }

            #endregion

            return PluginDbInteractor.Instance.GetPluginData<T>(plugin.Id, queries, start, count, exact, out total);
        }

        public Plugin Get(Guid pluginId)
        {
            return PluginDbInteractor.Instance.GetPlugin(pluginId);
        }

        #endregion

    }
}
