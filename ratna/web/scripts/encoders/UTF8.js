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
// Namespace UTF8
var UTF8 = (function () {
    return {
        // Encodes UCS2 into UTF8
        // Returns an array of numbers (bytes)
        encode: function (str) {
            var len = str.length;
            var result = [];
            var code;
            var i;
            for (i = 0; i < len; i++) {
                code = str.charCodeAt(i);
                if (code <= 0x7f) {
                    result.push(code);
                } else if (code <= 0x7ff) {                         // 2 bytes
                    result.push(0xc0 | (code >>> 6 & 0x1f),
                                0x80 | (code & 0x3f));
                } else if (code <= 0xd700 || code >= 0xe000) {      // 3 bytes
                    result.push(0xe0 | (code >>> 12 & 0x0f),
                                0x80 | (code >>> 6 & 0x3f),
                                0x80 | (code & 0x3f));
                } else {                                            // 4 bytes, surrogate pair
                    code = (((code - 0xd800) << 10) | (str.charCodeAt(++i) - 0xdc00)) + 0x10000;
                    result.push(0xf0 | (code >>> 18 & 0x07),
                                0x80 | (code >>> 12 & 0x3f),
                                0x80 | (code >>> 6 & 0x3f),
                                0x80 | (code & 0x3f));
                }
            }
            return result;
        },

        // Decodes UTF8 into UCS2
        // Returns a string
        decode: function (bytes) {
            var len = bytes.length;
            var result = "";
            var code;
            var i;
            for (i = 0; i < len; i++) {
                if (bytes[i] <= 0x7f) {
                    result += String.fromCharCode(bytes[i]);
                } else if (bytes[i] >= 0xc0) {                                   // Mutlibytes
                    if (bytes[i] < 0xe0) {                                       // 2 bytes
                        code = ((bytes[i++] & 0x1f) << 6) |
                                (bytes[i] & 0x3f);
                    } else if (bytes[i] < 0xf0) {                                // 3 bytes
                        code = ((bytes[i++] & 0x0f) << 12) |
                               ((bytes[i++] & 0x3f) << 6) |
                                (bytes[i] & 0x3f);
                    } else {                                                     // 4 bytes
                        // turned into two characters in JS as surrogate pair
                        code = (((bytes[i++] & 0x07) << 18) |
                                ((bytes[i++] & 0x3f) << 12) |
                                ((bytes[i++] & 0x3f) << 6) |
                                 (bytes[i] & 0x3f)) - 0x10000;
                        // High surrogate
                        result += String.fromCharCode(((code & 0xffc00) >>> 10) + 0xd800);
                        // Low surrogate
                        code = (code & 0x3ff) + 0xdc00;
                    }
                    result += String.fromCharCode(code);
                } // Otherwise it's an invalid UTF-8, skipped.
            }
            return result;
        }
    };
} ());
