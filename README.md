<p align="center">
<img alt="[Volume Control Banner]" src="https://i.imgur.com/rMbNIhU.png">
<a href="https://github.com/radj307/volume-control/releases"><img alt="GitHub tag (latest by date)" src="https://img.shields.io/github/v/tag/radj307/volume-control?color=e8e8e7&label=Latest%20Version&logo=github&logoColor=e8e8e7&style=for-the-badge"></a>
<a href="https://github.com/radj307/volume-control/releases"><img alt="Downloads" src="https://img.shields.io/github/downloads/radj307/volume-control/total?color=e8e8e7&logo=github&logoColor=e8e8e7&style=for-the-badge"></a>
 <br /> 
</p>
<h1></h1>

This is a simple Windows Forms program that adds _(configurable)_ hotkeys for adjusting the volume of specific applications instead of the global volume.

There is also a [commandline version](https://github.com/radj307/volume-control/releases/tag/3.3.4.1) available for scripting.  
See [CLI usage](https://github.com/radj307/volume-control#volumecontrolcli) for more information about the commandline version.


# Installation
 1. Download the [latest release](https://github.com/radj307/volume-control/releases).    
    - _VolumeControl.exe_ is the main GUI version that adds hotkeys.
    - _VolumeControlCLI.exe_ is the commandline version.  

 2. Place `VolumeControl.exe` somewhere and launch it.  

## A Note on Windows Defender / Microsoft Defender Smartscreen
__The first time you launch the program, you'll be greeted with this popup:__  

![image](https://user-images.githubusercontent.com/1927798/161876965-4092ec80-3302-45c5-8e9d-9668b27081f9.png)

This is because Microsoft charges >$300 per year for a publishing certificate in order to ~~rake in cash~~ _improve security_; since I don't have that kind of money to blow on Microsoft, run it through [VirusTotal](https://www.virustotal.com/gui/home/upload) if you're unsure.


# Getting Started
 1. First, download the [latest release](https://github.com/radj307/volume-control/releases) and launch it from a location of your choice.
 2. Click the ![Edit Hotkeys...](https://user-images.githubusercontent.com/1927798/161868661-4723424d-f3df-4665-b22a-3e15b5ef22b0.png) button to open the hotkey editor.  
    For instructions on how to use the hotkey editor, click the ![?](https://user-images.githubusercontent.com/1927798/161875057-c9cc4aef-3b3e-4248-a0a9-f5cf14e12b9a.png) button in the top-right corner.
 3. Once you have the hotkeys set up, you can close the hotkey editor.  
 4. Pressing the _Volume Up_, _Volume Down_, or _Toggle Mute_ buttons will affect the currently selected 'target', which is whatever _(case-insensitive)_ Process Name you put here: ![in this box](https://user-images.githubusercontent.com/1927798/161877354-9219c68e-eba5-40d1-bdf8-1d78487dd045.png).  
 5. The first time you launch volume control, the 'target' field will be blank - luckily, there are a few options for filling it in.
    1. Open the _Mixer_ by clicking the ![toggle mixer](https://user-images.githubusercontent.com/1927798/161878001-7e1b02af-ae5f-43df-8674-82518f7f12e7.png) button, then click the ![select](https://user-images.githubusercontent.com/1927798/161878224-6653f7ea-bccc-485b-9d97-de42d4acd588.png) button next to the process you want to set as the target.
    2. If you enabled the _Next Target_ / _Previous Target_ hotkeys, you can press one _(or both, I guess)_ to begin cycling through the list of programs that are currently playing audio.
    3. You can also just type in the name of any process, even if it isn't currently running.
 6. The built-in mixer is capable of doing everything that the Windows Audio Mixer can do, albeit without the snazzy volume sliders.  
    You can click the ___-___ / ___+___ buttons to adjust the volume or click on the volume to directly set it using the keyboard.  
    You can mute/unmute applications with the _Muted_ checkbox.
 7. To change how much the volume is adjusted with each hotkey/button press, use the ![volume step](https://i.imgur.com/exEliKh.png) spinbox.

## VolumeControl
Volume Control usually lives in the System Tray to keep it out of your way.  
To show the main window, double-click on the system tray icon.  

All of the most important settings are exposed through the user interface, however there are many more configurable options available by editing the config file located in `<USER>\AppData\Local\VolumeControl\<LONG_ASS_GUID>\<VERSION>\user.config`

### Toast Notification
The target list window is an optional toast notification that pops up when using the _Next Target_, _Previous Target_, and _Toggle Target Lock_ hotkeys. It disappears after a configurable amount of time _(Anywhere from 100ms to 1min)_.

It shows which target is currently selected, and changes color depending on whether the target is locked.

![Toast Notification in v4.0.0](https://i.imgur.com/IiXC3Co.png)  
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
