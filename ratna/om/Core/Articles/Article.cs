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
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    using Jardalu.Ratna.Core.Acls;
    using Jardalu.Ratna.Core.Tags;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Resource;
    using Jardalu.Ratna.Store;

    #endregion

    [DataContract]
    public class Article : TagResource, ICrudObject, ISerializableObject
    {

        #region private fields

        private string urlKey;
        private string title;
        protected string rawData;

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(Article));

        public const string ArticleTagKey = "cc36802b-3f3c-49a8-83ca-b01592365823";

        #endregion

        #region Public Properties

        public long Id
        {
            get;
            set;
        }

        public virtual string UrlKey
        {
            get
            {
                return this.urlKey;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                if (value[0] != '/' || value.Length == 1)
                {
                    // article url must start with '/'
                    throw new ArgumentException("value");
                }

                //if (this.Id != 0)
                //{
                //    // once the article has an Id, its url cannot be updated
                //    throw new InvalidOperationException("Id not 0");
                //}

                this.urlKey = value;
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.title = value;
            }
        }

        public Guid HandlerId { get; set; }

        public int Version { get; set; }
        public PublishingStage Stage { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime PublishedDate { get; set; }

        #endregion

        #region ctor

        public Article()
        {
            //make sure the tag is registered
            RegisterTagKey(new Guid(TagKey));
        }

        #endregion

        #region public properties

        public virtual string TagKey
        {
            get
            {
                return ArticleTagKey;
            }
        }

        #endregion

        #region public methods


        public void Update()
        {
            if (string.IsNullOrEmpty(this.UrlKey))
            {
                throw new InvalidOperationException("UrlKey not specified");
            }

            if (string.IsNullOrEmpty(this.Title))
            {
                throw new InvalidOperationException("Empty title");
            }

            Article updated = ArticleStore.Instance.Update(this.HandlerId, this);

            // make sure that the article is saved
            if (updated == null)
            {
                throw new ObjectNotFoundException(ArticleErrorCodes.NoArticleWithUrlKey, 
                    ResourceManager.GetMessage(ArticleMessages.NoArticleWithUrlKey));
            }

            if (this.Id == 0)
            {
                this.Id = updated.Id;
            }
            
            if (this.ResourceId == 0)
            {
                this.ResourceId = updated.ResourceId;
            }

            if (this.IsTagDirty && Tags.Count > 0)
            {
                TagStore.Instance.AddTags(new Guid(TagKey), this.ResourceId, this.Tags);
                this.IsTagDirty = false;
            }

            this.IsDirty = false;
        }

        public void Publish()
        {
            if (string.IsNullOrEmpty(this.UrlKey))
            {
                throw new InvalidOperationException("UrlKey not specified");
            }

            ArticleStore.Instance.Publish(this);
        }

        public void Delete()
        {
            if (string.IsNullOrEmpty(this.UrlKey))
            {
                throw new InvalidOperationException("UrlKey not specified");
            }

            ArticleStore.Instance.Delete(this.UrlKey);
        }

        public void Read()
        {
            Read(PublishingStage.Published);
        }

        public void Read(PublishingStage stage)
        {
            if (string.IsNullOrEmpty(this.UrlKey))
            {
                throw new InvalidOperationException("UrlKey not specified");
            }

            User user = UserStore.Instance.Guest;
            Context context = Context.Current;
            if (context != null && context.User != null)
            {
                user = context.User;
            }

            Article a = ArticleStore.Instance.GetArticle(this.UrlKey, stage);

            if (a == null)
            {
                throw new ObjectNotFoundException(
                                ArticleErrorCodes.NoArticleWithUrlKey, 
                                ResourceManager.GetMessage(ArticleMessages.NoArticleWithUrlKey)
                            );
            }

            if (stage != PublishingStage.Published)
            {
                // make sure the user is allowed to read the article.
                if (!a.HasPermission(user, AclType.Read))
                {
                    throw new AccessDeniedException(ArticleErrorCodes.AccessDenied, ResourceManager.GetMessage(ArticleMessages.AccessDenied));
                }
            }

            this.Copy(a);

            
            //read the tags
            //this.InitializeTags(TagStore.Instance.GetTags(new Guid(TagKey), this.Id));
        }

        public void Create()
        {
            if (string.IsNullOrEmpty(this.UrlKey))
            {
                throw new InvalidOperationException("UrlKey not specified");
            }

            Update();
        }

        /// <summary>
        /// SetTime will set the CreatedDate, LastModifiedDate and PublishedDate assigned to the
        /// article. This method should be used during migration to assign specific dates to the
        /// article.
        /// </summary>
        public void SetTimes()
        {
            ArticleStore.Instance.SetTimes(this);
        }

        public bool Exists()
        {
            return Exists(PublishingStage.Published);
        }

        public bool Exists(PublishingStage stage)
        {

            if (string.IsNullOrEmpty(this.UrlKey))
            {
                throw new InvalidOperationException("UrlKey not specified");
            }

            return ArticleStore.Instance.Exists(this.UrlKey, stage);
        }

        public virtual void Copy(Article article)
        {
            #region argument

            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            #endregion

            this.Title = article.Title;
            this.Owner = article.Owner;
            this.HandlerId = article.HandlerId;
            if (this.UrlKey == null)
            {
                this.UrlKey = article.UrlKey;
            }
            this.Version = article.Version;
            this.RawData = article.RawData;
            this.Stage = article.Stage;
            this.PublishedDate = article.PublishedDate;
            this.CreatedDate = article.CreatedDate;
            this.LastModifiedDate = article.LastModifiedDate;
            
            //copy the tags
            this.InitializeTags(article.Tags);

            // set the Id only if there was no Id to start with
            if (this.Id == 0)
            {
                this.Id = article.Id;
            }
        }

        public virtual void CopySpecific(Article article)
        {
        }

        #endregion

        #region ISerializableObject

        public virtual string RawData
        {
            get
            {
                return rawData;
            }
            set
            {
                rawData = value;
            }
        }

        public virtual string Type
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        public virtual DataContractSerializer Serializer
        {
            get
            {
                return serializer;
            }
        }

        public bool IsDirty
        {
            get;
            set;
        }

        public virtual void Prepare()
        {
            if (IsDirty)
            {
                XmlWriterSettings ws = new XmlWriterSettings();
                ws.OmitXmlDeclaration = false;
                StringBuilder buffer = new StringBuilder();

                //real preparation
                using (XmlWriter writer = XmlWriter.Create(buffer, ws))
                {
                    Serializer.WriteObject(writer, this);
                }

                //set the rawdata finally
                rawData = buffer.ToString();

                IsDirty = false;
            }
        }

        public virtual void Populate()
        {
            if (!string.IsNullOrEmpty(this.rawData))
            {

                try
                {

                    //real preparation
                    using (XmlReader reader = XmlReader.Create(new StringReader(this.rawData)))
                    {
                        Article ba = Serializer.ReadObject(reader) as Article;
                        this.CopySpecific(ba);
                    }

                }
                catch (InvalidOperationException)
                {
                    //invalid deserialization, just ignore.
                }

                IsDirty = false;

            }
        }

        public virtual void CopySpecific(ISerializableObject serialzableObject)
        {
        }

        #endregion

        #region overrides

        public override bool Equals(object obj)
        {
            Article a = obj as Article;
            if (a == null)
            {
                return false;
            }

            return (this.Id == a.Id);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

    }

}
