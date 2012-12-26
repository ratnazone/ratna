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
replace
 
usage : script replace.js token_file input_file output_file
*/

var log = false;

function Argument() {

    this.arguments = WScript.Arguments;
    this.tokenfile = null;
    this.inputfile = null;
    this.outputfile = null;
    this.valid = true;


    this.Process = function() {
        if (this.arguments.Count() != 3) {
            this.valid = false;
        }
        else {

            this.tokenfile = this.arguments(0);
            this.inputfile = this.arguments(1);
            this.outputfile = this.arguments(2);
        }
    }

}

function WriteLine(message) {
    WScript.Echo(message);
}

function Log(message) {
    if (log) {
        WScript.Echo(message);
    }
}

function Info(message) {
    WScript.Echo(message);
}

function replacetokens(tokenfile, inputfile, outputfile) {
    Log("replacetokens called");

    var ForReading = 1, ForWriting = 2, ForAppending = 8;
    var overwrite = true;

    var fso = new ActiveXObject("Scripting.FileSystemObject");

    if (!fso.FileExists(tokenfile)) {
        throw "Unable to read file " + tokenfile;
    }

    if (!fso.FileExists(inputfile)) {
        throw "Unable to read file " + inputfile;
    }

    Info("reading token file : " + tokenfile);

    var f = fso.OpenTextFile(tokenfile, ForReading);
    var tokenContents = f.ReadAll();
    f.Close();

    Info("reading input file : " + inputfile);

    f = fso.OpenTextFile(inputfile, ForReading);
    var fileContents = f.ReadAll();
    f.Close();

    Info("parsing token file");
    var tokens = parseTokens(tokenContents);
    Info("parsing done");
            
    for (var key in tokens) {
        Log("Token : " + key + " Value: " + tokens[key]);
        fileContents = fileContents.replace(new RegExp(key, "g"), tokens[key]);
    }

    Info("replacing tokens in memory done");

    Log(fileContents);

    Info("saving output file : " + outputfile);

    if (fso.FileExists(outputfile) && !overwrite) {
        throw "File " + outputfile + " already exists.";
    }

    var outfile = fso.OpenTextFile(outputfile, ForWriting, true);
    outfile.Write(fileContents);
    outfile.Close();

    Info("saved");
}

String.prototype.trim = function() {
    return (this.replace(/^[\s\xA0]+/, "").replace(/[\s\xA0]+$/, ""))
}

String.prototype.startsWith = function(str) {
    return (this.match("^" + str) == str)
}

String.prototype.removeExtraWhiteSpace = function() {
    return this.replace(/\s+/g,' ');
}

function parseTokens(contents) {
    var tokens = new Object();

    Log("parseTokens called");

    var allLines = contents.split("\r\n");

    Info("tokens found : " + allLines.length);

    for (var i = 0; i < allLines.length; i++) {
        var line = allLines[i];
        var compressed = line.removeExtraWhiteSpace();
        if (compressed.startsWith("#")) continue;

        var keyvalue = compressed.split(" ");
        if (keyvalue.length != 2) {
            Log("Ignoring [tokens=" + keyvalue.length + "]: " + compressed);
            continue;
        }

        tokens[keyvalue[0]] = keyvalue[1];
    }

    Log("returning from parseTokens");

    return tokens;
}

function main() {
    var argument = new Argument();
    argument.Process();

    if (!argument.valid) {
        WriteLine("Usage : ")
    }
    else {
        try {
            replacetokens(argument.tokenfile, argument.inputfile, argument.outputfile);
        } catch (err) {
            WriteLine("Error : " + err);
        }
    }
}


main();
