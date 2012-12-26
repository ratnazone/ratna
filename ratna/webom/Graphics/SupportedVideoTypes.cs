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
namespace Jardalu.Ratna.Web.Graphics
{
    #region using

    using System;
    using System.IO;

    #endregion

    public class SupportedVideoTypes
    {
        public static string[] Supported = { ".avi", ".mp4", ".wmv", ".flv" };

        public static bool IsSupported(string fileName)
        {
            bool supported = false;

            if (!string.IsNullOrEmpty(fileName))
            {
                string extension = Path.GetExtension(fileName);

                foreach (string s in Supported)
                {
                    if (s.Equals(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        supported = true;
                        break;
                    }
                }
            }

            return supported;
        }
    }

}
