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
namespace Jardalu.Ratna.Web.Admin.pages.media
{

    #region using

    using System;

    using Jardalu.Ratna.Core.Media;

    #endregion

    public class MediaBasePage : PagerSupportPage
    {
        #region private fields

        private string url;
        private MediaType view = MediaType.Photo;
        private bool isInitialized = false;
        private object syncRoot = new object();

        #endregion

        #region public properties

        public MediaType View
        {
            get
            {
                this.Initialize();
                return this.view;
            }
        }

        public bool IsEdit
        {
            get
            {
                return (!string.IsNullOrEmpty(Url));
            }
        }

        public string Url
        {
            get
            {
                this.Initialize();
                return this.url;
            }
        }

        #endregion

        #region protected methods

        protected void Initialize()
        {
            if (!isInitialized)
            {
                lock (syncRoot)
                {
                    if (!isInitialized)
                    {
                        string viewer = this.Request["view"];
                        if (!string.IsNullOrEmpty(viewer))
                        {
                            if (!Enum.TryParse<MediaType>(viewer, true, out view))
                            {
                                view = MediaType.Photo;
                            }
                        }

                        url = Request.QueryString[Constants.UrlIdentifier] as string;
                        string query = Query; // read once to initialize

                        isInitialized = true;
                    }
                }
            }
        }

        #endregion

    }

}
