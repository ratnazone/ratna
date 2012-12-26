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
using Jardalu.Ratna.Utilities;
        
    #endregion

    internal class ArticleDbInteractor : DbInteractor
    {

        #region private Fields

        private static ArticleDbInteractor instance;
        private static object syncRoot = new object();

        private Logger logger;

        #endregion

        #region ctor

        private ArticleDbInteractor()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region Public Properties

        public static ArticleDbInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new ArticleDbInteractor();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Public Methods

        public Article SaveArticle(Guid pluginId, long owner, string title, string urlKey, int readVersion, bool updateVersion, string rawData)
        {
            #region argument checking

            if (pluginId == Guid.Empty)
            {
                throw new ArgumentException("pluginId");
            }

            if (string.IsNullOrEmpty(urlKey))
            {
                throw new ArgumentNullException("urlKey");
            }

            #endregion

            Article article = null;

            //default title to be empty
            title = title ?? string.Empty;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Article_Save";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@HandlerId", SqlDbType.UniqueIdentifier, pluginId);
            AddParameter(command, "@Title", SqlDbType.NVarChar, title);
            AddParameter(command, "@UrlKey", SqlDbType.NVarChar, urlKey);
            AddParameter(command, "@OwnerId", SqlDbType.BigInt, owner);
            AddParameter(command, "@RawData", SqlDbType.NText, rawData);
            AddParameter(command, "@ReadVersion", SqlDbType.Int, readVersion);
            AddParameter(command, "@UpdateVersion", SqlDbType.Bit, updateVersion);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            logger.Log(LogLevel.Debug, "SaveArticle - urlKey : {0}, SiteId : {1}", urlKey, Jardalu.Ratna.Core.WebContext.Current.Site.Id);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        article = ObjectConverter.GetAs<Article>(reader);

                        if (article != null)
                        {

                            //set the owner for the article
                            long ownerId = (long)reader["ownerId"];
                            article.Owner = UserStore.Instance.LoadUser(ownerId);
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

 
            return article;
        }

        public Article GetArticle(string urlKey, int stage)
        {

            Article article = null;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Article_GetByUrl";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UrlKey", SqlDbType.NVarChar, urlKey);
            AddParameter(command, "@Stage", SqlDbType.Int, stage);

            logger.Log(LogLevel.Debug, "calling Proc_Ratna_Article_GetByUrl {0}, {1}, {2}", Jardalu.Ratna.Core.WebContext.Current.Site.Id, urlKey, stage);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader sqlReader = command.ExecuteReader();
                    if (sqlReader.Read())
                    {
                        article = ObjectConverter.GetAs<Article>(sqlReader);

                        //set the owner for the article
                        long owner = (long)sqlReader["ownerId"];
                        article.Owner = UserStore.Instance.LoadUser(owner);

                        article.IsDirty = false;
                    }

                    // in the next dataset, tags will be coming.
                    if (sqlReader.NextResult())
                    {
                        while (sqlReader.Read())
                        {
                            Tag tag = ObjectConverter.GetAs<Tag>(sqlReader);
                            article.Tags.Add(tag);
                        }
                    }

                    if (!sqlReader.IsClosed)
                    {
                        sqlReader.Close();
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            return article;
        }

        public IList<Article> GetArticles(string query, long ownerId, int stage, string tagKey, Guid handlerId, int start, int size, out int total)
        {
            List<Article> articles = new List<Article>();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Article_GetList";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@Query", SqlDbType.NVarChar, query);
            AddParameter(command, "@OwnerId", SqlDbType.BigInt, ownerId);
            AddParameter(command, "@Stage", SqlDbType.Int, stage);
            AddParameter(command, "@TagKey", SqlDbType.UniqueIdentifier, new Guid(tagKey));
            AddParameter(command, "@HandlerId", SqlDbType.UniqueIdentifier,handlerId);
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

                    SqlDataReader sqlReader = command.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        Article article = ObjectConverter.GetAs<Article>(sqlReader);
                        article.IsDirty = false;

                        //set the owner for the article
                        long owner = (long)sqlReader["ownerId"];
                        article.Owner = UserStore.Instance.LoadUser(owner);

                        articles.Add(article);
                    }

                    // in the next dataset, tags will be coming.
                    if (sqlReader.NextResult())
                    {
                        while (sqlReader.Read())
                        {
                            Tag tag = ObjectConverter.GetAs<Tag>(sqlReader);

                            //read the resourceId
                            long resourceId = (long)sqlReader["ResourceId"];

                            Article matched = articles.Find(delegate(Article a) { return a.Id == resourceId; });
                            if (matched != null)
                            {
                                matched.Tags.Add(tag);
                            }
                        }
                    }

                    if (!sqlReader.IsClosed)
                    {
                        sqlReader.Close();
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

            return articles;
        }

        public IList<Article> GetArticlesInPath(string path, long ownerId, int stage, string tagKey, Guid handlerId, int start, int size, out int total)
        {
            List<Article> articles = new List<Article>();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Article_GetListInPath";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UrlPath", SqlDbType.NVarChar, path);
            AddParameter(command, "@OwnerId", SqlDbType.BigInt, ownerId);
            AddParameter(command, "@Stage", SqlDbType.Int, stage);
            AddParameter(command, "@TagKey", SqlDbType.UniqueIdentifier, new Guid(tagKey));
            AddParameter(command, "@HandlerId", SqlDbType.UniqueIdentifier, handlerId);
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

                    SqlDataReader sqlReader = command.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        Article article = ObjectConverter.GetAs<Article>(sqlReader);
                        article.IsDirty = false;

                        //set the owner for the article
                        long owner = (long)sqlReader["ownerId"];
                        article.Owner = UserStore.Instance.LoadUser(owner);

                        articles.Add(article);
                    }

                    // in the next dataset, tags will be coming.
                    if (sqlReader.NextResult())
                    {
                        while (sqlReader.Read())
                        {
                            Tag tag = ObjectConverter.GetAs<Tag>(sqlReader);

                            //read the resourceId
                            long resourceId = (long)sqlReader["ResourceId"];

                            Article matched = articles.Find(delegate(Article a) { return a.Id == resourceId; });
                            if (matched != null)
                            {
                                matched.Tags.Add(tag);
                            }
                        }
                    }

                    if (!sqlReader.IsClosed)
                    {
                        sqlReader.Close();
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

            return articles;
        }

        public void Publish(string urlKey)
        {

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Article_Publish";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UrlKey", SqlDbType.NVarChar, urlKey);

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

        public DataTable GetVersions(string urlKey)
        {
            DataTable table = new DataTable();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Article_GetVersions";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UrlKey", SqlDbType.NVarChar, urlKey);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader sqlReader = command.ExecuteReader();
                    //if (sqlReader.Read())
                    {
                        // fill the table
                        table.Load(sqlReader);
                    }

                    if (!sqlReader.IsClosed)
                    {
                        sqlReader.Close();
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            return table;
        }

        public void Delete(string urlKey)
        {

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Article_Delete";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UrlKey", SqlDbType.NVarChar, urlKey);

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

        public void Delete(string urlKey, int version)
        {

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Article_DeleteVersion";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UrlKey", SqlDbType.NVarChar, urlKey);
            AddParameter(command, "@Version", SqlDbType.Int, version);

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

        public void Revert(string urlKey, int version)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Article_Revert";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UrlKey", SqlDbType.NVarChar, urlKey);
            AddParameter(command, "@Version", SqlDbType.Int, version);

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

        public Article GetArticleVersion(string urlKey, int version)
        {
            Article article = null;

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Article_GetByUrlVersion";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UrlKey", SqlDbType.NVarChar, urlKey);
            AddParameter(command, "@Version", SqlDbType.Int, version);

            logger.Log(LogLevel.Debug, "calling Proc_Ratna_Article_GetByUrl {0}, {1}, {2}", Jardalu.Ratna.Core.WebContext.Current.Site.Id, urlKey, version);

            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader sqlReader = command.ExecuteReader();
                    if (sqlReader.Read())
                    {
                        article = ObjectConverter.GetAs<Article>(sqlReader);

                        //set the owner for the article
                        long owner = (long)sqlReader["ownerId"];
                        article.Owner = UserStore.Instance.LoadUser(owner);

                        article.IsDirty = false;
                    }

                    // in the next dataset, tags will be coming.
                    if (sqlReader.NextResult())
                    {
                        while (sqlReader.Read())
                        {
                            Tag tag = ObjectConverter.GetAs<Tag>(sqlReader);
                            article.Tags.Add(tag);
                        }
                    }

                    if (!sqlReader.IsClosed)
                    {
                        sqlReader.Close();
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            return article;
        }

        public void Delete(IList<string> urlKeys)
        {
            foreach (string urlKey in urlKeys)
            {
                Delete(urlKey);
            }
        }

        public void Publish(IList<string> urlKeys)
        {
            foreach (string urlKey in urlKeys)
            {
                Publish(urlKey);
            }
        }

        public void SetArticleTimes(string urlKey, DateTime createdDate, DateTime lastUpdateDate, DateTime publishedDate)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_Article_SetTimes";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UrlKey", SqlDbType.NVarChar, urlKey);
            AddParameter(command, "@CreatedDate", SqlDbType.DateTime, createdDate);
            AddParameter(command, "@LastModifiedDate", SqlDbType.DateTime, lastUpdateDate);
            AddParameter(command, "@PublishedDate", SqlDbType.DateTime, publishedDate);

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
                case ArticleErrorCodes.UrlKeyAlreadyInUse:
                    throw new MessageException(ResourceManager.GetMessage(ArticleMessages.UrlKeyAlreadyInUse));

                case ArticleErrorCodes.VersionHasBeenUpdated:
                    throw new MessageException(ResourceManager.GetMessage(ArticleMessages.VersionHasBeenUpdated));

                case ArticleErrorCodes.NoArticleWithUrlKey:
                    throw new ObjectNotFoundException( ArticleErrorCodes.NoArticleWithUrlKey,  ResourceManager.GetMessage(ArticleMessages.NoArticleWithUrlKey));

                case ArticleErrorCodes.VersionNotFound:
                    throw new ObjectNotFoundException(ArticleErrorCodes.VersionNotFound, ResourceManager.GetMessage(ArticleMessages.NoArticleWithUrlKeyAndVersion));

                case ArticleErrorCodes.VersionCannotBeReverted:
                    throw new MessageException(ArticleErrorCodes.VersionCannotBeReverted, ResourceManager.GetMessage(ArticleMessages.ArticleCannotBeRevertedToVersion));

                default:
                    base.ThrowError(errorCode);
                    break;
            }
        }

        #endregion

    }

}
