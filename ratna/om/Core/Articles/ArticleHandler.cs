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
namespace Jardalu.Ratna.Core.Articles
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Store;
    
    #endregion

    [DataContract]
    public abstract class ArticleHandler : SystemPlugin
    {

        #region private fields
        
        [DataMember]
        private bool renderItem = true;
        [DataMember]
        private bool renderCollection;

        [DataMember]
        private string itemPage;
        [DataMember]
        private string collectionPage;
        
        #endregion

        #region ctor

        public ArticleHandler()
        {
        }

        #endregion

        #region public properties

        public string ItemPage 
        {
            get { return this.itemPage; }

            protected set
            {
                this.itemPage = value;
                this.IsDirty = true;
            }
        }

        public string CollectionPage
        {
            get
            {
                return this.collectionPage;
            }
            protected set
            {
                this.collectionPage = value;
                this.IsDirty = true;
            }
        }

        public bool CanRenderItem
        {
            get
            {
                return renderItem;
            }
            protected set
            {
                renderItem = value;
                this.IsDirty = true;
            }
        }

        public bool CanRenderCollection
        {
            get
            {
                return renderCollection;
            }
            protected set
            {
                renderCollection = value;
                this.IsDirty = true;
            }
        }

        public override string RawData
        {
            get
            {
                this.Prepare();
                return base.RawData;
            }
            set
            {
                base.RawData = value;
                this.Populate();
            }
        }

        #endregion

        #region public methods

        public virtual Article Read(string urlKey)
        {
            #region argument checking

            if (string.IsNullOrEmpty(urlKey))
            {
                throw new ArgumentNullException("urlKey");
            }

            #endregion

            throw new NotImplementedException();
        }

        public virtual IList<Article> ReadInPath(string path, int start, int size, out int total)
        {
            #region arguments

            if (start < 0)
            {
                throw new ArgumentException("start");
            }

            if (size < 0)
            {
                throw new ArgumentException("size");
            }

            #endregion
            
            //default path
            if (string.IsNullOrEmpty(path))
            {
                path = "/";
            }

            IList<Article> articles = ArticleStore.Instance.ReadInPath(this, path, start, size, out total);

            // promote the articles that can be handled by this handler.
            IList<Article> promotedArticles = new List<Article>(articles.Count);

            foreach (Article article in articles)
            {
                Article pArticle = article;
                if (article.HandlerId == this.Id)
                {
                    pArticle = new BlogArticle(article);
                }
                promotedArticles.Add(pArticle);
            }

            return promotedArticles;
        }

        public virtual IList<Article> Read(string query, int start, int size, out int total)
        {
            return Read(query, CoreConstants.User.AllUserId, start, size, out total);
        }

        public virtual IList<Article> Read(string query, long ownerId, int start, int size, out int total)
        {

            #region arguments

            if (start < 0)
            {
                throw new ArgumentException("start");
            }

            if (size < 0)
            {
                throw new ArgumentException("size");
            }

            #endregion

            IList<Article> articles = ArticleStore.Instance.Read(this, query, ownerId, start, size, out total);
            
            // promote the articles that can be handled by this handler.
            IList<Article> promotedArticles = new List<Article>(articles.Count);

            foreach (Article article in articles)
            {
                Article pArticle = article;
                if (article.HandlerId == this.Id)
                {
                    pArticle = new BlogArticle(article);
                }
                promotedArticles.Add(pArticle);
            }

            return promotedArticles;
        }

        public void Save(Article article)
        {
            #region argument checking

            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            if (article.Owner == null)
            {
                throw new InvalidOperationException("article.Owner");
            }

            #endregion

            Article updatedArticle = null;

            if (article.Id == 0)
            {
                updatedArticle = ArticleStore.Instance.Save(this.Id, article);
            }
            else
            {
                updatedArticle = ArticleStore.Instance.Update(this.Id, article);
            }

            if (updatedArticle != null)
            {
                article.Copy(updatedArticle);
                article.IsDirty = false;
            }

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

            ArticleStore.Instance.Publish(article);

        }

        #endregion

        #region ISerialzableObject

        public override void CopySpecific(ISerializableObject handler)
        {
            ArticleHandler ah = handler as ArticleHandler;
            if (ah != null)
            {
                this.ItemPage = ah.ItemPage;
                this.CollectionPage = ah.CollectionPage;
                this.CanRenderItem = ah.CanRenderItem;
                this.CanRenderCollection = ah.CanRenderCollection;
            }
        }

        public override string Type
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        #endregion

    }

}
