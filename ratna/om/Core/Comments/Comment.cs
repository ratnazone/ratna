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
namespace Jardalu.Ratna.Core.Comments
{
    #region using

    using System;
    using System.Runtime.Serialization;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Plugins;
    
    #endregion

    [DataContract]
    public class Comment : PluginData
    {

        #region Private Fields

        [DataMember]
        private string name;
        [DataMember]
        private string email;
        [DataMember]
        private string url;
        [DataMember]
        private DateTime time = DateTime.MinValue;
        [DataMember]
        private string image;
        [DataMember]
        private string body;
        [DataMember]
        private bool approved;
        [DataMember]
        private string permalink;
        [DataMember]
        private bool isSpam;

        private static DataContractSerializer serializer = new DataContractSerializer(typeof(Comment));

        #endregion

        #region ctor

        public Comment()
        {
        }

        public Comment(PluginData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.Key = data.Key;
            this.Id = data.Id;
            this.RawData = data.RawData;
        }

        #endregion

        #region Public Properties

        public bool Approved
        {
            get { return this.approved; }
            set { this.approved = value; this.IsDirty = true; }
        }

        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                this.IsDirty = true;
            }
        }

        public string Email
        {
            get { return this.email; }
            set
            {
                this.email = value;
                this.IsDirty = true;
            }
        }

        public string Url
        {
            get { return this.url; }
            set
            {
                this.url = value;
                this.IsDirty = true;
            }
        }

        public string PermaLink
        {
            get { return this.permalink; }
            set
            {
                this.permalink = value;
                this.IsDirty = true;
            }
        }

        public DateTime Time
        {
            get { return this.time; }
            set
            {
                this.time = value;
                this.IsDirty = true;
            }
        }

        public string Image
        {
            get { return this.image; }
            set
            {
                this.image = value;
                this.IsDirty = true;
            }
        }

        public string Body
        {
            get { return this.body; }
            set
            {
                this.body = value;
                this.IsDirty = true;
            }
        }

        public bool IsSpam
        {
            get { return this.isSpam; }
            set { this.isSpam = value; }
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

        public override DataContractSerializer Serializer
        {
            get
            {
                return serializer;
            }
        }

        #endregion

        #region Public Methods

        public override bool IsValid()
        {
            bool isValid = base.IsValid();

            if (isValid)
            {
                // make sure the required parameters are present.
                if (string.IsNullOrEmpty(Name) ||
                    string.IsNullOrEmpty(Email) ||
                    string.IsNullOrEmpty(Body))
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public override bool Equals(object obj)
        {
            bool isEqual = false;

            Comment comment = obj as Comment;
            if (comment != null)
            {
                isEqual = (comment.Id == this.Id) &&
                          (comment.Key == this.Key) &&
                          (comment.RawData == this.RawData);
            }

            return isEqual;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void CopySpecific(ISerializableObject serializableObject)
        {
            Comment c = serializableObject as Comment;

            if (c != null)
            {
                this.Name = c.Name;
                this.Email = c.Email;
                this.Image = c.Image;
                this.Time = c.Time;
                this.Url = c.Url;
                this.Body = c.Body;
                this.PermaLink = c.PermaLink;
                this.IsSpam = c.IsSpam;
                this.Approved = c.Approved;
            }
        }

        #endregion

    }

}
