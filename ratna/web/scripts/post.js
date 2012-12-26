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
var PostManager = {

    post: function (url, data, callback, callerdata, json) {
        callerdatapresent = false;

        if ((json == null) || (json != "false")) {
            json = true;
        }

        if ((callerdata != null) && (callerdata != "")) {
            callerdatapresent = true;
        }

        $.ajax({
            type: "POST",
            url: url,
            data: data,
            dataType: "xml",
            async: true,
            cache: false,
            success: function (xml) {
                if (callerdatapresent) {
                    if (json) {
                        callback(true, PostManager.extractJson(xml), callerdata);
                    }
                    else {
                        callback(true, xml, callerdata)
                    }
                }
                else {
                    if (json) {
                        callback(true, PostManager.extractJson(xml));
                    }
                    else {
                        callback(true, xml)
                    }
                }
            },
            error: function (xml) {
                if (callerdatapresent) {
                    if (json) {
                        callback(false, PostManager.extractJson(xml), callerdata);
                    }
                    else {
                        callback(false, xml, callerdata)
                    }
                }
                else {
                    if (json) {
                        callback(false, PostManager.extractJson(xml));
                    }
                    else {
                        callback(false, xml)
                    }
                }
            }
        });

    },

    extractJson: function (xml) {
        $json = null;
        var defaultJson = false;

        try {
            $json = $(xml).find("string").text();

            if ($json == null || $json == "") {
                defaultJson = true;
            }
        }
        catch (err) {
            //unable to get the json, that means something went
            //wrong in the server and didn't get xml output.
            defaultJson = true;
        }

        if (defaultJson) {
            $json = '{"success": false, "error" : "Unknown error - Please check server logs for more information or contact your administrator."}';
        }

        return $json;
    }

}
