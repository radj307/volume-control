# Addon Development
Addons are written in C# with .NET Core 6 - you don't need any previous experience with C# in order to create addons, but this is not a tutorial for learning C#.  
You are legally allowed to create and redistribute addons so long as you comply with the terms of the [GPLv3](https://github.com/radj307/volume-control/blob/main/LICENSE) license - You must make the full source code available along with any redistributed binaries (`.dll`s in this case) without any terms or conditions.  

![](https://i.imgur.com/DUI7k84.png)

## Pre-Requisites

- [Volume Control SDK](https://github.com/radj307/volume-control/releases)  
  Download `SDK.zip` from the latest release.
  
### Other Dependencies

**These are not required unless you want to use types/methods that depend on them.**  
In most cases, you won't need any of these; if you do need them, use Nuget to install them.  

- NAudio
- Semver

#### A Note on File Extensions

The actual files included in the SDK may change from version to version as code is organized and re-organized.  

| Extension | Purpose                                                                                                         |
|-----------|-----------------------------------------------------------------------------------------------------------------|
| `.dll`    | Contains the source code and references needed to use it.<br/>This is the bare minimum required to make addons. |
| `.pdb`    | Contains debugging symbols, which allow you to use the debugger to step through code contained within the DLL.  |
| `.xml`    | Contains API documentation used by Visual Studio intellisense to provide inline documentation & suggestions.    |

When using the SDK, keep files with the same filename *(not extension, somewhat obviously)* in the same directory to receive the benefits from each.

## Environment Setup
This assumes you're using Visual Studio with the .NET/C# workload installed.  

 1. Create a new .NET Core 6 Class Library type solution, and name it whatever you want.  
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
using VolumeControl.API;
using VolumeControl.Hotkeys.Attributes;
using VolumeControl.Log;

namespace VolumeControl.TestAddon
{
    [ActionAddon(nameof(TestActionAddon))] //< This is required for all addon classes that should be loaded.
    public class TestActionAddon
    {
        // Here we use a static property to shorten the 
        private static VCAPI API => VCAPI.Default;

        // This is the global logging object, which can be accessed through the static VCAPI.Log property.
        //  Here, we do the same as above so we can write log messages with shorter syntax.
        private static LogWriter Log => VCAPI.Log;


        // Now let's create a hotkey action named 'CustomAction', using the action event handler signature.
        // All hotkey actions must return void, and accept 2 parameters of types object? & HandledEventArgs.
        [HotkeyAction] //< this is required for all hotkey action methods
        public void CustomAction(object? sender, HandledEventArgs e)
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

**[API Documentation](https://github.com/radj307/volume-control/wiki/API-Documentation)**

## Building Your Addon

Change the VS Configuration to **Release**, then build the solution. *(Ctrl+Shift+B)*  
You'll be able to find your `.dll` in the `<SOLUTION>/<PROJECT>/bin/Release/net6.0-windows/<PROJECT>.dll`

## Loading Your Addon
Addons must be located within this directory, or in a subdirectory of this directory:  
`C:\Users\<USERNAME>\AppData\Local\radj307\Addons`  

To check if your addon was loaded, launch VolumeControl and click on the **Settings** tab. Find the **Filter** dropdown in the log settings area, and enable `DEBUG` messages by checking the box.  
Assuming you have logging enabled, you will now be able to see messages related to addons in the log file. *(by default, this is named `volumecontrol.log` and is located next to the executable)*  
![image](https://user-images.githubusercontent.com/1927798/169681684-f41649d3-49c0-4c33-aaf3-5454de4dfdad.png)

Note that any users of your redistributed addons must also place this file in the same location, so if you're writing installation instructions be sure to include that.  

#### Using Hotkey Action Addons

 1. Enable *Advanced Hotkeys* on the **Settings** tab.
 2. On the **Hotkeys** tab, open the dropdown box under the *Action* column of any hotkey.
 3. You can now select your custom action in the list.

#### What about the SDK DLLs?

The main program has these integrated inside of the `.exe` to facilitate a single-file release; all this is to say, once your addon is complete you don't need to include them anywhere, they are simply to allow Visual Studio to 'see' them.

## Publishing Addons
Once you've got your shiny new addon, you may want to distribute it to share your work.  
To encourage this, I've created an official repository for third-party addons [here](https://github.com/radj307/volume-control.Addons).  

**Note: When uploading your addon to github, be sure to use the GPLv3 license, or a legally compatible alternative.**


