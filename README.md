<a name="toTop"></a>

<div align="center">
  <img src="logo.png" alt="AwARe Logo" width="85%"  height="auto" />
</div>

# AwARe: AR Food Production
Utrecht University: Computer Science BA Software Project 2023-2024

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
[AwARe/Assets/FirstParty](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty)

#### Navigation <a name="navFirst"></a>
> Where to find what in 'FirstParty'.

* [AdminTools](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/AdminTools)             <span style='color:blue'> &emsp; &#8594; Administrator environment (not developed yet) </span>
* [Application](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application)           <span style='color:blue'> &emsp; &#8594; Application code. </span>
    * [Scenes](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Scenes)            <span style='color:blue'> &emsp; &#8594; Unity Scene files </span>
        * [AppScenes](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/AppScenes)      <span style='color:blue'> &emsp; &#8594; Actual scenes of the application. </span>
        * [Support](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Scenes/Support)        <span style='color:blue'> &emsp; &#8594; Special scenes providing special functionality or assistance. </span>
        * [Temporary](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Scenes/Temporary)      <span style='color:blue'> &emsp; &#8594; Scene-copies for safe development. </span>
    * [Scripts](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Scripts)  <span style='color:blue'> &emsp; &#8594; Unity Prefabs & C# code. </span>
        * [General](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Scripts/General)       <span style='color:blue'> &emsp; &#8594; Re-usable or non-specific. </span>
        * [MainFeatures](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Scripts/MainFeatures)  <span style='color:blue'> &emsp; &#8594; Specific to core-functionality. </span>
        * [Temporary](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Scripts/Temporary)     <span style='color:blue'> &emsp; &#8594; For the development process only, and should be replaced later. </span>
    * [Resources](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Resources)            <span style='color:blue'> &emsp; &#8594; Reserved for 3D models to spawn in AR. </span>
        * [Models](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Resources/Models)            <span style='color:blue'> &emsp; &#8594; Reserved for 3D models to spawn in AR. </span>
        * [Prefabs](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Resources/Prefabs) <span style='color:blue'> &emsp; &#8594; Unity Prefabs & C# code. </span>
            * [General](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Resources/Prefabs/General)       <span style='color:blue'> &emsp; &#8594; Re-usable or non-specific. </span>
            * [MainFeatures](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Resources/Prefabs/MainFeatures)  <span style='color:blue'> &emsp; &#8594; Specific to core-functionality. </span>
        * [Data](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Resources/Data) <span style='color:blue'> &emsp; &#8594; Additional data used in the application. </span>
    * [Images](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Images)            <span style='color:blue'> &emsp; &#8594; All images used. </span>
    * [Materials](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Application/Materials)            <span style='color:blue'> &emsp; &#8594; Materials for prefabs. </span>
* [Assembly](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/Assembly)              <span style='color:blue'> &emsp; &#8594; Instructions to automatically adjust the project files. </span>
* [DevTools](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/DevTools)              <span style='color:blue'> &emsp; &#8594; Development build only code. (not developed yet) </span>
    * [Prefabs](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/DevTools/Prefabs)
        * [DebugLog](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/DevTools/Prefabs/DebugLog)      <span style='color:blue'> &emsp; &#8594; Support for error, warning and debug messages. </span>
        * [DebugToggle](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/DevTools/Prefabs/DebugToggle)   <span style='color:blue'> &emsp; &#8594; Switch for User/Developer UI. </span>
        * [DebugUI](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/DevTools/Prefabs/DebugUI)       <span style='color:blue'> &emsp; &#8594; Various debug view modes. </span>
        * [MockLoaders](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/DevTools/Prefabs/MockLoaders)         <span style='color:blue'> &emsp; &#8594; Fake data and objects for instant setup. </span>
    * [Scripts](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/DevTools/Scripts)
        * [DebugLog](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/DevTools/Scripts/DebugLog)      <span style='color:blue'> &emsp; &#8594; Support for error, warning and debug messages. </span>
        * [DebugToggle](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/DevTools/Scripts/DebugToggle)   <span style='color:blue'> &emsp; &#8594; Switch for User/Developer UI. </span>
        * [DebugUI](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/DevTools/Scripts/DebugUI)       <span style='color:blue'> &emsp; &#8594; Various debug view modes. </span>
        * [MockLoaders](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/DevTools/Scripts/MockLoaders)         <span style='color:blue'> &emsp; &#8594; Fake data and objects for instant setup. </span>
* [QA](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/QA)                    <span style='color:blue'> &emsp; &#8594; Quality assurance files. </span>
    * [Analyzers](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/QA.Analyzers)         <span style='color:blue'> &emsp; &#8594; Analyzers/Linters settings. </span>
    * [Testing](https://github.com/Mackthis/AwARe/tree/main/Assets/FirstParty/QA/Testing)           <span style='color:blue'> &emsp; &#8594; Test code/environment. </span>

### Third-party <a name="third"></a>
> External packages and tools in this repository.

Unity does not support organizing folders of Build-in packages or Third Party Software well. In principal, everything in the Unity Project that is stored outside First-Party is not owned by us.

## Setup <a name="setup"></a>
> Installation and use of this product.

### Android (.apk) <a name="setupApk"></a>
The latest android build can be downloaded here:
[.apk download](https://github.com/Mackthis/AwARe/tree/main)
### IOS (.ipa) <a name="setupIpa"></a>
The latest IOS build can be downloaded here:
[.ipa download](https://github.com/Mackthis/AwARe/tree/main)
### Unity <a name="setupUnity"></a>
For development, we use unity editor version 2022.3.13f1. This editor can be downloaded from [unity hub]("https://unity.com/download")
### Server <a name="setupServer"></a>
For developing and testing we run the server locally. We also have access to a remote server from the UU, which runs on Ubuntu. On the remote server we have installed the main branch of our Github repository. Connecting to the server and running commands is done via SSH. On Windows we use the PuTTY SSH client. On mac we use the built in ssh commands. To upload/download files to/from the server we use the SSH File Transfer Protocol (the “sftp” command) and FileZilla.

## Authors <a name="auth"></a>
> The Development-team (Us!)

## Acknowledgements <a name="ack"></a>
> The disclaimers and references.

### Unity Framework <a name="ackUnity"></a>
### Third-party Software <a name="ackThird"></a>
### Cooperations <a name="ackCoop"></a>
### Inspirations <a name="ackInsp"></a>