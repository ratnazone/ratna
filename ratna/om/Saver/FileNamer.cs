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

namespace Jardalu.Ratna.Saver
{
    #region using 

    using System;
    using System.IO;

    #endregion

    public class FileNamer
    {

        #region Public Methods

        /// <summary>
        /// Generates a name for the file that can be used to save in the directory location.
        /// the input "fileName" is the original file name. If a file already exists with that
        /// name in the directory, this method will create a new name that can be used for saving
        /// the file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="saveLocation"></param>
        /// <returns>name of the file that can be used to save in the directory</returns>
        public static string GenerateFileName(string fileName, DirectoryInfo saveLocation)
        {

            #region argument

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (saveLocation == null)
            {
                throw new ArgumentNullException("saveLocation");
            }

            #endregion

            string finalFileName = fileName;

            //check if the file with the name exists
            if (File.Exists(Path.Combine(saveLocation.FullName, fileName)))
            {

                string extenstion = Path.GetExtension(fileName);
                string pattern;
                string prefix = fileName.Substring(0, fileName.LastIndexOf(extenstion));

                if (string.IsNullOrEmpty(extenstion))
                {
                    pattern = fileName + "_*";
                }
                else
                {
                    pattern = prefix + "_*" + extenstion;
                }

                FileInfo[] files = saveLocation.GetFiles(pattern, SearchOption.TopDirectoryOnly);
                finalFileName = prefix + "_" + (files.Length + 1) + extenstion;
            }

            return finalFileName;
        }

        #endregion
    }
}
