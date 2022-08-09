<p align="center">
<a href="https://radj307.github.io/volume-control"><img alt="[Volume Control Banner]" src="https://i.imgur.com/rMbNIhU.png"></a><br/>
<a href="https://github.com/radj307/volume-control/releases/latest"><img alt="GitHub tag (latest SemVer)" src="https://img.shields.io/github/v/tag/radj307/volume-control?color=e8e8e7&label=Latest%20Release&logo=github&logoColor=e8e8e7&sort=semver&style=flat-square"></a>&nbsp;&nbsp;&nbsp;<a href="https://github.com/radj307/volume-control/releases"><img alt="Downloads" src="https://img.shields.io/github/downloads/radj307/volume-control/total?color=e8e8e7&logo=github&logoColor=e8e8e7&style=flat-square"></a>&nbsp;&nbsp;&nbsp;<a href="https://github.com/radj307/volume-control-cli"><img alt="Volume Control CLI Latest Version" src="https://img.shields.io/github/v/tag/radj307/volume-control-cli?color=e8e8e7&logo=github&logoColor=e8e8e7&label=Latest%20VCCLI%20Version&style=flat-square"></a>
</p>

***

A universal, portable, and extensible hotkey framework that lets you control specific applications .  
Designed for adjusting the volume of your music independently of other programs, such as games and VoIP.  

> ### :new: Volume Control CLI
> There is also a commandline version of Volume Control without the hotkeys.  
> It is intended for use in shell scripts, and allows you to control **both input and output** audio devices/sessions.  
> You can find it here: https://github.com/radj307/volume-control-cli 


## What It Does

- Makes your keyboard's volume slider useful by letting you adjust the volume of any specific program.
- Allows you to have media keys on a keyboard without dedicated media keys.
- Adds a fully configurable hotkey framework that can be extended via [user-created addons](https://radj307.github.io/volume-control/html/md_docs__addon_development.html).
- Provides a superior alternative to Windows' Volume Mixer.  


## How does it work?

Volume Control maintains the concept of a "**selected**" audio session - also called a "**target**" - that is used to specify which audio session you want to perform an action on. Some actions only affect the current target; others, such as the media key actions, simulate context-sensitive key presses; and others, such as the next/previous target hotkeys, simply switch to the next or previous target regardless of context.  

In Volume Control, a ***hotkey*** is just a combination of keys that can be made to execute an ***action***. Any number of hotkeys can trigger the same action.  

A wide range of actions are provided by default, and you can add more via user-created addons.


# Getting Started

Getting started is simple; download the [latest release](https://github.com/radj307/volume-control/releases/latest).  


## Installation

Because Volume Control is portable, there is no installation required.  
Simply move `VolumeControl.exe` to a location of your choice, and run it.  

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
![View of the Hotkeys Tab](https://i.imgur.com/Qvkev52.png)


Next, let's set a target application to test the hotkeys with.  
Start playing some audio from any application, then return to the **Mixer** tab, click **Reload**, then click the **Select** button next to the test application, and try using the volume hotkeys.  

![View of the Mixer Tab](https://i.imgur.com/r5uaSx0.png)

In the settings tab, you can change how the application behaves such as which audio device is controlled, enable or disable the toast notification, enable advanced hotkeys, set the volume step (how much the volume will increase on decrease when the hotkeys are pressed), tell the application to run on startup, and more!

![View of the Settings Tab](https://i.imgur.com/jx8j1bC.png)

By enabling notifications, you will see a toast notification in the bottom right of your screen when you switch target sessions. This tells you which session is currently selected. Using the **Un/Lock Session** hotkey, you can prevent changing the targeted audio device. The border of the toast notification will be red when the currently targeted session is locked. You can press the hotkey again to unlock the session. 

![View of the toast notification](https://i.imgur.com/YWoXPxW.png)
![View of the toast notification when an audio session is locked](https://i.imgur.com/KOdYtGi.png)

If you want to add or remove hotkeys, you can press the **Edit Mode** button in the Settings tab to enable the advanced features. Now the Hotkeys tab will have additional customisability so you can create new hotkeys, and change the action of each hotkey when it is pressed. You can reset all hotkeys to their default value by pressing the **Reset Hotkeys** button in the Settings tab. Note that this will also remove any additional hotkeys you have created.  
![View of the advanced hotkeys](https://i.imgur.com/JccOVnO.png)


## Addon Development

Want to develop an addon for Volume Control?  
Get started with the [tutorial](https://radj307.github.io/volume-control/html/md_docs__addon_development.html)!  
We also have doxygen-generated [API Documentation](https://radj307.github.io/volume-control/html/annotated.html) available online.  
