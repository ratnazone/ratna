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
    using System.Xml.Serialization;

    using Jardalu.Ratna.Core.Tags;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Resource;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Core.Navigation;

    #endregion

    [DataContract]
    public class StaticArticle : MetaArticle, INavigationTag
    {

        #region private fields

        [DataMember]
        private string body;

        [DataMember]
        private string head;

        [DataMember]
        private string navtag;

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(StaticArticle));

        #endregion

        #region Public Properties

        string INavigationTag.Name
        {
            get { return this.navtag; }
            set { this.navtag = value; }
        }

        public string Head
        {
            get
            {
                return this.head;
            }
            set
            {
                this.head = value;
                this.IsDirty = true;
            }
        }

        public string Body
        {
            get
            {
                return this.body;
            }
            set
            {
                this.body = value;
                this.IsDirty = true;
            }
        }

        public override string RawData
        {
            get
            {
                this.Prepare();
                return rawData;
            }
            set
            {
                rawData = value;
                this.Populate();
            }
        }

        public override DataContractSerializer Serializer
        {
            get
            {
                return serializer;
            }
        }

        #endregion

        #region ctor

        public StaticArticle()
        {
            this.HandlerId = StaticArticleHandler.Instance.Id;
        }

        public StaticArticle(Article article)
            : this()
        {
            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            this.Copy(article);

        }

        #endregion

        #region public methods

        public override void CopySpecific(Article article)
        {

            base.CopySpecific(article);

            StaticArticle sa = article as StaticArticle;
            if (sa != null)
            {
                this.Head = sa.Head;
                this.Body = sa.Body;
                this.navtag = sa.navtag;
            }
        }

        #endregion

    }
}
