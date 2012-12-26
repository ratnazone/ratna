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
///
/// to use SetControlAtCenter method, get the parent control
/// and get the child control.
///
function SetControlAtCenter(parentControl, childControl) {
    var h = parentControl.height();
    var w = parentControl.width();

    var marginTop = (h - (childControl.height())) / 2;
    childControl.css("margin-top", marginTop);

    var marginLeft = (w - (childControl.width())) / 2;
    childControl.css("margin-left", marginLeft);
}

function SetMainDivAtCenter() {
    SetControlAtCenter($(document), $("#mainDiv"));
}

/* String extension */
String.prototype.format = function () {
    var s = this;
    for (var i = 0; i < arguments.length; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i]);
    }
    return s;
}

String.prototype.trim = function () {
    var s = this;
    return s.replace(/^\s+|\s+$/g, '');
}

String.prototype.formatEscape = function () {
    var s = this;
    for (var i = 0; i < arguments.length; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, encodeURIComponent(arguments[i]));
    }
    return s;
}

String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};
