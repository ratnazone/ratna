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

    public partial class Configuration : System.Web.UI.UserControl
    {

        #region private fields

        private Logger logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string ConfigurationJavascriptKey = "controls.configuration.js";

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateJavascriptIncludesAndVariables();

            configurationSaveButton.Value = ResourceManager.GetLiteral("Admin.Common.Save");
            int selectedIndex = 0;

            // logging information
            loggingSelect.Items.Add(new ListItem(ResourceManager.GetLiteral("Admin.Common.On"), Boolean.TrueString));
            loggingSelect.Items.Add(new ListItem(ResourceManager.GetLiteral("Admin.Common.Off"), Boolean.FalseString));

            //select logging on/off
            if (!Jardalu.Ratna.Web.Configuration.Instance.IsLoggingOn)
            {
                loggingSelect.SelectedIndex = 1;
            }

            selectedIndex = 0;

            //logging level
            foreach(string level in Enum.GetNames(typeof(LogLevel)))
            {
                loggingLevelSelect.Items.Add(level);
                if (Jardalu.Ratna.Web.Configuration.Instance.LoggingLevel.ToString().Equals(level))
                {
                    selectedIndex = loggingLevelSelect.Items.Count - 1;
                }
            }

            loggingLevelSelect.SelectedIndex = selectedIndex;
            selectedIndex = 0;

        }

        #endregion

        #region private methods

        private void PopulateJavascriptIncludesAndVariables()
        {
            this.clientJavaScript.RegisterClientScriptInclude(
                    ConfigurationJavascriptKey,
                    ResolveClientUrl(Constants.Urls.Scripts.ConfigurationControl)
                );

        }

        #endregion

    }
}
