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
/// this javascript code is to support editing of forms
///

$.validator.setDefaults({
    submitHandler: function (form) {
        OnSaveClick();
        return false;
    }
});

$(document).ready(function () {

    $("#editentryform").validate();


    $("#cancelbutton").click(function () {
        OnCancelClick();
    });


});


function OnCancelClick() {
    var url = Constants.Forms.EntriesUrl.format($("#formname").val());
    this.window.location = url;
}

function OnSaveClick() {

    SavingNotification.Show();

    try {
        // save the form
        var formname = $("#formname").val();
        var uid = $("#uid").val();
        var fieldName; // to rename id to appropriate fieldname

        var data = "form={0}&uid={1}".format(formname, uid);
        var fields = "";

        // grab all the app properties value
        $("#entrybody input").each(function () {
            var input = $(this);

            if (data != "") {
                data += "&";
            }

            if (fields != "") {
                fields += ",";
            }

            // get the fieldname 
            fieldName = GetNameFromId(input.attr("id"));

            data += fieldName + "=" + encodeURIComponent(input.val());
            fields += fieldName;
        });


        //grab the text area values
        $("#entrybody textarea").each(function () {
            var input = $(this);

            if (data != "") {
                data += "&";
            }

            if (fields != "") {
                fields += ",";
            }

            // get the fieldname 
            fieldName = GetNameFromId(input.attr("id"));

            data += fieldName + "=" + encodeURIComponent(input.val());
            fields += fieldName;
        });

        // add the fields
        data = "{0}&fields={1}".format(data, fields);

        PostManager.post(Constants.FormsManageService.EditEntryUrl, data, OnSaveComplete);

    }
    finally {

        return false;
    }
}

///
/// OnSaveComplete - called when the save is completed for the entry
///
function OnSaveComplete(success, data) {
    var response = jQuery.parseJSON(data);
    var message = "";

    if (!response.success) {
        message = response.error;
    }
    else {


        if (response.uid != null && response.uid != "") {
            var uid = $("#uid").val();

            if (uid == null || uid == "" || uid == "00000000-0000-0000-0000-000000000000") {
                // update the uid
                $("#uid").val(response.uid);

                // set the uid in the breadcrumb
                var bcrumburl = Constants.Forms.EditEntryUrl.format($("#formname").val(), response.uid);
                BreadCrumb.SetLastAnchorHref(bcrumburl);
            }
        }
    }

    SavingNotification.OnSavingComplete(response.success, message);
}



function GetNameFromId(id) {
    var name = id;

    if (name != null) {
        name = id.substr(3);
    }

    return name;
}

