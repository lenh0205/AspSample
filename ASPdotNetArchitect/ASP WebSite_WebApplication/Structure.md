> _https://learn.microsoft.com/en-us/previous-versions/aspnet/2wawkw1c(v=vs.100)_

# Default Pages
* config to server when users navigate to our site **`without specifying a particular page`**
* For example, create a **Default.aspx** page in site's root folder
* usually use as **`home page`** for site or a page contain code **`redirect users to other pages`** 

# Application Folders
* **`certain folder`** for specific types of content (`ASP.NET recognizes it`)
* not served in response to Web requests; but to be **`accessed from application code`**

* **App_Browsers** - contain `.browser files` (**browser definitions**)
* **App_Code** - contains **source code** for utility classes and business objects (.cs, .vb, .jsl) to compile as part of your application
* **App_Data** - contains **application data** files including MDF files, XML files, as well as other data store files
* **App_GlobalResources** - contains **resources** (.resx, .resources files) that are compiled into **`assemblies with global scope`**
* **App_LocalResources** - contains **resources** (.resx and .resources files) that are associated with **`a specific`** `page, user control, or master page` in an application
* **App_Themes** - a collection of files (**.skin, .css files**, as well as `image files and generic resources`) that define the **appearance** of ASP.NET Web pages and controls
* **App_WebReferences** - contains **`reference contract files`** (.wsdl files), **`schemas`** (.xsd files), and **`discovery document`** files (.disco and .discomap files) defining a Web reference for use in an application
* **Bin** - contains **compiled assemblies** (.dll files) for controls, components, or other code; classes represented by code are **automatically referenced** in our application

# Web.config
* to **manage configuration settings** for our site
* located in the site's **`root folder`** -> but we can **`creating a Web.config file in subfolder`** to maintain separate configuration settings for files in that folder
* we can config settings to **`restrict access to site content`** (individual files or subfolders) by individuals or by roles

# Web Site File Types
* Web site applications can contain file types supported and managed by **ASP.NET** or **IIS server**

## Managed by ASP.NET
* **.asax** - typically a **Global.asax** file represents the application and contains optional methods that **`run at the start or end of the application lifetime`**
* **.ascx** - a Web **`user control`** file that defines a **custom, reusable control**
* **.ashx** - a **generic handler** file, contains code that implements the IHttpHandler interface
* **.asmx** - an **XML Web services** file that **`contains classes and methods`** that are available to other Web applications by way of **`SOAP`**
* **.aspx** - an **ASP.NET Web forms file (page)**
* **.axd** - a `handler file` used to manage Web site **administration requests**, typically **`Trace.axd`**
* **.browser** (App_Browsers) - a browser definition file used to identify the **features of client browsers**
* **.compile** (Bin) - a **precompiled stub file** that **`points to an assembly`** representing a compiled Web site file. (.aspx, ascx, ...are precompiled and put in the Bin subdirectory) 

## Managed by IIS

## Static File Types