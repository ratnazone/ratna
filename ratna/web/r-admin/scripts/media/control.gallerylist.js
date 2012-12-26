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
//
// gallery.list.js
//

$(document).ready(function () {
    Setup();
});

function Setup() {

    $(".deleteGalleryImageButton").click(function () {
        OnDeleteGalleryButtonClick($(this));
    });

}

function OnDeleteGalleryButtonClick(deleteButton) {
    if (deleteButton != null) {
        var deleteInput = deleteButton.prev();
        var urlKey = deleteInput.val();

        ConfirmDialog.confirm(
            L_DeleteGalleryConfirmation.format(urlKey), function () {
                OnDeleteGalleryConfirmed(urlKey);
            }
        );
    }
}

function OnDeleteGalleryConfirmed(urlKey) {
    var data = "url={0}".formatEscape(urlKey);
    PostManager.post(Constants.GalleryService.DeleteUrl, data, OnDeleteGalleryCompleted);
}

function OnDeleteGalleryCompleted(success, data) {
    var response = jQuery.parseJSON(data);
    if (response.success) {

        var deletedUrlKey = response.url;
        var hiddenInputField = $('input[value="' + deletedUrlKey + '"]');

        //get the parent's parent
        var tr = hiddenInputField.parent().parent();
        var tbody = tr.parent();

        // remove the tr
        tr.remove();

        // if there are no tr's left, show the default tr
        // of "no gallery" ....
        var trsLeft = 0;
        tbody.find("tr").each(function () {
            trsLeft++;
        });

        // only one TR left (that is the none)
        if (trsLeft == 1) {
            var noneTR = tbody.find('tr[id^="none"]');
            if (noneTR != null) {
                noneTR.show();
            }
        }

        ShowNotification(L_DeleteGallerySuccess, true);
    }
    else {
        ShowNotification(response.error, false);
    }
}

function ShowNotification(message, autohide) {
    Notification.Show("#commonnotification", message, autohide)
}
