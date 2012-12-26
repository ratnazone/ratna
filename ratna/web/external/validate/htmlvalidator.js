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
/*
(c) http://jardalu.com
All Rights Reserved

Checks whether a string contains tag or not. It also
allows to allow certain tags.
*/

function HtmlValidator() {

    this.allowedTags = new Array();

    this.allowedTags[0] = "br";
    this.allowedTags[1] = "strong";
    this.allowedTags[2] = "em";
    this.allowedTags[3] = "i";
}

HtmlValidator.prototype.IsValid = function(html) {
    var valid = true;
    if (html != null) {

        var start = 0;

        //search for
        var sindex = html.indexOf('<', start);
        var eindex = 0;
        while (sindex != -1) {
            eindex = html.indexOf('>', sindex);
            if (eindex != -1) {
                var tag = html.substr(sindex + 1, eindex - sindex - 1);

                //if the tag starts with '/' or ends with '/', remove it
                if (tag != null) {
                    if (tag[0] == '/') {
                        tag = tag.substr(1, tag.length - 1);
                    }
                    else if (tag[tag.length - 1] == '/') {
                        tag = tag.substr(0, tag.length - 1);
                    }
                }

                if (!this.IsTagAllowed(tag)) {
                    valid = false;
                    break;
                }
                sindex = html.indexOf('<', eindex);
            }
            else {
                //not valid html as there is no ending tag
                valid = false;
                break;
            }

        } //while

    }
    return valid;
}

HtmlValidator.prototype.IsTagAllowed = function(tag) {
    var allowed = false;
    for (var i = 0; i < this.allowedTags.length; i++) {
        if (this.allowedTags[i] == tag) {
            allowed = true;
            break;
        }
    }
    return allowed;
}

HtmlValidator.prototype.GetAllowedTags = function() {
    var allowed = "";

    for (var i = 0; i < this.allowedTags.length; i++) {
        if (i != 0) {
            allowed += ", ";
        }
        allowed += this.allowedTags[i];
    }

    return allowed;
}
