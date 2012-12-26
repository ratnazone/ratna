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

CKEDITOR.dialog.add('form',function(a){var b={action:1,id:1,method:1,enctype:1,target:1};return{title:a.lang.form.title,minWidth:350,minHeight:200,onShow:function(){var e=this;delete e.form;var c=e.getParentEditor().getSelection().getStartElement(),d=c&&c.getAscendant('form',true);if(d){e.form=d;e.setupContent(d);}},onOk:function(){var c,d=this.form,e=!d;if(e){c=this.getParentEditor();d=c.document.createElement('form');!CKEDITOR.env.ie&&d.append(c.document.createElement('br'));}if(e)c.insertElement(d);this.commitContent(d);},onLoad:function(){function c(e){this.setValue(e.getAttribute(this.id)||'');};function d(e){var f=this;if(f.getValue())e.setAttribute(f.id,f.getValue());else e.removeAttribute(f.id);};this.foreach(function(e){if(b[e.id]){e.setup=c;e.commit=d;}});},contents:[{id:'info',label:a.lang.form.title,title:a.lang.form.title,elements:[{id:'txtName',type:'text',label:a.lang.common.name,'default':'',accessKey:'N',setup:function(c){this.setValue(c.data('cke-saved-name')||c.getAttribute('name')||'');},commit:function(c){if(this.getValue())c.data('cke-saved-name',this.getValue());else{c.data('cke-saved-name',false);c.removeAttribute('name');}}},{id:'action',type:'text',label:a.lang.form.action,'default':'',accessKey:'T'},{type:'hbox',widths:['45%','55%'],children:[{id:'id',type:'text',label:a.lang.common.id,'default':'',accessKey:'I'},{id:'enctype',type:'select',label:a.lang.form.encoding,style:'width:100%',accessKey:'E','default':'',items:[[''],['text/plain'],['multipart/form-data'],['application/x-www-form-urlencoded']]}]},{type:'hbox',widths:['45%','55%'],children:[{id:'target',type:'select',label:a.lang.common.target,style:'width:100%',accessKey:'M','default':'',items:[[a.lang.common.notSet,''],[a.lang.common.targetNew,'_blank'],[a.lang.common.targetTop,'_top'],[a.lang.common.targetSelf,'_self'],[a.lang.common.targetParent,'_parent']]},{id:'method',type:'select',label:a.lang.form.method,accessKey:'M','default':'GET',items:[['GET','get'],['POST','post']]}]}]}]};});
