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
namespace Jardalu.Ratna.Store
{

    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Database;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Resource;
    using System.Data;
    using Jardalu.Ratna.Utilities;
    
    #endregion

    public class ArticleStore
    {

        #region private fields

        private static ArticleStore store;
        private static object syncRoot = new object();

        private Logger logger;

        #endregion

        #region ctor

        private ArticleStore()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region public properties

        public static ArticleStore Instance
        {
            get
            {
                if (store == null)
                {
                    lock (syncRoot)
                    {
                        if (store == null)
                        {
                            store = new ArticleStore();
                        }
                    }
                }

                return store;
            }
        }
       
        #endregion

        #region public methods

        public IList<Article> ReadInPath(ArticleHandler handler, string path, int start, int size, out int total)
        {
            return ReadInPath(handler, path, UserStore.Instance.Guest.Id, PublishingStage.Published, start, size, out total);
        }

        public IList<Article> ReadInPath(ArticleHandler handler, string path, long ownerId, PublishingStage stage, int start, int size, out int total)
        {
            #region argument

            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            #endregion

            return ArticleDbInteractor.Instance.GetArticlesInPath(path, ownerId, (int)stage, Article.ArticleTagKey, handler.Id, start, size, out total);
        }

        public IList<Article> Read(string query, int start, int size, out int total)
        {
            return Read(query, UserStore.Instance.Guest.Id, PublishingStage.Published, start, size, out total);
        }

        public IList<Article> Read(string query, long ownerId, PublishingStage stage, int start, int size, out int total)
        {
            return ArticleDbInteractor.Instance.GetArticles(query, ownerId, (int)stage, Article.ArticleTagKey, BlogArticleHandler.HandlerIdGuid, start, size, out total);
        }

        public IList<Article> Read(ArticleHandler handler, string query, long ownerId, int start, int size, out int total)
        {
            return Read(handler, query, ownerId, PublishingStage.Published, start, size, out total);
        }

        public IList<Article> Read(ArticleHandler handler, string query, long ownerId, PublishingStage stage, int start, int size, out int total)
        {
            #region argument

            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            #endregion

            return ArticleDbInteractor.Instance.GetArticles(query, ownerId, (int)stage, Article.ArticleTagKey, handler.Id, start, size, out total);
        }
        
        public Article Save(Guid handlerId, Article article)
        {
            return InternalSave(handlerId, article, true);
        }

        public Article Update(Guid handlerId, Article article)
        {
            return InternalSave(handlerId, article, false);
        }

        public Article GetArticle(string urlKey, PublishingStage stage)
        {

            #region argument

            if (string.IsNullOrEmpty(urlKey))
            {
                throw new ArgumentNullException("urlKey");
            }

            #endregion

            return ArticleDbInteractor.Instance.GetArticle(Utility.SanitizeUrl(urlKey), (int)stage);

        }

        public Article GetArticle(string urlKey, int version)
        {
            #region argument

            if (string.IsNullOrEmpty(urlKey))
            {
                throw new ArgumentNullException("urlKey");
            }

            #endregion

            return ArticleDbInteractor.Instance.GetArticleVersion(Utility.SanitizeUrl(urlKey), version);
        }

        public bool TryGetArticle(string urlKey, PublishingStage stage, out Article article)
        {
            article = null;
            bool success = false;

            try
            {
                article = GetArticle(Utility.SanitizeUrl(urlKey), stage);
                success = true;
            }
            catch (MessageException mex)
            {
                logger.Log(LogLevel.Debug, "Article at [{0}] doesnot exist, exception - {1}", Utility.SanitizeUrl(urlKey), mex.Message);
            }

            return success;
        }

        public bool TryGetArticle(string urlKey, int version, out Article article)
        {
            article = null;
            bool success = false;

            try
            {
                article = GetArticle(Utility.SanitizeUrl(urlKey), version);
                success = true;
            }
            catch (MessageException mex)
            {
                logger.Log(LogLevel.Debug, "Article at [{0}] doesnot exist with version - {1}, exception - {2}", Utility.SanitizeUrl(urlKey), version, mex.Message);
            }

            return success;
        }

        public void Publish(Article article)
        {

            #region argument

            if (article == null)
            {
                throw new ArgumentNullException("article");
            }
            if (article.Id == 0)
            {
                throw new ArgumentException("article Id");
            }
            if (string.IsNullOrEmpty(article.UrlKey))
            {
                throw new ArgumentException("article UrlKey");
            }

            #endregion

            if (article.IsDirty)
            {
                throw new MessageException(ResourceManager.GetMessage(ArticleMessages.NeedsSavedForPublishing));
            }

            ArticleDbInteractor.Instance.Publish(article.UrlKey);

        }

        public bool Exists(string urlKey, PublishingStage stage)
        {
            Article a;

            return TryGetArticle(Utility.SanitizeUrl(urlKey), stage, out a);

        }

        public bool Exists(string urlKey)
        {
            bool exists = Exists(urlKey, PublishingStage.Published);
            if (!exists)
            {
                exists = Exists(urlKey, PublishingStage.Draft);
            }
            return exists;
        }

        public DataTable GetVersions(string urlKey)
        {
            return ArticleDbInteractor.Instance.GetVersions(urlKey);
        }

        public void Delete(string urlKey)
        {
            if (string.IsNullOrEmpty(urlKey))
            {
                throw new ArgumentNullException("urlKey");
            }

            ArticleDbInteractor.Instance.Delete(Utility.SanitizeUrl(urlKey));
        }

        public void Delete(IList<string> urlKeys)
        {
            if (urlKeys == null)
            {
                throw new ArgumentNullException("urlKeys");
            }

            ArticleDbInteractor.Instance.Delete(urlKeys);
        }

        public void Delete(string urlKey, int version)
        {
            if (string.IsNullOrEmpty(urlKey))
            {
                throw new ArgumentNullException("urlKey");
            }

            ArticleDbInteractor.Instance.Delete(Utility.SanitizeUrl(urlKey), version);
        }

        public void Publish(string urlKey)
        {
            if (string.IsNullOrEmpty(urlKey))
            {
                throw new ArgumentNullException("urlKey");
            }

            ArticleDbInteractor.Instance.Publish(Utility.SanitizeUrl(urlKey));
        }

        public void Publish(IList<string> urlKeys)
        {
            if (urlKeys == null)
            {
                throw new ArgumentNullException("urlKeys");
            }

            ArticleDbInteractor.Instance.Publish(urlKeys);
        }

        public void SetTimes(Article article)
        {
            if (article == null)
            {
                throw new ArgumentNullException();
            }

            ArticleDbInteractor.Instance.SetArticleTimes(article.UrlKey, 
                                article.CreatedDate, article.LastModifiedDate, article.PublishedDate);
        }

        public void Revert(string urlKey, int version)
        {
            if (string.IsNullOrEmpty(urlKey))
            {
                throw new ArgumentNullException("urlKey");
            }

            ArticleDbInteractor.Instance.Revert(Utility.SanitizeUrl(urlKey), version);
        }

        #endregion

        #region private methods

        private Article InternalSave(Guid handlerId, Article article, bool updateVersion)
        {
            #region argument checking

            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            if (string.IsNullOrEmpty(article.UrlKey))
            {
                throw new ArgumentException("UrlKey");
            }

            if (article.Owner == null)
            {
                throw new ArgumentException("Owner");
            }

            #endregion

            Article updatedArticle
                = ArticleDbInteractor.Instance.SaveArticle(
                     handlerId,
                     article.Owner.Id,
                     article.Title,
                     Utility.SanitizeUrl(article.UrlKey),
                     article.Version,
                     updateVersion,
                     article.RawData
                );
            return updatedArticle;
        }


        #endregion

    }
}
