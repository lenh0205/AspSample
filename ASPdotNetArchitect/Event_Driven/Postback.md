## Rendering
* **Postback** - **Server-side rendering** to update the whole page
* **AJAX** - **Client-side rendering** to update the portion of page

* Bản chất **Server-side rendering** sẽ tự lấy data từ database -> bind vào View -> trả về client full HTML
* Có thể sử dụng **Postback** để control state cho các controls;
* Ta chỉ nên s/d **AJAX** (**Client-side rendering**) đổi với 1 số công việc cụ thể (_mặc dù nó có thể làm gần như hết mọi việc_) như: **`Call API`** - để lấy phần HTML cần được update trả về từ server, ...

## PostBack
* hỗ trợ cho viewsate, đảm bảo state không bị mất đi sau khi submit form

* ASP.NET (MVC, Web Form, ...) uses the "PostBack" event
* ASP.NET Core don't use "Postback" mechanism

```c# - ".aspx" file
<script src="updatepanel.js" type="text/javascript"></script>
// ensure that the JavaScript code is executed when the page is loaded

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button ID="Button1" runat="server" Text="Update" />
            <asp:Label ID="Label1" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

//  Because of "UpdatePanel", the entire page will not be reloaded when the "Button1" button is clicked
// Instead, only the "Label1" control will be updated
```

## UpdatePanel
* The **UpdatePanel** control is a `server control`
* -> allows us to mark regions of a page as eligible for **`partial updates`**
* -> _when postback event is triggered_, `UpdatePanel control` intervenes to initiate the **`postback asynchronously`**, update just that _portion of the page_

* _`UpdateMode` attribute specifies whether the UpdatePanel should be updated asynchronously or synchronously_

```c# - ".aspx.cs" file
protected void Page_Load(object sender, EventArgs e)
{ 
    if (IsPostBack)
    {
        Label1.Text = "This is the new text for the label control.";
    }
}
protected void Button1_Click(object sender, EventArgs e)
{
    Label1.Text = "This is the new text for the label control.";
}

// "Label1" control can be updated in both of these situations:
// -> "Page_Load" method is executed every time the page is loaded, includes initial page load and subsequent postbacks
// -> "Button1_Click" method is executed when the "Button1 button" is clicked.
```

## AJAX
* execute on the client
* AJAX uses the DOM to update a partial page
* 

```js - "updatepanel.js" file
$(document).ready(function() {
    var updatePanel = document.getElementById("UpdatePanel1");

    updatePanel.addEventListener("updateCompleted", function() {
        // using the "updateCompleted" event 
        // Update the "Label1 control" with the "HTML" that is returned from the server:
        $("#Label1").html(updatePanel.innerHTML);
    });

    $("#Button1").click(function() {
        updatePanel.postBack(); // sing the "click" event to trigger a "postback" event
    });
});
```

