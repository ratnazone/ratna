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
namespace Jardalu.Ratna.Web.Controls
{

    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Core.Comments;

    #endregion

    public interface IThreadControl
    {
        /// <summary>
        /// The comments associated with the thread. The thread
        /// is identified with the property "Key"
        /// </summary>
        IList<Comment> Comments
        {
            get;
            set;
        }

        /// <summary>
        /// This property uniquely defines the key for each thread.
        /// For example, a page with url (/xyz) can define the thread
        /// key to be "xyz", or a page displaying the product "sku1"
        /// can define the key to be "sku1"
        /// </summary>
        string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the HTML code that is used to display one row of message.
        /// </summary>
        /// <param name="imageurl">Image used for the person</param>
        /// <param name="name">Name of the person commenting</param>
        /// <param name="url">This can be null</param>
        /// <param name="time">Time of the message</param>
        /// <param name="message">Message</param>
        /// <returns>HTML</returns>
        string GetMessageRow(string imageurl, string name, string url, string time, string message);

    }

}
