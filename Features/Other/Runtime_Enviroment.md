=================================================================
# .NET MAUI - .NET Multi-platform App UI development
* -> When you install the **.NET Multi-platform App UI development** workload in Visual Studio, it adds new tools, libraries and SDKs to our system
* -> specifically for creating cross-platform (Windows, macOS, iOS, Android) applications (mobile, desktop) with .NET MAUI

* => only need a single codebase for UI and logic across various platforms
* => it also provides a modern, responsive, and flexible UI framework that supports XAML to build responsive, adaptive UIs that look native to each platform

=================================================================
## the runtime (CLR) and platform-specific API
* -> the runtime (CLR) of .NET is cross-platform, but the libraries or frameworks that depend on platform-specific APIs determine whether a specific application can run on multiple platforms
* -> for .NET Core and .NET 5+, the CLR is designed (_abstract away platform differences using layer_) to work on Windows, macOS, and Linux; 
* -> for .NET Framework, the CLR only works on Windows - because it integrated with Windows-specific APIs (Win32, COM, etc.).

* -> the libraries/frameworks (like WinForms, WPF, .NET MAUI) provide additional functionality on top of the runtime
* -> if **a framework relies on platform-specific APIs**, it becomes tied to that platform (Windows, MacOS, ...)

* _Ex: a WinForms app targeting .NET Core still won’t run on macOS because WinForms relies on Windows-only APIs_

## Target framework
* -> when we set the **Target Framework** in our project, we are targeting the a specific **`CLR`** version and its associated libraries (**`Base Class Library(BCL)`** and other framework libraries) 
* _these come from the **.NET Runtime** we have installed_

* -> **.NET Runtime** includes **`CoreCLR (Common Language Runtime)`** (**`.NET Framework CLR`** if it's .NET Framework) and **`Base Class Library (BCL)`**; 
* -> the **.NET SDK** includes everything in the **`.NET Runtime`** and **`additional development tools`** (_compiler, Debugging, ..._)

* -> if a project is **targeting .NET Framework**, the compiled program (EXE or DLL) **`will only run on Windows systems where the .NET Framework runtime is installed`**
* -> if the project is **targeting .NET (Core or later)**, the program is **`cross-platform and can run on Windows, macOS, or Linux as long as the corresponding .NET runtime is installed`**

## WinForms, WPF vs .NET MAUI
* -> Note that .NET MAUI does not extend WinForms or WPF to multiple platforms
* -> WinForms and WPF are desktop-focused for **Windows development only** 
* -> they rely on Windows-specific APIs (_especially for UI rendering_): **WinForms** with **`Win32 API`**, **WPF** with **`DirectX`**; these APIs are not available on macOS or Linux
* -> for .NET MAUI, it uses platform abstractions (platform-independent libraries and APIs) to work on Windows, macOS, iOS, and Android

## Web Developement
* **ASP.NET Core:**
* -> Fully cross-platform.
* -> Can run on Windows, macOS, and Linux.
* -> Suitable for building web APIs, MVC applications, Razor Pages, and Blazor Server apps.

* **ASP.NET (Classic):**
* -> Windows-only because it depends on IIS (Internet Information Services), which is a Windows-specific web server.
* -> The reasons for this are similar to desktop apps:

* -> Cross-platform frameworks like ASP.NET Core are designed to use platform-independent abstractions.
* -> Windows-only frameworks like ASP.NET (Classic) rely on Windows-specific components like IIS

## .NET Runtime vs .NET SDK
* -> ta cần hiểu rằng nếu chỉ cái **.NET Runtime** thì nó sẽ không có những libraries như **`Microsoft.AspNetCore.`** hoặc **`Microsoft.Extensions.`** để dev 1 ASP.NET Core Web API app
* -> nhưng sau khi khi ta build/publish/compile these project thì resulting application will depend on the .NET Runtime to execute (SDK is no longer involved)

=================================================================
> đây là lý do 1 số NuGet package ví dụ như **NAPS2.Wia** trong phần dependencies của nó bao gồm cả **.NETFramework,Version=v4.6.2**, **net6.0**, **.NETStandard,Version=v2.0**
> nghĩa là **NAPS2.Wia** có thể được dùng trong cả project **Winforms target .NET** hoặc **Winforms target .NET Framework**, hoặc 1 **Class Library target .NET Standare**

# Multiple target frameworks
* -> nó có thể dùng trong application projects nhưng typical for **Class Library projects**  
* -> by package together different versions of its libraries that are built specifically for different target frameworks in a single NuGet package
* -> then when we install the package into our project, NuGet will automatically pick the version that matches our project’s target framework

* -> for this case, these libraries of NuGet package are organized into folders corresponding to different target frameworks
```cs
// net462 - contains the library built for .NET Framework 4.6.2
// net6.0 - contains the library built for .NET 6

lib/
├── net462/
│   └── NAPS2.WIA.dll
├── net6.0/
│   └── NAPS2.WIA.dll
```

## Mechanism: 'conditional compilation' and 'multi-targeting build tools'
* -> use **shared code** for functionality that is common across all frameworks
* -> write **framework-specific code** for functionality that depends on the target framework

```cs - Ex:
#if NET462
    // Code specific to .NET Framework 4.6.2
    Console.WriteLine("Running on .NET Framework 4.6.2");
#elif NET6_0
    // Code specific to .NET 6
    Console.WriteLine("Running on .NET 6");
#endif
```

=================================================================
# .NET Standard
* -> is designed **for libraries** (Ex: we create a **`Class Library`** project), **not applications** because these application require Runtime and .NET Standard alone doesn’t provide a runtime
* -> is **not a runtime or platform** but **`a specification`** that defines **`a set of APIs that are available across different .NET implementations (.NET Runtime)`**

* => acts as **a compatibility layer** ensure that **`libraries built against .NET Standard can work across multiple .NET runtimes`** 
* (_i **.NET Runtime** like: .NET Framework, .NET Core, .NET 5+, Xamarin, Mono, other .NET implementations_)
* (_Ex: **a library** targets .NET Standard 2.0 can run on `.NET Framework 4.6.1 or later`, `.NET Core 2.0 or later`, `.NET 5+`_)

## 'Class Library' project
* -> khi tạo project có template **Class Library** bằng Visual Studio, ở phần **Framework** nó sẽ cho ta chọn 1 version của **.NET** hoặc **`.NET Standard`**
*  -> _các template cho application project (Console app, WinForms, WPF) buộc phải target a runtime like .NET Framework, .NET Core, or .NET 5+ vì nó cần runtime và platform-specific functionality_

* _lúc khởi tạo 1 project khi ta đã chỉ định framework cho nó, thì khi develop trong **Target Framework** section của project property ta chỉ được thay đổi một cách giới hạn_
* _Ví dụ 1 project chỉ định framework là .NET 6 thì ta chỉ có thể thay đổi **Target Framework** thành các version của .NET hoặc .NET Core_
* _còn với 1 project chỉ định framework là .NET Framework 4.5 thì ta chỉ có thể thay đổi **Target Framework** thành các version của .NET Framework_

* => **a library targeting .NET Standard** can be **`reused across multiple platforms and frameworks`**

```xml - file ".csproj"
<TargetFramework>netstandard2.0</TargetFramework>
```

## .NET 5 and .NET Standard
* -> with the **`unification of runtimes`** starting from **.NET 5**, ta sẽ chỉ sử dụng .NET Standard nếu ta cần phát triển **Library** support older runtimes like .NET Framework or .NET Core 2.x
* -> **target .NET 5+** if possible, as it **`includes all the APIs of .NET Standard and more`**

* -> **`.NET 5`** is a **`Unified Platform`** - tạo 1 thằng .NET duy nhất mang tính thống nhất 
* -> phát triển các thể loại app khác nhau: Desktop (WinForms, WPF), Mobile (via Xamarin, which evolved into .NET MAUI), Web (ASP.NET Core), Cloud (Azure integration), IoT and Game Development (Unity)
* -> phát triển trên nhiều nền tảng khác nhau: Windows, macOS, and Linux
* => chỉ có duy nhất 1 single SDK, runtime, and set of tools for all platforms
* => easier library sharing with **`multi-targeting`**
* => supports running most existing .NET Core applications without modification

* -> .NET Core bị thiếu một số tính năng (thường liên quan đến Windows-specific do phải support cross-platform), ví dụ như
* -> **`WinForms and WPF`** chỉ được add vào .NET Core version 3.0
* -> .NET Core lacked full support **`Windows Communication Foundation (WCF)`**, thường thì phải migrate to gRPC or ASP.NET Core for similar functionality
* -> **`ASP.NET Web Forms`** was not ported to .NET Core because it was tightly tied to older technologies and design patterns  

## Best Practice
* -> không thể refer trực tiếp giữa 1 project target **.NET** và 1 project target **.NET Framework**, tốt nhất là ta nên extract **`shared code`** thành 1 **`.NET Standard library`** để các project khác tham chiếu tới
* -> vì **.NET** là evolution của **.NET Core**, nên ta hoàn toàn có thể tham chiếu nếu API are compatible
* -> có thể reference giữa các project có target **.NET** version khác nhau nếu API compatible, nhưng thường thì **.NET** có phiên bản cao sẽ khó refer đến **.NET** thấp hơn, trường hợp này ta nên dùng **`Multiple Targeting`**

=================================================================
# Publish/Deployment: 
* -> the **`Framework-Dependent Deployment (FDD)`** or **`Self-Contained Deployment (SCD)`** option is only available when we publish the project
* _setup **Publish** option cho project rồi vào `Profile Settings` của Publish thì ta sẽ thấy **Deployment mode**, thường sẽ là `Framework-Dependent`_

## Framework-Dependent Deployment (FDD)
* -> only our **`application code`** and **`any dependencies not already part of the runtime`** (e.g., NuGet packages) are included in the deployment
* -> others will **`depends on a pre-installed .NET runtime on the target machine`** - core libraries (e.g., System.*) and the required runtime version

```r
// we build a console app targeting .NET 6 use "System.Text.Json" (part of the .NET runtime) and "Newtonsoft.Json" (an external NuGet package)
// -> when we deploy, the application includes only Newtonsoft.Json in the deployment package
// -> the System.Text.Json library is provided by the .NET runtime installed on the target machine
```

## Self-Contained Deployment (SCD)
* -> the application includes its **`own copy of the .NET runtime and all dependencies`**, **`our application code`**, **`all NuGet dependencies`**
* => the application runs independently of any pre-installed .NET runtime; the runtime is bundled as part of the application

```r
// -> when we deploy, our application includes both Newtonsoft.Json and System.Text.Json
// -> also bundles the .NET runtime itself (e.g., the .NET 6 runtime binaries)
// -> the app will run even if no .NET runtime is installed on the target machine
```

## Example
* -> nếu ta có 1 **WinForms project targeting .NET 6** và ta publish it as **`a Self-Contained Deployment`**
* -> thì ta có hoàn toàn có thể chạy cái published application trên 1 Windows computer không install .NET 6

=================================================================
> refer to CPU architectures, not operating systems
> are supported across multiple operating systems, including Windows, macOS, and Linux

# Target Platform
* -> determines what is the type of processor architecture that the application is built for to run on

## Best Pratice
* -> use **`Any CPU`** whenever possible - most flexible option and should be the default choice for most .NET applications
* -> if app uses 32-bit-only native libraries thì ta nên dùng **`x86`** as Target Platform
* -> if app requires > 4GB memory (memory is available to 32-bit processes is ~4GB) thì ta nên dùng **`x86`** as Target Platform

```bash
// An x64-only application won’t run on a 32-bit system
// An ARM64 application won’t run on x86 systems
```

## Troubleshoot Architecture Issues
* -> if we encounter issues related to the platform target, use **`CorFlags.exe`** - a tool that shows the CPU architecture of our application's executable
* -> also check dependencies, ensure all referenced libraries and components match the platform target

* -> maybe analyze our Application’s Runtime Behavior Use tools like Dependency Walker or ILSpy to analyze the runtime dependencies of the package
* -> Use tools like Process Explorer to verify the architecture of your application and its loaded modules.


## Type 
* -> 32-bit (x86) architecture.
* -> 64-bit (x64) or AMD64 architecture (an extension of x86)
* -> ARM (ARM32 or ARM64) architecture
* -> Any CPU: this option allows the app to run on any processor architecture

### Any CPU:
* -> the application can run on either 32-bit or 64-bit systems
* -> runs as a 64-bit process by default and runs as a 32-bit process because 64-bit processes are not supported
* -> if running on a 32-bit OS, it runs as a 32-bit process (if the referenced a NuGet package is x64 only, the app will fail with a BadImageFormatException)
* -> if running on a 64-bit OS, it runs as a 64-bit process (if the referenced a NuGet package is x86 only, the app will fail with a BadImageFormatException)

### x86:
* -> Forces the application to run as a 32-bit process, even on a 64-bit OS.
* -> Required if your application depends on 32-bit components or libraries (e.g., COM objects, ActiveX controls, DLLs) that do not have 64-bit equivalents

### x64:
* -> forces the application to run as a 64-bit process
* -> used when your application needs to utilize more memory than a 32-bit process can access (32-bit processes are limited to ~4GB of memory)

### ARM32 / ARM64:
* -> targets ARM processors, typically used for devices like phones, tablets, IoT devices, and some modern Windows-on-ARM laptops

## Compatibility of OS
* -> Windows: Supports both x86 and x64.
* -> macOS: Historically supported x86 and x64 until Apple transitioned to ARM64 processors (Apple Silicon) starting with the M1 chip in 2020.
* -> Linux: Supports x86, x64, ARM, and other architectures

* -> most modern PCs (with **`WOW64`** that has **32-bit Compatibility Mode**) support **`both 32-bit and 64-bit apps`**
* -> nhưng chỉ có thể dùng 1 trong 2 để chạy 1 app; vậy nên 1 NuGet Package không thể cùng lúc bao gồm both x86 and x64 libraries
* -> by default it will use the 64-bit process
* -> but if the Windows OS is 64-bit but the process is forced to run in 32-bit mode (e.g., due to legacy dependencies), an x86 library can load successfully, but x64 libraries will fail
* => nếu ta muốn xử lý 1 application chạy cho 2 CPU architect khác nhau thì ta sẽ cần tạo 2 bản build với 1 target to x86, 1 target to x64

## Compatibility of NuGet package
* -> **managed code** - written entirely in .NET languages (C#, VB.NET, F#), which are platform-agnostic; these packages typically support **`Any CPU`** and are safe for cross-platform use
* -> **native Code** - if the package includes native libraries (e.g., C++ DLLs), it **`may only support specific architectures`** (e.g., x86 or x64)

* => a NuGet package that uses native C++ libraries, such as **`image processing`** or **`hardware integration`** tools, may have architecture-specific binaries

## Multi-Architecture Libraries
* -> Some libraries include both x86 and x64 versions in their NuGet package (e.g., in runtimes/x86 and runtimes/x64 folders)
* -> **`the correct version is automatically selected at runtime`**

### Target Framework
* -> if a package targets **`.NET Standard`** or a modern **`.NET`** runtime (e.g., .NET 6), it is more likely to support **Any CPU**
* -> packages targeting older frameworks (e.g., **`.NET Framework`**) might depend on **architecture-specific dependencies** or runtime environments

### TroubleShoot
* -> if in the **`Dependencies`** section of **NuGet Package Manager**, the package lists platform-specific dependencies, it may not support "Any CPU"
* -> a NuGet package is essentially a .zip file, we can extract it to **`look for folders named runtimes, x86, x64, ARM`**, etc
if we see architecture-specific binaries (e.g., x86/native.dll), the package does not fully support "Any CPU"
* -> we can also visit the package’s page on the **`NuGet Gallery`** to check architecture-specific

### Using "Any CPU" application with a Platform-Specific Package