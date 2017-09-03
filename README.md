# Toastify

![toastify-showcase][toastify-showcase]

Toastify adds global hotkeys and toast notifications to Spotify.

This application uses [SpotifyAPI-NET](https://github.com/JohnnyCrazy/SpotifyAPI-NET).

## Features
* Display the current playing track in a customizable toast-style popup
* Global hotkeys for media actions (Play/Pause, Next/Previous track, Mute, Fast Forward, Rewind)
* :heavy_exclamation_mark: Compatible with the Windows Store version of Spotify

## Requirements
* Windows (7, 8/8.1, 10)
* .NET Framework 4.5.*
* Spotify

## Notices
* Toastify is **not** a Spotify **replacement**. You need both running at the same time.
* **Windows 10**: In the latest versions of Windows, SmartScreen will probably prevent the installer from starting, because it is not signed.  
  You have two options: either *Don't run* the installer and forget about Toastify, or click on *More info* and *Run anyway*.  
  Whatever is your choice, I strongly advise you to **not** disable Windows SmartScreen — its warnings are meaningful.  

  If you have any concerns, feel free to take a look at the code (it's free :wink:) and ask questions in the [issues](https://github.com/aleab/toastify/issues) section.

## Information for contributors
* The language version used to build the projects is C# 7
* The solution requires Visual Studio 2017 (or Visual Studio Code, alternatively)

## License
This software is licensed under the GNU GPL-2.0; the complete license text can be found [here](/LICENSE).  
See also the [original project](https://github.com/nachmore/toastify) this repository is a fork of, and its old [codeplex page](http://toastify.codeplex.com/).

This project uses third-party libraries; their verbatim licenses can be found [here](/LICENSE-3RD-PARTY).


[toastify-showcase]: https://raw.githubusercontent.com/aleab/toastify/gh-pages/images/toastify-showcase.png "Toastify"
