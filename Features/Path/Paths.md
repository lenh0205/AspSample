
# Specifying Paths for Resources
* In many cases, `elements or controls` on your page must **refer to an external resource** such as a file
* Depend on where we working with **a client-side element** or **a Web server control**, ASP.NET supports `various methods` for referencing external resources

# Client Element
* Client Elements are passed through as-is to the browser (not Web server controls on a page)
* => when referring to a resource from a client element, we must **`construct paths according to standard rules for URLs in HTML`**
* => use a **`fully qualified (absolute) URL path`** or various types of **`relative paths`**

## an absolute URL path
* -> if referencing resources in another location, such as an **external Web site**

```html
<img src="https://www.contoso.com/MyApplication/Images/SampleImage.jpg" />
```

## a site-root relative path
* -> if keeping **cross-application resources** (images, client script files, ...) in **`a folder that is located under the Web site root`** 
* -> is resolved against the **site root** (not the application root)

```js
// assumes that an "Images folder" is located under the "Web site root"
<img src="/Images/SampleImage.jpg" />
// -> If your Web site is "https://www.contoso.com", the path would resolve to "https://www.contoso.com/Images/SampleImage.jpg"
```

## A relative path that is resolved against the current page path
```js
<img src="Images/SampleImage.jpg" />
```
## A relative path that is resolved as a peer of the current page path
```js
<img src="../Images/SampleImage.jpg" />
```

# Server Controls
* use **`absolute`** or **`relative paths`**

## Relative paths
* -> they are resolved relative to the path of the page, user control, or theme **`in which the control is contained`**
```cs
// Assume we have a user control in a "Controls" folder
// user control contains an "Image Web server control", with ImageUrl="Images/SampleImage.jpg"
// => When the user control runs, the path will resolve to "/Controls/Images/SampleImage.jpg"

// this is true no matter the location of the page that hosts the user control.
```

## Problem wih "path" and Solution
* **`Absolute paths`** are not portable between applications. If you move the application that the absolute path points to, the links will break.
* **`Relative paths`** in the style of client elements can be difficult to maintain if you move resources or pages to different folders

* Solution is **~** - ASP.NET includes the **`Web application root operator`** 
* -> use when specifying a path in server controls
* -> ASP.NET resolves the `~ operator` to the **root of the current application**
* -> recognized only for server controls and in server code, not for client elements

```cs - specify a root-relative path for an image when using the "Image server control"
//  Assume the image file is read from the "Images" folder that is located directly under the root of the Web application (regardless of where in the Web site the page is located)
<asp:image runat="server" id="Image1" ImageUrl="~/Images/SampleImage.jpg" />
```

# Determining Physical File Paths for the Current Web Site

## Demand
* In our application, you might need to **`determine/supply the path of`** a specific file or resource on the server
* -> For example, we must `supply the file's complete physical path` to _the methods used for reading and writing_ if our application reads or writes a text file programmatically
* -> not a good practice to `hard-code physical file paths` (such as C:\Website\MyApplication) into your application
* -> because the paths can change if we `move or deploy` your application

## Solution
* ASP.NET provides ways to get any physical file path within your application programmatically
* => we can then **`use the base file path to create a full path`** to the resource you need

* -> properties of the **HttpRequest** object that return path information
* -> the **MapPath** method
 
## Security:
* **`Physical file paths should not be sent to the client`** because they could be used by a malicious user to gain information about your application

## Determining the Path from "Request Properties"
* Assumption:
* -> A browser request: `https://www.contoso.com/MyApplication/MyPages/Default.aspx`
* -> physical path for the root of the Web site: `C:\inetpub\wwwroot\MyApplication\`
* -> "virtual path" refers to the portion of the request URL that follows the server identifier (`/MyApplication/MyPages/Default.aspx`)
* -> the physical path contains a folder named `MyPages`

* properties of the **HttpRequest object** help us determine the paths of resources in application
* -> **`ApplicationPath`**: /
* -> **`CurrentExecutionFilePath`**: /MyApplication/MyPages/Default.aspx
* -> **`FilePath`**: /MyApplication/MyPages/Default.aspx
* -> **`Path`**: /MyApplication/MyPages/default.aspx
* -> **`PhysicalApplicationPath`**: C:\inetpub\wwwroot\
* -> **`PhysicalPath`**: C:\inetpub\wwwroot\MyApplication\MyPages\default.aspx

## Determining the Path using the MapPath Method
*  returns the complete physical path for a virtual path that you pass to the method **`HttpServerUtility.MapPath(String)`**
