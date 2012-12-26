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
namespace Jardalu.Ratna.Web.Applications
{

    #region using

    using System;
    using System.IO;
    using System.Web;
    using System.Xml;

    using Jardalu.Ratna.Core.Apps;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;

    using Ionic.Zip;


    #endregion

    internal static class AppInstaller
    {

        #region private fields

        private static Logger logger =  Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string allowedExtensions = ".xml;.dll;.asmx;.aspx;.jpg;.txt;.png";

        #endregion

        #region public methods

        public static App Install(string zipLocation)
        {
            #region argument
            
            if (string.IsNullOrEmpty(zipLocation))
            {
                throw new ArgumentNullException("zipLocation");
            }

            //must be zip file
            if (!zipLocation.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("zipLocation : {0} is not a zip file", zipLocation);
            }

            if (!File.Exists(zipLocation))
            {
                throw new ArgumentException("zipLocation : {0} not found", zipLocation);
            }

            HttpContext context = HttpContext.Current;

            if (context == null)
            {
                // cannot install without the HttpContext
                throw new InvalidOperationException("cannot perform without HttpContext");
            }

            #endregion

            logger.Log(LogLevel.Debug, "Install called for - {0}", zipLocation);

            // unzip the files and delete the uploaded file
            string appFolder = Utility.GetUniqueString();

            // get the install location for appFolder
            string appExtractPath = GetInstallLocation(appFolder);
            logger.Log(LogLevel.Debug, "App files uploaded at : {0}.", appExtractPath);

            App app;
            bool appValid = true;

            ExtractZip(zipLocation, appExtractPath);

            try
            {

                string manifest = null;

                #region manifest check
                if (!GetManifestContents(appExtractPath, out manifest))
                {
                    appValid = false;

                    // manifest file is missing for the app.
                    MessageException me = new MessageException(
                            Jardalu.Ratna.Web.Resource.ResourceManager.GetLiteral("Admin.Apps.ManifestMissing")
                        );
                    throw me;
                }
                #endregion

                app = GetAppFromManifest(manifest, out appValid);

                bool isUpgrade = false;
                bool isOverride = false;

                // check if the app is an upgrade/reload path
                App existingApp = AppStore.Instance.GetApp(app.UniqueId);
                if (existingApp != null)
                {
                    if (existingApp.UniqueId == app.UniqueId)
                    {
                        isOverride = true;

                        //check for version for upgrade
                        if (app.Version > existingApp.Version)
                        {
                            isUpgrade = true;
                        }
                        else if (app.Version < existingApp.Version)
                        {
                            // throw because the app cannot be downgraded.
                        }
                    }
                }

                //set the app location
                app.Location = appFolder;

                logger.Log(LogLevel.Debug, "Saving the created app");
                app.Update();

                logger.Log(LogLevel.Info, "App saved successfully");

                //remove the existing app
                if (isUpgrade || isOverride)
                {
                    // app has moved to a new location, delete from older folder
                    string existingAppFolder = GetInstallLocation(existingApp.Location);

                    logger.Log(LogLevel.Info, "Removing bits from {0} because app is upgraded.", existingAppFolder);

                    Directory.Delete(existingAppFolder, true);
                }
            }
            finally
            {
                //remove the uploaded folder, if the app was not valid
                if (!appValid)
                {
                    logger.Log(LogLevel.Info, "Cleaning folder {0} as app is invalid.", appExtractPath);
                    Directory.Delete(appExtractPath, true);
                }
            }

            return app;
        }

        public static void Uninstall(App app)
        {
            #region argument

            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            #endregion

            string location = GetInstallLocation(app.Location);
            logger.Log(LogLevel.Debug, "App installed at folder : {0}", location);

            if (Directory.Exists(location))
            {
                logger.Log(LogLevel.Debug, "Deleting folder : {0}", location);

                //delete the installed directory
                Directory.Delete(location, true);
            }


            AppStore.Instance.Delete(app.Id);
        }

        #endregion

        #region private methods

        private static void ExtractZip(string zipLocation, string extractPath)
        {
            using (ZipFile zip = ZipFile.Read(zipLocation))
            {

                if (Directory.Exists(extractPath))
                {
                    logger.Log(LogLevel.Info, "Cleaning folder {0}.", extractPath);
                    Directory.Delete(extractPath, true);
                }

                // create or empty the existing app files
                if (!Directory.Exists(extractPath))
                {
                    logger.Log(LogLevel.Info, "Creating folder {0}.", extractPath);
                    Directory.CreateDirectory(extractPath);
                }

                logger.Log(LogLevel.Info, "Extracting app at folder {0}.", extractPath);

                foreach (ZipEntry entry in zip)
                {
                    // ignore extensions
                    string fileName = entry.FileName;
                    string extension = Path.GetExtension(fileName);

                    if (allowedExtensions.Contains(extension))
                    {
                        entry.Extract(extractPath, ExtractExistingFileAction.OverwriteSilently);
                    }
                    else
                    {
                        logger.Log(LogLevel.Debug, "Ignoring file : {0}, as the extension is ignorable", fileName);
                    }
                }

            }

        }

        private static string GetAppVirtualPath(string appPath)
        {
            if (string.IsNullOrEmpty(appPath))
            {
                throw new ArgumentNullException("appPath");
            }

            string virtualPath = System.IO.Path.Combine(SitePaths.App, appPath);

            return virtualPath;
        }

        private static bool GetManifestContents(string appExtractPath, out string contents)
        {
            contents = null;
            bool success = false;

            string manifestFile = Path.Combine(appExtractPath, "manifest.xml");
            if (File.Exists(manifestFile))
            {
                contents = File.ReadAllText(manifestFile);

                success = true;
            }

            return success;
        }

        private static App GetAppFromManifest(string manifest, out bool appValid)
        {
            XmlDocument document = new XmlDocument();
            App app = null;
            appValid = true;

            try
            {
                logger.Log(LogLevel.Debug, "Loading app manifest xml");
                document.LoadXml(manifest);

                logger.Log(LogLevel.Debug, "Parsing app manifest xml to create app");
                app = AppManifestParser.Parse(document);
            }
            catch (Exception exception)
            {
                // unable to create app.
                appValid = false;

                //log the failure
                logger.Log(LogLevel.Info, "Creating app failed - {0}.", exception);

                // manifest file is missing for the app.
                MessageException me = new MessageException(
                        Jardalu.Ratna.Web.Resource.ResourceManager.GetLiteral("Admin.Apps.UnableToCreateApp")
                    );
                throw me;
            }

            return app;
        }

        private static string GetInstallLocation(string appFolder)
        {
            HttpRequest request = HttpContext.Current.Request;

            // get the virtual path of the template extraction
            string appVirtualPath = GetAppVirtualPath(appFolder);

            //convert virtual path to real path
            string appExtractPath = request.MapPath(appVirtualPath);
            logger.Log(LogLevel.Debug, "App files uploaded at : {0}.", appExtractPath);

            return appExtractPath;
        }

        #endregion


    }

}
