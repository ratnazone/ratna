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

(function(){CKEDITOR.dialog.add('pastetext',function(a){return{title:a.lang.pasteText.title,minWidth:CKEDITOR.env.ie&&CKEDITOR.env.quirks?368:350,minHeight:240,onShow:function(){this.setupContent();},onOk:function(){this.commitContent();},contents:[{label:a.lang.common.generalTab,id:'general',elements:[{type:'html',id:'pasteMsg',html:'<div style="white-space:normal;width:340px;">'+a.lang.clipboard.pasteMsg+'</div>'},{type:'textarea',id:'content',className:'cke_pastetext',onLoad:function(){var b=this.getDialog().getContentElement('general','pasteMsg').getElement(),c=this.getElement().getElementsByTag('textarea').getItem(0);c.setAttribute('aria-labelledby',b.$.id);c.setStyle('direction',a.config.contentsLangDirection);},focus:function(){this.getElement().focus();},setup:function(){this.setValue('');},commit:function(){var b=this.getValue();setTimeout(function(){a.fire('paste',{text:b});},0);}}]}]};});})();
