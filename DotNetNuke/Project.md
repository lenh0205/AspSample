# Step để add thành công Module trên 1 trang DNN hosting
* Hosting thành công DNN trên IIS trước

* `onClick deploymment` để tạo file "_install.zip" và cài Extension cho trang DNN successfully
* => để tạo folder `"MVC/rootFolder/MyProject"` gồm Views, App_LocalResources, module.css, Scripts,...

* -> Lấy 3 file `build Release` (**.dll, .dll.config, .pdb**) cho vô folder **"bin"** của DNN

# Cấu trúc thư mục
```
RootFolder
|-GUI (MVC Module)
|--Script
|---React
|---build
|-HelperCommon (MVC Module)
|-SER
```

# Cấu hình tránh zip luôn "React, node_modules" thành file "_install.zip"
<ItemGroup>     
    <InstallInclude Include="**\*.ascx" Exclude="packages\**" />
    <InstallInclude Include="**\*.asmx" Exclude="packages\**" />
    <InstallInclude Include="**\*.css" Exclude="packages\**" />
    <InstallInclude Include="**\*.html" Exclude="packages\**" />
    <InstallInclude Include="**\*.cshtml" Exclude="packages\**" />
    <InstallInclude Include="**\*.htm" Exclude="packages\**" />
    <InstallInclude Include="**\*.resx" Exclude="packages\**" />
    <InstallInclude Include="**\*.aspx" Exclude="packages\**" />
    <InstallInclude Include="**\*.js" Exclude="packages\**" />
    <InstallInclude Include="**\*.html" Exclude="packages\**;**\Scripts\React\**" />
    <InstallInclude Include="**\*.txt"  Exclude="**\obj\**;**\_ReSharper*\**;packages\**;**\.git\**;" />
    <InstallInclude Include="**\images\**" Exclude="packages\**" />
    <InstallInclude Include="**\*.css" Exclude="packages\**;**\Scripts\React\**" />
    <InstallInclude Include="**\*.js" Exclude="packages\**;**\Scripts\React\**" />
    <InstallInclude Include="**\*.ts" Exclude="packages\**;**\Scripts\React\**" />
    <InstallInclude Include="**\*.txt"  Exclude="**\obj\**;**\_ReSharper*\**;packages\**;**\.git\**;**\Scripts\React\**" />
    <InstallInclude Include="**\images\**" Exclude="packages\**;**\Scripts\React\**" />
</ItemGroup>

# React 