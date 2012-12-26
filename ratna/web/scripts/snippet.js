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
function SnippetManager() {
    this.enabled = 0
    this.keys = "";
    this.name = "";

    this.enable = function(name){
        this.name = name;
        this.enabled = 1;
    }

    this.add = function (key) {
        if (this.keys == "") {
            this.keys = key;
        }
        else {
            this.keys = this.keys + "," + key;
        }
    }

    this.getQueryString = function () {
        var query = "_s={0}".format(this.enabled);
        if (this.enabled == 1) {
            query = query + "&_sn={0}".format(this.name);
            if (this.keys != "") {
                query = query + "&_sk={0}".format(this.keys);
            }
        }

        return query;
    }
};


