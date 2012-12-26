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
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Resource;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Core.Acls;
    using Jardalu.Ratna.Profile;
        
    #endregion

    internal class AclsDbInteractor : DbInteractor
    {

        #region Private Fields

        private static AclsDbInteractor instance;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private AclsDbInteractor()
        {
        }

        #endregion

        #region Public Properties

        public static AclsDbInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new AclsDbInteractor();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets ACL on the resource for the given principal.
        /// </summary>
        /// <param name="resourceId">The resource on which ACL needs to be set.</param>
        /// <param name="actorId">The principal who is changing the ACL. This must be the owner of the resource or has "Grant" permissions.</param>
        /// <param name="principalId">The principal whose ACL is being changed</param>
        /// <param name="acls">ACL</param>
        public void SetAcls(long resourceId, long actorId, long principalId, AclType acls)
        {
            
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Acls_Set";

            AddParameter(command,"@SiteId",SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@ResourceId", SqlDbType.BigInt, resourceId);
            AddParameter(command, "@ActorId", SqlDbType.BigInt, actorId);
            AddParameter(command, "@PrincipalId", SqlDbType.BigInt, principalId);
            AddParameter(command, "@Acls", SqlDbType.Int, acls);
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

        public AclType GetAcls(long resourceId, long actorId, long principalId)
        {
            int acls = 0;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Acls_Get";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@ResourceId", SqlDbType.BigInt, resourceId);
            AddParameter(command, "@PrincipalId", SqlDbType.BigInt, principalId);
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
                        acls =  acls | (int)reader["Acls"];
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

            return (AclType)acls;
        }

        public IDictionary<Principal, AclType> GetAcls(long resourceId, long actorId)
        {
            Dictionary<Principal, AclType> acls = new Dictionary<Principal, AclType>();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Acls_GetAll";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@ResourceId", SqlDbType.BigInt, resourceId);
            AddParameter(command, "@ActorId", SqlDbType.BigInt, actorId);
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
                        string name = reader["Name"] as string;
                        long principalId = (long)reader["PrincipalId"];
                        int pacls = (int)reader["Acls"];

                        Principal principal = GetPrincipal(principalId, name);
                        acls[principal] = (AclType)pacls;
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

            return acls;
        }

        public void DeleteAcls(long resourceId, long actorId, long principalId)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Acls_Delete";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@ResourceId", SqlDbType.BigInt, resourceId);
            AddParameter(command, "@ActorId", SqlDbType.BigInt, actorId);
            AddParameter(command, "@PrincipalId", SqlDbType.BigInt, principalId);
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

        #region private methods

        private Principal GetPrincipal(long principalId, string name)
        {
            return new User() { Name = name, PrincipalId = principalId };
        }

        #endregion

        #region Protected Methods

        protected override void ThrowError(long errorCode)
        {
            switch (errorCode)
            {
                case AclsErrorCodes.NoResourceFound:
                    throw new MessageException(ResourceManager.GetMessage(AclsMessages.NoResourceFound));

                case AclsErrorCodes.NoPermissionForChanging:
                    throw new MessageException(ResourceManager.GetMessage(AclsMessages.NoPermissionForChanging));

                default:
                    base.ThrowError(errorCode);
                    break;
            }
        }

        #endregion

    }

}
