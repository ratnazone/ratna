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

    using Jardalu.Ratna.Administration;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Resource;
    using Jardalu.Ratna.Utilities;

    #endregion

    internal class SiteDbInteractor : DbInteractor
    {

        #region Private Fields

        private static SiteDbInteractor instance;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private SiteDbInteractor()
        {
        }

        #endregion

        #region Public Properties

        public static SiteDbInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new SiteDbInteractor();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Public Methods


        public IList<Tuple<int, string, string>> GetSites()
        {

            List<Tuple<int, string, string>> sitesData = new List<Tuple<int, string, string>>();

            #region db call
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_GetSites";

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
                        int id = (int)reader["Id"];
                        string host = reader["Host"] as string;
                        string title = reader["Title"] as string;

                        Tuple<int, string, string> tuple = new Tuple<int, string, string>(id, host, title);
                        sitesData.Add(tuple);
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            #endregion


            return sitesData;
        }

        public Tuple<int, string, string> AddSite(string host, string title)
        {
            Tuple<int, string, string> siteData = null;

            #region db call
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_CreateSite";

            AddParameter(command, "@Host", SqlDbType.NVarChar, host);
            AddParameter(command, "@Title", SqlDbType.NVarChar, title);

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
                        int id = (int)reader["Id"];
                        string rhost = reader["Host"] as string;
                        string rtitle = reader["Title"] as string;

                        siteData = new Tuple<int, string, string>(id, host, title);
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            #endregion


            return siteData;
        }

        public void DeleteSite(string host)
        {
            throw new NotImplementedException();
        }

        public void ProvisionSite(int siteId, string email, string alias, string password)
        {

            #region arguments

            if (string.IsNullOrEmpty(email)) 
            {
                throw new ArgumentNullException("email");
            }

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            #endregion

            #region db call
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_ProvisionSite";

            AddParameter(command, "@SiteId", SqlDbType.Int, siteId);
            AddParameter(command, "@AdminEmail", SqlDbType.NVarChar, email);
            AddParameter(command, "@AdminAlias", SqlDbType.NVarChar, alias);
            AddParameter(command, "@AdminPassword", SqlDbType.NVarChar, PasswordUtility.GetPasswordHash(password));
 
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

            #endregion
        }

        #endregion

        #region Protected Methods

        protected override void ThrowError(long errorCode)
        {
            switch (errorCode)
            {
                case SiteErrorCodes.HostAlreadyInUse:
                    throw new MessageException(SiteErrorCodes.HostAlreadyInUse, ResourceManager.GetMessage(SiteMessages.HostAlreadyInUse));

                case SiteErrorCodes.DefaultCannotBeDeleted:
                    throw new MessageException(SiteErrorCodes.DefaultCannotBeDeleted, ResourceManager.GetMessage(SiteMessages.DefaultCannotBeDeleted));

                default:
                    base.ThrowError(errorCode);
                    break;
            }
        }

        #endregion
    }

}
