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
Copyright (c) 2003-2011, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.dialog.add('hiddenfield',function(a){return{title:a.lang.hidden.title,hiddenField:null,minWidth:350,minHeight:110,onShow:function(){var e=this;delete e.hiddenField;var b=e.getParentEditor(),c=b.getSelection(),d=c.getSelectedElement();if(d&&d.data('cke-real-element-type')&&d.data('cke-real-element-type')=='hiddenfield'){e.hiddenField=d;d=b.restoreRealElement(e.hiddenField);e.setupContent(d);c.selectElement(e.hiddenField);}},onOk:function(){var g=this;var b=g.getValueOf('info','_cke_saved_name'),c=g.getValueOf('info','value'),d=g.getParentEditor(),e=CKEDITOR.env.ie&&!(CKEDITOR.document.$.documentMode>=8)?d.document.createElement('<input name="'+CKEDITOR.tools.htmlEncode(b)+'">'):d.document.createElement('input');e.setAttribute('type','hidden');g.commitContent(e);var f=d.createFakeElement(e,'cke_hidden','hiddenfield');if(!g.hiddenField)d.insertElement(f);else{f.replace(g.hiddenField);d.getSelection().selectElement(f);}return true;},contents:[{id:'info',label:a.lang.hidden.title,title:a.lang.hidden.title,elements:[{id:'_cke_saved_name',type:'text',label:a.lang.hidden.name,'default':'',accessKey:'N',setup:function(b){this.setValue(b.data('cke-saved-name')||b.getAttribute('name')||'');},commit:function(b){if(this.getValue())b.setAttribute('name',this.getValue());else b.removeAttribute('name');}},{id:'value',type:'text',label:a.lang.hidden.value,'default':'',accessKey:'V',setup:function(b){this.setValue(b.getAttribute('value')||'');},commit:function(b){if(this.getValue())b.setAttribute('value',this.getValue());else b.removeAttribute('value');}}]}]};});
