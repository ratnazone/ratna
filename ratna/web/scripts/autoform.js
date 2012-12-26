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
var AutoForm = {

    selfCallback: false,

    submit: function (formname) {
        AutoForm.selfCallback = false;
        AutoForm.submit(formname, AutoForm.OnFormSubmitted);
    },

    submit: function (formname, callback) {

        if (callback == null) {
            callback = AutoForm.OnFormSubmitted;
        }

        if (!AutoForm.selfCallback) {
            $("#" + formname).css("opacity", "0.5");
        }

        var data = "";
        var $inputs = $('#' + formname + ' :input');

        var data = "form=" + formname;
        var fields = "";

        $inputs.each(function () {
            data += "&" + this.name + "=" + escape($(this).val());
            if (fields == "") {
                fields = this.name;
            }
            else {
                fields += "," + this.name;
            }
        });

        data += "&fields=" + fields;

        PostManager.post('/service/forms.asmx/AddReponse', data, callback, formname);
    },

    OnFormSubmitted: function (success, data, formname) {

        var response = jQuery.parseJSON(data);
        if (success && response.success) {
            $("#" + formname).css("opacity", "0.5");
            $("#" + formname + " :input").prop("disabled", true);

            var div = $("#" + formname).append("div");
            div.css("text-align:center");
            div.css("width:100%");
            div.css("height:100%");
            div.text("Form submitted.");
        }
        else {

            var random = Math.floor((Math.random() * 100000) + 1);
            var divid = "___autoformfailed_" + random;

            var divhtml = "<div id='" + divid + "'><span class='errorValidate'>Form submission failed. Error - " + response.error + "</span></div>";
            $(divhtml).insertAfter($("#" + formname));

            $("#" + divid).fadeOut(4000);

            //submission failed. resurface the form
            $("#" + formname).css("opacity", "1");
        }
    }

}
