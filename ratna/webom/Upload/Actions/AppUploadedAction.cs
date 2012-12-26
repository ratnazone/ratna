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
    using System.Web;

    using Jardalu.Ratna.Core.Apps;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Applications;

    #endregion

    public class AppUploadedAction : AbstractMediaUploadedAction
    {

        #region private fields

        private Logger logger;

        #endregion

        #region ctor

        public AppUploadedAction()
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

            try
            {

                App app = AppInstaller.Install(actualPath);
                Dictionary<string, object> properties = new Dictionary<string, object>(1);
                properties["Id"] = app.Id;
                this.Properties = properties;
                this.Success = true;

            }
            finally
            {
                //delete the uploaded zip file
                this.DeleteUploaded = true;
            }
        }

    }

}
