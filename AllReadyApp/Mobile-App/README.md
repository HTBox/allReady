# AllReady Mobile App

## Prerequisites

1. Visual Studio 2017 Community Edition 15.4.1
2. Node 6.11.4 
3. Cordova 7.1.0
4. Ionic 3.1.3
5. Typescript 2.5.3

## Installation

1. Download and install Visual Studio 2017 from https://www.visualstudio.com. During installation, select these components to install. If you already have Visual Studio installed, you may have to modify your existing installation.

- **Mobile development with Javascript**. This will install Tools for Apache Cordova, which includes a (older) version of Cordova itself. 

- **Node.js development**. This will install a (older) version node and npm. You could also install node separately https://nodejs.org/dist/v6.11.4/node-v6.11.4-x64.msi.

- **Python development**. This could be optional, depending on a number of factors. This will allow you run *npm rebuild node-sass* which requires node-gyp, which uses Python. You should select 2.7; 3.x may be incompatible. You might not need this, depending on system configuration.  If you want, you can try just downloading the build tools separately, if you ever determine you need it from https://www.python.org/downloads/.

- **Desktop development with C++**. This could be optional, depending on a number of factors. C++ Build Tools are required to recompile node-gyp on certain occassions.  If you want, you can try just downloading the build tools separately, if you ever determine you need them http://landinghub.visualstudio.com/visual-cpp-build-tools.

- **Windows 8.1 SDK**. This could be optional. It's required to rebuild node-gyp on certain occassions. If you want, you can try downloading this seperately if you ever determine you need it https://developer.microsoft.com/en-us/windows/downloads/windows-8-1-sdk

2. Download and install Node. Get 6.11.4.  (7.x/8.x may be incompatible; you'd have to try)
3. npm install -g cordova@latest (this will install latest  version; overriding any older version)
4. npm install -g ionic@latest 
5. npm install -g typescript@latest

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
2. Configure your installation of Nodejs to be FIRST when called/used by Visual Studio. Read https://ryanhayes.net/synchronize-node-js-install-version-with-visual-studio-2015/.
3. Open AllReadyMobileApp.sln
4. Wait for npm packages to restore. The status will be found in the Output window.
5. From the root of the project directory, run *npm rebuild node-sass*
6. Apply Live Reload Fix: https://stackoverflow.com/questions/43953687/ionic-2-visual-studio-template-live-reload-does-not-work
7. Bind npm's Watch to Project Open.
8. Ensure watch is running in Task Runner Explorer.
9. Click Simulate (play) in Browser

## Troubleshooting

If Visual Studio 2017 displays an error in task runner explorer window while trying to run the Watch script, take a close look at https://ryanhayes.net/synchronize-node-js-install-version-with-visual-studio-2015/.
The gist is to for Visual Studio to use your installed version of node FIRST instead of any other version that might be on your system. This is especially true if the error you see says
something like: Node Sass could not find a binding for your current environment: Windows 64-bit with Node.js 5.x. Found bindings for the following environments: - Windows 64-bit with Node.js 6.x.
