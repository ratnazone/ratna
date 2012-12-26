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

        
    #endregion

    internal class RedirectDbInteractor : DbInteractor
    {

        #region Private Fields

        private static RedirectDbInteractor instance;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private RedirectDbInteractor()
        {
        }

        #endregion

        #region Public Properties

        public static RedirectDbInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new RedirectDbInteractor();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Public Methods

        public bool GetMapping(string url, out string redirect)
        {
            bool success = false;
            redirect = null;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Redirect_GetUrl";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Url", SqlDbType.NVarChar, url);
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
                        redirect = reader["RedirectUrl"] as string;
                        success = true;
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

            return success;
        }

        public void AddMapping(string url, string redirect)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (string.IsNullOrEmpty(redirect))
            {
                throw new ArgumentNullException("redirect");
            }

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Redirect_AddUrl";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Url", SqlDbType.NVarChar, url);
            AddParameter(command, "@RedirectUrl", SqlDbType.NVarChar, redirect);
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

        public void DeleteMapping(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Redirect_Delete";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Url", SqlDbType.NVarChar, url);
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

    }

}
