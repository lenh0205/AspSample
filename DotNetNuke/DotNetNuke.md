# https://docs.dnncommunity.org/index.html

# Install DotNetNuke in a local development enviroment on a virtual machine :
## Setup Folder 
* Lên trang chủ tải file zip **`DNN_Platform_Install.zip`** (`_Source.zip` chỉ dùng để develop DNN Platform)
* Vào _properties_ của file zip -> check **Unblock** (trust file - allow to extract all content of the file)
* giải nén thành 1 Folder 
* bỏ content của folder đó vào `C:\Websites\dnndev.me` (domain name owned by DNNsoftware with DNS point to **`127.0.0.1`**`) (_tức là nếu access cái domain này thì nó sẽ redirect đến local IP address)
* **Add User, permission** used by Application pool/Web Server (`NETWORK SERVICE`): right click at folder -> properties -> Security

## Setup IIS
* Add Website vào **Sites**
* Turn on những **`Window Feature`** cần thiết
* Physical path là `C:\Websites\dnndev.me`
* hostname là **`dnndev.me`** , vào **Rules -> Request Filtering** để sửa lại dung lượng file có thể upload (để upload Extension)
* Setup SQL Server: cần SQL Server Authentication và tạo sẵn 1 Database

## DNN Wizard Installation
* install `local instance`` of DNN9

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


# Create Module for DotNetNuke
## Tích hợp DotNetNuke Extension Development `Templates` cho Visual Studio 
* -> Tìm Git repository hỗ trợ  
* -> chọn templates phù hợp với phiên bản DotNetNuke và Visual Studio đang dùng
* -> Tải file **`DNNTemplates.vsix`** về thư mục Download của máy
* -> chạy Setup Wizard của nó (nên tắt hết các ứng dụng chạy ngầm trước khi chạy)

## Thêm Host vào máy:
* Try cập **C:\Windows\System32\drivers\etc\hosts**
* Thêm dòng **`127.0.0.1 dnndev.me`** vào cuối file
* Command Prompt: `ipconfig /flushdns` to clear the DNS cache

## Khởi tạo Project MVC Module for DotNetNuke:
* Ta cần chạy trang DotNetNuke ta host trên IIS trước
* Sử dụng Visual Studio với quyền **`Administrator`** (to access the IIS configuration file) -> chọn Template
* chọn **Local Dev URL** là **`www.dnndev.me`**

## Develop DotNetNuke MVC Module:
* add controllers, views, and models
* 

## use Module for DotNetNuke page:
* Build Release -> Publish app -> zip it -> ném vào phần “Install Extension” của trang DotNetNuke
* 