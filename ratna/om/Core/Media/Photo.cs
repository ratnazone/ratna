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
namespace Jardalu.Ratna.Core.Media
{
    #region using

    using System;
    using System.Runtime.Serialization;

    #endregion

    [DataContract]
    public class Photo : BaseMedia
    {
        
        #region private fields

        [DataMember]
        private int width;

        [DataMember]
        private int height;

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(Photo));

        #endregion

        #region ctor

        public Photo()
        {
            this.MediaType = MediaType.Photo;
        }

        public Photo(BaseMedia media)
            : this()
        {
            if (media == null)
            {
                throw new ArgumentNullException("media");
            }

            this.Copy(media);
            this.RawData = media.RawData;
        }

        #endregion

        #region public properties

        public int Width
        {
            get { return this.width; }
            set {
                if (this.width < 0)
                {
                    throw new ArgumentException("width");
                }
                this.width = value;
                this.IsDirty = true;
            }
        }

        public int Height
        {
            get { return this.height; }
            set
            {
                if (this.height < 0)
                {
                    throw new ArgumentException("height");
                }
                this.height = value;
                this.IsDirty = true;
            }
        }

        public override DataContractSerializer Serializer
        {
            get
            {
                return serializer;
            }
        }

        public override string RawData
        {
            get
            {
                this.Prepare();
                return rawData;
            }
            set
            {
                rawData = value;
                this.Populate();
            }
        }

        public override void CopySpecific(ISerializableObject media)
        {
            Photo p = media as Photo;
            if (p != null)
            {
                this.Height = p.Height;
                this.Width = p.Width;
            }
        }

        #endregion

    }

}
