<p align="center">
<a href="https://radj307.github.io/volume-control"><img alt="[Volume Control Banner]" src="https://i.imgur.com/rMbNIhU.png"></a><br/>
<a href="https://github.com/radj307/volume-control/releases"><img alt="GitHub tag (latest by date)" src="https://img.shields.io/github/v/tag/radj307/volume-control?color=e8e8e7&label=Latest%20Version&logo=github&logoColor=e8e8e7&style=for-the-badge"></a>&nbsp;&nbsp;&nbsp;<a href="https://github.com/radj307/volume-control/releases"><img alt="Downloads" src="https://img.shields.io/github/downloads/radj307/volume-control/total?color=e8e8e7&logo=github&logoColor=e8e8e7&style=for-the-badge"></a>
</p>

***

Simple, universal application-specific volume hotkeys & more.  
Designed for adjusting the volume of your music independently of other programs, such as games and VoIP.  


### What it does

- Adds completely user-configurable global hotkeys to Windows.
  - You can add, remove, and rename hotkeys, as well as configure the 'action' that they perform.  
    This requires enabling *'Advanced Hotkey Mode'*, which is a fancy way of saying *'Check the box labelled with '**Advanced Hotkeys**', which is located in the **Settings** tab'*
  - The hotkey actions mentioned earlier are fairly diverse, and can be expanded in the form of [user-created plugins](https://radj307.github.io/volume-control).
- Integrates a functionally superior alternative to the Windows Audio Mixer.  
  - Includes volume slider trackbars and textboxes so you can specify a volume level with the keyboard.
    - Like the Windows Audio Mixer, you can do this with any running process independently at any time.
  - Allows you to specify the 'target' application using the mixer, in addition to the target switching hotkeys.

### Requirements

- A mouse & keyboard.

#### Windows Version Compatibility
We've tested Volume Control on **Windows 10**, however it should be compatible with any version of Windows since Vista however this is untested.  

##### Unconfirmed

If you use one of these operating systems, please fill out a [compatibility report](https://github.com/radj307/volume-control/issues/new?assignees=radj307&labels=os-support&template=Compatibility.yml&title=Windows+Version+Compatibility+Report&version=Windows+Vista) form.  
*(If everything works it should take less than 30 seconds. If there is an issue, we'll fix it ASAP)*
- Windows Vista
- Windows 7
- Windows 8
- Windows 8.1
- Windows 11

# Getting Started

## Installation
- Download the [latest release](https://github.com/radj307/volume-control/releases)
- Save `VolumeControl.exe` to a location of your choice.  
If you're unsure about where to choose as a location, create a directory in your user folder and place it inside of that:
`C:\Users\<USERNAME>\VolumeControl\VolumeControl.exe`

## Setup
Before starting the program for the first time, you have to unblock the executable from the properties menu.  
This is necessary because Windows Defender requires a paying *&gt;$300* a year for a [Microsoft-approved publishing certificate](https://docs.microsoft.com/en-us/windows-hardware/drivers/dashboard/get-a-code-signing-certificate) in order to prevent Windows Defender from blocking it.  
*If you're unsure, you're can always run it through [VirusTotal](https://www.virustotal.com/gui/home/upload) first, or check the source code yourself.*

 1. R+Click on `VolumeControl.exe` in the file explorer and select *Properties* in the context menu.  
 2. Check the box next to *Unblock:*  
 ![](https://i.imgur.com/NMI4m4F.png)  
 3. Click **Ok** to save the changes.  


## Usage
First, enable the **Volume Up** & **Volume Down** hotkeys from the **Hotkeys** tab by checking the box to the left of the hotkey name. If you don't have a volume slider, change the key from the dropdown. You can also set a modifier key with the checkboxes to the right of the dropdown. 

**NOTE:** Hotkeys cannot be enabled if their associated key is set to `None`.
![](https://i.imgur.com/FA06Pt9.png)

Next, let's set a target application to test the hotkeys with.  
Start playing some audio from any application, then return to the **Mixer** tab, click **Reload**, then click the **Select** button next to the test application, and try using the volume hotkeys.  

![](https://i.imgur.com/Pjym4A7.png)

In the settings tab, you can change how the application behaves such as which output device is controlled, enable or disable the toast notification, enable advanced hotkeys, set the volume step (how much the volume will increase on decrease when the hotkeys are pressed), tell the application to run on startup, and more!

![](https://i.imgur.com/fUWAlqj.png)

## Addon Development
Want to develop an addon for Volume Control?  
**See the [github pages site](https://radj307.github.io/volume-control) for instructions and API documentation.**


## VolumeControlCLI
*(This section is for people who want to adjust application volumes from the commandline or with shell scripts.)*

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
