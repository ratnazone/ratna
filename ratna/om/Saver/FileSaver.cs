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


    public class FileSaver
    {
        #region private fields

        private static FileSaverFolderPattern folderPattern = FileSaverFolderPattern.Day;
 
        #endregion

        #region ctor

        public FileSaver()
        {
        }

        #endregion

        #region public properties

        public static FileSaverFolderPattern FolderPattern
        {
            get
            {
                return folderPattern;
            }
            set
            {
                folderPattern = value;
            }
        }

        #endregion

        #region public methods

        public string GetSaveFolderName()
        {
            DateTime now = DateTime.Now;

            string folder = string.Format("{0}", now.Year);
            switch (FolderPattern)
            {
                case FileSaverFolderPattern.Month:
                    folder = string.Format("{1}{0}{2}", Path.DirectorySeparatorChar, now.Year, now.Month);
                    break;
                case FileSaverFolderPattern.Day:
                    folder = string.Format("{1}{0}{2}{0}{3}", Path.DirectorySeparatorChar, now.Year, now.Month, now.Day);
                    break;
            }
            return folder;
        }

        public virtual string Save(Stream stream, string fileName, DirectoryInfo parentLocation)
        {
            string saveFolder = GetSaveFolderName();

            return Save(stream, fileName, parentLocation, saveFolder);
        }

        public virtual string Save(byte[] data, string fileName, DirectoryInfo parentLocation)
        {

            string saveFolder = GetSaveFolderName();

            return Save(data, fileName, parentLocation, saveFolder);
        }

        public virtual string Save(Stream stream, string fileName, DirectoryInfo parentLocation, string saveFolder)
        {
            #region arguments

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion

            EnsureChildFolders(parentLocation, saveFolder);

            string saveLocation = Path.Combine(parentLocation.FullName, saveFolder);
            string saveTo = Path.Combine(saveLocation, fileName);

            int Length = 2048;
            Byte [] buffer = new Byte[Length];

            using(FileStream writeStream = new FileStream(saveTo, FileMode.Create, FileAccess.Write))
            {
                 int bytesRead = stream.Read(buffer,0,Length);

                 // write the required bytes
                 while( bytesRead > 0 ) 
                {
                     writeStream.Write(buffer,0,bytesRead);
                     bytesRead = stream.Read(buffer,0,Length);
                 }

                 writeStream.Flush();
            }
 
            //saved location
            return Path.Combine(saveLocation, fileName);
        }

        public virtual string Save(byte[] data, string fileName, DirectoryInfo parentLocation, string saveFolder)
        {

            #region args
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (parentLocation == null)
            {
                throw new ArgumentNullException("parentLocation");
            }

            #endregion

            EnsureChildFolders(parentLocation, saveFolder);

            string saveLocation = Path.Combine(parentLocation.FullName, saveFolder);

            File.WriteAllBytes(Path.Combine(saveLocation, fileName), data);

            //saved location
            return Path.Combine(saveLocation, fileName);
        }

        #endregion

        #region Protected Methods

        protected static void EnsureChildFolders(DirectoryInfo parentLocation, string childsPath)
        {
            string[] paths = childsPath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            DirectoryInfo current = parentLocation;
            foreach (string path in paths)
            {
                string childPath = Path.Combine(current.FullName, path);
                if (!Directory.Exists(childPath))
                {
                    Directory.CreateDirectory(childPath);
                }
                current = new DirectoryInfo(childPath);
            }
        }

        #endregion

    }

}
