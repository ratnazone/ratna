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
var SavingNotification = {
    Show: function () {
        $("#progressMode").css("display", "");
        $("#successMode").css("display", "none");
        $("#errorMode").css("display", "none");

        $("#savingnotification").fadeIn();
    },

    Hide: function () {
        $("#savingnotification").fadeOut();
    },

    OnSavingComplete: function (success, message) {
        $("#progressMode").css("display", "none");

        if (success) {
            $("#successMode").css("display", "");
            if ((message != null) && (message != "")) {
                $("#successModeMessage").text(message);
            }
            setTimeout(this.Hide, 2000);
        }
        else {
            $("#errorMode").css("display", "");
            if ((message != null) && (message != "")) {
                $("#errorModeMessage").text(message);
            }

            $("#savingnotificationclosebutton").click(function () {
                SavingNotification.Hide();
            });
        }
    }
}

/*
function OnSavingComplete(success, data) {

    $("#progressMode").css("display", "none");

    if (success) {
        $("#successMode").css("display", "");
    }
    else {
        $("#errorMode").css("display", "");
    }

    setTimeout(HideSavingNotification, 2000);
}

function ShowSavingNotification() {
    $("#progressMode").css("display", "");
    $("#successMode").css("display", "none");
    $("#errorMode").css("display", "none");

    $("#savingnotification").fadeIn();
}

function HideSavingNotification() {
    $("#savingnotification").fadeOut();
}
*/
