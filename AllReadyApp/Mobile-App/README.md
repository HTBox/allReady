# AllReady Mobile App

## Prerequisites

1. Visual Studio 2017 Community Edition >= 15.4.0
2. Node >= 7.10.0
3. NPM >= 5.4.2
4. Cordova >= 7.1.0
5. Ionic >= 3.1.3
6. Typescript >= 2.5.3
7. Simulate >= 0.3.13

## Installation

1. Download and install Visual Studio 2017 from https://www.visualstudio.com. 
Be sure to select Mobile Development with Javascript components, which includes Tools For Apache Cordova, during installation.
Be sure to select Node.js development components.
3. Download and install Node
4. Download and install NPM
5. npm install -g cordova (this will upgrade Cordova version to latest)
6. npm install -g ionic (this will install latest ionic version)
7. npm install -g typescript (this will upgrade latest version of Typescript)
7. npm install -g cordova-simulate

### Visual Studio 2017 Extensions

### NPM Task Runner
https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner

This will bind Visual Studio events (open, build, clean) to npm tasks defined in package.json.

### GitHub Extension for Visual Studio
https://marketplace.visualstudio.com/items?itemName=GitHub.GitHubExtensionforVisualStudio

This will allow you to easily download and update the source code found in the GitHub repository.

### Markdown Editor Extension for Visual Studio (optional)
https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor

### Productivity Power Tools 2017 for Visual Studio (optional)
https://marketplace.visualstudio.com/items?itemName=VisualStudioProductTeam.ProductivityPowerPack2017

## Configuration

1. Configure Visual Studio 2017 to start as Administrator - https://stackoverflow.com/questions/42723232/vs2017-run-as-admin-from-taskbar
2. Open AllReadyMobileApp.sln
3. Wait for npm packages to restore. The status will be found in the Output window.
4. From the root of the project directory, run *npm rebuild node-sass*
5. Apply Live Reload Fix: https://stackoverflow.com/questions/43953687/ionic-2-visual-studio-template-live-reload-does-not-work
6. Bind npm's Watch to Project Open.
7. Ensure watch is running in Task Runner Explorer.
8. Click Simulate (play) in Browser