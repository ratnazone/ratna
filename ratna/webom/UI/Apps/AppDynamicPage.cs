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
namespace Jardalu.Ratna.Web.UI.Apps
{

    #region using

    using System;

    using Jardalu.Ratna.Core.Apps;

    #endregion

    /// <summary>
    /// When a path or an url is served by a App, it should extend AppDynamicPage. 
    /// At runtime, Ratna will automatically resolve the App property, so that App
    /// can know the context in which it was invoked.
    /// </summary>
    public class AppDynamicPage : DynamicPage
    {

        #region private fields

        private string fetchUrl;

        #endregion

        #region public properties

        /// <summary>
        /// The URL that is served by this Page
        /// </summary>
        public string FetchUrl
        {
            get
            {
                return fetchUrl;
            }
            set
            {
                fetchUrl = value;
            }
        }

        /// <summary>
        /// The App that is serving this Page
        /// </summary>
        public App InvokedApp
        {
            get;
            set;
        }

        #endregion

        #region protected methods

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            RatnaMasterPage rmp = this.Master as RatnaMasterPage;
            if (rmp != null)
            {
                rmp.FetchUrl = FetchUrl;
            }
        }

        #endregion

    }
}
