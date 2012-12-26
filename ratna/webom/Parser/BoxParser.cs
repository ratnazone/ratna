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
namespace Jardalu.Ratna.Web.Parser
{

    #region using

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// A very simple parser used to get contents within tokens.
    /// 
    /// Rules, the content must start with "[", if it does not, the parser
    /// will return false when MoveNext is done.
    /// 
    /// tokens must contain just numbers and alphabets, underscore allowed.
    /// </summary>
    public class BoxParser
    {

        #region private fields

        private IList<string> tags;
        private string content;

        private int current;
        private string currentTag;
        private string currentData;

        #endregion

        #region ctor

        public BoxParser(params string[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new ArgumentNullException("args");
            }

            List<string> tags = new List<string>(args.Length);
            tags.AddRange(args);

            Initialize(tags);
        }

        public BoxParser(IList<string> tags)
        {
            Initialize(tags);
        }

        #endregion

        #region public properties

        public string CurrentTag
        {
            get
            {
                return currentTag;
            }
        }

        public string CurrentData
        {
            get
            {
                return currentData;
            }
        }

        #endregion

        #region public methods

        public void Parse(string content)
        {
            this.content = content;
            this.current = 0;
        }

        public bool MoveNext()
        {
            bool nextfound = false;

            if (current < content.Length)
            {
                //locate the next start of '['
                int start = content.IndexOf('[', current);

                //is it the end
                if (start == -1)
                {
                    current = content.Length;
                }
                
                while (start != -1)
                {
                    int end = content.IndexOf(']', start + 1);

                    if (end != -1)
                    {
                        // check if the token is supposed to be cared for

                        string tag = content.Substring(start+ 1, end - start - 1);

                        if (tags.Contains(tag))
                        {
                            // must be cared for.
                            // search for the end.

                            string tagEnd = string.Format("[/{0}]", tag);

                            int tagEndStart = content.IndexOf(tagEnd, end);

                            if (tagEndStart == -1)
                            {
                                throw new InvalidOperationException();
                            }

                            currentTag = tag;
                            currentData = content.Substring(end + 1, tagEndStart - end - 1);

                            //found
                            nextfound = true;

                            int tagEndEnd = tagEndStart + tagEnd.Length;

                            current = tagEndEnd + 1;
                            break;
                        }
                        else
                        {
                            // tag not cared for, find the next start
                            start = content.IndexOf('[', start + 1);
                        }

                    }
                    else
                    {
                        // [ found but no ] found.
                        current = content.Length;
                        break;
                    }


                }

            }

            return nextfound;
        }

        #endregion

        #region private methods

        private void Initialize(IList<string> tags)
        {
            if (tags == null || tags.Count == 0)
            {
                throw new ArgumentException("tokens");
            }

            foreach (string token in tags)
            {
                if (string.IsNullOrEmpty(token) ||
                    !Jardalu.Ratna.Utilities.Utility.IsValidAlphaNumbericUnderscore(token))
                {
                    throw new ArgumentException(string.Format("{0} token is invalid", token));
                }
            }

            this.tags = tags;
        }

        #endregion

    }

}
