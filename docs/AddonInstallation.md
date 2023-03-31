@page Addon Installation for Users

# Installing Addons

The exact addon installation location depends on your version of Volume Control, but all of them are located in your local AppData directory by default.

*Tip: You can quickly open your `AppData/Local` directory in file explorer by pressing Win+R and entering `%localappdata%`*
![localappdata](https://user-images.githubusercontent.com/1927798/228995919-8253994f-b1f6-4191-9670-aca9b466defc.gif)

## Default Location

### Since v6.0.0

Starting in v6.0.0, the default addon location is `C:/Users/<USERNAME>/AppData/Local/VolumeControl/Addons`.  
![directory](https://user-images.githubusercontent.com/1927798/228998612-649ffd67-0c9f-4259-8b25-7e621fdc9456.png)  
Addons can also be placed inside subdirectories.


### Until v5.2.5

Prior to v6.0.0, the default addon location was `C:/Users/<USERNAME>/AppData/Local/radj307/Addons`.  
![directory](https://user-images.githubusercontent.com/1927798/228998308-9ac90cb8-0105-49b0-9f4d-c27d474740e6.png)


## `CustomAddonDirectories` (Since v5.1.0)

> ### ⚠️ Note
> When entering file paths in JSON, make sure you use forward slashes (`/`) instead of backslashes (`\`)!  

Starting in v5.1.0, you can specify your own directories to load addons from by adding the path to `CustomAddonDirectories` in `volumecontrol.json`:  
![volumecontrol.json](https://user-images.githubusercontent.com/1927798/229000630-a427df0c-a0bc-41b2-ab1d-b762cf215b02.png)  
![directory](https://user-images.githubusercontent.com/1927798/228997862-6b063098-bb22-4f8d-8d59-913402451fa2.png)

Relative paths are also supported, and are especially useful for portable installations of Volume Control.  
This is an example of a portable installation with an addon directory located alongside the executable:  
![volumecontrol.json](https://user-images.githubusercontent.com/1927798/229000269-ca4f4383-d2a8-49d9-9d2c-d7fe2d19822b.png)  
![directory](https://user-images.githubusercontent.com/1927798/229000214-c8a08405-656d-422e-8e71-ebc102b695b2.png)
