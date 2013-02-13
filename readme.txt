#######################################################################################

                       README.TXT - For Ratna development
                       (C) Jardalu.com

#######################################################################################

0. License

   Read the attached license (license.txt). If you build Ratna software (or read the
   code of Ratna software), it will be assumed that you have read the license and agree 
   to the terms and conditions of the license. If you do not agree with the terms and
   conditions of the license, don't build Ratna software locally and don't read code
   of Ratna software.

1. Prerequisites 

   To successfully build Ratna software, following prerequisites are required.

   a) Operating System - Windows 2008 Server or Windows 7 Professional
   b) Powershell v2.0 or above
   c) .NET 4.0 
   d) Visual Studio 2010
          Free versions "Visual Web Developer 2010 Express" and "Microsoft Visual C#  
          2010 Express" will also work.
   e) SQL Server 2008
          Free version "Microsoft SQL Server 2008 Express" will also work
          

2. Build

   In this document "c:\ratna_code" is the install folder. If you have unzipped Ratna
   code at "c:\mylocation", replace "c:\ratna_code" with "c:\mylocation"
   
   One Time Changes
   
   a) Open tools/profile.ps1 in notepad and set variable "$cobra_base" value to "c:\ratna_code"
   
   Compilation         
   
   a) Open powershell in c:\ratna_code
   
   b) Load cobra build system, running the following command in powershell
      PS c:\ratna_code>. .\tools\profile.ps1
      
      [when the cobra system loads, the title of the window and the prompt will read cobra]
   
   c) To build ratna, use the following syntax
      cobra c:\ratna_code>cd ratna
      cobra c:\ratna_code\ratna>build
      
      [To build the ship version, use the following command]
      cobra c:\ratna_code\ratna>build -ship
      
      The above build call will build all the necessary files.
      
   d) Creating installable files
      
      To create the install folder, use the following commands
      
      cobra c:\ratna_code>cd .\boxer\creator
      cobra c:\ratna_code\boxer\creator>.\GatherFiles.ps1 c:\ratna_code\target\install debug
      
      [To create ship version, use the following command]
      cobra c:\ratna_code\boxer\creator>.\GatherFiles.ps1 c:\ratna_code\target\install ship
      
   e) Install Ratna
   
      Ratna can now be installed from c:\ratna_code\target\install\installer
      
      Visit http://ratnazone.com/ for more information on installing Ratna software.   
 
--