<a name="toTop"></a>

<div align="center">
  <img src="logo.png" alt="AwARe Logo" width="85%"  height="auto" />
</div>

# AwARe: AR Food Production
Utrecht University: Computer Science BA Software Project 2023-2024

<p align="center">
    <a href="/AwARe/CodeCoverage" alt="Code Coverage">
        <img src="/AwARe/CodeCoverage/Report/badge_linecoverage.png"/></a>
</p>

## Table of Contents
- [Property](#prop)
- [Description](#descr)
- [Content](#content)
  - [First-party](#first)
    - [Navigation](#navFirst)
  - [Third-party](#third)
- [Setup](#setup)
  - [Android](#setupApk)
  - [IOS](#setupIpa)
  - [Unity](#setupUnity)
  - [Server](#setupServer)
- [Authors](#auth)
- [Acknowledgements](#ack)

## Property <a name="prop"></a>
> All-rights reserved to...

(c) Copyright Utrecht University (Department of Information and Computing Sciences)

## Description <a name="descr"></a>
> What is AwARe?

AwARe is a mobile app designed to enhance awareness of the resources required for visualizing resources needed for the production of ingredients through the means of augmented reality (AR). The main idea of this application is that users will become more aware of the impact that their food consumption has when the materials are visualized in their own environment, for example in their own kitchen. Researchers will be able to use this app to send questionnaires to users to study the behavioral changes of the users.


## Content <a name="content"></a>
### First-party <a name="first"></a>
> Code and other content developed by us.

All authentic software created can be found here: <br/>
[AwARe/Assets/FirstParty](/AwARe/Assets/FirstParty)

#### Navigation <a name="navFirst"></a>
> Where to find what in 'FirstParty'.

* [AdminTools](/AwARe/Assets/FirstParty/AdminTools)             <span style='color:blue'> &emsp; &#8594; Administrator environment (not developed yet) </span>
* [Application](/AwARe/Assets/FirstParty/Application)           <span style='color:blue'> &emsp; &#8594; Application code. </span>
    * [Scenes](/AwARe/Assets/FirstParty/Application/Scenes)            <span style='color:blue'> &emsp; &#8594; Unity Scene files </span>
        * [AppScenes](/AwARe/Assets/FirstParty/Application/AppScenes)      <span style='color:blue'> &emsp; &#8594; Actual scenes of the application. </span>
        * [Support](/AwARe/Assets/FirstParty/Application/Scenes/Support)        <span style='color:blue'> &emsp; &#8594; Special scenes providing special functionality or assistance. </span>
        * [Temporary](/AwARe/Assets/FirstParty/Application/Scenes/Temporary)      <span style='color:blue'> &emsp; &#8594; Scene-copies for safe development. </span>
    * [Scripts](/AwARe/Assets/FirstParty/Application/Scripts)  <span style='color:blue'> &emsp; &#8594; Unity Prefabs & C# code. </span>
        * [General](/AwARe/Assets/FirstParty/Application/Scripts/General)       <span style='color:blue'> &emsp; &#8594; Re-usable or non-specific. </span>
        * [MainFeatures](/AwARe/Assets/FirstParty/Application/Scripts/MainFeatures)  <span style='color:blue'> &emsp; &#8594; Specific to core-functionality. </span>
        * [Temporary](/AwARe/Assets/FirstParty/Application/Scripts/Temporary)     <span style='color:blue'> &emsp; &#8594; For the development process only, and should be replaced later. </span>
    * [Resources](/AwARe/Assets/FirstParty/Application/Resources)            <span style='color:blue'> &emsp; &#8594; Reserved for 3D models to spawn in AR. </span>
        * [Models](/AwARe/Assets/FirstParty/Application/Resources/Models)            <span style='color:blue'> &emsp; &#8594; Reserved for 3D models to spawn in AR. </span>
        * [Prefabs](/AwARe/Assets/FirstParty/Application/Resources/Prefabs) <span style='color:blue'> &emsp; &#8594; Unity Prefabs & C# code. </span>
            * [General](/AwARe/Assets/FirstParty/Application/Resources/Prefabs/General)       <span style='color:blue'> &emsp; &#8594; Re-usable or non-specific. </span>
            * [MainFeatures](/AwARe/Assets/FirstParty/Application/Resources/Prefabs/MainFeatures)  <span style='color:blue'> &emsp; &#8594; Specific to core-functionality. </span>
        * [Data](/AwARe/Assets/FirstParty/Application/Resources/Data) <span style='color:blue'> &emsp; &#8594; Additional data used in the application. </span>
    * [Images](/AwARe/Assets/FirstParty/Application/Images)            <span style='color:blue'> &emsp; &#8594; All images used. </span>
    * [Materials](/AwARe/Assets/FirstParty/Application/Materials)            <span style='color:blue'> &emsp; &#8594; Materials for prefabs. </span>
* [Assembly](/AwARe/Assets/FirstParty/Assembly)              <span style='color:blue'> &emsp; &#8594; Instructions to automatically adjust the project files. </span>
* [DevTools](/AwARe/Assets/FirstParty/DevTools)              <span style='color:blue'> &emsp; &#8594; Development build only code. (not developed yet) </span>
    * [Prefabs](/AwARe/Assets/FirstParty/DevTools/Prefabs)            <span style='color:blue'> &emsp; &#8594; prefabs for development build. </span>
        * [DebugLog](/AwARe/Assets/FirstParty/DevTools/Prefabs/DebugLog)      <span style='color:blue'> &emsp; &#8594; Support for error, warning and debug messages. </span>
        * [DebugToggle](/AwARe/Assets/FirstParty/DevTools/Prefabs/DebugToggle)   <span style='color:blue'> &emsp; &#8594; Switch for User/Developer UI. </span>
        * [DebugUI](/AwARe/Assets/FirstParty/DevTools/Prefabs/DebugUI)       <span style='color:blue'> &emsp; &#8594; Various debug view modes. </span>
        * [MockLoaders](/AwARe/Assets/FirstParty/DevTools/Prefabs/MockLoaders)         <span style='color:blue'> &emsp; &#8594; Fake data and objects for instant setup. </span>
    * [Scripts](/AwARe/Assets/FirstParty/DevTools/Scripts)       <span style='color:blue'> &emsp; &#8594; All source code. </span>
        * [DebugLog](/AwARe/Assets/FirstParty/DevTools/Scripts/DebugLog)      <span style='color:blue'> &emsp; &#8594; Support for error, warning and debug messages. </span>
        * [DebugToggle](/AwARe/Assets/FirstParty/DevTools/Scripts/DebugToggle)   <span style='color:blue'> &emsp; &#8594; Switch for User/Developer UI. </span>
        * [DebugUI](/AwARe/Assets/FirstParty/DevTools/Scripts/DebugUI)       <span style='color:blue'> &emsp; &#8594; Various debug view modes. </span>
        * [MockLoaders](/AwARe/Assets/FirstParty/DevTools/Scripts/MockLoaders)         <span style='color:blue'> &emsp; &#8594; Fake data and objects for instant setup. </span>
* [QA](/AwARe/Assets/FirstParty/QA)                    <span style='color:blue'> &emsp; &#8594; Quality assurance files. </span>
    * [Analyzers](/AwARe/Assets/FirstParty/QA.Analyzers)         <span style='color:blue'> &emsp; &#8594; Analyzers/Linters settings. </span>
    * [Testing](/AwARe/Assets/FirstParty/QA/Testing)           <span style='color:blue'> &emsp; &#8594; Test code/environment. </span>

### Third-party <a name="third"></a>
> External packages and tools in this repository.

* [Doxygen](https://www.doxygen.nl/) Used for generating documentation from the doc-comments in the codebase.
* [CodeCoverage](https://docs.unity3d.com/Packages/com.unity.testtools.codecoverage@0.2/manual/index.html) Used for finding out which lines of code are covered/uncovered by our tests.

Unity does not support organizing folders of Build-in packages or Third Party Software well. In principal, everything in the Unity Project that is stored outside First-Party is not owned by us.

## Setup <a name="setup"></a>
> Installation and use of this product.

### Android (.apk) <a name="setupApk"></a>
The latest android build can be downloaded here:
[.apk download](/AwARe)

### IOS (.ipa) <a name="setupIpa"></a>
The latest IOS build can be downloaded here:
[.ipa download](/AwARe)

### Unity <a name="setupUnity"></a>
For development, we use unity editor version 2022.3.13f1. We recommend you to use this version too. This editor can be downloaded from [unity hub]("https://unity.com/download")
These steps can be used to add the project to Unity:

1.  Clone the repository using your favorite git tool.
2.  Open unity hub.
3.  Press "Add" under the projects tab.
4.  Select the AwARe folder inside the cloned repository.

### Server <a name="setupServer"></a>
For developing and testing we run the server locally. We also have access to a remote server from the UU, which runs on Ubuntu. On the remote server we have installed the main branch of our Github repository. Connecting to the server and running commands is done via SSH. On Windows we use the PuTTY SSH client. On mac we use the built in ssh commands. To upload/download files to/from the server we use the SSH File Transfer Protocol (the “sftp” command) and FileZilla.

See [Server](/AwARe/Server) for detailed server setup instructions.

## Acknowledgements <a name="ack"></a>
> The disclaimers and references.

### Unity Framework <a name="ackUnity"></a>
* [Unity](https://unity.com/)
### Third-party Software <a name="ackThird"></a>
* [Doxygen](https://www.doxygen.nl/) 
* [CodeCoverage](https://docs.unity3d.com/Packages/com.unity.testtools.codecoverage@0.2/manual/index.html)
### Resources <a name="ackRes"></a>
* [Crop models](https://craftpix.net/freebies/free-farming-crops-3d-low-poly-models/?utm_campaign=Website&utm_source=Sketchfab.com&utm_medium=public)
* [animal models](https://assetstore.unity.com/packages/3d/farm-animals-set-97945)

## Documentation <a name="docs"></a>
> Further reading.
* [Drive documentation](https://google.com)
* [Doxygen Documentation](/Docs/Doxygen)