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

CKEDITOR.dialog.add('button',function(a){function b(c){var e=this;var d=e.getValue();if(d){c.attributes[e.id]=d;if(e.id=='name')c.attributes['data-cke-saved-name']=d;}else{delete c.attributes[e.id];if(e.id=='name')delete c.attributes['data-cke-saved-name'];}};return{title:a.lang.button.title,minWidth:350,minHeight:150,onShow:function(){var e=this;delete e.button;var c=e.getParentEditor().getSelection().getSelectedElement();if(c&&c.is('input')){var d=c.getAttribute('type');if(d in {button:1,reset:1,submit:1}){e.button=c;e.setupContent(c);}}},onOk:function(){var c=this.getParentEditor(),d=this.button,e=!d,f=d?CKEDITOR.htmlParser.fragment.fromHtml(d.getOuterHtml()).children[0]:new CKEDITOR.htmlParser.element('input');this.commitContent(f);var g=new CKEDITOR.htmlParser.basicWriter();f.writeHtml(g);var h=CKEDITOR.dom.element.createFromHtml(g.getHtml(),c.document);if(e)c.insertElement(h);else{h.replace(d);c.getSelection().selectElement(h);}},contents:[{id:'info',label:a.lang.button.title,title:a.lang.button.title,elements:[{id:'name',type:'text',label:a.lang.common.name,'default':'',setup:function(c){this.setValue(c.data('cke-saved-name')||c.getAttribute('name')||'');},commit:b},{id:'value',type:'text',label:a.lang.button.text,accessKey:'V','default':'',setup:function(c){this.setValue(c.getAttribute('value')||'');},commit:b},{id:'type',type:'select',label:a.lang.button.type,'default':'button',accessKey:'T',items:[[a.lang.button.typeBtn,'button'],[a.lang.button.typeSbm,'submit'],[a.lang.button.typeRst,'reset']],setup:function(c){this.setValue(c.getAttribute('type')||'');},commit:b}]}]};});
