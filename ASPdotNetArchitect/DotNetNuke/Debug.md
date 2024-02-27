# Prerequisite: 
* have `local instance` of DotNetNuke running at `dnndev.me`
* Install DotNetNuke `Module development template` on Visual Studio
* Run Visual Studio in `Administrator` mode

# Starting
* Tạo project từ DotNetNuke C# DAL Mvc Module Template
* -> chọn đường dẫn là đến thư mục **\desktopmodules\MVC** của Dnn instance 
* -> uncheck `create directory for solution`
* **Lưu ý**: no **ascx files** like in a `normal WebForms DNN Module` ; chỉ có **Views** 

* Build project in **Release** mode 
* -> để tạo `installable files` mà ta cần để s/d được module trên DNN websites
* -> Vì ta build project nên nó sẽ tạo `dll mới`; Dnn website sẽ cần thời gian để load lại

* ta login vào as `a host` (**Super User Account**) và install Extension bằng file **_intall.zip** (hoặc `_Source.zip`) ta vừa tạo  
* -> Dnn website sẽ cần thời gian để load lại vì ta đã modified the dll in bin folder 

* Tạo Page mới -> Add Module vào Page

## Debug within Visual Studio utilizing the Module project
* Ta sẽ `đặt breakpoint` vào 1 method của Controller
* switch thành **Debug** mode + build Solution -> ensures Visual Studio build `DLLs in with Debug symbols` for our module 

* **Attach to the Debugger to the IIS process to**
* Visual Studio -> Debug -> Attach to Processes -> attach the **w3wp.exe** worker process 