# https://docs.dnncommunity.org/index.html

# Develop DotNetNuke MVC C# Module project:
## Init
* chọn Template **`C# DAL2 MVC Module`**
* chọn path là **`C:\Websites\dnndev.me\desktopmodules\mvc`** (_những project với template khác `MVC` và `theme` thì phải cho vào **\desktopmodules**_)
* bỏ check `Create directory for solution` đi
* chọn **Local Dev URL** là **`dnndev.me`** (hoặc `www.dnndev.me`)

## Build
* build project ở _Debug Mode_ (những module template sẽ không tạo ra `installable package` - chỉ có thể thực hiện _build hoặc compile_ ở **Release** model)
* build ở **Release** mode -> create a new folder trong `/mvc/<root folder>/desktopmodules` call **install** (nó nằm trong _MVC Module project_ của ta)
* Inside _install_ folder, ta có 2 file: `_Install.zip` , `_Source.zip`

* Switch to `Release` mode; compile and build project -> create a `package` for module
* use it to install via the extension page in _Dnndev instance_

## Usage:
* ta vừa mới compiled new new code in this module -> that new code đc put into the **bin** folder of the `DNN website`
* Refresh trang DNN để show code  mới (cần đợi chút để code mới đc recompiled trong website to reload)
* Đăng nhập -> chọn `Install Extension` 
* chọn 1 trong 2 file (**`C:\Websites\dnndev.me\DesktopModules\MVC\<OurModuleProjectFolder>\install\`**)
* ta vừa replace the **code/DLL** in **bin** folder
* vậy là ta đã tạo compiled package and install a new DNN Module

* H ta add our new Module vào page

## Content
* `Solution file` for an MVC project sẽ nằm trong `\DesktopModules\MVC\<our project>`

* **Properties -> AssemblyInfo.cs** - change version number of module (nếu ta cần tạo new release cho project)

* **App_LocalResources -> `.resx` file** (`resource files`) for DNN project (nếu ta cần localize in different languages ta có thể tạo thêm resource file trong folder này)

* **"BuildScripts" folder** - những file trong này làm công việc packaging (in zip file) when we build our module

* **Components folder**: 
* -> **`FeatureController.cs`** - have optional interface (`IPortable, ISearchable, IUpgradeable`); 
* -> **`ItemManager.cs`** - is utilized within particular module; tức là nếu ta s/d MVC Module template nó sẽ create 1 simple module that allows us to create a `collection of items` => _ItemManager_ create `definitions` for methods for our items (for **creating, deleting, get, update item** base on ID and moduleId or object itself)

* **Controllers** 
* -> **`ItemController.cs`** - provide functionality for CRUD items - services call get handled within MVC module through the frontend
* -> **`SettingsController.cs`** - similar to `ItemController` but allow to retrieve and update **module setting** (2 module setting come with MVC project: **.Setting1, .Setting2**)

**Documentation** - ta có thể delete thư mục này

* **Models** - chứ 2 Model khi khởi tạo là `Item, Setting`

* **Providers -> DataProviders -> SqlDataProvider**
* -> **`00.00.01.SqlDataProvider`** - nó là SQL script đc DNN execute to create table necessary to contain our items for our Module; 
* -> nếu ta muốn `manual execute` thì ta cần replace token cho `databaseOwner` and `objectQualifier`
* => change `databaseOwner` to `dbo.`
* => change `objectQualifier` to empty string

## use Module for DotNetNuke page:
* Build Release -> Publish app -> zip it -> ném vào phần "Install Extension" của trang DotNetNuke

===============================
# Develop DotNetNuke "Web Forms" C# Module project:
* prerequiste: local copy of DNN running

* choose template **`C# DAL2 Compiled Module`**
* Đặt tên cho project
* chọn path là **`C:\Websites\dnndev.me\desktopmodules`** 
* bỏ check `Create directory for solution` đi
* chọn **Local Dev URL** là **`dnndev.me`** 

* switch to `Release` mode -> build => create an installable zip file and installable source control for project


===============================
# Create Module for DotNetNuke
## Tích hợp DotNetNuke Extension Development `Templates` cho Visual Studio 
* **prerequisites**: a local instance of DNN running on `dnndev.me`
* -> Tìm Git repository hỗ trợ (https://github.com/ChrisHammond/DNNTemplates/releases)
* -> chọn templates phù hợp với phiên bản DotNetNuke và Visual Studio đang dùng
* -> Tải file **`DNNTemplates.vsix`** về thư mục Download của máy
* -> chạy Setup Wizard của nó (nên tắt hết các ứng dụng chạy ngầm trước khi chạy)

* Hoặc vào Visual Studio (administrator) -> Tools -> Get Tools and Features -> Online -> search "DotNetNuke" -> download -> Setup wizard -> reOpen Visual Studio (administrator)

## Thêm Host vào máy:
* Try cập **C:\Windows\System32\drivers\etc\hosts**
* Thêm dòng **`127.0.0.1 dnndev.me`** vào cuối file
* Command Prompt: `ipconfig /flushdns` to clear the DNS cache

# Create and Packaging a Module in DNN 9
## Condition:
* Ta cần chạy trang DotNetNuke ta host trên IIS trước (có DNN instance)
* Sử dụng Visual Studio với quyền **`Administrator`** (to access the IIS configuration file) 


=================================
# Install DotNetNuke in a local development enviroment on a virtual machine :
## Setup Folder 
* Lên trang chủ tải file zip **`DNN_Platform_Install.zip`** (`_Source.zip` chỉ dùng để develop DNN Platform)
* Vào _properties_ của file zip -> check **Unblock** (trust file - allow to extract all content of the file)
* giải nén thành 1 Folder 
* bỏ content của folder đó vào `C:\Websites\dnndev.me` (domain name owned by DNNsoftware with DNS point to **`127.0.0.1`**`) (_tức là nếu access cái domain này thì nó sẽ redirect đến local IP address)

* right click at folder -> properties -> Security -> Add -> Check Names: iis `apppool\dnndev.me` -> click `Locations` -> cho quyền Modified -> Apply
* **Add User, permission** used by Application pool/Web Server (`NETWORK SERVICE`): right click at folder -> properties -> Security

## Setup IIS
* Add Website vào **Sites**
* Turn on những **`Window Feature`** cần thiết
* Physical path là `C:\Websites\dnndev.me`
* hostname là **`dnndev.me`** , vào **Request Filtering -> Rules** để sửa lại dung lượng file có thể upload (để upload Extension)
* Setup SQL Server: cần SQL Server Authentication và tạo sẵn 1 Database

## DNN Wizard Installation
* install `local instance` of DNN9


========================================
# DNN Overview
* a platform have ability to run multiple websites out of one installation, each of those sites sometimes referred to as a portal
* **personal bar** - content editor and admins of DNN site: manage pages, content, security, file system trong CMS của ta
* manage content through use of pages, modules 
* control look, fields of pages, modules through the use of `themes, skins, containers`
* Security - restrict access to pages; who can view/edit

* **Site Assests** - manage files, images, documents upload to CMS
* **Global Assests** - manage high-level of files, images, documents (multiple websites can have their own collection of assets or pull from `Global Assets`)
* **Site Setting** - can change website name, URL
* **Extensions** - add modules, content within DNN platform
* **pencil icon** go to "Edit" mode to edit a content within DNN website

# Personal Bar (_replace Control Panel, admin,host menu items_)

## Content Management:
* The only way to add pages from within personal bar
* Add mutilple page 
* change page structure: drag, xem page children
* Recycle Bin: show Pages, Modules, User get deleted  

## MANAGE Area:
* **Themes**: control and configure the themes that our website are using
* **Sites** (login as a host super user) : if having mutilple sites, we can navigate to `Site's area` which allow to create additional sites within DNN Installation
* **Admin Log**: events occurring within our DNN 9 Installation

## SETTINGS Area:
* **Site Settings**: Config the title of site, basic description, keywords for Search Engine optimization,... 
* **Security**: control `Login Settings` (restrict IP,), `Member Accounts`, `Security Analyzer` (things need to configured from a security perspective)
**SEO**: manage URL (setting, extension use for URL provider), sitemap
* **Vocabularies**: manage tags and lists within our DNN site
* **Extension**: install new Modules Themes
* **Servers**: server settings, restart **`application`**, **`Clear cache`**
* **Config Management**: allow to configure `web.config` files, and orther config file

## Edit Mode:
* Add Module, Add Existing Module (copy module from another page)
* **Close** - close edit mode, take back to View mode

# Create Pages
* Add `Page` (_so called `Tab`_)
* `apply content to page`` - throught the use of a **module** (the module - a plugin container provides functionality within CMS)
* can have Module for photo gallery, Module for video player -> the most common is **HTML Module** - an interface provide **rich text editor** allow to put _HTML content_ into a page
* **Modules** can be placed into different `sections` of the page; 
* `sections` are controlled with the **theme or skin**
* **Theme** - provide a `layout for a page` controlled through **panes**
* **panes**/location - những ô để ta bỏ modules, contents vào
* **Container** - ta có thể wrap these modules, contents in a wrapper (container) VD: have border, background,...

## Edit mode:
* **Add Module to page** 
* **Copy Existing Module**
* **Page Setting** -> Permissions - cho quyền user xem/chỉnh sửa

* Icon - Module Action
* **pencil Icon** - inline edit
* **Edit Content** option - go to rich text editor -> nó cho phép Editors who might `not familiar with HTML` có thể change content (làm chữ đậm hơn, create hyperlink,...)

=============================
# "Always do" Setup
_`Set up a new DNN 9 website (always do)`_ : _Delete content on default home page, site setting: turn off default feature, config registration, disable DNN copyright_

## Delete Modules
* Go to Edit mode -> Di chuyến Module -> setting icon -> Delete
* Xoá hết Module ở Home page


=================================
## Site Setting

### Site Info
* Điền `Title`, `Description`, `Keywords` will be utilize by DNN on any page that don't have their own config (for the SEO)
* `Site Time Zone`: choose time zone where the server is located
* `Copyright`: the copy right message is being displayed within the _skin or theme_ that controls look and feel of websites ; if remove all together what DNN will take the title + current year 
* `Logo and icon`: upload a new logo for site
* -> Save -> Refresh page

### Site Behavior
* -> More
* Uncheck `Check for Software Upgrades`, uncheck `participate in DNA improvement option`

## Security
* **Member Accounts** -> **Registration Settings** -> **User Registration**: 
* `None` - not allow users to register; not allow them to create new accounts as a host or an admin; can still create accounts for our users
* `Private` - allows people to register but they cannot login until approved by an administrator
* `Public` - anyone can register on the site and immediately login 
* `Verified` - allows to register on the site but have to go through a verification process (click on a link in an email that the site will send) to them for now in our development
* -> Save

## Delete the DNN commment on head tag of HTML source on every pages of DNN websites
* **SETTING** -> **SQL Console** -> Hiện tại không thể Write đc, cần Reload lại -> paste in sequel: `update hostsettings set settingvalue ='N' where settingname = 'Copyright'` (để update _hostsettings_ table; vì bình thương ta query thì nó sẽ ra 'Y', ta cần đổi thành 'N') -> Run Script
* **SETTING** -> **Server** -> `Clear Cache` -> `Restart Application` -> Reload page


----------------------------
_`Site Setting details of DNN 9`_

## Site Info:
* **Site Logo** : browse logo in DNN, Upload, Link
* **Favicon**: image, logo (`.ico` file) -> will show up at the Tab
* -> Save -> Reload page

## Site Behavior