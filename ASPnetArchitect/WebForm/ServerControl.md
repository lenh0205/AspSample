# Basic Server Controls
* 3 types of `server control`: **HTML Server Controls** , **Web Server Controls**, **Validation Server Controls**
* to be `server control`, tag must contain **runat="server"** attribute

## HTML Server Controls
* -> **`<form runat="server">`** and **`Master Template`**
* -> `traditional HTML elements` have `runat="server"`
* -> allows you to interact with HTML element in server-side code 
```C#
// ".aspx" file:
<div class="message">Welcome</div>
// -> chuyển thành HTML server control:
<div runat="server" id="divMessage" class="message">Welcome</div>

// ".aspx.cs" file:
public partial class Contact : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        disMessage.Visible = false; // hide element
        divMessage.Attributes.Add("style", "color:blue;");
    }
}
```
* -> **id** - to access element in the **`code behind`** 

## Web Server Controls
* -> server control: `button, textbox, dropdownlist` (prefix with **<asp:**)
* -> allow to **interact** with and **create events** for these controls
* -> **bind data** to controls for `dynamic content`
```c#
<div>
    <label>Name</label>
    <asp:TextBox ID="txtName" runat="server" />
</div>
<div>
    <label>Emai</label>
    <asp:TextBox ID="txtEmail" CssClass="text-box" runat="server" />
</div>
<div>
    <label>Age</label>
    <asp:TextBox ID="txtAge" runat="server" />
</div>
<div>
    <label>Your favorite color:</label>
    <asp:DropDownList ID="ddlColor" runat="server">
        <asp:ListItem Text="Blue" Value="Blue"/>
        <asp:ListItem Text="Red" Value="Red"/>
        <asp:ListItem Text="Green" Value="Green"/>
        <asp:ListItem Text="Yellow" Value="Yellow"/>
    </asp:DropDownList>
</div>
<div>
    <asp:Button ID="btnSubmit" runat="server" Text="Submit Info" OnClick="btnSubmit_Click" /> // render "input" to client
</div>
<div>
    <asp:Literal ID="ltMessage" runat="server"/> // a placeholder for "output" in "code-behind"
</div>

// .aspx.cs
protected void btnSubmit_Click (object sender, EventArgs e)
{
    // base on what user input create a message:
    string message = string.Format(
        "you said your name is {0} and your age is {1} and your email address is {2}.Your favorite color is {3}",
        txtName.Text,
        txtAge.Text,
        txtEmail.Text,
        ddlColor.SelectedValue
    )

    // output it:
    ltMessage.Text = message;
}
```
* `Adding a class` -> **CssClass** _will become "class" attribute when control is rendered to HTML_

## Validation Server Controls
* -> ability to **validate form fields** 
* -> required `fields, data types, regex matches on fields`
```c#
<div>
    <label>Name</label>
    <asp:TextBox ID="txtName" runat="server" />

    <asp:RequiredFieldValidator 
        runat="server" 
        ID="rfvName" 
        ControlToValidate="txtName" 
        ErrorMessage="*" 
        Display="Dynamic" // tránh message validate nhảy lung tung trên UI
    /> 
    // khi ta blur mà không nhập Name thì nó sẽ hiện thêm dấu "*" trong UI (client-side validation)
    // khi ta ấn "submit" mà không nhập Name thì nó sẽ không cho submit và sẽ hiện thêm dấu "*" trong UI
</div>
<div>
    <label>Emai</label>
    <asp:TextBox ID="txtEmail" CssClass="text-box" runat="server" />

    <asp:RegularExpressionValidator 
        runat="server" 
        ID="revEmail" 
        ControlToValidate="txtEmail" 
        ErrorMessage="Valid email address is required" 
        ValidationExpression="^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"
        Display="Dynamic"
    /> 
    // khi ta blur ra ngoài mà không nhập đúng pattern của "Email" thì nó sẽ hiện message "Valid email address is required" trong UI (client-side validation)
</div>

<asp:DropDownList ID="ddlColor" runat="server">
    <asp:ListItem Text="Please choose a color" Value=""/>
    <asp:ListItem Text="Blue" Value="Blue"/>
    <asp:ListItem Text="Red" Value="Red"/>
    <asp:ListItem Text="Green" Value="Green"/>
    <asp:ListItem Text="Yellow" Value="Yellow"/>

    <asp:RequiredFieldValidator 
        runat="server" 
        ID="rfvColor"
        ControlToValidate="ddlColor"
        ErrorMessage="Color Required"
    />
</asp:DropDownList>
```

# Other Advance server controls
* provide advanced `functionality` or display of **dynamic data**

## Calendar control
* display `a calendar` on a page and allow user to select a date
* **<asp:Calendar/>**
```c#
<div class="form-group">
    <label>Event date:</label>
    <asp:Calendar ID="calendarEvents" runat="server" />
</div>
```

## Repeater control
* **<asp:Repeater />**

* we can have
* -> **`a header template`** - display once at the top
* -> **`a footer template`**` - display once at the bottom
* -> **`an alternating item template`** - ability to display every other row displayed differently
* -> **`an item template`** - display once for every items that we bind to repeater
* in a **separator template**

* VD: ta có 1 cái app tạo "event"; have control that will repeat out the events that we submit on the page
; mỗi khi user nhập "event name", "event date" sau đó nhấn "Add Event" button -> ta sẽ add "MyEvent" object to a list -> data bind the full list to repeater
```c#
// MyPage.aspx:
<div>
    <asp:Repeater ID="rptEvents" runat="server">
        <ItemTemplate>
            <h3>
                <%# DataBinder.Eval(Container.DataItem, "EventDate")%>
                <small>
                    <%# DataBinder.Eval(Container.DataItem, "EventName")%>
                </small>
            </h3>
        </ItemTemplate>
    </asp:Repeater>
</div>

// MyPage.aspx.cs:
public partial class MyPage : System.Web.UI.Page
{
    private List<MyEvent> Events;

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
            Session["MyEvents"] = new List<> // Initial "MyEvents" in the session for it to be not null
    }
    protected void btnEvent_Click(object sender, EventArgs e) 
    {
        UpdateEvents();
        BindEvents()
    }
    private void UpdateEvents()
    {
        if(Session["MyEvents"] != null) 
            myEvents = (List<MyEvent>) Session["MyEvents"];
        else 
            myEvents = new List<MyEvent>();
        
        myEvents.Add(new MyEvent(txtEvent.Text, calendarEvents.SelectedDate));

        Session["MyEvents"] = MyEvents;
    }
}

public class MyEvent
{
    public MyEvent(string eventName,  DateTime eventDate)
    {
        EventName = eventName;
        EventDate = eventDate.ToShortDateString(); // for display date, but not time
    }
    public string EventName
    {
        get;
        private set;
    }
    public string EventDate
    {
        get;
        private set;
    }

    private void BindEvents()
    {
        rptEvents.DataSource = myEvents;
        rptEvents.DataBind();
    }
}
```

=======================
# Web Server Controls Event
* 