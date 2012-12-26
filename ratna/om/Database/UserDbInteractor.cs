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
        
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Resource;

    #endregion

    internal class UserDbInteractor : DbInteractor
    {

        #region Private Fields

        private static UserDbInteractor instance;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private UserDbInteractor()
        {
        }

        #endregion

        #region public properties

        public static UserDbInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new UserDbInteractor();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region public methods

        public bool DoesUserExists(string alias)
        {
            return (Read(alias) != null);
        }

        public User Read(string alias)
        {
            #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            #endregion

            User user = null;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_GetUser";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        user = ObjectConverter.GetAs<User>(reader);
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return user;
        }

        public User Read(long userId)
        {
            User user = null;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_GetUserById";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UserId", SqlDbType.BigInt, userId);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        user = ObjectConverter.GetAs<User>(reader);
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return user;
        }

        public IList<User> Find(string query, bool isActive, bool isDeleted, int start, int count, out int total)
        {
            //set default total
            total = 0;

            #region argument checking

            if (start < 0)
            {
                throw new ArgumentException("start");
            }

            if (count < 0)
            {
                throw new ArgumentException("count");
            }

            #endregion

            query = query ?? string.Empty;

            List<User> users = new List<User>();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_GetUsers";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Query", SqlDbType.NVarChar, query);
            AddParameter(command, "@IsActive", SqlDbType.Bit, isActive);
            AddParameter(command, "@IsDeleted", SqlDbType.Bit, isDeleted);
            AddParameter(command, "@Start", SqlDbType.Int, start);
            AddParameter(command, "@Count", SqlDbType.Int, count);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);
            SqlParameter totalParameter = AddOutputParameter(command, "@Records", SqlDbType.Int);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        User user = ObjectConverter.GetAs<User>(reader);
                        users.Add(user);
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

            return users;
        }

        public void Create(string alias, string email, string passwordHash, string displayName, string firstName, string lastName, string description, out long userId, out long principalId)
        {
            // alias, email, password, firstname, lastname cannot be null/empty
            #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentNullException("passwordHash");
            }

            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentNullException("displayName");
            }

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException("firstName");
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException("lastName");
            }

            #endregion

            //set default description if not set
            description = description ?? string.Empty;

            userId = -1;
            principalId = 0;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_CreateUser";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);
            AddParameter(command, "@Email", SqlDbType.NVarChar, email);
            AddParameter(command, "@Password", SqlDbType.NVarChar, passwordHash);
            AddParameter(command, "@DisplayName", SqlDbType.NVarChar, displayName);
            AddParameter(command, "@FirstName", SqlDbType.NVarChar, firstName);
            AddParameter(command, "@LastName", SqlDbType.NVarChar, lastName);
            AddParameter(command, "@Description", SqlDbType.NVarChar, description);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    // read the information from datareader
                    if (reader.Read())
                    {
                        userId = (long)reader["Id"];
                        principalId = (long)reader["PrincipalId"];
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

            // if no userId given its an error
            if (userId <= 0)
            {
                ThrowError(UserErrorCodes.Unknown);
            }

        }

        public void Update(string alias, string displayName, string firstName, string lastName, string description)
        {
            // alias, firstname, lastname cannot be null/empty
            #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }
            
            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentNullException("displayName");
            }

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException("firstName");
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException("lastName");
            }

            #endregion

            //set default description if not set
            description = description ?? string.Empty;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_UpdateUser";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);
            AddParameter(command, "@DisplayName", SqlDbType.NVarChar, displayName);
            AddParameter(command, "@FirstName", SqlDbType.NVarChar, firstName);
            AddParameter(command, "@LastName", SqlDbType.NVarChar, lastName);
            AddParameter(command, "@Description", SqlDbType.NVarChar, description);

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

        public void Update(string alias, string photo)
        {
            // alias, firstname, lastname cannot be null/empty
            #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            if (string.IsNullOrEmpty(photo))
            {
                throw new ArgumentNullException("photo");
            }


            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_UpdateUserPhoto";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);
            AddParameter(command, "@Photo", SqlDbType.NVarChar, photo);

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

        public bool Signin(string alias, string passwordHash, out string cookie, out DateTime expiry)
        {
            #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentNullException("passwordHash");
            }

            #endregion

            bool isValid = false ;
            cookie = null;
            expiry = DateTime.MinValue;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_IsValidUserPassword";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);
            AddParameter(command, "@Password", SqlDbType.NVarChar, passwordHash);

            SqlParameter validParameter = AddOutputParameter(command, "@Valid", SqlDbType.Bit);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        //read the cookie and expiry time
                        cookie = ((Guid)reader["Cookie"]).ToString();
                        expiry = (DateTime)reader["CookieExpiry"];
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            isValid = (bool)validParameter.Value;

            return isValid;
        }

        public void Signout(string alias)
        {
        #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_ExpireUserCookie";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    command.ExecuteNonQuery();
                }
            }

        }

        public bool IsUserCookieValid(string alias, string cookie)
        {
            #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            if (string.IsNullOrEmpty(cookie))
            {
                throw new ArgumentNullException("cookie");
            }

            #endregion

            bool isValid = false;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_IsUserCookieValid";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);
            AddParameter(command, "@Cookie", SqlDbType.NVarChar, cookie);

            SqlParameter validParameter = AddOutputParameter(command, "@IsValid", SqlDbType.Bit);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            isValid = (bool)validParameter.Value;

            return isValid;

        }

        public void ChangePassword(string alias, string oldpasswordHash, string passwordHash)
        {
            #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            if (string.IsNullOrEmpty(oldpasswordHash))
            {
                throw new ArgumentNullException("oldpasswordHash");
            }

            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentNullException("passwordHash");
            }

            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_ChangeUserPassword";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);
            AddParameter(command, "@OldPassword", SqlDbType.NVarChar, oldpasswordHash);
            AddParameter(command, "@NewPassword", SqlDbType.NVarChar, passwordHash);

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

        public void DeleteUser(string alias, long actorId)
        {
            #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_DeleteUser";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);
            AddParameter(command, "@ActorId", SqlDbType.BigInt, actorId);

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

        public void ActivateUser(string alias, string activationKey)
        {
            #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            if (string.IsNullOrEmpty(activationKey))
            {
                throw new ArgumentNullException("activationKey");
            }

            #endregion

            Guid guid = Guid.Empty;
            if (!Guid.TryParse(activationKey, out guid))
            {
                ThrowError(UserErrorCodes.ActivationCodeMismatch);
            }

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_ActivateUser";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);
            AddParameter(command, "@ActivationCode", SqlDbType.UniqueIdentifier, guid);

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

        public void ActivateUser(string alias, long actorId)
        {
            #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_ActivateUserWithoutKey";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);
            AddParameter(command, "@ActorID", SqlDbType.BigInt, actorId);

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

        public string GetUserActivationCode(string alias)
        {
            #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            #endregion

            string code = null;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_GetActivationCode";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    // read the information from datareader
                    if (reader.Read())
                    {
                        code = reader["ActivationCode"] as string;
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            // throw if the user was not found.
            if (string.IsNullOrEmpty(code))
            {
                ThrowError(UserErrorCodes.UserNotFound);
            }

            return code;
        }

        public IList<Group> GetMembershipGroups(string alias, int start, int count, out int total)
        {
            //set default total
            total = 0;

            #region argument checking

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            if (start < 0)
            {
                throw new ArgumentException("start");
            }

            if (count < 0)
            {
                throw new ArgumentException("count");
            }

            #endregion

            List<Group> groups = new List<Group>();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_GetUserGroups";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Alias", SqlDbType.NVarChar, alias);
            AddParameter(command, "@Start", SqlDbType.Int, start);
            AddParameter(command, "@Count", SqlDbType.Int, count);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);
            SqlParameter totalParameter = AddOutputParameter(command, "@Total", SqlDbType.Int);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Group group = ObjectConverter.GetAs<Group>(reader);
                        groups.Add(group);
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

            return groups;
        }

        #endregion

        #region Protected Methods

        protected override void ThrowError(long errorCode)
        {
            switch (errorCode)
            {
                case UserErrorCodes.AliasAlreadyInUse:
                    throw new MessageException(ResourceManager.GetMessage(UserMessages.AliasAlreadyInUse));

                case UserErrorCodes.EmailAlreadyInUse:
                    throw new MessageException(ResourceManager.GetMessage(UserMessages.EmailAlreadyInUse));

                case UserErrorCodes.PasswordMismatch:
                    throw new MessageException(UserErrorCodes.PasswordMismatch, ResourceManager.GetMessage(UserMessages.PasswordMismatch));

                case UserErrorCodes.ActivationCodeMismatch:
                    throw new MessageException(UserErrorCodes.ActivationCodeMismatch, ResourceManager.GetMessage(UserMessages.ActivationCodeMismatch));

                default:
                    base.ThrowError(errorCode);
                    break;
            }
        }

        #endregion

    }

}
