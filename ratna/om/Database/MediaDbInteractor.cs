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
    using System.Text;

    using Jardalu.Ratna.Core.Media;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Resource;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Core;
    
        
    #endregion

    internal class MediaDbInteractor : DbInteractor
    {

        #region Private Fields

        private static MediaDbInteractor instance;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private MediaDbInteractor()
        {
        }

        #endregion

        #region Public Properties

        public static MediaDbInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new MediaDbInteractor();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Public Methods

        public BaseMedia SaveMedia(long owner, MediaType mediaType, string name, string url, string rawData)
        {
            #region argument checking

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            #endregion

            BaseMedia media = null;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Media_Save";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@MediaType", SqlDbType.Int, (int)mediaType);
            AddParameter(command, "@Name", SqlDbType.NVarChar, name);
            AddParameter(command, "@Url", SqlDbType.NVarChar, url);
            AddParameter(command, "@OwnerId", SqlDbType.BigInt, owner);
            AddParameter(command, "@RawData", SqlDbType.NText, rawData);

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
                        media = GetMedia(reader);
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

 
            return media;
        }

        public BaseMedia GetMedia(string url)
        {

            #region arguments

            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            #endregion

            BaseMedia media = null;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Media_GetByUrl";

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
                        media = GetMedia(reader);
                    }

                    // in the next dataset, tags will be coming.
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            Tag tag = ObjectConverter.GetAs<Tag>(reader);
                            media.Tags.Add(tag);
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

            return media;
        }

        public IList<BaseMedia> GetMedia(IList<string> urls)
        {
            #region arguments

            if (urls == null)
            {
                throw new ArgumentNullException("urls");
            }

            #endregion

            IList<BaseMedia> medium = new List<BaseMedia>(urls.Count);
            Dictionary<long, BaseMedia> resourceMedium = new Dictionary<long, BaseMedia>(urls.Count);

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Media_GetByUrls";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UrlsXml", SqlDbType.NVarChar, GetUrlsXml(urls));

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    BaseMedia media = null;

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        media = GetMedia(reader);
                        resourceMedium[media.ResourceId] = media;
                    }

                    // in the next dataset, tags will be coming.
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            long resourceId = (long)reader["ResourceId"];
                            Tag tag = ObjectConverter.GetAs<Tag>(reader);
                            if (resourceMedium.TryGetValue(resourceId, out media))
                            {
                                media.Tags.Add(tag);
                            }
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

            //grab all the media in the list
            foreach (BaseMedia bMedia in resourceMedium.Values)
            {
                medium.Add(bMedia);
            }

            return medium;

        }

        public IList<BaseMedia> GetMedium(string query, MediaType mediaType, long ownerId, string tagKey, int start, int size, out int total)
        {
            List<BaseMedia> medium = new List<BaseMedia>();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Media_GetList";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Query", SqlDbType.NVarChar, query);
            AddParameter(command, "@OwnerId", SqlDbType.BigInt, ownerId);
            AddParameter(command, "@MediaType", SqlDbType.Int, (int) mediaType);
            AddParameter(command, "@TagKey", SqlDbType.UniqueIdentifier, new Guid(tagKey));
            AddParameter(command, "@Start", SqlDbType.Int, start);
            AddParameter(command, "@Count", SqlDbType.Int, size);

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
                        BaseMedia media = GetMedia(reader);

                        medium.Add(media);
                    }

                    // in the next dataset, tags will be coming.
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            Tag tag = ObjectConverter.GetAs<Tag>(reader);

                            //read the resourceId
                            long resourceId = (long)reader["ResourceId"];

                            BaseMedia matched = medium.Find(delegate(BaseMedia a) { return a.Id == resourceId; });
                            if (matched != null)
                            {
                                matched.Tags.Add(tag);
                            }
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

            total = (int)totalParameter.Value;

            return medium;
        }

        public void Delete(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Media_Delete";

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

        #region protected methods

        protected override void ThrowError(long errorCode)
        {
            switch (errorCode)
            {
                case MediaErrorCodes.UrlAlreadyInUse:
                    throw new MessageException(ResourceManager.GetMessage(MediaMessages.UrlAlreadyInUse));

                default:
                    base.ThrowError(errorCode);
                    break;
            }
        }

        #endregion

        #region private methods

        private BaseMedia GetMedia(SqlDataReader reader)
        {
            BaseMedia media = null;

            if (reader != null)
            {
                MediaType mediaType = (MediaType)reader["MediaType"];

                switch (mediaType)
                {
                    case MediaType.Photo:
                        media = ObjectConverter.GetAs<Photo>(reader);
                        break;
                    case MediaType.Document:
                        media = ObjectConverter.GetAs<Document>(reader);
                        break;
                    case MediaType.Video:
                        media = ObjectConverter.GetAs<Video>(reader);
                        break;
                    case MediaType.Other:
                        media = ObjectConverter.GetAs<OtherMedia>(reader);
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                //set the owner for the article
                long ownerId = (long)reader["ownerId"];
                media.Owner = UserStore.Instance.LoadUser(ownerId);

            }

            return media;
        }

        private string GetUrlsXml(IList<string> urls)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<Urls>");

            foreach (string url in urls)
            {
                builder.AppendFormat("<Url Value=\"{0}\" />", url);
            }

            builder.Append("</Urls>");

            return builder.ToString();
        }

        #endregion

    }

}
