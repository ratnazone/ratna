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
var Notification = {

    Show: function (notificationid, message, autohide) {
        var notificationDiv = $(notificationid);
        var messageDiv = $("#message", notificationDiv);

        if (notificationDiv != null) {
            messageDiv.html(message);
            notificationDiv.css("display", "");

            if (autohide) {
                // call auto hide
                setTimeout(function () { Notification.Hide(notificationid) }, 3000);
            }

            //always show the close button
            $("#notificationclosebutton").click(function () {
                Notification.Hide(notificationid);
            });
        }
    },

    Hide: function (notificationid) {
        var notificationDiv = $(notificationid);

        if (notificationDiv != null) {
            notificationDiv.css("display", "none");
        }
    }


}
