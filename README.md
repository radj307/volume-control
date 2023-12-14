<p align="center">
<a href="https://radj307.github.io/volume-control"><img alt="[Volume Control Banner]" src="https://i.imgur.com/rMbNIhU.png"></a><br/>
<a href="https://github.com/radj307/volume-control/releases/latest"><img alt="GitHub tag (latest SemVer)" src="https://img.shields.io/github/v/tag/radj307/volume-control?color=e8e8e7&label=Latest%20Release&logo=github&logoColor=e8e8e7&sort=semver&style=flat-square"></a>&nbsp;&nbsp;&nbsp;<a href="https://github.com/radj307/volume-control/releases"><img alt="Downloads" src="https://img.shields.io/github/downloads/radj307/volume-control/total?label=Downloads&color=e8e8e7&logo=github&logoColor=e8e8e7&style=flat-square"></a>&nbsp;&nbsp;&nbsp;<a href="https://github.com/radj307/volume-control-cli"><img alt="Volume Control CLI Latest Version" src="https://img.shields.io/github/v/tag/radj307/volume-control-cli?color=e8e8e7&logo=github&logoColor=e8e8e7&label=Latest%20VCCLI%20Version&style=flat-square"></a>
</p>

***

<p align="center">
Application-specific volume control that supports the keybindings you already use.<br/>
Designed for effortless music volume control <i>(Spotify, Deezer, Chrome, Firefox, etc.)</i> without disrupting gaming or VoIP audio.
</p>

## What It Does

- Lets you control the volume of specific programs using only the keyboard.
- Can override pre-existing keys & key combinations, including all of the media keys.
- All hotkeys are completely user-customizable and can be bound to a variety of actions.
- New actions can be added via [user-created addons](https://radj307.github.io/volume-control/html/md_docs__addon_development.html).
- Offers more features and a more compact UI than the Windows Volume Mixer.
- And more!

## How does it work?

Volume Control leverages the Win32 API to establish seamless native hotkeys, effectively superseding default Windows keybindings with imperceptible latency. Employing the same approach as the native Windows volume mixer, it offers compatibility with all applications.

Volume Control empowers users with an unlimited array of unique hotkey combinations, each fully customizable with specific actions. The default options include common actions like "Volume Up", "Volume Down", and "Toggle Mute". Furthermore, you have the flexibility to create and integrate your own custom actions in C# to enhance Volume Control's functionality.

# Getting Started

Getting started is simple. Download `VolumeControl-Installer.exe` from the [latest release](https://github.com/radj307/volume-control/releases/latest), and run it. It will guide you through the installation process.

If you prefer to use a package manager, you can use [Winget](https://learn.microsoft.com/en-us/windows/package-manager/) to install Volume Control:
```
winget install radj307.volume-control
```

A basic usage guide is available on [the wiki](https://github.com/radj307/volume-control/wiki).

## Manual Installation

Download `VolumeControl.exe` from the [latest release](https://github.com/radj307/volume-control/releases/latest) and move it to a location of your choice.

Before starting the program for the first time, you have to unblock the executable from the properties menu.  
This is necessary because Windows requires paying *&gt;$300* a year for a [Microsoft-approved publishing certificate](https://docs.microsoft.com/en-us/windows-hardware/drivers/dashboard/get-a-code-signing-certificate) in order to prevent Windows Defender from blocking it.  
*If you're unsure, you can always run it through [VirusTotal](https://www.virustotal.com/gui/home/upload) first, or check the source code yourself.*

 1. R+Click on `VolumeControl.exe` in the file explorer and select *Properties* in the context menu.  
 2. Check the box next to *Unblock:*  
 ![](https://i.imgur.com/NMI4m4F.png)  
 3. Click **Ok** to save the changes.  

All that's left now is to run the application.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for more information.  

## Addon Development

Want to develop an addon for Volume Control?  
Get started with the [tutorial](https://radj307.github.io/volume-control/html/md_docs__addon_development.html)!  
We also have doxygen-generated [API Documentation](https://radj307.github.io/volume-control/html/annotated.html) available online.  
