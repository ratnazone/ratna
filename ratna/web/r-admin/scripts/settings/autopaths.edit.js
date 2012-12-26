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
/// this javascript code is to support templates edit page
///

$(document).ready(function () {
    Setup();
});

function Setup() {

    $("#cancelbutton").click(function () {
        OnCancelClick();
    });
   
    $("#savebutton").click(function () {
        //OnSaveClick();
    });

    SetValidation();
}

function SetValidation() {

    $.validator.setDefaults({
        submitHandler: function () {
            OnSaveClick();
            return false;
        }
    });

    $("#autopath_form").validate();

}

function OnCancelClick() {
    this.window.location = Constants.Settings.AutoPathsUrl;
}


function OnSaveClick() {

    try {
        var urlPath = $("#pathurl").val();
        var title = $("#pathtitle").val();
        var pathType = $("#pathTypesSelect").val();
        var pagesize = $("#pagesize").val();
        var navigation = $("#pathNavigation").val();
        
        var data = "path={0}&title={1}&pathType={2}&pagesize={3}&nav={4}".format(urlPath, title, pathType, pagesize, navigation);

        SavingNotification.Show();
        PostManager.post(Constants.AutoPathsService.SaveUrl, data, OnSaveComplete, true /* json */);
    }
    finally {

        return false;
    }
}

///
/// OnSaveComplete - called when the save is completed for the auto path
///
function OnSaveComplete(success, data) {
    var response = jQuery.parseJSON(data);
    var message = "";

    if (!response.success) {
        message = response.error;
    }
    else {
        // disable the urlpath
        $("#pathurl").prop('disabled', true);
    }

    SavingNotification.OnSavingComplete(response.success, message);
}
