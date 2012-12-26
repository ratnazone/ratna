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
namespace Jardalu.Ratna.Web.Upload.Actions
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Web;

    using Jardalu.Ratna.Core.Apps;
    using Jardalu.Ratna.Core.Media;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Graphics;
    using Jardalu.Ratna.Web.Security;
    using Jardalu.Ratna.Web.Applications;


    #endregion

    public class PhotoUploadedAction : AbstractMediaUploadedAction
    {

        #region private fields

        private Logger logger;

        #endregion

        #region ctor

        public PhotoUploadedAction()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        public override void Saved(HttpRequest request, string actualPath, string vpath)
        {
            #region arguments

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            #endregion

            if (!SupportedImageTypes.IsSupported(vpath))
            {
                // image type is not supported. delete the file
                // and report failure.
                this.Success = false;
                this.DeleteUploaded = true;
            }
            else
            {

                logger.Log(LogLevel.Debug, "Supported file. Will add entry to media.");

                // call apps to work on the uploaded image
                AppEngine.ExecuteApps(AppEvent.ImageUploaded, actualPath);

                // add an entry to media
                Photo photo = new Photo();
                photo.Url = vpath;

                try
                {
                    // get the width and height of the photo
                    Image image = Image.FromFile(actualPath);

                    // get the width and height of the image
                    photo.Width = image.Width;
                    photo.Height = image.Height;
                    photo.Name = Path.GetFileName(actualPath);
                    photo.Owner = AuthenticationUtility.Instance.GetLoggedUser();

                    //save the media
                    BaseMedia media = MediaStore.Instance.Save(photo);

                    Dictionary<string, object> properties = new Dictionary<string, object>(1);
                    properties["Id"] = media.Id;
                    properties["name"] = media.Name;
                    properties["width"] = photo.Width;
                    properties["height"] = photo.Height;
                    this.Properties = properties;

                    this.Success = true;
                }
                catch(Exception exception)
                {
                    logger.Log(LogLevel.Error, "Unable to update the media information - {0}.", exception);

                    // failure mapping the image.
                    this.Success = false;
                    this.DeleteUploaded = true;
                }
            }

        }
    }

}
