# Volume Control
<a href="https://github.com/radj307/volume-control/releases"><img alt="GitHub tag (latest by date)" src="https://img.shields.io/github/v/tag/radj307/volume-control?color=e8e8e7&label=Latest%20Version&logo=github&logoColor=e8e8e7&style=for-the-badge"></a>
<a href="https://github.com/radj307/volume-control/releases"><img alt="Downloads" src="https://img.shields.io/github/downloads/radj307/volume-control/total?color=e8e8e7&logo=github&logoColor=e8e8e7&style=for-the-badge"></a>  
Simple, universal application-specific volume hotkeys using the Windows Mixer.  

I made this because of my frustration with being unable to use keyboard hotkeys to control the volume of Deezer after switching from Spotify, and I figured why not just make hotkeys that work for any process.

There is also a CLI version available on the releases page.  
_Note: The CLI version accepts PID numbers or Process Names._

# Installation
 1. Download the [latest release](https://github.com/radj307/volume-control/releases).  
 2. Extract the `.zip` file to a directory of your choice.  
 3. Launch `VolumeControl.exe` for the main program, or `VolumeControlCLI.exe` for [CLI usage](https://github.com/radj307/volume-control/main/README.md#volumecontrolcli).

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

![The Main Window](https://i.imgur.com/Yw5LIqb.png)  
_NOTE: I am in no way affiliated with Deezer._

Here's an overview of the available settings:  _(as of v3.0.0)_

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


## VolumeControlCLI
For best results, place `VolumeControlCLI.exe` in a directory on your PATH. If you don't know how to do that, see [here](https://stackoverflow.com/a/44272417/8705305).  

Use `VolumeControlCLI -h` or `VolumeControlCLI --help` to see usage instructions.
