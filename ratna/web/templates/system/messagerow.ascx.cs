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
namespace Jardalu.Ratna.Web.templates.system
{
    #region using

    using System;

    using Jardalu.Ratna.Web.UI.Snippets;

    #endregion

    public partial class messagerow : SnippetControl
    {
        
        #region public properties

        public string Name { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }

        public bool IsAlt { get; set; }

        public DateTime TimeInDateTime
        {
            get
            {
                return DateTime.Parse(Time);
            }
        }

        #endregion

        #region protected

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateControl();
        }

        public override void PopulateControl()
        {
            if (string.IsNullOrEmpty(ImageUrl))
            {
                ImageUrl = GetDefaultAvatar();
            }

            this.avatar.Src = ImageUrl;
            this.avatar.Alt = Name;
            this.avatar.Attributes["title"] = Name;

            if (IsAlt)
            {
                this.commentli.Attributes["class"] = "comment_even";
            }


            // formatted time as Friday, 6<sup>th</sup> April 2000 @08:15:00
            string format = string.Format("{0}, {1}<sup>th</sup> {2} {3} @ {4}", TimeInDateTime.DayOfWeek, TimeInDateTime.Day,
                TimeInDateTime.ToString("MMMM"), TimeInDateTime.Year, TimeInDateTime.ToString("hh:mm:ss"));
            this.timeformatted.Text = format;

            // datetime - 2000-04-06T08:15+00:00
            this.utctime.Attributes["datetime"] = TimeInDateTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        }

        #endregion

        #region private methods

        private string GetDefaultAvatar()
        {
            return "/images/gravatar.jpg";
        }

        #endregion

    }
}
