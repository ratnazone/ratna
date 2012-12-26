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
namespace Jardalu.Ratna.Web.Upload.Actions
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Web;

    #endregion

    public interface IFileUploadAction
    {
        /// <summary>
        /// Tells if the action was success
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Tells if the uploaded file should be kept or deleted.
        /// </summary>
        bool DeleteUploaded { get; }

        /// <summary>
        /// A dictionary containing the key/value pair of properties
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Called when the file is uploaded and saved.
        /// </summary>
        /// <param name="request">The HttpRequest</param>
        /// <param name="actualPath">actual path to the saved file.</param>
        /// <param name="vpath">virtual path of the saved file.</param>
        void Saved(HttpRequest request, string actualPath, string vpath); 


    }


}
