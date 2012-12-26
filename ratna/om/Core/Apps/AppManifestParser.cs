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
namespace Jardalu.Ratna.Core.Apps
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Xml;
    
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Resource;

    #endregion


    public static class AppManifestParser
    {
        /// <summary>
        /// Constructs App from the manifest
        /// </summary>
        /// <param name="root">Root of the manifest xml</param>
        /// <returns>App object</returns>
        public static App Parse(XmlDocument root)
        {

            #region argument
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            #endregion

            XmlNode manifest = root.SelectSingleNode("manifest");

            App app = new App();

            try
            {
                app.Name = manifest.SelectSingleNode("name").InnerText;
                app.Publisher = manifest.SelectSingleNode("publisher").InnerText;
                app.Description = manifest.SelectSingleNode("description").InnerText;
                app.UniqueId = new Guid(manifest.SelectSingleNode("uniqueid").InnerText);
                app.Version = Version.Parse(manifest.SelectSingleNode("version").InnerText);
                app.Scope = (AppScope)Enum.Parse(typeof(AppScope), manifest.SelectSingleNode("scope").InnerText, true);
                app.File = manifest.SelectSingleNode("file").Attributes["name"].Value;
            }
            catch (NullReferenceException)
            {
                throw new MessageException(ResourceManager.GetMessage(AppsMessages.ManadatoryFieldsMissing));
            }

            // for abstract apps, file entry must be defined.
            if (app.IsAbstractApp())
            {
                try
                {
                    app.FileEntry = manifest.SelectSingleNode("file").Attributes["entry"].Value;
                }
                catch (NullReferenceException)
                {
                    throw new MessageException(ResourceManager.GetMessage(AppsMessages.ManadatoryFieldsMissing));
                }

                // make sure the file is an assembly extension.
                if (!app.File.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    throw new MessageException(ResourceManager.GetMessage(AppsMessages.FileNameMustBeAssembly));
                }
            }

            XmlNode node = manifest.SelectSingleNode("url");
            if (node != null)
            {
                app.Url = node.InnerText;
            }

            //icon
            node = manifest.SelectSingleNode("icon");
            if (node != null)
            {
                app.IconUrl = node.InnerText;
            }

            //add fields
            AddFields(app, manifest);


            //add references to the app.
            AddReferences(app, manifest);

            return app;
        }

        private static void AddReferences(App app, XmlNode manifest)
        {
            //add references to the app.
            XmlNodeList nodeList = manifest.SelectNodes("file/references/reference");
            if (nodeList != null)
            {
                List<string> referencesList = new List<string>(nodeList.Count);
                foreach (XmlNode n in nodeList)
                {
                    string referenceName = n.InnerText;
                    if (!string.IsNullOrEmpty(referenceName))
                    {
                        referencesList.Add(referenceName);
                    }
                }

                if (referencesList.Count > 0)
                {
                    app.References = referencesList.ToArray();
                }
            }
        }

        private static void AddFields(App app, XmlNode manifest)
        {
            // add fields to the app
            XmlNodeList nodeList = manifest.SelectNodes("fields/field");
            if (nodeList != null)
            {
                foreach (XmlNode n in nodeList)
                {
                    string name = n.Attributes["name"].Value;
                    FieldType fType = FieldType.String;
                    if (n.Attributes["type"] != null)
                    {
                        if (!Enum.TryParse<FieldType>(n.Attributes["type"].Value, true, out fType))
                        {
                            fType = FieldType.String;
                        }
                    }
                    bool required = false;
                    if (n.Attributes["required"] != null)
                    {
                        if (!Boolean.TryParse(n.Attributes["required"].Value, out required))
                        {
                            required = false;
                        }
                    }

                    bool collection = false;
                    if (n.Attributes["collection"] != null)
                    {
                        if (!Boolean.TryParse(n.Attributes["collection"].Value, out collection))
                        {
                            collection = false;
                        }
                    }

                    Field field = new Field();
                    field.Name = name;
                    field.FieldType = fType;
                    field.IsRequired = required;
                    field.IsCollection = collection;

                    app.AddField(field);
                }
            }
        }
    }
}
