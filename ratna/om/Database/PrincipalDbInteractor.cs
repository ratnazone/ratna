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
    using System.Data;
    using System.Data.SqlClient;
        
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Resource;
    
    #endregion

    internal class PrincipalDbInteractor : DbInteractor
    {

        #region Private Fields

        private static PrincipalDbInteractor instance;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private PrincipalDbInteractor()
        {
        }

        #endregion

        public static PrincipalDbInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new PrincipalDbInteractor();
                        }
                    }
                }

                return instance;
            }
        }

        #region

        public Principal Find(string query)
        {
            #region argument checking

            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException("query");
            }

            #endregion

            Principal principal = null;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Principal_Find";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Query", SqlDbType.NVarChar, query);

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

                        bool isGroup = (bool)reader["IsGroup"];
                        long principalId = (long)reader["PrincipalId"];
                        string name = reader["Name"] as string;
                        if (isGroup)
                        {
                            principal = new Group() { Name=name, PrincipalId = principalId };
                        }
                        else
                        {
                            principal = new User() { Name = name, PrincipalId = principalId };
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

            return principal;
        }

        #endregion

    }

}
