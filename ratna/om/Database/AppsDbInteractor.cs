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

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Acls;
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Resource;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;

        
    #endregion

    internal class AppsDbInteractor : DbInteractor
    {

        #region Private Fields

        private static AppsDbInteractor instance;
        private static object syncRoot = new object();

        private static Logger logger;

        #endregion

        #region ctor

        static AppsDbInteractor()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        private AppsDbInteractor()
        {
        }

        #endregion

        #region Public Properties

        public static AppsDbInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new AppsDbInteractor();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Public Methods

        public void Activate(
                long id,
                bool enable
            )
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_App_Activate";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@ID", SqlDbType.BigInt, id);
            AddParameter(command, "@Enable", SqlDbType.Bit, enable);

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

        public long AddApp(
            string name, 
            Guid uniqueId, 
            string publisher, 
            string description, 
            string url, 
            int scope, 
            string version, 
            string location,
            string file,
            string fileEntry,
            string references,
            string iconurl)
        {

            #region arguments

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(publisher))
            {
                throw new ArgumentNullException("publisher");
            }

            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException("version");
            }

            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentNullException("location");
            }

            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_App_AddUpdate";

            AddParameter(command,"@SiteId",SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Name", SqlDbType.NVarChar, name);
            AddParameter(command, "@UniqueId", SqlDbType.UniqueIdentifier, uniqueId);
            AddParameter(command, "@Publisher", SqlDbType.NVarChar, publisher);
            AddParameter(command, "@Description", SqlDbType.NVarChar, description ?? string.Empty);
            AddParameter(command, "@Url", SqlDbType.NVarChar, url ?? string.Empty);
            AddParameter(command, "@Scope", SqlDbType.Int, scope);
            AddParameter(command, "@Version", SqlDbType.NVarChar, version);
            AddParameter(command, "@File", SqlDbType.NVarChar, file);
            AddParameter(command, "@FileEntry", SqlDbType.NVarChar, fileEntry ?? string.Empty);
            AddParameter(command, "@References", SqlDbType.NVarChar, references ?? string.Empty);
            AddParameter(command, "@Location", SqlDbType.NVarChar, location);
            AddParameter(command, "@IconUrl", SqlDbType.NVarChar, iconurl ?? string.Empty);

            SqlParameter idParameter = AddOutputParameter(command, "@Id", SqlDbType.BigInt);
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

            long id = (long)idParameter.Value;
            return id;
        }

        public Tuple<DataTable> GetAppList()
        {
            DataTable appTable = new DataTable();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_App_GetList";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    //load the table
                    appTable.Load(reader);

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


            return new Tuple<DataTable>(appTable);
        }

        public Tuple<DataTable> GetAppList(bool enabled)
        {
            DataTable appTable = new DataTable();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_App_GetEnabledList";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Enabled", SqlDbType.Bit, enabled);
            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    //load the table
                    appTable.Load(reader);

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


            return new Tuple<DataTable>(appTable);
        }

        public Tuple<DataTable> GetApp(long appId)
        {
            DataTable appTable = new DataTable();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_App_Get";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Id", SqlDbType.BigInt, appId);
            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    //load the table
                    appTable.Load(reader);

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


            return new Tuple<DataTable>(appTable);
        }

        public Tuple<DataTable> GetApp(Guid uniqueId)
        {
            DataTable appTable = new DataTable();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_App_GetWithUniqueId";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UniqueId", SqlDbType.UniqueIdentifier, uniqueId);
            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    //load the table
                    appTable.Load(reader);

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


            return new Tuple<DataTable>(appTable);
        }

        public Tuple<DataTable> GetAppList(int scope)
        {
            DataTable appTable = new DataTable();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_App_GetListWithScope";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Scope", SqlDbType.Int, scope);
            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    //load the table
                    appTable.Load(reader);

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


            return new Tuple<DataTable>(appTable);
        }

        public void SetAppRawData(long appId, string rawData)
        {
            if (rawData == null)
            {
                rawData = string.Empty;
            }

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_App_SetRawData";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Id", SqlDbType.BigInt, appId);
            AddParameter(command, "@RawData", SqlDbType.NText, rawData);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            logger.Log(LogLevel.Debug, "Updating RawData for App - {0}, SiteId - {1}", appId, Jardalu.Ratna.Core.WebContext.Current.Site.Id);

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

        public void DeleteApp(long appId)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_App_Delete";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@ID", SqlDbType.BigInt, appId);

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
                case AppsErrorCodes.PublisherNameMismatch:
                    throw new MessageException(ResourceManager.GetMessage(AppsMessages.PublisherNameMismatch));

                case AppsErrorCodes.NameCannotBeUpdated:
                    throw new MessageException(ResourceManager.GetMessage(AppsMessages.NameCannotBeUpdated));

                default:
                    base.ThrowError(errorCode);
                    break;
            }
        }

        #endregion

    }

}
