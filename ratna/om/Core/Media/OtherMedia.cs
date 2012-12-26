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
    public class OtherMedia : BaseMedia
    {
        
        #region private fields

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(OtherMedia));

        #endregion

        #region ctor

        public OtherMedia()
        {
            this.MediaType = MediaType.Other;
        }

        public OtherMedia(BaseMedia media)
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

        #endregion

    }

}
