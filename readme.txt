Name:    MFilesImporter
Author:  martin kramer <martin.kramer@lostindetails.com>
License: GPL. If you need a different license, shoot me an e-Mail.

Description
===========
MFilesImporter is a windows commandline tool that allows importing files specified in a CSV file into an M-Files Vault.
It uses the M-Files API (namely Interop.MFilesApiInternal.dll) to allow a mass file import on the server. This dll is part of the M-Files API (which is installed with every M-File installation) and not contained inside this repository.

Build
=====
1) Locate the Interop.MFilesApiInternal.dll of your M-Files installation
   Hint: look in c:\Program Files\M-Files\[Version]\bin\anycpu for example
2) Put it into the "Dll" folder
3) Rebuild the Application

How to use
==========
1) Create an M-Files User named "importer" with password "importer" and give it access to the specific vault.
   Hint: username, password and vault name is picked up from the .config file but can be overriden using commandline arguments.
2) execute "MFilesImporter.exe --test-connection" to see if it works

Warning
=======
As always use common sense:
Do not run it on a production system!
Make a backup if you are not prepared to lose the data.
