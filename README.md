<p align="center">
<img alt="[Volume Control Banner]" src="https://i.imgur.com/rMbNIhU.png">
<a href="https://github.com/radj307/volume-control/releases"><img alt="GitHub tag (latest by date)" src="https://img.shields.io/github/v/tag/radj307/volume-control?color=e8e8e7&label=Latest%20Version&logo=github&logoColor=e8e8e7&style=for-the-badge"></a>
<a href="https://github.com/radj307/volume-control/releases"><img alt="Downloads" src="https://img.shields.io/github/downloads/radj307/volume-control/total?color=e8e8e7&logo=github&logoColor=e8e8e7&style=for-the-badge"></a>
 <br />
Simple, universal application-specific hotkeys using the Windows Mixer.  
</p>
<h1></h1>

&nbsp;&nbsp;&nbsp;&nbsp;I made this because of my frustration with being unable to use keyboard hotkeys to control the volume of Deezer after switching from Spotify, and I figured why not just make hotkeys that work for any process.  
You can use the GUI to switch targets, or use configurable hotkeys to cycle between them.  

&nbsp;&nbsp;&nbsp;&nbsp;There is also a commandline version available, it can get or set the volume of specific applications using a __PID__ or __Process Name__, and is useful for making scripts.  

# Installation
 1. Download the [latest release](https://github.com/radj307/volume-control/releases).    
    - _VolumeControl.exe_ is the main GUI version that adds hotkeys.
    - _VolumeControlCLI.exe_ is the commandline utility for adjusting or querying an application's volume.  
      See [CLI usage](https://github.com/radj307/volume-control#volumecontrolcli) for more information about the commandline version.
 2. Place `VolumeControl.exe` somewhere and launch it.  

## A Note on Windows Defender / Microsoft Defender Smartscreen
The first time you launch the program, Microsoft Defender Smartscreen will show a pop-up window informing you that the application isn't signed, and is "unrecognized".  
This is because Microsoft charges >$300 per year for a publishing certificate, and that's a lot of money that I don't have.  

To ignore the popup and unblock the program, click on ___More Info___ -> ___Run anyway___.  

If you're unsure, feel free to run it through [VirusTotal](https://www.virustotal.com/gui/home/upload) or another virus scanner before trying it.

# Usage
To get started, set a target application in the _General_ tab, then go through each of the Hotkey tabs and set up the hotkeys you want.  
By default, the _Volume Up/Down_ & _Toggle Mute_ hotkeys are set to _Ctrl+VolumeUp_, _Ctrl+VolumeDown_, & _Ctrl+VolumeMute_ respectively.  
If the list is empty, try clicking _Reload_, or starting an application that plays audio.  
If you know the __Process Name__ of the application you want to control, you can enter it whether it's on the list or not.

__Note:__ For an application to appear in the list of targets, it must be visible in the Windows Audio Mixer -- this happens as soon as it begins playing audio in most cases.

## Rebinding Keyboard Media Keys

This is fully supported, and is the original intent of the program.

There are two ways to accomplish this:
- Use a modifier key in combination with the volume up/down media keys to control application volume.  
- Use the volume up/down media keys to control application volume, then to change the global volume just press any modifier key as well.  

## Available Hotkeys

 - Application Volume Up
 - Application Volume Down
 - Application Toggle Mute
 - Next Track
 - Previous Track
 - Play/Pause
 - Next Target
 - Previous Target
 - Show Target List

## VolumeControl
Volume Control usually lives in the System Tray to keep it out of your way.  
To show the main window, double-click on the system tray icon.  

As of version 3.3.3, a dark theme is available for both the main window and the target list window. Comparison image between both themes:
![Light & Dark Theme Comparison](https://i.imgur.com/lm5OuIe.png)  

### Target List Window
The target list window is an optional toast notification that pops up when using the _Next/Prev Target_ hotkeys, and disappears after a configurable timeout.  
It shows which target is currently selected, and an approximation of its current volume level.  
![Target List Window](https://i.imgur.com/DWIvBHG.png) _Target List Window in v3.3.4_  
You can also use the _Show Targets_ hotkey to toggle the window without having a timeout.  
Clicking on a target in the list will select that target.

## VolumeControlCLI
For best results, place `VolumeControlCLI.exe` in a directory on your PATH. If you don't know how to do that, see [here](https://stackoverflow.com/a/44272417/8705305).  

Use `VolumeControlCLI -h` or `VolumeControlCLI --help` to see more up-to-date usage instructions.  
_Note: The CLI version accepts PID numbers or Process Names._

### Commandline Options
_This list was last updated as of `3.3.0-pr4`_

| Option                                  | Description                                            |
|-----------------------------------------|--------------------------------------------------------|
| `-h`  `--help`                          | Shows a brief description of each option, then exits.  |
| `-V`  `--version`                       | Shows the current version number, then exits.          |
| `-p <Name/PID>`  `--process <Name/PID>` | Selects the target process. This option is required.   |
| `-v <0-100>`  `--volume <0-100>`        | Specify a volume level to apply to the target process. |
| `--mute`                                |  Mute the target application.                          |
| `--unmute`                              | Unmute the target application.                         |
| `-s`  `--set`                           | Implicit when `-v` or `--volume` is specified.         |
| `-g`  `--get`                           | Implicit when `-v`/`--volume` isn't specified.         |
