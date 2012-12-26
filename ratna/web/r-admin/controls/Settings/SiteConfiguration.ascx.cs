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
namespace Jardalu.Ratna.Web.Admin.controls.Settings
{

    #region using

    using System;
    using System.Globalization;
    using System.Web.UI.WebControls;

    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Utilities;

    #endregion

    public partial class SiteConfiguration : System.Web.UI.UserControl
    {

        #region private fields

        private Logger logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string ConfigurationJavascriptKey = "controls.siteconfiguration.js";

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateJavascriptIncludesAndVariables();

            siteConfigurationSaveButton.Value = ResourceManager.GetLiteral("Admin.Common.Save");

            int selectedIndex = 0;
            int index = 0;


            Jardalu.Ratna.Web.SiteConfiguration config = Jardalu.Ratna.Web.SiteConfiguration.Read();

            foreach (string locale in ResourceManager.AvailableLocales)
            {
                CultureInfo ci = null;

                try
                {
                    ci = CultureInfo.GetCultureInfo(locale);
                    localeSelect.Items.Add(new ListItem(ci.EnglishName, ci.Name));

                    if (config.Locale.Equals(ci.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedIndex = index;
                    }

                    index++;
                }
                catch (System.Globalization.CultureNotFoundException)
                {
                    // invalid file name.
                    logger.Log(LogLevel.Error, "{0} is not a valid Locale for literal file", locale);
                }
            }

            //select locale in dropdown
            localeSelect.SelectedIndex = selectedIndex;

            commentModerationSelect.Items.Add(new ListItem(ResourceManager.GetLiteral("Admin.Common.On"), Boolean.TrueString));
            commentModerationSelect.Items.Add(new ListItem(ResourceManager.GetLiteral("Admin.Common.Off"), Boolean.FalseString));

            //select moderation on/off
            if (!config.IsCommentModerationOn)
            {
                commentModerationSelect.SelectedIndex = 1;
            }
        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    ConfigurationJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.SiteConfigurationControl)
                );

        }

        #endregion
    }
}
