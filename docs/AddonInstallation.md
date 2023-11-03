@page Addon Installation for Users

# Installing Addons

Installing an addon for Volume Control is easy, all you have to do is copy the addon's `.dll` file to the `Addons` directory.

Since version `6.0.0`, the default addon location is:  
![`C:\Users\<username>\AppData\Local\radj307\VolumeControl\Addons`](https://github.com/radj307/volume-control/assets/1927798/3d30b63f-f72e-4bfc-af31-99879b23a3f1)  

*Tip: To open the Addons directory quickly, press **Win+R** and enter `%LocalAppData%/radj307/VolumeControl/Addons`*

## CustomAddonDirectories

`5.1.0` added the ability to load addons from other directories by adding the path to `CustomAddonDirectories` in `VolumeControl.json`. 

> ### ⚠️ Note
> When entering file paths in JSON, make sure you use forward slashes (`/`) instead of backslashes (`\`)!  

Starting in v5.1.0, you can specify your own directories to load addons from by adding the path to `CustomAddonDirectories` in `volumecontrol.json`:  
![volumecontrol.json](https://user-images.githubusercontent.com/1927798/229000630-a427df0c-a0bc-41b2-ab1d-b762cf215b02.png)  
![directory](https://user-images.githubusercontent.com/1927798/228997862-6b063098-bb22-4f8d-8d59-913402451fa2.png)

Relative paths are also supported, and are especially useful for portable installations of Volume Control.  
This is an example of a portable installation with an addon directory located alongside the executable:  
![volumecontrol.json](https://user-images.githubusercontent.com/1927798/229000269-ca4f4383-d2a8-49d9-9d2c-d7fe2d19822b.png)  
![directory](https://user-images.githubusercontent.com/1927798/229000214-c8a08405-656d-422e-8e71-ebc102b695b2.png)
