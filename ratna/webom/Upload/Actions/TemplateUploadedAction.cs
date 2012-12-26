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
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Templates;
    using Jardalu.Ratna.Web.Templates;
    using Jardalu.Ratna.Web.Graphics;
    using Jardalu.Ratna.Web.Security;
    using Jardalu.Ratna.Utilities;

    using Ionic.Zip;

    #endregion

    public class TemplateUploadedAction : AbstractMediaUploadedAction
    {

        #region private fields

        private Logger logger;

        #endregion

        #region ctor

        public TemplateUploadedAction()
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

            // once the template has been uploaded.
            // unzip the files and delete the uploaded file
            string templateName = request["uploadtemplatename"];
            string templateUrlPath = request["uploadtemplateurlPath"];
            if (string.IsNullOrEmpty(templateName))
            {
                templateName = Constants.DefaultTemplateName;
            }

            if (string.IsNullOrEmpty(templateUrlPath))
            {
                templateUrlPath = Constants.RootUrl;
            }

            //get the virtual path of the template extraction
            string templateVirtualPath = TemplatePlugin.GetTemplateVirtualPath(templateName);

            //convert virtual path to real path
            string templateExtractPath = request.MapPath(templateVirtualPath);
            logger.Log(LogLevel.Debug, "Template file uploaded at : {0}.", actualPath);

            try
            {

                string master = null;

                using (ZipFile zip = ZipFile.Read(actualPath))
                {
                    master = GetMasterFileName(zip);

                    if (Directory.Exists(templateExtractPath))
                    {
                        logger.Log(LogLevel.Info, "Cleaning folder {0}.", templateExtractPath);
                        Directory.Delete(templateExtractPath, true);
                    }

                    // create or empty the existing template data
                    if (!Directory.Exists(templateExtractPath))
                    {
                        logger.Log(LogLevel.Info, "Creating folder {0}.", templateExtractPath);
                        Directory.CreateDirectory(templateExtractPath);
                    }

                    logger.Log(LogLevel.Info, "Extracting template at folder {0}.", templateExtractPath);

                    foreach (ZipEntry entry in zip)
                    {
                        entry.Extract(templateExtractPath, ExtractExistingFileAction.OverwriteSilently);
                    }
                }

                // save the template.
                Template template = TemplatePlugin.Instance.GetTemplateUrlPath(templateUrlPath);
                Template newTemplate = new Template()
                {
                    Name = templateName,
                    TemplatePath = templateName,
                    MasterFileName = master,
                    UrlPath = templateUrlPath
                };

                if (template != null)
                {
                    //delete the existing template.
                    TemplatePlugin.Instance.Delete(template.UId);
                }

                TemplatePlugin.Instance.Add(newTemplate);
                Dictionary<string, object> properties = new Dictionary<string, object>(1);
                properties["Id"] = newTemplate.Id;
                this.Properties = properties;
                this.Success = true;
            }
            finally
            {
                //delete the uploaded zip file
                this.DeleteUploaded = true;
            }
        }


        #region private methods

        private string GetMasterFileName(ZipFile zip)
        {
            string master = null;

            foreach (ZipEntry entry in zip)
            {
                logger.Log(LogLevel.Debug, "ZipEntry : {0}", entry.FileName);

                if (entry.FileName.EndsWith(".master", StringComparison.OrdinalIgnoreCase))
                {
                    if (master == null)
                    {
                        master = entry.FileName;
                    }
                    else
                    {
                        // zip file cannot have more than one master.
                        MessageException me = new MessageException(
                                Resource.ResourceManager.GetLiteral("Admin.Templates.Zip.MoreThanOneMaster")
                            );
                        throw me;
                    }
                }
            }

            if (master == null)
            {
                // must have atleast one master file.
                MessageException me = new MessageException(
                            Resource.ResourceManager.GetLiteral("Admin.Templates.Zip.NoMaster")
                        );
                throw me;
            }

            return master;
        }

        #endregion

    }

}
