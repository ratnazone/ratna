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
    using Jardalu.Ratna.Resource;

    #endregion

    internal abstract class DbInteractor
    {
        #region public fields

        public const int NoErrorCode = 0;

        #endregion

        #region public methods

        public static void AddParameter(SqlCommand command, string name, SqlDbType dbType, object value)
        {

            #region argument checkin

            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            #endregion

            SqlParameter parameter = new SqlParameter(name, dbType);
            parameter.Value = value;
            parameter.Direction = ParameterDirection.Input;
            command.Parameters.Add(parameter);
        }

        public static SqlParameter AddOutputParameter(SqlCommand command, string name, SqlDbType dbType)
        {

            #region argument checkin

            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            #endregion

            SqlParameter parameter = new SqlParameter(name, dbType);
            parameter.Direction = ParameterDirection.Output;

            if (dbType == SqlDbType.NVarChar)
            {
                parameter.Value = "";
            }
            else if (dbType == SqlDbType.BigInt)
            {
                parameter.Value = 0;
            }

            command.Parameters.Add(parameter);
            return parameter;
        }

        public static SqlParameter AddOutputParameter(SqlCommand command, string name, SqlDbType dbType, int size)
        {

            #region argument checkin

            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            #endregion

            SqlParameter parameter = new SqlParameter(name, dbType);
            parameter.Direction = ParameterDirection.Output;
            parameter.Size = size;

            command.Parameters.Add(parameter);
            return parameter;
        }

        #endregion

        #region Protected Methods

        protected virtual void ThrowError(long errorCode)
        {
            switch (errorCode)
            {
                default:
                    throw new MessageException(ResourceManager.GetMessage(CommonMessages.Unknown));
            }
        }

        #endregion

    }
}
