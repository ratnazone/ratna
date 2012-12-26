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
    using Jardalu.Ratna.Core.Media;
    
    #endregion

    public class MediaStore
    {

        #region private fields

        private static MediaStore store;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private MediaStore()
        {
        }

        #endregion

        #region public properties

        public static MediaStore Instance
        {
            get
            {
                if (store == null)
                {
                    lock (syncRoot)
                    {
                        if (store == null)
                        {
                            store = new MediaStore();
                        }
                    }
                }

                return store;
            }
        }
       
        #endregion

        #region public methods

        public IList<BaseMedia> Read(string query, MediaType mediaType, long ownerId, int start, int size, out int total)
        {
            return MediaDbInteractor.Instance.GetMedium(query, mediaType, ownerId, BaseMedia.MediaTagKey, start, size, out total);
        }

        public IList<BaseMedia> Read(string query, MediaType mediaType, int start, int size, out int total)
        {
            return Read(query, mediaType, -1, start, size, out total);
        }

        public IList<BaseMedia> Read(MediaType mediaType, int start, int size, out int total)
        {
            return Read(String.Empty, mediaType, start, size, out total);
        }

        public BaseMedia Save(BaseMedia media)
        {
            #region argument checking

            if (media == null)
            {
                throw new ArgumentNullException("media");
            }

            if (string.IsNullOrEmpty(media.Url))
            {
                throw new ArgumentNullException("media.Url");
            }

            if (string.IsNullOrEmpty(media.Name))
            {
                throw new ArgumentNullException("media.Name");
            }

            if (media.Owner == null)
            {
                throw new InvalidOperationException("media.Owner");
            }

            #endregion

            return MediaDbInteractor.Instance.SaveMedia(media.Owner.Id, media.MediaType, media.Name, media.Url, media.RawData ?? string.Empty);
        }

        public BaseMedia GetMedia(string url)
        {

            #region argument

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            #endregion

            return MediaDbInteractor.Instance.GetMedia(SanitizeUrlKey(url));

        }

        public IList<BaseMedia> GetMedia(IList<string> urls)
        {
            #region argument

            if (urls == null)
            {
                throw new ArgumentNullException("urls");
            }

            #endregion

            //sanitize urls
            IList<string> sanitizedUrls = new List<string>(urls.Count);
            foreach (string url in urls)
            {
                sanitizedUrls.Add(SanitizeUrlKey(url));
            }

            return MediaDbInteractor.Instance.GetMedia(sanitizedUrls);

        }

        public bool TryGetMedia(string url, out BaseMedia media)
        {
            media = null;
            bool success = false;

            try
            {
                media = GetMedia(SanitizeUrlKey(url));
                success = true;
            }
            catch (MessageException)
            {
            }

            return success;
        }

        public bool Exists(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            BaseMedia a;
            
            return TryGetMedia(SanitizeUrlKey(url), out a);

        }

        public void Delete(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            MediaDbInteractor.Instance.Delete(url);
        }

        #endregion

        #region private methods

        private static string SanitizeUrlKey(string urlKey)
        {
            // UrlKey should never end with /
            string sanitized = urlKey;

            if (!string.IsNullOrEmpty(sanitized) && sanitized.Length > 1)
            {
                if (sanitized[sanitized.Length - 1] == '/')
                {
                    sanitized = sanitized.Substring(0, sanitized.Length - 1);
                }
            }

            return sanitized;
        }

        #endregion

    }
}
