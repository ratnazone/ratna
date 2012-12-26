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
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    #endregion

    internal class ImageSaver : FileSaver
    {
       #region private fields

        private int imageQuality = 90;

        #endregion

        #region ctor

        public ImageSaver()
        {
        }

        #endregion

        #region public properties

        public int ImageQuality
        {
            get
            {
                return this.imageQuality;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException("value");
                }

                this.imageQuality = value;
            }
        }

        #endregion

        #region public methods

        public string Save(Image image, string fileName, DirectoryInfo parentLocation)
        {
            #region args
            if (image == null)
            {
                throw new ArgumentNullException("image");
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

            string saveFolder = GetSaveFolderName();
            EnsureChildFolders(parentLocation, saveFolder);

            string saveLocation = Path.Combine(parentLocation.FullName, saveFolder);

            //save with the quality
            EncoderParameters encoderParameters = new EncoderParameters();
            Encoder encoder = Encoder.Quality;

            ImageCodecInfo jpegCodecInfo = GetCodecInfo();

            EncoderParameter encoderParameter = new EncoderParameter(encoder, ImageQuality);
            encoderParameters.Param[0] = encoderParameter;

            string fileLocation = Path.Combine(saveLocation, fileName);

            image.Save(fileLocation, jpegCodecInfo, encoderParameters);

            //saved location
            return Path.Combine(saveLocation, fileName);
        }

        #endregion

        #region private methods

        private static ImageCodecInfo GetCodecInfo()
        {
            ImageCodecInfo codecInfo = null;

            ImageFormat format = System.Drawing.Imaging.ImageFormat.Jpeg;
            ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    codecInfo = codec;
                    break;
                }
            }

            return codecInfo;
        }

        #endregion

    }

}
