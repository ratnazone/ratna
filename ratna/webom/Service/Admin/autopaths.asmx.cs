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
namespace Jardalu.Ratna.Web.Admin.service
{
    #region using

    using System;
    using System.Web;
    using System.Web.Services;
    using System.Web.Script.Services;

    using Jardalu.Ratna.Web.Service;
    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Web.Templates;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Web.Plugins;

    #endregion

    [Access(Group = "Administrator")]
    [ScriptService]
    public class AutoPathsService : ServiceBase
    {

        static AutoPathsService()
        {
            // ensure that the collection path plugin is activated
            CollectionPathPlugin plugin = new CollectionPathPlugin();
            plugin.Register();
            plugin.Activate();
        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Save(string path, string title, string pathType, int pagesize, string nav)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            CollectionPath collectionPath = CollectionPathPlugin.Instance.Read(path);

            if (collectionPath == null)
            {
                collectionPath = new CollectionPath();
                collectionPath.Path = path;
            }

            collectionPath.Title = title;
            collectionPath.Navigation = nav ?? string.Empty;
            collectionPath.PageSize = pagesize > 0 ? pagesize : 4;

            CollectionType collectionType;
            if (!Enum.TryParse<CollectionType>(pathType, true, out collectionType))
            {
                collectionType = CollectionType.BlogArticle; 
            }

            // set the collection type
            collectionPath.CollectionType = collectionType;

            try
            {
                //save
                CollectionPathPlugin.Instance.Update(collectionPath);

                output.Success = true;
            }
            catch (MessageException me)
            {
                output.AddOutput(Constants.Json.Error, me.Message);
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Delete(string path)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            CollectionPathPlugin.Instance.Delete(CollectionPath.KeyName, path);
            output.AddOutput("path", path);
            output.Success = true;

            return output.GetJson();
        }

    }

}
