# Init
* Visual Studio -> chọn template `Web Application` -> `Web Forms`

================================
# Flow
* Những control sẽ có "Id" 
* ta sẽ dùng giá trị của "Id" để đại diện cho element -> ta có thể access những element này trong "aspx.cs"


================================
# Project structure
* **Site.Master** - **`Master Template`**
* => layout of the page: 
* => **Content placeholder tag** - every active server pages is render through this (_render out the `aspx page` within the `master template` for the `page user is on`_)
```
<asp:ContentPlaceHolder ID="MainContent" runat="server">
</asp:ContentPlaceHolder>
```

* **Default.aspx** - like the `Index Page/ Home Page` of the site
* => **Default.aspx.cs** sẽ là **code behind** của `Default.aspx`
* => **ASP.NET features server controls** could be wired up to events in the `code behind`
* => VD: add a "button" and wired up events -> to redirect to "/Contact" page
```
<asp:Button ID="btnRedirect" runat="server" OnClick="btnRedirect_Click" Text="Redirect to Contact Page" />

// code behind:
protected void btnRedirect_Click(object sender, EventArgs e)
{
    Response.Redirect("~/Contact");
}
``` 
* => Right click on `project` -> View -> View in Browser
* => the `"button"` in HTML page source look like this:
```
<input type="submit" name="ctl00$MainContent$btnRedirect" value="Redirect to Contact Page" id="MainContent_btnRedirect">
```

## Master page
* include server side code, user controls, server controls, allow dynamic content

* first line is **master declaration**
* -> define the page as a **`Master Template`**
* -> also define `code-behind` and **code-behind classname** that the page **inherit** from
* => allow to create `server controls` with `server events` on this pages + interact with **server-side objects** 
```
// server-side code gets "Title" property from "Page" object
// access to "Page" object through inherit from "SiteMaster" in code-behind file
<%@ Master Language="C#" AutoEventWireup="true" CodeBehind="Site.master.cs" Inherits="MySolution.SiteMaster" %>
<>
```

## Form Tag
* **runat="server"** to make Tag a **`server-control`** 
* in Web Forms, <form> is reserved for server interaction

* if we get a third party block of code from a newsletter signup that has a "<form/>" tag in it
* => we will not be able to reliably use it in an active server page
* => issue: a <form/> within a <form/> -> functionality can break on the page 

## "LoginView" server tag - <asp:LoginView>
* this `server control` give a `template` use for users **logged in** and **not logged in**
* -> **`<asp:LoginStatus>`** control inside **<LoggedInTemplate>** has an _`event defined`_ in the _code-behind_ for **OnLoggingOut** (sign out logic) 

## server-side code
```
<%: DateTime.Now.Year %>
```