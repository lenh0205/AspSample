# Step để add thành công Module trên 1 trang DNN hosting
* Hosting thành công DNN trên IIS trước
* git init + add + commit với **bin** của `DNN` -> để đảm bảo những `dll` ban đầu không bị thay đổi (_nếu thay đổi có thể dẫn đến dnn bị vấn đề_)

* phải chọn `output path` cho 3 file `build Release` (**.dll, .dll.config, .pdb**) của Module project vô folder **"bin"** của DNN

* `onClick deploymment` để tạo file "_install.zip" và cài Extension cho trang DNN successfully
* => để tạo folder `"MVC/rootFolder/MyProject"` gồm Views, App_LocalResources, module.css, Scripts,...

## Nếu sau khi "Build Release" có vấn đề
* VD: trang Dnn mất "PersonaBar", hoặc không load được trang do "Newtonsoft" có vấn đề
* Rất có thể quá trình `Build Release`, những `References` của `Module project` đã ghi đè `dll` trong `bin` của `Dnn`
* cần kiểm tra lại `path` cũng như `version` mà `Reference` của `Module project` đang tham chiếu có đồng nhất với `bin` của `Dnn` không

## Upload "_install.zip" vô trang DNN bị lỗi
* Đầu tiên thử install lại mấy lần nếu vẫn lỗi thì:

* Phần lớn là do thiếu **`Resource`**
* -> Ta thử extract file zip ra
* -> Rất có thể là thiếu thư mục **`View`** (do thư mục "DesktopModules/MVC" cần Module cung cấp ) 
* -> có thể là do trong file **`.csproj`** ta đã quên include `_Layout.cshtml`
<Content Include="Shared\_Layout.cshtml" />

## Sau khi chọn Module rồi load ra UI mà bị lỗi
* Nếu là lỗi **`Default.aspx không tìm thấy controller`**, thì do ta đã không `output path` đúng cho `Module project` vào **bin** của DNN (_nên `dll` của project không tìm thấy trong bin_)
* Nếu là lỗi **`không tìm thấy view`**, thì do ta chưa ném file **.zip** cho **`Extension install`** của trang DNN, hoặc **.csproj** chưa **`Include`** các `View.cshtml, _Layout.cshtml`

## Gọi vào Helper gặp lỗi không thể tạo Controller:
* Do ta chưa config vào **`web.config`** của DNN
<appSettings>
<!-- Config cho module: Quản lý văn bản  -->
<add key="APIHOSTdnnQLVB" value="/dnn_demo/DesktopModules/DataExchangeQLVB/API/DataProcessor/" />
<add key="APIKEYQLVB" value="ZoY0a5r2D8QyVUUGnbzJkaYuKvjDs1RW" />
<add key="APIQLVB" value="http://localhost/qlvb_ser/api/" />

<!-- Config cho module: HeThong -->
<add key="AlfrescoFileSize" value="100" />
</appSettings>

============================
# Cấu trúc thư mục
* **RootFolder**
* **|-GUI** (Dnn MVC Module)
* **|--Script**
* **|---React**
* **|---build**
* **|-HelperCommon** (Dnn MVC Module)
* **|-SER** (ASP.NET Core Web API)

# Architect
* **`GUI`** - MVC cho View
* **`HelperCommon`**: lớp này để xử lý những tác vụ của DNN (DNN authen/author, ....); gọi lớp service để xử lý Business
* **`SER`**`: tách ra deploy lên service riêng -> giảm tải cho DNN; làm nhiệm vụ xử lý business (không có authen, ...); 

## React
* gọi tới `http://[host_name]:[port]/GET?url=[SER_Controller]/[SER_action]`

* -> `axiosServices.get(url)`
```
class AxiosServices {
    get = async (url: any, header = {}) => {
        var apiURL = "";
        if (IS_DEVELOPMENT === "true")
            apiURL = process.env.REACT_APP_LOCAL_HOST + url
        else
            apiURL = appCommon.getBaseUrl() + "GET?url=" + url
        return await instance.get(
            apiURL,
            header
        )
    }
}
export let axiosServices = new AxiosServices();
```



==========================
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