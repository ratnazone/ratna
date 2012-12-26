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
/*
* SimpleModal Confirm Modal Dialog
* http://www.ericmmartin.com/projects/simplemodal/
* http://code.google.com/p/simplemodal/
*
* Copyright (c) 2010 Eric Martin - http://ericmmartin.com
*
* Licensed under the MIT license:
*   http://www.opensource.org/licenses/mit-license.php
*
* Revision: $Id: confirm.js 254 2010-07-23 05:14:44Z emartin24 $
*/

/*
jQuery(function ($) {
$('#confirm-dialog input.confirm, #confirm-dialog a.confirm').click(function (e) {
e.preventDefault();

// example of calling the confirm function
// you must use a callback function to perform the "yes" action
confirm("Continue to the SimpleModal Project page?", function () {
window.location.href = 'http://www.ericmmartin.com/projects/simplemodal/';
});
});
});
*/

var ConfirmDialog = {
    confirm: function (message, callback) {
        $('#confirm').modal({
            closeHTML: "<a href='#' title='Close' class='modal-close'>x</a>",
            position: ["20%", ],
            overlayId: 'confirm-overlay',
            containerId: 'confirm-container',
            onShow: function (dialog) {
                var modal = this;

                $('.message', dialog.data[0]).append(message);

                // if the user clicks "yes"
                $('.yes', dialog.data[0]).click(function () {
                    // call the callback
                    if ($.isFunction(callback)) {
                        callback.apply();
                    }
                    // close the dialog
                    modal.close(); // or $.modal.close();
                });
            }
        });
    }
}
