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
namespace Jardalu.Ratna.Database
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Text;
    using System.Xml;

    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Exceptions.ErrorCodes;
    using Jardalu.Ratna.Utilities;

    #endregion

    internal class UtilDbInteractor : DbInteractor
    {

        #region Private Fields

        private static UtilDbInteractor instance;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private UtilDbInteractor()
        {
        }

        #endregion

        #region Public Properties

        public static UtilDbInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new UtilDbInteractor();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Public Methods

        public IList<Tuple<string, string>> GetNavigationData(string defaultUrl, string urlKey)
        {
            if (string.IsNullOrEmpty(urlKey))
            {
                throw new ArgumentNullException("urlKey");
            }

            if (string.IsNullOrEmpty(defaultUrl))
            {
                throw new ArgumentNullException("defaultUrl");
            }

            List<string> urls = new List<string>();
            urls.Add(urlKey);

            return GetNavigationData(defaultUrl, urls);
        }

        public IList<Tuple<string, string>> GetNavigationData(string defaultUrl, IList<string> urls)
        {
            #region arguments

            if (string.IsNullOrEmpty(defaultUrl))
            {
                throw new ArgumentNullException("defaultUrl");
            }

            if (urls == null)
            {
                throw new ArgumentNullException("urls");
            }

            #endregion

            List<Tuple<string, string>> navdata = new List<Tuple<string, string>>();

            // breadcrumb rule
            // /news/2012/4 --> split them as /default, /news, /news/2012, /news/2012/4
            // fetch breadcrumb data from db
            List<string> allPaths = new List<string>();

            foreach (string urlKey in urls)
            {
                Uri uri = null;
                if (Uri.TryCreate(urlKey, UriKind.Relative, out uri))
                {
                    allPaths.AddRange(Utility.GetUrlPaths(urlKey));
                }
            }

            StringBuilder builder = new StringBuilder();

            #region create tags xml
            XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder));
            writer.WriteStartDocument();
            writer.WriteStartElement("UrlKeys");

            foreach (string path in allPaths)
            {

                string p = path;

                if (p.Equals("/", StringComparison.OrdinalIgnoreCase))
                {
                    p = defaultUrl;
                }

                writer.WriteStartElement("UrlKey");
                writer.WriteAttributeString("Value", p);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            #endregion

            #region db call
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_GetNavigationBuilderData";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UrlKeysXml", SqlDbType.NText, builder.ToString());
            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string name = reader["Title"] as string;
                        string key = reader["UrlKey"] as string;

                        if (key.Equals(defaultUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            key = "/";
                        }

                        Tuple<string, string> tuple = new Tuple<string, string>(name, key);
                        if (key == "/")
                        {
                            // home is the first item.
                            navdata.Insert(0,tuple);
                        }
                        else
                        {
                            navdata.Add(tuple);
                        }
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            #endregion


            return navdata;
        }

        public IList<Tuple<string, string>> GetChildNavigationData(string defaultUrl, string url)
        {
            #region arguments

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            #endregion

            List<Tuple<string, string>> navdata = new List<Tuple<string, string>>();

            
            #region db call
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = @"Proc_Ratna_GetChildNavigationBuilderData";

            AddParameter(command, "@SiteId", SqlDbType.Int, Jardalu.Ratna.Core.WebContext.Current.Site.Id);
            AddParameter(command, "@UrlKey", SqlDbType.NVarChar, url);
            SqlParameter errorCodeParameter = AddOutputParameter(command, "@ErrorCode", SqlDbType.BigInt);

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionInformation.Instance.ConnectionString))
            {
                using (command)
                {
                    command.Connection = sqlConnection;
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string name = reader["Title"] as string;
                        string key = reader["UrlKey"] as string;

                        if (key.Equals(defaultUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            key = "/";
                        }

                        Tuple<string, string> tuple = new Tuple<string, string>(name, key);
                        navdata.Add(tuple);
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            //read the error code
            long errorCode = (long)errorCodeParameter.Value;
            if (errorCode != NoErrorCode)
            {
                ThrowError(errorCode);
            }

            #endregion


            return navdata;
        }

        #endregion


    }

}
