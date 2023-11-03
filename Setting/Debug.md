
# Debugger
* The **debugger** in .NET is a built-in program in Visual Studio that aids in _detecting and correcting errors_ in code

# Debugging Idea
* **Visual Studio** has its own `integrated debugging engine` to run and debug web sites inside Visual Studio
* -> **`built-in IIS server`** serves as the `default debug server` for `asp.net and.net core projects`

* When we host/deploy sites on IIS, 
* -> the **Worker Process (w3wp.exe)** is used to run the web application
* -> which takes care of all execution and maintenance of deployed web applications
* -> We need to `attach to this particular process from Visual Studio` to debug the web application

=======================================================
# w3wp.exe - Worker Process of IIS 
* Worker process của IIS có tên là **w3wp.exe**
* _when running ASP.NET applications within IIS_, `All ASP.NET functionality` runs under the scope of the Worker Process
* _When a request comes to the server from a client_, the Worker Process is **`responsible for generating the request and response`**; also maintains the **`InProc session data`**

> _If we recycle the Worker Process, we will lose its state_

==========================================================
# Application Pool    
* responsible for **isolate one or more applications into their own process**
* `In the Production Environment`, it is **one of the most important things** that we must create for our application

* Application Pools are used to **separate sets of IIS Worker Processes** that share the same configuration
* -> enable us to **`isolate our web application`** for better security, reliability, and availability
* -> _The Worker Process_ serves as the process boundary that separates each _Application Pool_

* => when one Worker Process or application has an issue or recycles, other applications or Worker Processes are not affected

*  An Application Pool containing more than one Worker Process called a **Web Garden**
* -> ta có thể `view nó` bằng cách: right click vào 1 Application pool -> Advanced Setting -> `Maximum number of worker processes`

```cs
// VD: ta có 2 Website A và B và muốn deploy on same server -> Application pool isolate Website A run on Application pool A and Website B run on Application pool B
```

## Default Application Pool of IIS 6.0
* is **DefaultAppPool**

* we can `view it` by: hosting the site on IIS -> right click on the application / Virtual directory của ta -> Manage Application -> Advanced Setting
* to check the list of all Application Pools in IIS: expand the Application Pool node on IIS Server (chắc chắn sẽ có `DefaultAppPool`)

## Creating and Assigning an Application Pool
* trên IIS, right click vào "Application Pool" -> chọn `"Add Application Pool"` -> đặt tên cho nó -> Ok
* Khi ta Add Application / Virtual Directory -> ta có thể select Application Pool cho nó (_hoặc vào Advanced Setting của Application để assign lại Application Pool cho nó_)

==========================================================
# ASP.NET Debugging 
* when an ASP.NET application is running in Visual Studio, it is under the control of the **ASP.NET Engine**
* -> the execution breaks when certain a breakpoint is reached
* -> trên Visual Studio, `Debug > Debug Properties` hoặc chạy Debugging rồi `Debug > Windows > Processes` để xem thông tin của Process 

* Behind the running process is **WebDev.WebServer.exe** (_nếu ta dùng `ASP.NET Development Server` để debug_)
* -> to run a web application from the command prompt: Open command prompt -> Run WebDev.WebServer

==========================================================
# Debug "ASP.NET Core" apps hosting on IIS
* -> Sửa lại **`Setting`** của **`Publish`** với **`Configuration`** thành **Debug** 
* -> publish vào 1 folder 
* -> host App với physical path tới folder đó
* -> **Ctrl + Alt + P** để **`attach process`**     

==========================================================

# Debug "ASP.NET Web Application" hosted on IIS
* By attaching the **`worker process`** - **w3wp.exe** of the intended web application to Visual Studio

## Enable IIS
* "window + R" to open `Run window` -> "appwiz.cpl" to open `Program and Features windows` 
* -> Turn Windows features on or off -> check all the subcomponents inside `Internet Information Services`
* -> Ok -> browse `http://localhost` to get default page of IIS

## Hosted Web Application 
* `Create Sample Asp.net Core Project`
* `Add web application in IIS`: Open IIS -> Add WebSites -> give "sitename" and "physical path" + choose the **Application Pool**
* check Task Manager whether or not the `w3wp.exe` Worker Process is running

## Enable Development-Time IIS support: 
* Open _Visual Studio Installer_ -> Check _Development time IIS support_ component (to install **ASP.NET Core Module**, which need to run .Net Core apps on IIS) 

## Attach the running "w3process" to the Visual Studio
* Open Project in Visual Studio + Attach debug point -> build the project

* In Visual Studio, go to Debug → **`Attach to Processor`** (or **Ctrl + Alt + P**)  
* -> check on `"Show process from all users"`
* -> Click on the `"Refresh"` Button
* -> find the **w3wp.exe process** + click `Attach`

* -> thử đặt breakpoint xem có hiện warning không (_nếu có là process chưa được attach_)
* -> click the `Debug button`

* _For a single Worker Process, thường thì attach không có vấn đề gì. but we have `multiple Worker Processes running on IIS`, then it might have some confusion_

## Attach to one of many running Worker Processes
* When we have multiple sites hosted on IIS, and those sites have their own Application Pool
* -> multiple Application Pools means multiple Worker Processes are running

* Open the **`Process Attach`** window, giả sử 3 thấy **w3wp.exe** Worker Proccess đang chạy với 3 `ID` khác nhau; và ta phải attach đúng cái Worker Process của Application Pool ta cần
* Open cmd -> cd tới `C:\Windows\System32` -> nhập lệnh **cscript iisapp.vbs** sẽ show list of `running Worker Process, Process ID and Application Pool Name` (_hoặc trên IIS local desktop connection -> chọn Worker Processes option -> ta sẽ thấy cái Application Pool ta vừa tạo để chạy hosted App -> ghi nhớ "Process Id" của nó_)
-> H ta có thể chọn xác định được correct Application Pool name and its process ID

============================================================
# Debug Asp.Net Website hosted on IIS
* Create Application in **Default Web Site** of IIS
* Don't open the project using the Visual Studio project file
* In Visual Studio, go to `File -> Open -> Web Site` (Shift+Alt+O)
* Select our Application -> now the `Solution Explorer` will uses the URL in place of the project name (_VD: http://localhost/mysite/_)
* Now, Click the debug arrow like normal

===========================================================
# Remote Debug
* If running IIS on a different machine
* install the Remote Debugger which is on Visual Studio disks 
* connect to hat by using Debug|Attach To Process in Visual Studio.

===========================================================
# Example: Debug IIS hosted ASP.NET MVC Application In Visual Studio Using Worker Process
* Host 1 ASP.NET MVC Application vào `Default Web Site` của IIS
* Mở IIS -> right click on "Application Pool" -> `Add Application Pool` -> đặt tên rồi ok
* Vào `Advanced Setting` của Application Pool đó -> Đổi **Identity** thành **`NetworkService`** (_for realtime project_) (_By Default is Local Service_)
* _`Map Application Pool to hosted MVC Application`_ : Vào `Basic setting` của hosted app -> select _Application Pool_ ta vừa tạo

* Trên IIS local desktop connection -> chọn **Worker Processes** option -> ta sẽ thấy cái Application Pool ta vừa tạo để chạy hosted App -> ghi nhớ **Process Id** của nó
* Vào Visual Studio -> Debug -> Attach to Process 



