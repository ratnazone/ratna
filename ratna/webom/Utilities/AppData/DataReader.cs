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
namespace Jardalu.Ratna.Web.AppData
{
    #region using

    using System;
    using System.IO;
    using System.Web;

    #endregion

    public static class DataReader
    {
        #region public methods

        public static string ReadFileContents(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            string virtualPath = Path.Combine("~/App_Data/", fileName);

            // get the file from app data
            string fName = HttpContext.Current.Server.MapPath(virtualPath);
            return File.ReadAllText(fName);

        }

        #endregion
    }
}
