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

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Resource;
    using Jardalu.Ratna.Plugins;
        
    #endregion

    internal class TagDbInteractor : DbInteractor
    {

        #region Private Fields

        private static TagDbInteractor instance;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private TagDbInteractor()
        {
        }

        #endregion

        #region Public Properties

        public static TagDbInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new TagDbInteractor();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Public Methods

        public void RegisterKey(Guid key)
        {
            #region argument checking

            if (key == Guid.Empty)
            {
                throw new ArgumentException("key");
            }

            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Tags_RegisterKey";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Key", SqlDbType.UniqueIdentifier, key);

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

        public Tag AddTag(Guid key, long resourceId, string tag, int weight)
        {
            #region argument checking

            if (key == Guid.Empty)
            {
                throw new ArgumentException("pluginId");
            }

            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException("tag");
            }

            if (weight < 0)
            {
                throw new ArgumentException("weight");
            }

            #endregion

            Tag tagCreated = null;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Tags_Add";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Key", SqlDbType.UniqueIdentifier, key);
            AddParameter(command, "@ResourceId", SqlDbType.BigInt, resourceId);
            AddParameter(command, "@Tag", SqlDbType.NVarChar, tag);
            AddParameter(command, "@Weight", SqlDbType.Int, weight);

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
                        tagCreated = ObjectConverter.GetAs<Tag>(reader);
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


            return tagCreated;
        }

        public void AddTags(Guid key, long resourceId, IList<Tag> tags)
        {
            #region argument checking

            if (key == Guid.Empty)
            {
                throw new ArgumentException("key");
            }

            if (tags == null)
            {
                throw new ArgumentNullException("tags");
            }

            #endregion

            if (tags.Count == 0)
            {
                return;
            }

            StringBuilder builder = new StringBuilder();

            #region create tags xml
            XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder));
            writer.WriteStartDocument();
            writer.WriteStartElement("Tags");

            foreach(Tag tag in tags)
            {
                writer.WriteStartElement("Tag");
                writer.WriteAttributeString("Name", tag.Name);
                writer.WriteAttributeString("Weight", tag.Weight.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Tags_AddAll";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Key", SqlDbType.UniqueIdentifier, key);
            AddParameter(command, "@ResourceId", SqlDbType.BigInt, resourceId);
            AddParameter(command, "@TagsXml", SqlDbType.NText, builder.ToString());

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

        public void DeleteTag(Guid key, long resourceId, string tag)
        {
            #region argument checking

            if (key == Guid.Empty)
            {
                throw new ArgumentException("key");
            }

            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException("tag");
            }

            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Tags_Delete";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Key", SqlDbType.UniqueIdentifier, key);
            AddParameter(command, "@ResourceId", SqlDbType.BigInt, resourceId);
            AddParameter(command, "@Tag", SqlDbType.NVarChar, tag);

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

        public IList<Tag> GetTags(Guid key, long resourceId)
        {
            #region argument checking

            if (key == Guid.Empty)
            {
                throw new ArgumentException("key");
            }

            #endregion

            List<Tag> tags = new List<Tag>();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Tags_Get";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Key", SqlDbType.UniqueIdentifier, key);
            AddParameter(command, "@ResourceId", SqlDbType.BigInt, resourceId);

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
                        Tag tagCreated = new Tag();
                        tagCreated.Name = reader["Tag"] as string;
                        tagCreated.Weight = (int)reader["Weight"];
                        tags.Add(tagCreated);
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

           
            return tags;
        }

        #endregion

        #region Protected Methods

        protected override void ThrowError(long errorCode)
        {
            switch (errorCode)
            {
                default:
                    base.ThrowError(errorCode);
                    break;
            }
        }

        #endregion

    }

}
