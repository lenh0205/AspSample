# Init
* Visual Studio -> chọn template `Web Application` -> `Web Forms`

# Project structure
* **Site.Master** - **`Master Template`**
* => layout of the page
* => **content placeholder tag** - every active server pages is render through this
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
