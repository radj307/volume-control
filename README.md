# Toastify

[![Codacy grade](https://api.codacy.com/project/badge/Grade/dcbbd6b1f6cf45658a0f9232a5f35706)][aleab/toastify@codacy]
[![GitHub last commit (master)](https://img.shields.io/github/last-commit/aleab/toastify/master.svg?label=Last%20Commit&maxAge=60)][commits]
[![GitHub release](https://img.shields.io/github/release/aleab/toastify.svg?label=Release&maxAge=60)][release-latest]
[![Github license](https://img.shields.io/badge/License-GPL%20v2-blue.svg?maxAge=86400)][license]
[![Gitter](https://badges.gitter.im/aleab/toastify.svg)][aleab/toastify@gitter]

![toastify-showcase][toastify-showcase]

Toastify adds global hotkeys and toast notifications to Spotify.

This application uses [SpotifyAPI-NET][SpotifyAPI-NET].

## Features
* Display the current playing track in a customizable toast-style popup
* Global hotkeys for media actions (Play/Pause, Next/Previous track, Mute, Fast Forward, Rewind)
* :heavy_exclamation_mark: Compatible with the Microsoft Store version of Spotify

## Requirements
* Windows (7, 8/8.1, 10)
* .NET Framework 4.5.*
* Spotify

## Notices
* Toastify is **not** a Spotify **replacement**. You need both running at the same time.
* **Windows 10**: In the latest versions of Windows, SmartScreen will probably prevent the installer from starting, because it is not signed.  
  You have two options: either *Don't run* the installer and forget about Toastify, or click on *More info* and *Run anyway*.  
  Whatever is your choice, I strongly advise you to **not** disable Windows SmartScreen — its warnings are meaningful.  

  If you have any concerns, feel free to take a look at the code (it's free :wink:) and ask questions in the [issues][issues] section.
* If you are updating from the original version of Toastify (v1.8.3), be sure to uninstall it completely before installing this version. You should also remove any file from the following directories, if they exist:
  - *"%LocalAppData%\Toastify"* (for example: *"C:\Users\\{UserName}\AppData\Local\Toastify"*)
  - *"%AppData%\Toastify"* (for example: *"C:\Users\\{UserName}\AppData\Roaming\Toastify"*)

## Information for contributors
* The language version used to build the project is C# 7
* The solution requires Visual Studio 2017 (or Visual Studio Code, alternatively)
* Dependencies are not included. Use of NuGet is highly recommended.

## Donations
I dedicate most of my free time to improving and keeping Toastify alive. Although absolutely not necessary, if you'd like to support me and the project, you can buy me a coffee **[here](https://aleab.github.io/toastify/#donations)**! :coffee:

## License
This software is licensed under the GNU GPL-2.0; the complete license text can be found [here][license].  
See also the [original project](https://github.com/nachmore/toastify) this repository is a fork of, and its old [codeplex page][toastify@codeplex].

This project uses third-party libraries; their verbatim licenses can be found [here][license-3rdparty].


[//]: # (Links)

[toastify-showcase]: <https://raw.githubusercontent.com/aleab/toastify/gh-pages/img/toastify-showcase.png>
[license]: </LICENSE>
[license-3rdparty]: </LICENSE-3RD-PARTY>

[release-latest]: <https://github.com/aleab/toastify/releases/latest>
[commits]: <https://github.com/aleab/toastify/commits/master>
[issues]: <https://github.com/aleab/toastify/issues>

[aleab/toastify@codacy]: <https://www.codacy.com/app/aleab/toastify>
[aleab/toastify@gitter]: <https://gitter.im/aleab-toastify>
[SpotifyAPI-NET]: <https://github.com/JohnnyCrazy/SpotifyAPI-NET>
[toastify@codeplex]: <http://toastify.codeplex.com/>
