
# Debugger
* The **debugger** in.net is a built-in program in Visual Studio that aids in _detecting and correcting errors_ in code
* `built-in IIS server` serves as the `default debug server`  for `asp.net and.net core projects`

# "w3wp.exe" worker process
* All ASP.NET application runs under the worker process.
* When a client sent a request to the server, the worker process will be responsible for generating the request and response. 
* => worker process is one of the main functional parts of the ASP.Net Web application running on IIS.

# Debug Web Application hosted on IIS
* By attaching the **`worker process`** - **w3wp.exe** of the intended web application to Visual Studio

## Enable IIS
* "window + R" to open `Run window` -> "appwiz.cpl" to open `Program and Features windows` 
* -> Turn Windows features on or off -> check all the subcomponents inside `Internet Information Services`
* -> Ok -> browse `http://localhost` to get default page of IIS

## Debug IIS Hosted Web Application in Asp.Net Core Project
* `Create Sample Asp.net Core Project`
* `Add web application in IIS`: Open IIS -> Add WebSites -> give "sitename" and "physical path"
* `Enable Development-Time IIS support`: Open _Visual Studio Installer_ -> Check _Development time IIS support_ component (to install **ASP.NET Core Module**, which need to run .Net Core apps on IIS) 

## Attach w3process in Visual Studio
* Open Project in Visual Studio + Attach debug point -> build the project

* In Visual Studio, go to Debug → **`Attach to Processor`** (or **Ctrl + Alt + P**)  
* -> check on `"Show process from all users"`
* -> Click on the `"Refresh"` Button
* -> find the **w3wp.exe process** + click `Attach`