# WatchNRun
Waits for a file to get created, or a file to be modified, then runs whatever .bat or .exe you choose.
This might be useful to detect when a 3rd party application does something (via temp file creation or log file addition) and you want to...

    - send an email (via bmail or sendemail)
    - shutdown the computer (via psshutdown)
    - launch a program remotely (via psexec)
    - start or stop a Windows service (via net or sc)
    - save a picture from a nearby IP camera (via Wget)

WatchNRun can also be used to run a batch file (on the server) from any ASP, PHP, or other page.
Simply code your page to drop a file, when WatchNRun finds that file was created it runs your other process.

Tested on Windows XP, Vista, 7, 8, 8.1 and Server 2003, 2008, 2012

A scripted install/uninstall is not included with this software.

This program runs as a service; without any GUI, taskbar, or system tray icon.

<b>Installation:</b>

1) Ensure the Microsoft .NET Framework 4.x is installed
2) Right-click on the .zip file you downloaded, select properties, click the Unblock button (if this button is not present just proceed)
3) Extract the contents of the .zip file (usually to C:\WatchNRun)
4) Modify WatchNRunConfig.xml as indicated below
5) Right-click on _setup_.bat and select "Run as administrator" to install the service (see note at bottom of this text)

View the default .xml file that's included. Anytime you make a change in WatchNRunConfig.xml you must restart the service for it to take effect.

<b>.xml Settings:</b>

There are two sections <FileCreatedHandler> and <FileModifiedHandler>, if you are not going to use one simply delete it. To watch several different files simply duplicate the entire section and specify different settings, or use * in the filename.

For the <FileCreatedHandler> section...
    
    <Path>C:\folder</Path> set this to the directory where the file will be
    <FileNameFilter>filecreated.txt</FileNameFilter> set this to the name of the file (or file*.txt is also acceptable for multiple)
    <Delay>0</Delay> change this to 1 or 2 if the software is too quick in detecting the file and running your process
    <RunProcess>C:\folder\filewascreated.bat</RunProcess> full path to the .bat or .exe you want to run

For the <FileModifiedHandler> section...
    
    <Path>C:\folder</Path> set this to the directory where the file being modified is located
    <FileNameFilter>filemodified.txt</FileNameFilter> set this to the name of the file (or file*.txt is also acceptable for multiple)
    <SkipEmptyLines>1</SkipEmptyLines> under most circumstances leave this set to 1
    <PatternToMatch>modified now</PatternToMatch> this is the text that must be found in the last line to trigger running your process
        (set this to blank: <PatternToMatch></PatternToMatch> if you want to run your process any time the file changes)
    <Delay>0</Delay> change this to 1 or 2 if the software is too quick in detecting the change and running your process
    <RunProcess>C:\folder\filewasmodified.bat</RunProcess> full path to the .bat or .exe you want to run

<b>Usage:</b>

No GUI is displayed, check Task Manager or Control Panel > Administrative Tools > Services to see that it's running.

Note: Under Windows Vista and later OS versions you may receive an error (HRESULT: 0x80131515) when installing the service.
The solution is to save the .zip file, right-click on it, select properties, and click the Unblock button.
Then extract the contents of the .zip file. Right-click on _setup_.bat and select "Run as administrator" and it will work.
