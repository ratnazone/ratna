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
    using System.Data;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Apps;
    using Jardalu.Ratna.Database;
    using Jardalu.Ratna.Utilities;

    #endregion

    internal class AppStore
    {

        #region private fields

        private static AppStore store;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private AppStore()
        {
        }

        #endregion

        #region public properties

        public static AppStore Instance
        {
            get
            {
                if (store == null)
                {
                    lock (syncRoot)
                    {
                        if (store == null)
                        {
                            store = new AppStore();
                        }
                    }
                }

                return store;
            }
        }
       
        #endregion

        #region public methods

        public App GetApp(long id)
        {
            Tuple<DataTable> tuple = AppsDbInteractor.Instance.GetApp(id);

            DataTable appTable = tuple.Item1;

            App app = null;

            if (appTable.Rows.Count == 1)
            {
                app = ConvertToApp(appTable, appTable.Rows[0]);
            }


            return app;
        }

        public IList<App> GetAppList(AppScope scope)
        {
            Tuple<DataTable> appsTuple = AppsDbInteractor.Instance.GetAppList((int)scope);
            List<App> appsList = new List<App>();

            DataTable appTable = appsTuple.Item1;
            foreach (DataRow row in appTable.Rows)
            {
                appsList.Add(ConvertToApp(appTable, row));
            }

            return appsList;
        }

        public IList<App> GetAppList(bool enabled)
        {
            Tuple<DataTable> appsTuple = AppsDbInteractor.Instance.GetAppList(enabled);
            List<App> appsList = new List<App>();

            DataTable appTable = appsTuple.Item1;
            foreach (DataRow row in appTable.Rows)
            {
                appsList.Add(ConvertToApp(appTable, row));
            }

            return appsList;
        }

        public App FindAppRegisteredForPath(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            App matchedApp = null;

            //first find app for the url scope, if none found that find one
            //for the path

            string sanitizedUrl = Utility.SanitizeUrl(url);
            IList<App> apps = GetAppList(AppScope.Url);
            if (apps != null)
            {
                foreach (App app in apps)
                {
                    //get the field
                    Field field = app.GetField(KnownAppFields.Url);

                    if (field != null)
                    {
                        // the field can be collection.
                        if (field.IsCollection)
                        {
                            ICollection<object> urls = app.GetFieldValue(KnownAppFields.Url) as ICollection<object>;
                            if (urls != null &&
                                urls.Contains(sanitizedUrl))
                            {
                                matchedApp = app;
                                break;
                            }
                        }
                        else
                        {
                            string appUrl = app.GetFieldValue(KnownAppFields.Url) as string;
                            if (appUrl != null &&
                                appUrl == sanitizedUrl)
                            {
                                matchedApp = app;
                                break;
                            }
                        }
                    }
                }
            }

            if (matchedApp == null)
            {
                apps = GetAppList(AppScope.Path);
                if (apps != null)
                {
                    foreach (App app in apps)
                    {
                        string path = app.GetFieldValue(KnownAppFields.Path) as string;
                        if (path != null
                            && sanitizedUrl.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                        {
                            matchedApp = app;
                            break;
                        }
                    }
                }
            }

            return matchedApp;
        }

        public App GetApp(Guid uniqueId)
        {
            Tuple<DataTable> tuple = AppsDbInteractor.Instance.GetApp(uniqueId);

            DataTable appTable = tuple.Item1;

            App app = null;

            if (appTable.Rows.Count == 1)
            {
                app = ConvertToApp(appTable, appTable.Rows[0]);
            }


            return app;
        }
        
        #endregion

        #region internal methods

        internal void SaveRawData(App app)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            if (app.Id == 0)
            {
                throw new InvalidOperationException("app is not persisted");
            }

            AppsDbInteractor.Instance.SetAppRawData(app.Id, app.RawData);
        }

        internal void Activate(long id, bool enable)
        {
            AppsDbInteractor.Instance.Activate(id, enable);
        }

        internal void Delete(long id)
        {
            AppsDbInteractor.Instance.DeleteApp(id);
        }


        /// <summary>
        /// Adds or update information about the App.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        internal App Save(App app)
        {

            #region arguments

            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            if (string.IsNullOrEmpty(app.Location))
            {
                throw new ArgumentNullException("app.Location");
            }

            #endregion

            long id = AppsDbInteractor.Instance.AddApp(
                                        app.Name,
                                        app.UniqueId,
                                        app.Publisher,
                                        app.Description,
                                        app.Url,
                                        (int)app.Scope,
                                        app.Version.ToString(),
                                        app.Location,
                                        app.File,
                                        app.FileEntry,
                                        SerializeReferences(app.References),
                                        app.IconUrl
                                    );

            if (app.Id == 0)
            {
                app.Id = id;
            }

            return app;
        }

        #endregion

        #region private methods

        private static App ConvertToApp(DataTable table, DataRow row)
        {
            App app = new App();
            app.Id = (long)row["Id"];
            app.Name = row["Name"] as string;
            app.Description = row["Description"] as string;
            app.Url = row["Url"] as string;
            app.Publisher = row["Publisher"] as string;
            app.Scope = (AppScope)((int)row["Scope"]);
            app.UniqueId = (Guid)row["UniqueId"];
            app.Version = Version.Parse(row["Version"] as string);
            app.IsEnabled = (bool)row["Enabled"];
            app.File = row["File"] as string;
            app.FileEntry = row["FileEntry"] as string;
            app.References = DeserializeReferences(row["References"] as string);
            app.IconUrl = row["IconUrl"] as string;

            // set the app location
            if (table.Columns.Contains("Location"))
            {
                app.Location = row["Location"] as string;
            }

            //set the rawData
            if (table.Columns.Contains("RawData"))
            {
                app.RawData = row["RawData"] as string;
            }

            return app;
        }

        private static string SerializeReferences(string[] references)
        {
            string serialized = string.Empty;

            if (references != null)
            {
                foreach (string reference in references)
                {
                    if (string.IsNullOrEmpty(serialized))
                    {
                        serialized = reference;
                    }
                    else
                    {
                        serialized += ";" + reference;
                    }
                }
            }

            return serialized;
        }

        private static string[] DeserializeReferences(string references)
        {
            string[] refArray = null;

            if (!string.IsNullOrEmpty(references))
            {
                refArray = references.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            }

            return refArray;
        }

        #endregion

    }
}
