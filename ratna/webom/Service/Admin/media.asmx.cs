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
namespace Jardalu.Ratna.Web.Admin.service
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Web.Script.Services;
    using System.Web.Services;

    using Jardalu.Ratna.Web.Service;
    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Core.Media;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Utilities;

    #endregion

    [ScriptService]
    public class MediaService : ServiceBase
    {

        #region private fields

        private Logger logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Save(string url, string name, string tags, string mediaType)
        {
            // make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(url))
            {

                try
                {
                    BaseMedia media = null;

                    if (MediaStore.Instance.TryGetMedia(url, out media))
                    {
                        string mname = name ?? string.Empty;
                        if (!mname.Equals(media.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            media.Name = mname;
                        }

                        if (!string.IsNullOrEmpty(tags))
                        {
                            // update the tags
                            media.AddTags(tags);
                        }

                        media.Update();
                    }
                    else
                    {
                        // create a new media type
                        MediaType mType;

                        if (Enum.TryParse<MediaType>(mediaType, true, out mType))
                        {
                            switch (mType)
                            {
                                case MediaType.Photo:
                                    
                                    //try to get the width and height of the image
                                    Tuple<int, int> size = Jardalu.Ratna.Web.Utility.GetImageSize(url);

                                    Photo photo = new Photo();
                                    photo.Width = size.Item1;
                                    photo.Height = size.Item2;
                                    media = photo;

                                    output.AddOutput("width", photo.Width);
                                    output.AddOutput("height", photo.Height);
                                    break;
                                case MediaType.Video:
                                    media = new Video();
                                    break;
                                case MediaType.Document:
                                    media = new Document();
                                    break;
                            }
                        }

                        if (media != null)
                        {
                            media.Owner = user;
                            media.Url = url;
                            media.Name = name ?? string.Empty;
                            if (!string.IsNullOrEmpty(tags))
                            {
                                media.AddTags(tags);
                            }
                            media.Update();

                            output.AddOutput("location", url);
                        }
                    }

                    output.Success = true;

                }
                catch (MessageException me)
                {
                    // error saving the media
                    output.AddOutput(Constants.Json.Error, me.Message);
                }
                catch (Exception ex)
                {
                    // all other failures
                    logger.Log(LogLevel.Error, "Unable to save media, exception : {0}", ex);
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Delete(string url)
        {
            // make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(url))
            {
                MediaStore.Instance.Delete(url);
                output.Success = true;
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Search(string query, int start, int size)
        {
            // make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            int total;

            IList<BaseMedia> mediaList = MediaStore.Instance.Read(query, MediaType.Photo, start, size, out total);
            if (mediaList != null)
            {
                IList<string> urlList = new List<string>(mediaList.Count);
                foreach (BaseMedia media in mediaList)
                {
                    urlList.Add(media.Url);
                }

                output.AddOutput("urls", urlList);
                output.AddOutput("total", total);
            }

            output.Success = true;
            
            return output.GetJson();
        }

    }
}
