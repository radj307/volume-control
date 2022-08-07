
# Contributing

Thanks for taking the time to contribute to Volume Control!  

## Guidelines for Code Contributions

- Follow existing code style
- Test your code before submitting
- Provide comments for publicly-accessible members in [XMLDoc](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/) format  
  *(This is only **required** for things that are accessible through the Volume Control SDK, but comments are always appreciated for everything else too!)*
/
## Guidelines for Translation Contributions

**Before Submitting:**  
- Ensure the spelling and grammar is correct
- Ensure the structure of the file is correct by testing the language config.

### How to Embed Language Configs

In order for your translation to actually be included in Volume Control, you have to perform a few extra steps.  

> ### ⚠️ Note
> If you don't have Visual Studio *(or another IDE compatible with C#)*, you can skip this step as long as you indicate in the Pull Request that you haven't embedded the language config.  
> That way, we can ensure that it gets included.

 1. Ensure your language configs are located in `<REPOSITORY_ROOT>/VolumeControl/Localization`.
 2. Open `VolumeControl.sln` in Visual Studio or your preferred IDE.  
 3. Add your language configs to `VolumeControl.Localization` in the solution explorer:  
    ![image](https://user-images.githubusercontent.com/1927798/183272414-bd0835d8-a4ba-492a-b991-b1ae7c3b02eb.png)  
 4. R+Click on your language config in the solution explorer, and select **Properties**. Set the **Build Action** to '**Embedded resource**':  
    ![image](https://user-images.githubusercontent.com/1927798/183272902-77c24af3-5de0-429e-a4d7-23edb48dd345.png)

That's all! Your language config will be embedded in the Volume Control executable, and when the `CreateDefaultTranslationFiles` setting is set to `true`, it will be written to `C:\Users\<USERNAME>\AppData\Local\radj307\Localization` along with the other default language configs.
  
