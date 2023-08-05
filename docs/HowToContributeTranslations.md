This guide is intended as a guide for contributors looking to provide language translations for Volume Control. It describes in depth how to edit existing translations, create new translations, and submit pull requests on Github. 

For a comprehensive overview of how Volume Control implements translations, see the documentation for the localization nuget package we use: [CodingSeb.Localization](https://github.com/codingseb/Localization).

# Getting Started

This is the simplest way for a complete beginner to get started; if you're already familiar with Github you can use any method that you prefer.

 1. Log in to Github and navigate to the [Volume Control](https://github.com/radj307/volume-control) repository in your browser.
 2. Click the **Fork** button.  
    ![](https://i.imgur.com/1bFwA6V.png)  
    The following page will open. Select your Github account under the **Owner** dropdown box, then click **Create Fork**.
    ![](https://i.imgur.com/Uk7VjTB.png)  
 3. After a short wait, your forked repository should load.  
    
    If it doesn't open automatically, you can find your fork at this URL:  
    `https://github.com/<YOUR_GITHUB_USERNAME>/volume-control`  
 4. Press the PERIOD key (`.`/`>`) to open Github's browser-based version of VS Code.
 5. In the **Explorer** tab, navigate to `VolumeControl/Localization`.  
    ![](https://i.imgur.com/799CQAo.png)  

# File Format

The `VolumeControl/Localization` directory contains translation configuration files that are responsible for providing translated strings to Volume Control. Each translation config file should contain translated strings for exactly **ONE** (1) language.  
Translation configs can be in either JSON or YAML format, but JSON is preferred. Don't worry if you aren't familiar with JSON files, you don't need to know anything about them to provide translations *(but it will help with understanding the instructions)*.

## File Naming Conventions

Translation config files must be named `<LOCALE_ID>.loc.json`, where `<LOCALE_ID>` is the 2-character [ISO 639-1 language code](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes) of the language defined within that file. You're probably already familiar with the locale ID of your language as you've probably seen it in other places before. If you're unsure, or just double-checking, you can find the code associated with your language under the `639-1` column of [this table on wikipedia](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes).  

For example, the locale ID of English is `en`, so the `en.loc.json` file contains translations for the English language.

## String "Paths"

Each translated [string](https://en.wikipedia.org/wiki/String_(computer_science)) used by Volume Control is accessed via a **PATH** that corresponds to the structure of the translation config file. These paths are named and organized in such a way as to *(loosely)* represent the underlying XAML code that defines the structure of the window.  
**These path names MUST be in English and cannot be changed!**

For example, the path `VolumeControl.MainWindow.CaptionBar.Title.Content` refers to the window title:  
![](https://i.imgur.com/w05XhDN.png)  
and appears in the translation config as a series of nested JSON objects with a JSON string value in it:  
![](https://i.imgur.com/nl4rV6L.png)  

Translated strings are provided as JSON string values in the format `"<LANGUAGE_NAME>": "<STRING>"`, where `<STRING>` is the translated string and `<LANGUAGE_NAME>` is the language name shown in the settings tab:  
![](https://i.imgur.com/3IzCcNv.png)  

> Language names **MUST** be consistent throughout the entire file! Any translated strings defined with a typo in the language name will be interpreted as a seperate language.  
> For this reason, it is **HIGHLY** recommended to use the Find & Replace feature *(CTRL+F ➔ Click on the arrow icon)* instead of typing the name every time:  
> ![](https://i.imgur.com/ySNkurc.png)

# Edit an Existing Translation

Open the translation config file *(found in `VolumeControl/Localization`)* associated with the translation you want to modify.

## Add a Missing Translated String

 1. Copy the missing section from `en.loc.json` and paste it into the correct position in the translation config file.
 2. Change `English (US/CA)` to the language name.
 3. Replace the english string with the translated string.

## Change an Existing Translated String

 1. Find the string you want to change. *(CTRL+F)*
 2. Replace the string with the new translated string.

# Create a New Translation

 1. Create a new file in the `VolumeControl/Localization` directory *(R+Click ➔ New File)*.  
    Make sure you follow the [File Naming Conventions](#file-naming-conventions)!
 2. Copy the contents of the `en.loc.json` file into the new file.
 3. Using the find & replace tool *(CTRL+F ➔ Click on the arrow icon)*,
    search for `English (US/CA)` and replace it with the name of your language.
 4. Go through each english string and replace it with the equivalent string in your language.  
    > Make sure you preserve special characters such as newlines (`\n`), tabs (`\t`), and macros (`${ID}`), or the translated string will not appear correctly.  
    > Some entries also make strategic use of spaces to align text across different lines, for example:  
    > `"Are you sure you want to delete hotkey ${ID}?\n\n- 'Yes'     Delete the hotkey.\n- 'No'      Do not delete the hotkey.\n- 'Cancel'  Don't show this again. (Press again to delete)\n"`  
    > When changing these entries, be sure to use the correct number of spaces to preserve the alignment!

# Submit a Pull Request

Pull Requests (PRs) are Github's way of pushing changes across forked repositories. By submitting a pull request, you can contribute to Volume Control directly, and your Github username and picture *(if you have one)* will appear in the **Contributors** section of the main repository:  
![](https://i.imgur.com/au6O6ow.png)

 1. Open https://github.com/radj307/volume-control/pulls in your browser of choice.
 2. Click **New pull request**.  
    ![](https://i.imgur.com/VVGlFEt.png)
 3. Click **compare across forks**.  
    ![](https://i.imgur.com/L3jLiSD.png)
 4. Change the **base repository** dropdown box to your fork, then click **Create pull request**.  
    ![](https://i.imgur.com/N71GFoS.png)

You're all done!  
The developers will review your pull request and merge the changes if everything looks good, or they may ask you to make changes first.  

## Updating a Pull Request

If the developers ask you to make changes to your pull request before merging it, you can 
