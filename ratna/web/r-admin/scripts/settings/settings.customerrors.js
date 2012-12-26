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
/// javascript code for Configuration.ascx
$(document).ready(function () {
    CustomErrorsSettingsSetup();
});

function CustomErrorsSettingsSetup() {

    $("#customErrorsCancelButton").click(function () {
        OnCancelClick();
    });

    $("#customErrorsSaveButton").click(function () {
        OnCustomErrorsSaveButtonClick($(this));
    });
}


function OnCustomErrorsSaveButtonClick(saveButton) {

    var error404 = $("#error404").val();
    var error500 = $("#error500").val();
    var otherErrors = $("#errorothers").val();

    var data = "error404={0}&error500={1}&otherErrors={2}".format(error404, error500, otherErrors);

    PostManager.post(Constants.SiteConfigurationService.UpdateCustomResponsesUrl, data, OnCustomErrorsSaveCompleted);

}

function OnCustomErrorsSaveCompleted(success, data) {
 
    var response = jQuery.parseJSON(data);
    var message = "";

    if (!response.success) {
        message = response.error;
    }
    else {
        // save has been successful.
        message = L_SavedSuccees;
    }

    Notification.Show("#cnotification", message, response.success);
}

function OnCancelClick() {
    window.location = CancelUrl;
}
