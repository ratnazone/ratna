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
namespace Jardalu.Ratna.Database
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Text;
    using System.Xml;

    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Resource;
    using Jardalu.Ratna.Utilities;
    
    #endregion

    internal class PluginDbInteractor : DbInteractor
    {

        #region Private Fields

        private static PluginDbInteractor instance;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private PluginDbInteractor()
        {
        }

        #endregion

        #region Public Properties

        public static PluginDbInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new PluginDbInteractor();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region public methods

        public void Register(string name, Guid id, string type, string rawData)
        {
            #region argument checking

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException("type");
            }

            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("id");
            }

            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_Register";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Name", SqlDbType.NVarChar, name);
            AddParameter(command, "@Id", SqlDbType.UniqueIdentifier, id);
            AddParameter(command, "@Type", SqlDbType.NVarChar, type);
            AddParameter(command, "@RawData", SqlDbType.NText, (rawData == null) ? string.Empty : rawData);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }
        }

        public void ChangeActivationState(Guid id, bool state)
        {
            #region argument checking

            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("id");
            }

            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_ChangeActiveState";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Id", SqlDbType.UniqueIdentifier, id);
            AddParameter(command, "@State", SqlDbType.Bit, state);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }
        }

        public void SavePluginData(Guid pluginId, string key, string id, Guid uid, string rawData)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_SavePluginData";

            rawData = rawData ?? string.Empty;

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@PluginId", SqlDbType.UniqueIdentifier, pluginId);
            AddParameter(command, "@Key", SqlDbType.NVarChar, key);
            AddParameter(command, "@Id", SqlDbType.NVarChar, id);
            AddParameter(command, "@UId", SqlDbType.UniqueIdentifier, uid);
            AddParameter(command, "@RawData", SqlDbType.NText, rawData);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }
        }

        public T GetPluginData<T>(Guid pluginId, string key, string id)
        {
            T data = default(T);

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_GetPluginData";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@PluginId", SqlDbType.UniqueIdentifier, pluginId);
            AddParameter(command, "@Key", SqlDbType.NVarChar, key);
            AddParameter(command, "@Id", SqlDbType.NVarChar, id);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        data = ObjectConverter.GetAs<T>(reader);
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            return data;
        }

        public T GetPluginData<T>(Guid pluginId, Guid uid)
        {
            T data = default(T);

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_GetPluginDataWithUid";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@PluginId", SqlDbType.UniqueIdentifier, pluginId);
            AddParameter(command, "@UId", SqlDbType.UniqueIdentifier, uid);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        data = ObjectConverter.GetAs<T>(reader);
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            return data;
        }

        public IList<T> GetPluginData<T>(Guid pluginId, string key)
        {
            int total;

            return GetPluginData<T>(pluginId, key, 0, -1, out total);
        }

        public IList<T> GetPluginData<T>(Guid pluginId, DateTime after)
        {
            List<T> datum = new List<T>();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_GetPluginDataInRange";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@PluginId", SqlDbType.UniqueIdentifier, pluginId);
            AddParameter(command, "@UpdateAfter", SqlDbType.DateTime, after);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        T data = ObjectConverter.GetAs<T>(reader);
                        datum.Add(data);
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            return datum;
        }

        public IList<T> GetPluginData<T>(Guid pluginId, string key, int start, int count, out int total)
        {
            List<T> datum = new List<T>();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_GetPluginDataForKey";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@PluginId", SqlDbType.UniqueIdentifier, pluginId);
            AddParameter(command, "@Key", SqlDbType.NVarChar, key);
            AddParameter(command, "@Start", SqlDbType.Int, start);
            AddParameter(command, "@Count", SqlDbType.Int, count);

            SqlParameter totalParameter = AddOutputParameter(command, "@Records", SqlDbType.Int);
            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        T data = ObjectConverter.GetAs<T>(reader);
                        datum.Add(data);
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            total = (int)totalParameter.Value;

            return datum;
        }

        public IList<T> GetPluginData<T>(Guid pluginId, PluginDataQueryParameter[] queries, int start, int count, bool exact, out int total)
        {
            List<T> datum = new List<T>();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_GetPluginDataForPlugin";

            StringBuilder queryBuilder = new StringBuilder();

            if (queries != null)
            {
                foreach (PluginDataQueryParameter query in queries)
                {
                    if (exact)
                    {
                        queryBuilder.AppendFormat("%<{0}>{1}</{0}>%", query.PropertyName, query.PropertyValue);
                    }
                    else
                    {
                        queryBuilder.AppendFormat("%<{0}>%{1}%</{0}>%", query.PropertyName, query.PropertyValue);
                    }
                }
            }

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@PluginId", SqlDbType.UniqueIdentifier, pluginId);
            AddParameter(command, "@LikeQuery", SqlDbType.NVarChar,queryBuilder.ToString());
            AddParameter(command, "@Start", SqlDbType.Int, start);
            AddParameter(command, "@Count", SqlDbType.Int, count);

            SqlParameter totalParameter = AddOutputParameter(command, "@Records", SqlDbType.Int);
            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        T data = ObjectConverter.GetAs<T>(reader);
                        datum.Add(data);
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            total = (int)totalParameter.Value;

            return datum;

        }

        public IDictionary<string, int> GetPluginDataCount<T>(Guid pluginId, IList<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            // initialize to zero
            Dictionary<string, int> dataCounts = new Dictionary<string, int>(keys.Count);

            StringBuilder builder = new StringBuilder();

            #region create keys xml and initialize datacount

            XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder));
            writer.WriteStartDocument();
            writer.WriteStartElement("Keys");

            foreach (string key in keys)
            {
                writer.WriteStartElement("Key");
                writer.WriteAttributeString("Value", key);
                writer.WriteEndElement();

                //initialize
                dataCounts[key] = 0;
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_GetPluginDataCountForKey";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@PluginId", SqlDbType.UniqueIdentifier, pluginId);
            AddParameter(command, "@KeysXml", SqlDbType.NText, builder.ToString());

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string key = reader["Key"] as string;
                        int count = (int)reader["Count"];

                        dataCounts[key] = count;
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            return dataCounts;
        }

        public Plugin GetPlugin(Guid pluginId)
        {
            Plugin plugin = null;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_GetPlugin";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@PluginId", SqlDbType.UniqueIdentifier, pluginId);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string type = reader["Type"] as string;
                        if (type != null)
                        {
                            //try to load the type
                            Type t = ReflectionUtility.GetType(type);
                            if (t == null)
                            {
                                //unable to load the type
                                ThrowError(PluginErrorCodes.NotFound);
                            }

                            //create the object instance
                            plugin = ObjectConverter.GetAs(t, reader) as Plugin;
                        }
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            return plugin;
        }

        public void DeletePluginData(Guid pluginId, Guid uid)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_DeletePluginData";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@PluginId", SqlDbType.UniqueIdentifier, pluginId);
            AddParameter(command, "@UId", SqlDbType.UniqueIdentifier, uid);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }
        }

        public void DeletePluginData(Guid pluginId, IList<Guid> uids)
        {
            #region argument checking

            if (uids == null)
            {
                throw new ArgumentNullException("uids");
            }

            #endregion

            if (uids.Count == 0)
            {
                //nothing to execute
                return;
            }

            StringBuilder builder = new StringBuilder();

            #region create uids xml
            XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder));
            writer.WriteStartDocument();
            writer.WriteStartElement("UIds");

            foreach (Guid uid in uids)
            {
                writer.WriteStartElement("UId");
                writer.WriteAttributeString("Value", uid.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_DeletePluginDataWithUids";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@PluginId", SqlDbType.UniqueIdentifier, pluginId);
            AddParameter(command, "@UIdsXml", SqlDbType.NText, builder.ToString());

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }
        }

        public void DeletePluginData(Guid pluginId, string key, string id)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Plugin_DeletePluginDataWithKeyId";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@PluginId", SqlDbType.UniqueIdentifier, pluginId);
            AddParameter(command, "@Key", SqlDbType.NVarChar, key);
            AddParameter(command, "@Id", SqlDbType.NVarChar, id);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }
        }

        #endregion

        #region Protected Methods

        protected override void ThrowError(long errorCode)
        {
            switch (errorCode)
            {
                case PluginErrorCodes.NameAlreadyInUse:
                    throw new MessageException(PluginErrorCodes.NameAlreadyInUse, ResourceManager.GetMessage(PluginMessages.NameAlreadyInUse));

                case PluginErrorCodes.IdAlreadyInUse:
                    throw new MessageException(PluginErrorCodes.IdAlreadyInUse, ResourceManager.GetMessage(PluginMessages.IdAlreadyInUse));

                case PluginErrorCodes.NotFound:
                    throw new MessageException(PluginErrorCodes.NotFound, ResourceManager.GetMessage(PluginMessages.NotFound));

                case PluginErrorCodes.PluginDataNotFound:
                    throw new MessageException(PluginErrorCodes.PluginDataNotFound, ResourceManager.GetMessage(PluginMessages.PluginDataNotFound));

                default:
                    base.ThrowError(errorCode);
                    break;
            }
        }

        #endregion

    }

}
