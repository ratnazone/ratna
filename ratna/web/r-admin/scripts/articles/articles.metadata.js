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
/// this javascript code is to support articles' edit page
///

$(document).ready(function () {
    Setup();
});

function Setup() {

    $("#cancelbutton").click(function () {
        OnCancelClick();
    });

    $("#savebutton").click(function () {
        OnSaveClick();
    });

}

function OnSaveClick() {

    var urlkeyval = $("#urlkey").val();
    var tags = $("#tags").val();
    var navtab = $("#navigationtab").val();
    var description = $("#description").val();
    var view = $("#articleview").val();
    var isPageView = false;

    if (view == "page") {
        isPageView = true;
    }

    var data = null;
    var posturl = null;

    if (!isPageView) {
        var summary = $("#summary").val();
        data = "urlKey={0}&navigationtab={1}&tags={2}&description={3}&summary={4}".formatEscape(urlkeyval, navtab, tags,description, summary);
        posturl = Constants.ArticleService.SaveMetadataUrl;
    }
    else {
        var head = $("#headtext").val();
        data = "urlKey={0}&navigationtab={1}&tags={2}&description={3}&head={4}".formatEscape(urlkeyval, navtab, tags, description, head);
        posturl = Constants.ArticleService.SavePageMetadataUrl;
    }


    SavingNotification.Show();
    PostManager.post(posturl, data, OnSaveComplete);
}


///
/// OnSaveComplete - called when the save is completed for the article's metadata
///
function OnSaveComplete(success, data) {
    var response = jQuery.parseJSON(data);
    var message = "";

    if (!response.success) {
        message = response.error;
    }
    else {

        // save has been successful.
        // do nothing.

    }

    SavingNotification.OnSavingComplete(response.success, message);
}

function OnCancelClick() {
    window.location = CancelUrl;
}
