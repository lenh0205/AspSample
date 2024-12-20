
# .NET MAUI - .NET Multi-platform App UI development
* -> When you install the **.NET Multi-platform App UI development** workload in Visual Studio, it adds new tools, libraries and SDKs to our system
* -> specifically for creating cross-platform (Windows, macOS, iOS, Android) applications (mobile, desktop) with .NET MAUI

* => only need a single codebase for UI and logic across various platforms
* => it also provides a modern, responsive, and flexible UI framework that supports XAML to build responsive, adaptive UIs that look native to each platform

## the runtime (CLR) and platform-specific API
* -> the runtime (CLR) of .NET is cross-platform, but the libraries or frameworks that depend on platform-specific APIs determine whether a specific application can run on multiple platforms
* -> for .NET Core and .NET 5+, the CLR is designed (_abstract away platform differences using layer_) to work on Windows, macOS, and Linux; 
* -> for .NET Framework, the CLR only works on Windows - because it integrated with Windows-specific APIs (Win32, COM, etc.).

* -> the libraries/frameworks (like WinForms, WPF, .NET MAUI) provide additional functionality on top of the runtime, if **a framework relies on platform-specific APIs**, it becomes tied to that platform (Windows, MacOS, ...)

* _Ex: a WinForms app targeting .NET Core still won’t run on macOS because WinForms relies on Windows-only APIs_

## Target framework
* -> when we set the **Target Framework** in our project, we are targeting **`a specific runtime version and its associated libraries`** that come from the **.NET Runtime** we have installed
* -> **.NET Runtime** includes **`CoreCLR (Common Language Runtime)`** and **`Base Class Library (BCL)`**; the **.NET SDK** includes everything in the **`.NET Runtime`** and **`additional development tools`** (_compiler, Debugging, ..._)

* -> if a project is **targeting .NET Framework**, the compiled program (EXE or DLL) **`will only run on Windows systems where the .NET Framework runtime is installed`**
* -> if the project is targeting .NET (Core or later), the program is **`cross-platform and can run on Windows, macOS, or Linux as long as the corresponding .NET runtime is installed`**

## WinForms, WPF vs .NET MAUI
* -> Note that .NET MAUI does not extend WinForms or WPF to multiple platforms
* -> WinForms and WPF are desktop-focused for **Windows development only** 
* -> they rely on Windows-specific APIs (_especially for UI rendering_): **WinForms** with **`Win32 API`**, **WPF** with **`DirectX`**; these APIs are not available on macOS or Linux
* -> for .NET MAUI, it uses platform abstractions (platform-independent libraries and APIs) to work on Windows, macOS, iOS, and Android

## Web Developement
ASP.NET Core:

Fully cross-platform.
Can run on Windows, macOS, and Linux.
Suitable for building web APIs, MVC applications, Razor Pages, and Blazor Server apps.
ASP.NET (Classic):

Windows-only because it depends on IIS (Internet Information Services), which is a Windows-specific web server.
The reasons for this are similar to desktop apps:

Cross-platform frameworks like ASP.NET Core are designed to use platform-independent abstractions.
Windows-only frameworks like ASP.NET (Classic) rely on Windows-specific components like IIS