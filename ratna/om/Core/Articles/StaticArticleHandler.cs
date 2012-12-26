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

    public class StaticArticleHandler : ArticleHandler
    {
        #region private fields

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(StaticArticleHandler));

        public const string HandlerId = "88b1e4a1-5c12-4b7e-b68c-25b6a0836e24";
        private static object syncRoot = new object();
        public static StaticArticleHandler handler;

        #endregion

        #region ctor

        public StaticArticleHandler()
        {
            this.Name = "Static Article Handler";
            this.Id = new Guid(HandlerId);
            this.ItemPage = "~/Pages/static.aspx?url={UrlKey}&stage={Stage}&version={Version}";

            this.Register();
            this.Activate();
        }

        #endregion

        #region public properties

        public static StaticArticleHandler Instance
        {
            get
            {
                if (handler == null)
                {
                    lock (syncRoot)
                    {
                        if (handler == null)
                        {
                            handler = new StaticArticleHandler();
                        }
                    }
                }

                return handler;
            }
        }

        #endregion

        #region ISerialzableObject

        public override DataContractSerializer Serializer
        {
            get
            {
                return serializer;
            }
        }

        #endregion

        #region public methods

        public static bool CanHandle(Article article)
        {
            bool canHandle = false;

            if (article != null &&
                article.HandlerId.ToString() == HandlerId)
            {
                canHandle = true;
            }

            return canHandle;
        }

        #endregion
    }

}
