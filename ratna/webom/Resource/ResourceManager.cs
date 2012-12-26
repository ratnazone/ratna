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
namespace Jardalu.Ratna.Web.Resource
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Xml;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Utilities;

    #endregion

    public static class ResourceManager
    {

        #region private fields

        private const string EnUSLocale = "en-us";
        private static object syncRoot = new object();

        private static Logger logger;

        private const string AppDataPath = "~/App_Data";
        private const string LiteralsFileFormat = "literals.{0}.xml";

        private static IDictionary<int, string> siteLocales = new Dictionary<int, string>(1);
        private static IDictionary<string, IDictionary<string, string>> localeLiterals = new Dictionary<string, IDictionary<string, string>>(1);

        #endregion

        static ResourceManager()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //set the site locale for "0"
            SetSiteLocale(EnUSLocale);
        }

        public static string[] AvailableLocales
        {
            get
            {
                List<string> list = new List<string>(2);

                string filter = string.Format(LiteralsFileFormat,"*");

                if (HttpContext.Current != null)
                {
                    string appDir = HttpContext.Current.Server.MapPath(AppDataPath);

                    string[] literals = Directory.GetFiles(appDir, filter);

                    foreach (string literal in literals)
                    {
                        string[] tokens = literal.Split('.');
                        list.Add(tokens[tokens.Length - 2]);
                    }
                }
                else
                {
                    // this path is supported only for testing purpose
                    list.Add("en-us");
                }

                return list.ToArray();
            }
        }

        public static void SetSiteLocale(string locale)
        {
            #region arguments

            if (string.IsNullOrEmpty(locale))
            {
                throw new ArgumentNullException("locale");
            }

            bool match = false;

            //check if the locale is available
            foreach (string avalLocale in AvailableLocales)
            {
                if (avalLocale.Equals(locale, StringComparison.OrdinalIgnoreCase))
                {
                    match = true;
                    break;
                }
            }

            if (!match)
            {
                // not possible through UI.
                throw new ArgumentException("locale");
            }

            #endregion

            int siteId = WebContext.Current.Site.Id;

            siteLocales[siteId] = locale;

            if (!localeLiterals.ContainsKey(locale))
            {
                //if the locale is not loaded, load it
                lock (syncRoot)
                {
                    if (!localeLiterals.ContainsKey(locale))
                    {
                        IDictionary<string, string> literals = ReadLiterals(string.Format(LiteralsFileFormat, locale));
                        localeLiterals[locale] = literals;
                    }
                }
            }
        }

        public static string GetLiteral(string code)
        {
            string locale = EnUSLocale;

            int siteId = WebContext.Current.Site.Id;

            if (siteLocales.ContainsKey(siteId))
            {
                locale = siteLocales[siteId];
            }

            IDictionary<string, string> literals = localeLiterals[locale];

            if (literals.ContainsKey(code))
            {
                return literals[code];
            }

            return  locale + "-" + code;
        }

        #region private methods

        private static IDictionary<string, string> ReadLiterals(string fileName)
        {
            Dictionary<string, string> literals = new Dictionary<string, string>();

            try
            {
                string appDataFileName = string.Format("{0}/{1}", AppDataPath, fileName);

                // get the file from app data
                string fName = HttpContext.Current.Server.MapPath(appDataFileName);
                XmlDocument document = new XmlDocument();
                document.Load(fName);

                //read the literals
                XmlNodeList nodeList = document.DocumentElement.SelectNodes("/Literals/Area");
                foreach(XmlNode node in nodeList)
                {
                    XmlElement areaNode = node as XmlElement;
                    if (areaNode != null)
                    {
                        // get the area name
                        string areaName = areaNode.Attributes["Name"].Value;

                        XmlNodeList literalsNodeList = areaNode.SelectNodes("Literal");
                        foreach (XmlNode literalNode in literalsNodeList)
                        {
                            string literalName = literalNode.Attributes["Name"].Value;
                            string literalValue = literalNode.InnerText;

                            string qualifiedName = string.Format("{0}.{1}", areaName, literalName);
                            literals[qualifiedName] = literalValue;
                        }
                    }
                }
            }
            catch
            {
                logger.Log( LogLevel.Warn, "Unable to read literals from : {0}", fileName);
            }

            return literals;
        }

        #endregion

    }
}
