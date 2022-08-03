# Building From Source

There are walkthroughs for using .NET CLI or an IDE like Visual Studio.

## From the Commandline

### Requirements

- [Git for Windows](https://gitforwindows.org/)
- [.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/)

### Process

 1. Clone the repository, and `cd` into it:  
   `git clone https://github.com/radj307/volume-control && cd volume-control`
 2. Build the executable using the provided [Publish Profile](https://docs.microsoft.com/en-us/visualstudio/deployment/publish-overview):  
   `/p:PublishProfile="VolumeControl/Properties/PublishProfiles/FolderProfile.pubxml"`  
 3.  Retrieve the executable from `<REPOSITORY_ROOT>/publish/VolumeControl.exe`

## Using an IDE

### Requirements

- [Git for Windows](https://gitforwindows.org/)
- [Visual Studio](https://visualstudio.microsoft.com) *(or a preferred alternative)*

### Process

 1. Clone the repository & open the solution file:  
   `git clone https://github.com/radj307/volume-control && ./volume-control/*.sln`
 2. In the solution explorer, R+Click on the `VolumeControl` project and choose **Publish...**:  
   ![choose the publish... option](https://i.imgur.com/WDCBPg4.png)
 3. Click the **Publish** button:  
   ![](https://i.imgur.com/BbNnOkX.png)
 3.  Retrieve the executable from `<REPOSITORY_ROOT>/publish/VolumeControl.exe`.  
   ![](https://i.imgur.com/g1yS0l4.png)
