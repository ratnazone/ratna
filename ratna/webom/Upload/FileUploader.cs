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
namespace Jardalu.Ratna.Web.Upload
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Management;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Saver;
    using Jardalu.Ratna.Web.Security;
    using Jardalu.Ratna.Web.Service;
    using Jardalu.Ratna.Web.Upload.Actions;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public class FileUploader : IHttpHandler
    {

        #region private fields

        private Logger logger;

        #endregion

        #region ctor

        public FileUploader()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region public methods

        public void ProcessRequest(HttpContext context)
        {
            logger.Log(LogLevel.Debug, "ProcessRequest called");

            ServiceOutput serviceOutput = new ServiceOutput();

            try
            {
                UploadType uploadType = GetUploadType(context);

                logger.Log(LogLevel.Debug, "Upload type for the uploaded file : {0}", uploadType);

                if (uploadType != UploadType.None)
                {

                    // uploads cannot be anonymous
                    User user = AuthenticationUtility.Instance.GetLoggedUser();
                    if (user != null)
                    {
                        HttpPostedFile file = context.Request.Files[0];
                        if (file != null)
                        {
                            string filepath = Save(file);
                            string vpath = ResolveFilePathWithoutSite(filepath);
                            logger.Log(LogLevel.Debug, "Virtual path for the saved file : {0}", vpath);

                            IFileUploadAction action = null;

                            #region action init

                            switch (uploadType)
                            {
                                case UploadType.ProfilePhoto:
                                    action = new ProfilePhotoUploadedAction();
                                    break;

                                case UploadType.Photo:
                                    action = new PhotoUploadedAction();
                                    break;

                                case UploadType.BlogPhoto:
                                    action = new BlogPhotoUploadedAction();
                                    break;

                                case UploadType.Document:
                                    action = new DocumentUploadedAction();
                                    break;

                                case UploadType.Video:
                                    action = new VideoUploadedAction();
                                    break;

                                case UploadType.Template:
                                    action = new TemplateUploadedAction();
                                    break;

                                case UploadType.App:
                                    action = new AppUploadedAction();
                                    break;

                                case UploadType.Other:
                                    action = new OtherUploadedAction();
                                    break;
                            }

                            #endregion

                            if (action != null)
                            {

                                logger.Log(LogLevel.Debug, "Calling action on the saved file : {0}", action);

                                // call action Saved to ensure the action
                                // is taking other steps necessary for the uploaded.
                                action.Saved(context.Request, filepath, vpath);

                                if (action.Success)
                                {

                                    IDictionary<string, object> properties = action.Properties;

                                    // add all the properties from action
                                    // to the output.
                                    if (properties != null)
                                    {
                                        foreach (KeyValuePair<string, object> kvp in properties)
                                        {
                                            serviceOutput.AddOutput(kvp.Key, kvp.Value);
                                        }
                                    }

                                    serviceOutput.Success = true;
                                }
                                else
                                {
                                    serviceOutput.Success = false;
                                }

                                // make sure, that action is asking to keep the file intact
                                if (action.DeleteUploaded)
                                {
                                    // delete the uploaded file.
                                    File.Delete(filepath);
                                }

                            }
                            else
                            {
                                serviceOutput.Success = true;
                            }

                            serviceOutput.AddOutput("location", vpath);
                        }
                    }
                }
            }
            catch (System.Web.HttpException hex)
            {
                logger.Log(LogLevel.Error, "Error uploading, error code : {0}, exception : {1}", hex.GetHttpCode(), hex);
                if (hex.WebEventCode == WebEventCodes.RuntimeErrorPostTooLarge)
                {
                    serviceOutput.AddOutput(Constants.Json.Error,
                        string.Format(ResourceManager.GetLiteral("Errors.UploadMaxSizeExceeded"),
                        Configuration.GetMaxUploadSize())
                    );
                }
                else
                {
                    serviceOutput.AddOutput(Constants.Json.Error, hex.Message);
                }
            }
            catch (Exception ex)
            {
                //serviceOutput false by default
                logger.Log(LogLevel.Error, "Error uploading : {0}", ex);
                serviceOutput.AddOutput(Constants.Json.Error, ex.Message);
            }

            context.Response.ContentType = "text/html";
            context.Response.Write(serviceOutput.GetJson());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region private methods

        private UploadType GetUploadType(HttpContext context)
        {
            UploadType uploadType = UploadType.None;

            string upType = context.Request["uploadtype"];

            if (!Enum.TryParse<UploadType>(upType, true, out uploadType))
            {
                uploadType = UploadType.None;
            }

            return uploadType;
        }

        private string Save(HttpPostedFile file)
        {
            string originalFileName = file.FileName;

            //don't allow space in the file name
            if (originalFileName != null)
            {
                originalFileName = originalFileName.Replace(" ", "_");
            }

            FileSaver saver = new FileSaver();

            //get the save folder name
            string savePath = saver.GetSaveFolderName();

            DirectoryInfo pathInfo = new DirectoryInfo(
                                                ResolveSavePathForSite(savePath)
                                            );

            DirectoryInfo sitePathInfo = new DirectoryInfo(ResolveSavePathForSite(""));

            logger.Log(LogLevel.Debug, "Path were file will be saved : {0}", pathInfo.FullName);

            string fileName = FileNamer.GenerateFileName(originalFileName, pathInfo);

            //save the original file
            string actualFilePath = saver.Save(file.InputStream, fileName, sitePathInfo);

            logger.Log(LogLevel.Debug, "File saved at : {0}", actualFilePath);

            return actualFilePath;
        }

        private string ResolveSavePathForSite(string savePath)
        {

            // get the pathInfo for the site
            DirectoryInfo sitePathInfo = new DirectoryInfo(
                                                Path.Combine(Configuration.Instance.UploadFolderInfo.FullName, savePath)
                                            );

            return sitePathInfo.FullName;
        }

        private string ResolveFilePathWithoutSite(string filePath)
        {
            // reconstruct the virtual path by removing the site-id information.
            // example filePath - C:\inetpub\wwwroot\ratna\sites\3\content\2012\10\29\front.jpg
            // will translate to ~/r-content/2012\10\29\front.jpg

            // get the site Id
            string siteId = WebContext.Current.Site.Id.ToString();

            // get the pathInfo for the site
            DirectoryInfo sitePathInfo = Configuration.Instance.UploadFolderInfo;

            logger.Log(LogLevel.Debug, "sitePathInfo : {0}", sitePathInfo.FullName);

            string suffixPath = filePath.Substring(sitePathInfo.FullName.Length);

            if (suffixPath.StartsWith("\\"))
            {
                suffixPath = suffixPath.Substring(1);
            }

            logger.Log(LogLevel.Debug, "Suffix path for the actual file [{0}] is : {1}", filePath, suffixPath);

            string path = Path.Combine(SitePaths.ContentVirtualPath, suffixPath);
            path = path.Replace('\\', '/');

            logger.Log(LogLevel.Debug, "Virtual path for the actual file [{0}] is : {1}", filePath, path);

            return path;
        }

        #endregion

    }

}
