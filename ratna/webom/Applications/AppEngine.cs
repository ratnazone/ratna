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
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.UI;


    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Apps;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.UI.Apps;
    using Jardalu.Ratna.Web.Utilities;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    /// <summary>
    /// Executes App to gets its output.
    /// </summary>
    internal static class AppEngine
    {

        #region private fields

        private static Logger logger;

        #endregion

        #region ctor

        static AppEngine()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region public methods

        public static App FindAppRegisteredForPath(string rawUrl, out Page page)
        {

            logger.Log(LogLevel.Debug, "FindAppRegisteredForPath called for [{0}]", rawUrl);

            App app = AppStore.Instance.FindAppRegisteredForPath(rawUrl);
            page = null;

            string appPageHandler = null;

            //App available, resolve the page handler
            if (app != null)
            {
                logger.Log(LogLevel.Debug, "App found for url - Name : {0} File : {1}", app.Name, app.File);
                appPageHandler = GetFileHandler(app);
                logger.Log(LogLevel.Debug, "Page Handler resolved as - {0}.", appPageHandler);
            }

            if (!string.IsNullOrEmpty(appPageHandler))
            {
                try
                {
                    logger.Log(LogLevel.Debug, "Loading the app defined at - {0}.", appPageHandler);
                    page = BuildManager.CreateInstanceFromVirtualPath(appPageHandler, typeof(Page)) as Page;

                }
                catch (Exception exception)
                {
                    logger.Log(LogLevel.Warn, "Unable to load the app defined at - {0}. Error - {1}", appPageHandler, exception);
                }
            }

            return app;

        }

        public static IList<UserControl> GetPageHeadApps(MasterPage master)
        {

            if (master == null)
            {
                throw new ArgumentNullException("master");
            }

            logger.Log(LogLevel.Debug, "GetPageHeadApps called");

            IList<UserControl> pageHeads = new List<UserControl>();
            IList<App> apps = AppStore.Instance.GetAppList(AppScope.PageHead);

            foreach (App app in apps)
            {
                logger.Log(LogLevel.Debug, "Loading App : {0}", app);
                string appPageHeadHandler = null;

                //App available, resolve the pagehead handler
                if (app != null)
                {
                    logger.Log(LogLevel.Debug, "App found for url - Name : {0} File : {1}", app.Name, app.File);
                    appPageHeadHandler = GetFileHandler(app);
                    logger.Log(LogLevel.Debug, "PageHead Handler resolved as - {0}.", appPageHeadHandler);
                }

                if (!string.IsNullOrEmpty(appPageHeadHandler))
                {
                    try
                    {
                        logger.Log(LogLevel.Debug, "Loading the app defined at - {0}.", appPageHeadHandler);
                        
                        // create the user control.
                        UserControl control = master.Page.LoadControl(appPageHeadHandler) as UserControl;

                        if (control != null)
                        {
                            AppUserControl appControl = control as AppUserControl;

                            if (appControl != null)
                            {
                                appControl.InvokedApp = app;
                            }

                            pageHeads.Add(control);
                        }
                        else
                        {
                            logger.Log(LogLevel.Warn, "Unable to load PageHead - {0}. Please ensure it is UserControl.", appPageHeadHandler);
                        }

                    }
                    catch (Exception exception)
                    {
                        logger.Log(LogLevel.Warn, "Unable to load the app defined at - {0}. Error - {1}", appPageHeadHandler, exception);
                    }
                }
            }

            return pageHeads;
        }

        public static void ExecuteApps(AppEvent appEvent, object eventObject)
        {
            logger.Log(LogLevel.Debug, "ExecuteApps called for event [{0}]", appEvent);

            IList<App> apps = null;

            switch(appEvent)
            {
                case AppEvent.CommentSaved:
                case AppEvent.CommentSaving:

                    apps = AppStore.Instance.GetAppList(AppScope.Comment);

                    break;

                case AppEvent.ImageUploaded:
                    apps = AppStore.Instance.GetAppList(AppScope.Image);
                    break;
            }


            if (apps != null)
            {
                ExecuteApp(apps, appEvent, eventObject);
            }

        }

        public static long RecordAppExecutionStart(App app)
        {
            return 1;
        }

        public static void RecordAppExecutionEnd(long executionId, bool success, string error)
        {
        }

        public static void ValidateDefinedApp(App app)
        {
            GetDefinedApp(app);
        }

        #endregion

        #region private methods

        private static AbstractApp GetDefinedApp(App app)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            AbstractApp definedApp = null;

            if (app.IsAbstractApp())
            {
                // get the app file and entry.
                string file = app.File;
                string entry = app.FileEntry;

                // load the assembly, it will throw exception if needed.
                Assembly assembly = AssemblyUtility.Load(
                                                HttpContext.Current.Server.MapPath(string.Format("{0}/{1}/{2}", SitePaths.App, app.Location, app.File))
                                            );

                // load the app from the assembly
                Type type = assembly.GetType(entry, false);

                if (type == null)
                {
                    // unable to load the class from the assembly
                    throw new MessageException(
                            string.Format(ResourceManager.GetLiteral("Admin.Apps.AppEntryNotFound"), entry, app.File)
                        );
                }

                // create the abstractapp
                try
                {
                    definedApp = Activator.CreateInstance(type) as AbstractApp;
                    definedApp.InvokedApp = app;
                }
                catch (Exception exception)
                {
                    // not a valid type for abstractapp
                    logger.Log(LogLevel.Warn, "Unable to create abstractapp of type : {0}. Exception - {1}", type, exception);
                }

                if (definedApp == null)
                {
                    // unable to create app
                    throw new MessageException(
                        string.Format(ResourceManager.GetLiteral("Admin.Apps.InvalidAppEntry"), app.FileEntry)
                    );
                }

            }
            else
            {
                // throw as the app is not of the scope
                // that can be translated into AbstractApp.

                throw new InvalidOperationException("app");
            }

#if DEBUG
            if (definedApp == null)
            {
                //something went wrong
                throw new InvalidOperationException("definedApp should not be null now");
            }
#endif

            return definedApp;
        }

        private static string GetFileHandler(App app)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            return string.Format("{0}/{1}/{2}", SitePaths.App, app.Location, app.File);
        }

        private static void ExecuteApp(IList<App> apps, AppEvent appEvent, object eventObject)
        {
            logger.Log(LogLevel.Debug, "ExecuteApp called for event [{0}] - eventObject [{1}]", appEvent, eventObject);

            if (apps != null && apps.Count > 0)
            {
                foreach (App app in apps)
                {
                    logger.Log(LogLevel.Debug, "Executing app [{0}] for [{1}]", app, eventObject);

                    AbstractApp abstractApp = AppEngine.GetDefinedApp(app);
                    if (abstractApp != null)
                    {
                        long executionId = AppEngine.RecordAppExecutionStart(app);
                        bool success = false;
                        string error = string.Empty;

                        try
                        {
                            AppExecutor.Execute(abstractApp, appEvent, eventObject);
                        }
                        catch (Exception exception)
                        {
                            error = exception.Message;
                            logger.Log(LogLevel.Warn, "Error executing app [{0}] - Exception : {1}", app, exception);
                        }
                        finally
                        {
                            AppEngine.RecordAppExecutionEnd(executionId, success, error);
                        }
                    }
                }
            }
        }

        #endregion

    }

}
