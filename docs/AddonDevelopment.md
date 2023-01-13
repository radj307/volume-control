# Addon Development
Addons are written in C# with .NET Core 6 - you don't need any previous experience with C# in order to create addons, but this is not a tutorial for learning C#.  
![](https://i.imgur.com/DUI7k84.png)

## Volume Control SDK

There are 2 methods for accessing the Volume Control SDK:
- **[Nuget](#using-nuget)**  
  This is the preferred method, and is available from version [5.2.0](https://github.com/radj307/volume-control/releases/5.2.0) onwards.  
  See [here](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio) for a guide on how to use Nuget in Visual Studio.  
- **[Manual](#manual-installation)**  
  You can also manually download the `SDK.zip` archive from the [releases](https://github.com/radj307/volume-control/releases) page.  

The installation steps for both methods can be found below.  
  
### Other Dependencies

**These are not required unless you want to use types/methods that depend on them.**  
In most cases, you won't need any of these; if you do need them, use Nuget to install them.  

- [NAudio](https://github.com/naudio/NAudio)
- [Semver](https://github.com/maxhauser/semver)

# What's in the SDK

Each dependency from the Volume Control SDK has 3 files with different extensions and purposes.  

| Extension | Purpose                                                                                                         |
|-----------|-----------------------------------------------------------------------------------------------------------------|
| `.dll`    | Contains the source code and references needed to use it.<br/>This is the only mandatory dependency, the others are optional but recommended. |
| `.pdb`    | Contains debugging symbols, which allow you to use the debugger to step through code contained within the DLL.<br/>*This must be located in the same directory as the associated DLL for it to work.*  |
| `.xml`    | Contains the API documentation used by IDEs to provide documented intellisense.<br/>This API documentation is also available [online](https://radj307.github.io/volume-control/html/index.html), but it's much easier to use when integrated.<br/>*This must be located in the same directory as the associated DLL for it to work.*   |

## Environment Setup
This assumes you're using Visual Studio with the **.NET/C#** workload installed.  

### Using NuGet
*Available since [v5.2.0](https://github.com/radj307/volume-control/releases/5.2.0)*

 1. Create a new **.NET Core 6 Class Library** type solution, and name it whatever you want.  
    ![image](https://user-images.githubusercontent.com/1927798/169677073-716d9c3b-5928-4414-8985-3dc22a79c160.png)  
    ![image](https://user-images.githubusercontent.com/1927798/169677083-622c7461-11bb-4b57-8eeb-664ced0fde6f.png)  
 2. R+Click on your `.csproj` file and click **Properties**. In the property pages, change the **Target OS** to **Windows**:  
    ![image](https://user-images.githubusercontent.com/1927798/169683966-573f9c1c-4971-4304-a8b5-c01c931af5c2.png)  
    Press *Ctrl+S* to save the project file, then close the tab.
 3. In the solution explorer, R+Click on your project and select **Manage NuGet Packages for Project**  
    ![image](https://i.imgur.com/oLeVqaM.png)  
 4. Install the Volume Control SDK package.  
    ![image](https://i.imgur.com/Zfka6S1.png)  
    

### Manual Installation

 1. Create a new **.NET Core 6 Class Library** type solution, and name it whatever you want.  
    ![image](https://user-images.githubusercontent.com/1927798/169677073-716d9c3b-5928-4414-8985-3dc22a79c160.png)  
    ![image](https://user-images.githubusercontent.com/1927798/169677083-622c7461-11bb-4b57-8eeb-664ced0fde6f.png)  
 2. In the solution explorer, expand your new project and R+Click on the **Dependencies** entry, then select any of the **Add ... Reference** options.   
    ![image](https://user-images.githubusercontent.com/1927798/169680877-f5a0b8dc-153b-46bd-b566-3e25c6af01b4.png)
 3. Click on the **Browse** tab & add the DLLs from the sdk you downloaded earlier using the **Browse** button at the bottom.  
    ![image](https://user-images.githubusercontent.com/1927798/169681046-b9b1d092-8500-42d7-a587-3f75a5f3698c.png)  
    Once you're done, click **OK**.
 4. R+Click on your `.csproj` file and click **Properties**. In the property pages, change the **Target OS** to **Windows**:  
    ![image](https://user-images.githubusercontent.com/1927798/169683966-573f9c1c-4971-4304-a8b5-c01c931af5c2.png)  
    Press *Ctrl+S* to save the project file, then close the tab.
 5. You are now able to create custom addons for Volume Control.  
    Add `using VolumeControl;` to the first line, and you'll be able to use Visual Studio's autocomplete for the rest.

## Writing an Addon

Here's an example of an addon that adds a hotkey action named 'Custom Action' that writes a message to the log when triggered.  
```csharp
using System.ComponentModel;
//using VolumeControl.API; //< until version 5.2.0
using VolumeControl.SDK;   //< since version 5.2.0
using VolumeControl.Hotkeys.Attributes;
using VolumeControl.Log;

namespace VolumeControl.TestAddon
{
    [ActionAddon(nameof(TestActionAddon))]
    public class TestActionAddon
    {
        private static VCAPI API => VCAPI.Default;
        private static LogWriter Log => VCAPI.Log;


        [HotkeyAction]
        public void PrintSelected(object? sender, HandledEventArgs e)
        {
            Log.Debug($"Successfully triggered addon method {nameof(CustomAction)}!",
                      $"Currently selected session name: '{API.AudioAPI.SelectedSession?.ProcessName}'");
        }

        [HotkeyAction("My Action Name")]
        public void CustomAction2(object? sender, Handled)
        
        
    }
}
```

You can find the full code example [here](https://github.com/radj307/volume-control.TestAddon).

See the **[API Documentation](https://github.com/radj307/volume-control/wiki/API-Documentation)** for more information on what is available through the Volume Control SDK.

## Building Your Addon

Assuming you're using Visual Studio, you can use **Publish Profiles** to build your addon and place it in an easy-to-access directory.  

First, you'll need to create a new publish profile.

 1. R+Click your addon project in the solution explorer, and select **Publish...**  
    ![](https://i.imgur.com/Ao5qYF0.png)
 2. When asked to choose a target, select **Folder**.  
    ![](https://i.imgur.com/CYMEnam.png)
 3. Choose an output directory; this should be a **relative** path that points somewhere within your solution directory / git repository.  
    ![](https://i.imgur.com/T3nt2Ga.png)  
 4. Do any additional configuration you want on your new publish profile.  
    ![](https://i.imgur.com/5Xzbvt1.png)  
 5. Click the **Publish** button to build your addon.  
    The output files *(and only the output files)* can be found at the location you set in step 3.   
    ![](https://i.imgur.com/fbxy7VW.png)

## Loading Your Addon

In order to see log messages related to the loading of addons, you need to enable the **`DEBUG`** log filter.  
 1. Launch `VolumeControl.exe`, and navigate to the **Settings** tab.  
 2. Find the **Filter** dropdown in the log settings area, and check the box next to **`DEBUG`**  

### Where to Put Addons

Volume Control will attempt to load addons from a few locations, depending on the version.  
All versions of Volume Control will *(recursively)* load addons from `C:\Users\<USERNAME>\AppData\Local\VolumeControl\Addons` first, if it exists.  
**Do not place Volume Control SDK dlls in addon directories, and do not include them when redistributing your addon.**  

Since [v5.1.0](https://github.com/radj307/volume-control/release/5.1.0), addons may also be located in any directories specified by the `CustomAddonDirectories` setting, which can be found in the configuration file:  
*(until [v5.2.0](https://github.com/radj307/volume-control/releases/5.2.0)):* `~/AppData/Local/radj307/VolumeControl_Url_<HASH>/<VERSION>/user.config`  
*(since [v5.2.0](https://github.com/radj307/volume-control/releases/5.2.0)):* `volumecontrol.json` located in the same directory as `VolumeControl.exe`  

Assuming you enabled `DEBUG` log messages, you will see messages in the log confirming that your addon was loaded when launching Volume Control:  
![image](https://user-images.githubusercontent.com/1927798/169681684-f41649d3-49c0-4c33-aaf3-5454de4dfdad.png)

## Using Your Addon

In the case of action addons, they are used in the same way as the built-in actions:    

 1. Enable *Edit Mode* on the **Settings** tab.
 2. On the **Hotkeys** tab, open the dropdown box under the *Action* column of any hotkey.
 3. Select your custom action in the list.

Now, when you press the associated hotkey, your action method will be triggered! How you observe the outcome of that action depends on the action, of course; the action we created earlier for instance, does this:  
![custom-action-output](https://user-images.githubusercontent.com/1927798/184083946-959e3f2d-4b7b-4523-b597-0965d6145076.png)


## Redistributing Addons

Once you've got your shiny new addon, you may want to distribute it to share your work.  
To encourage this, I've created an official repository for third-party addons [here](https://github.com/radj307/volume-control.Addons).  

> ### Legal
> Because Volume Control is licensed under the [GPLv3](https://github.com/radj307/volume-control/blob/main/LICENSE), any code that uses the Volume Control SDK **must be licensed under the GPLv3 as well**; however **you are the sole owner** of the code that isn't from the SDK; assuming it isn't already licensed.  
> #### Summary:
> - Make sure you use the [GNU General Public License 3 (GPLv3)](https://github.com/radj307/volume-control/blob/main/LICENSE) license for your addon when uploading it to github or elsewhere.
