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

    using Jardalu.Ratna.Exceptions;
    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Plugins;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.Service;

    #endregion

    [ScriptService]
    public class gallery : ServiceBase
    {

        #region private fields

        private Logger logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region public methods

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Save(string uid, string url, string name, string description, string nav)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            Guid guid = Guid.Empty;
            if (Guid.TryParse(uid, out guid))
            {
                try
                {
                    // make sure name, and url are not empty
                    if (string.IsNullOrEmpty(name) || 
                        string.IsNullOrEmpty(url) || 
                        !Uri.IsWellFormedUriString(url, UriKind.Relative) ||
                        !url.StartsWith("/"))
                    {
                        throw new MessageException(ResourceManager.GetLiteral("Admin.Media.Gallery.Error.NameUrlInvalid"));
                    }

                    Gallery gallery = ReadOrThrow(guid, name, url, description, nav);

                    if (gallery != null)
                    {

                        GalleryPlugin.Instance.Save(gallery);

                        output.AddOutput("uid", gallery.UId);
                        output.Success = true;
                    }
                }
                catch (MessageException me)
                {
                    output.AddOutput(Constants.Json.Error, me.Message);
                }

            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Delete(string url)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(url))
            {
                GalleryPlugin.Instance.Delete(Gallery.KeyName, url);

                // add the location to the output as well.
                output.AddOutput("url", url);
                output.Success = true;
            }

            return output.GetJson();
        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string AddPhotos(string url, string[] photos)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(url))
            {
                // read the gallery
                Gallery gallery = GalleryPlugin.Instance.Read(url);

                if (gallery != null && photos != null && photos.Length > 0)
                {
                    foreach (string photo in photos)
                    {
                        if (!string.IsNullOrEmpty(photo))
                        {
                            // add image to the gallery
                            gallery.Add(photo);
                        }
                    }
                }

                //save 
                GalleryPlugin.Instance.Save(gallery);
                output.Success = true;
            }

            return output.GetJson();
        }

        #endregion


        #region private methods

        private Gallery ReadOrThrow(Guid guid, string name, string url, string description, string nav)
        {
            Gallery gallery = null;

            // no changes for non-guid entry.
            if (guid == Guid.Empty)
            {
                gallery = new Gallery();
                gallery.Url = url;
            }
            else
            {
                // read for the url
                gallery = GalleryPlugin.Instance.Read(url);
                if (gallery == null || gallery.UId != guid)
                {
                    // cannot overwrite existing gallery
                    throw new MessageException(ResourceManager.GetLiteral("Admin.Media.Gallery.Error.AlreadyExists"));
                }
            }

            if (gallery != null)
            {
                gallery.Title = name;
                gallery.Description = description;
                gallery.Navigation = nav;
            }

            return gallery;
        }

        #endregion

    }
}
