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

    using Jardalu.Ratna.Core.Media;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web.Graphics;
    using Jardalu.Ratna.Web.Security;

    #endregion

    public class DocumentUploadedAction : AbstractMediaUploadedAction
    {
        public override void Saved(HttpRequest request, string actualPath, string vpath)
        {
            #region arguments

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            #endregion

            // add an entry to media
            Document document = new Document();
            document.Url = vpath;

            try
            {
                document.Owner = AuthenticationUtility.Instance.GetLoggedUser();
                document.Name = Path.GetFileName(actualPath);

                //save the media
                BaseMedia media = MediaStore.Instance.Save(document);

                Dictionary<string, object> properties = new Dictionary<string, object>(1);
                properties["Id"] = media.Id;
                properties["name"] = media.Name;
                this.Properties = properties;

                this.Success = true;
            }
            catch
            {
                // failure mapping the image.
                this.Success = false;
                this.DeleteUploaded = true;
            }

        }
    }

}
