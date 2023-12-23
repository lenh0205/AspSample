
* **IIS Physical Path** - **`actual path`** the file is located by IIS
* **IIS Virtual Path** - **`logical path`** to access the file which is pointed to **from outside of the IIS application folder**

* VD: display image from "E:\images" using a virtual directory in IIS Default web site
* -> **`Physical Path`**: E:\Files
* -> **`Virtual Path`**: http://localhost/Files/
* -> File Name: image_60140ec0-ce46-4dbf-a14f-4210eab7f42c.png
* -> Full Path: http://localhost/Files/image_60140ec0-ce46-4dbf-a14f-4210eab7f42c.png

* -> By default, **Directory Browsing is disabled** for **`security`** reason (we can enable it)
* -> now we can load the file which is out of IIS application folde

===============================================
> https://www.codeproject.com/Articles/142013/There-is-something-about-Paths-for-Asp-net-beginne 

# Overview:

* **common problem**:
* using different **`HTML elements`** or **`ASP.NET server controls`** along with setting different attributes (**`href, src, ...`**) values
* -> the images were appearing in one page and not appearing in another 
* -> the download links were braking often

* the **path related issues** for `HTML elements` and `server controls` are not the same

# Path

# How browser loads elements containing "path" attributes
* -> HTML markups are generated from the server for a page
* -> **browser loads the HTML first** and render immediately
* -> right then, it starts **loading the elements in the HTML markup that have "path" attributes** (such as, **`<img>, <a> <link> <script>, ...`**)
* ->  for each of those elements, browser **`sends an asynchronous request`** again to the server with the URL provided in their **`"path" attributes`** (src or href)
```r
// Async request for each image -> Render each image once loaded success
```

> _`Asynchronous request means browser sends the requests behind the scene, and doesn’t wait for the response from server while doing other activity`_
> _`the UI remains responsive to the user, when the response is received from the server, the UI is updated with the response data`_

# Absolute Path
* _Assuming that, the resources resides within the web application’s root folder structure_
* we can of course `set absolute URLs for all images and resources in our application`
* -> but in case **the site URL is changed** later for some reason, **`all of the src attribute values will needed to be changed`**

* => Absolute URLs are only used if the **`resources are kept in different server for optimizing performance`**, those servers are called **CDN - Content Delivery Network**

# basic Path calculate rule of Browser
* resources (images,...) are **mostly kept within the web application's root folder** in _a designated directory_
* -> we **usually provide a relative path of resource** as the "src" (_or href_) attribute so that **`browser can calculate the absolute image URL`** 

```r - VD
// in page "http://www.mysite.com/default.aspx" contain "<img src ='/images/action.jpg'>"
// -> we can guess: there is an "images" folder contain an "action.jpg" file within the web application’s root folder 
// -> while encountering this element, browser will try to determine an absolute URL: "http://www.mysite.com/images/action.jpg"
```
* -> Browser'll use the absolute path that it's just calculated to send an asynchronous request 
* -> If the **`web server finds the image at this location`**, it will **`read and send the image in the response`** 
* -> then browser will shows it in the output

# Problem: Pages are not implemented directly within the Web Application's root folder 
*  its pretty common to implement pages **`under some folder structure`** (_out of Web Application's root folder_)

```r - VD: 
// The currently browsed page is : "http://www.mysite.com/pages/default.aspx" that contain "<img src ='images/action.jpg'>"
```
* => this will **fail to load the image and show in the browser**
```r
// The browser calculates the absolute image path as "http://www.mysite.com/pages/images/action.jpg"
// while the correct image URL should be "http://www.mysite.com/images/action.jpg"
```

# Solution:
* there're 2 approach to fix this:
* -> use a relative path based upon **`the current directory of current page`** 
* -> use a relative path based upon **`the site’s base URL`**

## Relative path based on current directory
```r - VD:
// Current directory of your page is: http://www.mysite.com/pages/
// directory of requested image is: http://www.mysite.com/images/
```
* we can use **../** to **`go one level up`** from the "pages" directory and then `add the relative path of the image` (_/images/action.jpg_)
```r 
// the "<img src ='../images/action.jpg'>" in the "/pages/default.aspx"
// <=> http://www.mysite.com/images/action.jpg
```

## Relative path based on Site’s base URL:
* use an image path **/** that would let the browser calculate the absolute url based upon the web site's base URL address
* -> **`no matter in what folder structure your page is under the web root folder`**, the absolute path will **relative to the root directory**

```r
// the "<img src ='/images/action.jpg'>" in the "/pages/default.aspx"
// <=> http://www.mysite.com/images/action.jpg
```

* **Caution**
* -> should be used only if the web application is **deployed under a web site, rather than a virtual directly** in the IIS
* -> the reason is **`browser`** calculates the full URL path **`based upon the web site's root URL not the virtual directory's root url`**, than plus it to the _relative url_
* _A Virtual directory is a directory/path name that is appended after the web site’s base URL, pointing to a web application's root folder_
* _There could be `multiple virtual directories` deployed under a site, each pointing to a different Asp.net application code base_

```r - issue when A web application deployed under a "virtual directory"
// url as follow: http://www.mysite.com/app/pages/default.aspx; "app" is the virtual directory here

// so the '<img src ="images/action.jpg">' <=> absolute URL: "http://www.mysite.com/images/action.jpg"
// request fail because correct URL is "http://www.mysite.com/app/images/action.jpg"
```

=================================================
# Path related issues with Asp.net Server controls
* Almost every HTML element has **a corresponding Asp.net Server control**

* A corresponding `Asp.net Server control` of "HTML <img> element" is: <asp:image id="Image1" ImageUrl="~/image/action.png" runat="server"/>
// -> renders an <img> HTML element in the browser
// -> **`ImageUrl`** property value is set to the **`src`** property value of the <img> element

# What is "~"
* -> is usually used to `set URL paths for Asp.net Server controls`
* -> **`instructs the Asp.net run time`** to **resolve the relative path** of the server control

```r - VD 1:
// a web page at the following URL: "http://www.mysite.com/pages/default.aspx" contain:
// an "</asp:Image ImageUrl="~/image/action.png" id="Image1" runat="server"/>"
```
* _when the  page is requested by the browser_, the Asp.net runtime will **replace the "~"** with the **`relative path`** 
* -> that navigates the browser **from the current folder to the web application's root folder** (_`http://www.mysite.com/pages/` to `http://www.mysite.com/`_)
* -> that is one directory up **..** to build the correct relative URL of the img element
* => hence, the full relative URL be resolved as `"~/image/action.png"` is **image/action.jpg**

```r - VD 2:
// a web page at the following URL: "http://www.mysite.com/default.aspx" contains 
// an "<asp:Image ImageUrl="~/image/action.png" id="Image1" runat="server"> "
```
* _when this page is requested by the browser_, the Asp.net runtime will replace the "~" with the relative path 
* -> that navigates the browser from the current directory to the web application's root directory (_`http://www.mysite.com/` to `http://www.mysite.com/`_)
* -> That is with a blank **_** as both current directory paths are same now
* => full relative URL be resolved as `~/image/action.png` into `/image/action.jpg`

## issue of User Control (.ascx) 
* _Assumpt our Asp.net web application has an organized folder structure for aspx pages and user controls (ascx), for laying out them in a logical structure within the root folder_
```r 
// we have 2 page located at "/Pages/Products/ProductDetails.aspx" and "/Pages/Home.aspx" use the same user control
// the user control is located at "/UserControls/Products/UCProductDetails.ascx"
// the user control has an HTML img element "<img src="">"
```

* when the page is requested from the browser, the Asp.net runtime **loads the page along with the user control**, executes it and sends the HTML output to the browser
* _location of user control is not important; the important is **`location of the page`**_
* -> at browser’s end, there is **`only HTML markup`**
* -> the browser will **`simply try to determine the relative path`** of the image based upon the current location of the page

```r - làm sao để 1 path thoã mãn nhiều page
// if the currently browsed page is "http://www.mysite.com/Pages/Products/ProductDetails.aspx"
// => the "src" of <img> element should be: "../../images/details.jpg"
// -> Because browser has to navigate two folder up, then search the image inside “images” folder

// But, if the currently browsed page is "http://www.mysite.com/Pages/Home.aspx"
// The "src" value of the <img> element should be: "../images/details.jpg"
// Because, browser has to navigate one folder up to access the image within the “images” folder
```

* To solve this, there're 2 solution:
* -> **replace the HTML <img> element with an Server control** and set the **`ImageUrl`** property that starts with **~** : `ImageUrl = "~/images/details.jpg"`
* -> turn the **<img> element to a server control** as follows: <img runat="server" id="imgDetails" src="~/images/details.jpg" />

* => **Asp.net application will be able to calculate the appropriate relative directory at the server-end**
* -> based upon the **`location of the currently requested page`**
* -> **no matter where the page or user control is** within the folder structure of the web application

## Problem with other elements
* **Not all HTML elements have a corresponding server control in Asp.net**
* -> for these kinds of elements, adding the **`runat="server"`** attribute and set the path starting with **`~`**  will not work
* -> because, adding the runat="server" converts these elements as `HtmlGenericControl` at the server-end, 
* -> and this type of object **`doesn't have anything to resolve the "~" operator`** to determine the correct relative URL

* For these kinds of elements, **Page.ResolveClientUrl() is the perfect solution**

```js
// a user control has the following HTML elements:
<script src="../js/common.js" type="text/javascript" language="javascript"/>
<link rel="Stylesheet" href="../style/common.css" type="text/css" />
// -> If paths properties are set for one page appropriately, the same path will not work other pages in different directory

// we should modified with the ResolveClientUrl():
<script src='<%=ResolveClientUrl("~/js/common.js")%>' type="text/javascript" language="javascript"/>
<link rel="Stylesheet" href='<%=ResolveClientUrl("~/style/common.css")%>' type="text/css" /> 
```

## Path related issues in the Code behind and back-end - ResolveUrl()
* _Now, we're able to set path/urls at the XHTML markups if the paths/urls are well known in design time_
* **Problem**:  if for any reason if **`the path/url is not known in advance`** (_VD: the path/url will vary based upon some conditions_) or simply if you are more interested to **`set the paths/urls in the back-end`**

* **Solution**: 
* -> At the most basic level, each **Server control has a ResolveUrl()** method (_inherited from the base `Control` class_), 
* -> then supply a **root relative** path that starts with the **~** to get the correct relative path
```cs
// if there's a Server control:
<asp:Image ID="Image1" runat="server" />

// we can set the ImageUrl property in the CodeBehind as follows:
Image1.ImageUrl = Image1.ResolveUrl("~/images/action.jpg");
// Or simply:
Image1.ImageUrl = “~/images/action.jpg”;
```

* the HTML markup is rendered for **`setting the ImageUrl in the markup`** and for **`setting ImageUrl using Image1.ResolveUrl("~/images/action.jpg"); in the CodeBehind`** is difference
* -> First one is **relative in terms of current page location within /Pages/**
* -> Second one is **relative in terms of web application's root folder**
* => _the request image will be rendered from correct location in the browser_
```cs
// HTML generated for setting ImageUrl at Markup:
<img style="border-width: 0px;" src="../images/action.jpg" id="Image1">

// HTML Markup generated for setting ImageUrl at CodeBehind:
<img style="border-width: 0px;" src="/WebSite3/images/action.jpg" id="Image1">
```

* As a **Page or a User control** is also a control, we can also call the ResolveUrl() in the following way also:
```cs
Image1.ImageUrl = this.ResolveUrl("~/images/action.jpg");
// or simply
Image1.ImageUrl = ResolveUrl("~/images/action.jpg");
```

# VirtualPathUtility.ToAbsolute(string path)
* **Problem**: what if you have to `resolve a relative path` from within the **`HttpHandler`** or a **`HttpModule`** or even from within a **`class library`**
* -> _won’t get the Control objects within the Httphandler or HttpModule_
* -> _so we can’t call the ResolveUrl() method_

* **Solution**: there is an easy way, the **VirtualPathUtility** has many other utility methods also that `aid in calculation of various path related logic`
* -> the following method that accepts the same parameter argument (Like ResolveUrl()): **`VirtualPathUtility.ToAbsolute("~/images/action.jpg")`**

# Getting the physical file paths from relative URLs
* _determining the physical location of a file, based upon a relative URL path is a pretty common need_

* Solution: **Server.MapPath()**
* -> when we _call the Server.MapPath() with a relative path/URL_, it returns the **`complete physical location of the file that is stored within the web application folder`**

* Validation:
* -> the physical path of the file/folder that is returned by the Server.MapPath() `does not guarantee that the file or folder exists there physically`
* -> It just maps the supplied relative path to a physical path location based upon the web application’s root directory
* to ensure that the physical location of the file or folder actually exists: Using **System.IO.Path.Exists(FilePath)**

```cs
// our web application’s root folder location is: return "C:\applications\aspnet\www.mysite.com\"
string RootPath = Server.MapPath("~"); // returns "C:\applications\aspnet\www.mysite.com\"
string FilePath = Server.MapPath("~/images/search.jpg") // return "C:\applications\aspnet\www.mysite.com\images\search.jpg"
```

# Path properties in Request object
* There are some **`important Path properties`** in the **Request object** that are kind of **Must know** for Asp.net geeks

## Request.ApplicationPath
* This returns the **`Application’s path relative to the Site’s path`**

* the **Application** can be a `virtual directory` or the `site` itself. 
* -> if the current application is deployed under **a site**; VD: `for a URL http://www.mysite.com/home.aspx, this would simply return "/"`
* -> if the current application is deployed under a **virtual directory**; VD: `for a URL http://www.mysite.com/account/home.aspx (Where "account" is the virtual directory name) this would return the path "/account"`

* => this property is handy if you need to calculate a relative URL of any resource within the application, if you know where in the web application this resource exists.

## Request.FilePath
* returns the **`relative or virtual path of the current request`**, in terms of the site’s URL. * VD: `for a URL http://www.mysite.com/account/home.aspx, this would return /account/home.aspx`
* It doesn’t matter whether the application is a virtual directory or a site.

## Request.CurrentExecutionFilePath
* _the same as `Request.FilePath`_, 
* -> except that, it returns the path even if the current page is executing as a result of invoking Server.Execute() or Server.Transfer().

```r - VD: 
// You hit a URL "http://www.mysite.com/pages/home.aspx"
// and called "Server.Execute"(~/pages/common/CheckStatus.aspx) or "Server.Transfer"(~/pages/common/CheckStatus.aspx) within the "Home.aspx"

// => Now, the "Request.CurrentExecutionFilePath" within the "CheckStatus.aspx" would return "/pages/common/checkstatus.aspx" (The page that is being executed from the Home.aspx)
// "Request.FilePath" within the "CheckStatus.aspx" would return "/Pages/Home.aspx" (The original page that executed the Server.Execute() or Server.Transfer())
``` 

## Request.Path
* This returns the **`Request.FilePath`** including any **`query string parameters`** if present.

## Request.PhysicalApplicationPath
* returns the **`web application’s physical folder location`** (_no matter whether it is a Site or Virtual directory_)

* For example, `C:\applications\aspnet\www.mysite.com\`

## Request.PhysicalPath
* returns the **`physical path of the currently requested file in the URL`**
* For example, `C:\applications\aspnet\www.mysite.com\home.aspx`