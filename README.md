# Volume Control
<a href="https://github.com/radj307/volume-control/releases"><img alt="GitHub tag (latest by date)" src="https://img.shields.io/github/v/tag/radj307/volume-control?color=e8e8e7&label=Latest%20Version&logo=github&logoColor=e8e8e7&style=for-the-badge"></a>
<a href="https://github.com/radj307/volume-control/releases"><img alt="Downloads" src="https://img.shields.io/github/downloads/radj307/volume-control/total?color=e8e8e7&logo=github&logoColor=e8e8e7&style=for-the-badge"></a>  
Simple, universal application-specific volume hotkeys using the Windows Mixer.  

I made this because of my frustration with being unable to use keyboard hotkeys to control the volume of Deezer after switching from Spotify, and I figured why not just make hotkeys that work for any process.

There is also a CLI version available on the releases page.  
_Note: The CLI version accepts PID numbers or Process Names._

# Installation
 1. Download the [latest release](https://github.com/radj307/volume-control/releases).    
    - _VolumeControl.exe_ is the main GUI version that adds hotkeys.
    - _VolumeControlCLI.exe_ is the commandline utility for adjusting or querying an application's volume.  
      See [CLI usage](https://github.com/radj307/volume-control#volumecontrolcli) for more information about the commandline version.
 2. Place `VolumeControl.exe` somewhere and launch it.  

## A Note on Windows Defender / Microsoft Defender Smartscreen
The first time you launch the program, Microsoft Defender Smartscreen will show a pop-up window informing you that the application isn't signed, and is "unrecognized".  
This is because Microsoft charges >$300 per year for a publishing certificate, and that's a lot of money that I don't have.  

If you're unsure, feel free to run it through [VirusTotal](https://www.virustotal.com/gui/home/upload) before launching it.

Click on ___More Info___ -> ___Run anyway___ to launch the application.  

# Usage
To get started, set a target application in the _General_ tab.  
If the list is empty, try clicking _Reload_, or starting an application that plays audio.  
If you know the process name of the application you want to control, you can enter it whether it's on the list or not.

### Default Hotkeys
If your keyboard doesn't have media keys, change the bound key in the _Hotkeys_ tab.

- Increase Volume  
  `Ctrl + VolumeUp`
- Decrease Volume  
  `Ctrl + VolumeDown`
- Toggle Mute  
  `Ctrl + VolumeMute`
- Next Track  
  `Ctrl + MediaNext`  _(Disabled by Default)_
- Previous Track  
  `Ctrl + MediaPrevious`  _(Disabled by Default)_
- Play / Pause  
  `Ctrl + MediaPlayPause`  _(Disabled by Default)_

## VolumeControl
Volume Control usually lives in the System Tray to keep it out of your way.  
To show the main window, double-click on the system tray icon.  

![The Main Window](https://i.imgur.com/Kp8qUeO.png)  
_NOTE: I am in no way affiliated with Deezer._

Here's an overview of the available settings:  _(as of v3.1.0)_

- Run on Startup  
  - When checked, Volume Control will automatically start when you log in.  
- Minimize on Startup  
  - When checked, the application will start in the system tray instead of opening the main window.  
- Visible in Taskbar  
  - When checked, Volume Control will have a taskbar icon. _(Disabled by default)_
- __Target Process__
  - The name of the application you want to control.  
    You can enter any string here, or use the drop-down to select an application. Use the _Reload_ button to refresh the list.  
    _Note: Some applications will only show up when they are actually playing audio._  
- Volume Adjustment - Step
  - How much to adjust the application's volume each time the Volume Up / Volume Down hotkeys are pressed.   
- Hotkeys
  - Enabled  
    When this is checked, the associated hotkey is active.
  - Key Selector  
    This is the textbox/dropdown in the top-right. You can enter the name of any valid key to select it.  
  - Shift / Ctrl / Alt / Win  
    These are modifier keys that you have to hold down when pressing the hotkey. By default, the `Ctrl` key is enabled.
- _(3.3.0+)_ Target Switching Hotkeys
  - These allow you to change the target process without interacting with the GUI.
  - By default, when you use the `Next Target` or `Previous Target` hotkeys a notification window showing the currently selected target & all of the detected audio sessions will appear in the bottom right corner of the main screen. It will disappear after a configurable amount of time.
  - The target list window can be disabled by unchecking _Enable Notifications_ under _Target Switch Notifications_.  
    It will only appear if the `Show Targets` hotkey is enabled & pressed.
  

## VolumeControlCLI
For best results, place `VolumeControlCLI.exe` in a directory on your PATH. If you don't know how to do that, see [here](https://stackoverflow.com/a/44272417/8705305).  

Use `VolumeControlCLI -h` or `VolumeControlCLI --help` to see more up-to-date usage instructions.  

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
