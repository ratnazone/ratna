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
namespace Jardalu.Ratna.Web.Admin.pages.settings
{
    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Web.Resource;

    #endregion

    public class SettingsBasePage : System.Web.UI.Page
    {
        protected IList<Tuple<string, string>> GetMenuItems()
        {
            List<Tuple<string, string>> menuItems = new List<Tuple<string, string>>();

            menuItems.Add(new Tuple<string, string>(
                    ResourceManager.GetLiteral("Admin.Settings.Title") /* title */,
                    Constants.Urls.Settings.Url)
             );

            menuItems.Add(new Tuple<string, string>(
                   ResourceManager.GetLiteral("Admin.Templates.Title") /* title */,
                   Constants.Urls.Settings.TemplatesUrl)
            );

            menuItems.Add(new Tuple<string, string>(
                    ResourceManager.GetLiteral("Admin.Settings.Notification.Title") /* title */,
                    Constants.Urls.Settings.NotificationUrl)
             );

            menuItems.Add(new Tuple<string, string>(
                    ResourceManager.GetLiteral("Admin.Settings.Smtp.Title") /* title */,
                    Constants.Urls.Settings.SmtpUrl)
             );

            menuItems.Add(new Tuple<string, string>(
                    ResourceManager.GetLiteral("Admin.Settings.CustomErrors.Title") /* title */,
                    Constants.Urls.Settings.CustomErrorsUrl)
             );

            menuItems.Add(new Tuple<string, string>(
                    ResourceManager.GetLiteral("Admin.Settings.AutoPaths.Title") /* title */,
                    Constants.Urls.Settings.AutoPathsUrl)
             );

            return menuItems;
        }
    }

}
