# ASP.NET Web Forms Page Life Cycle  
* -> broke down into 8 general stages:
* **The Page Request**
* **Start**
* **Page Initialization**
* **Page Load**
* **Page Validation**
* **Postback Event Handling**
* **Rendering**
* **Unload**

* -> trong development thường ta chỉ quan tâm: **`Page Load`**, **`Page Validation`**, **`Postback Event Handling`**
* -> nó quan trọng vì ta cần biết lúc nào thì implement _events_, initialize _controls_ 

## Problem nếu không dùng đúng life cycle
* we have `method` on `Page_Load` that bind data to a drop-down; 
* -> _Nếu_ ta không wrap nó trong `if(postback)` 
* -> _user selection_ will get remove when the form is submitted
* -> _because_ the `Page_Load` comes before `postback event` handling in the page lifecycle

## "Page Request"
* when the **`page is requested by a user`** (_client_ `make request` to _server_) 
* -> the **caching, parsing, compiling** is determined by ASP.NET

## "Start"
* The **page properties** `are set`

* In ASP.NET, **Page** is an **`object`** with _properties_ like **Request**, **IsPostBack**
* -> **Request** property contain `HTTP request data`(_query string param, headers_)
* -> **IsPostBack** property - whether the page has been **`loaded for the first time`** or if the **`user is posting the page`** 

* can _access_ these `properties` from **code-behind** of `.aspx` pages

## "Page Initial"
* **page control** become _available_ and **themes** are _applied_

* But if this is a **postback**, the **`properties of the controls`** will not yet have the data from `postback`
```cs
// A page has a `text input` to enter email address and the form is submitted;
// -> the `textbox` will not yet have the user's email address in the "Text" property of the control
```
## "Page Load"
* _During the Page Load phase_,the **control properties** are set from the `view state`
* => any `user selected values` being `POST` are **`now available`** in the `control properties`

## "Page Validation"
* _During the Page validation phase_, the **Validate()** method is _called_ for all of the `validation control` of the page
* the **validation control properties** are also set (_such as **`IsValid`**_)
 
## Postback Event Handling
* _During the Postback Event Handling stage_, if the user is **`submitting data`** and the page is **`posting back`** to itself, then **events are handled**
```cs
// A "button control" with "onClick" event -> when user click the button:
// The "page life cycle" will go all of the steps before this one
// the "onClick" method will be call during this step 
```

## Rendering
* _During the Rendering phase_, **`view state`** is **saved for the page** 
* -> `view state` is responsible for maintaining the _state of the view_ or the HTML (text input, select values accross postback)
* -> make form sticky => field values don't disappear after submitting the form

* this phase call **Render** method for each _control_ that `renders the HTML to be returned in the response`     

## Unload
* **cleanup** stage
* -> occur `after` the **`response`** is sent to the _client_
* -> **`Page properties`** are _unload_
* -> **`page lifecycle`** is now complete


## Life Cycle Page Event Methods
* -> _"Page_Load" is where magic happen_
* -> _follow the page life cycle theo đúng thứ tự này_

```cs
// Normally, the only life cycle method we get when create a new aspx with a code behind is "Page_Load", "btnSubmit_Click"

protected void Page_PreInit (object sender, EventArgs e)
{
    // The "Start Stage" is complete and "Page properties" have been loaded and we enter the "Initialization Phase"
    // You now have access to properties such as "Page.IsPostBack"
    bool isPostBack = Page.IsPostBack;
}
protected void Page_Init(object sender, EventArgs e)
{
    // All controls and controls properties have now been initialized.
    // I can set control properties, such as the Text Property of a Label control.
    lblInit. Text = "I set this text during Page_Init.";
}
protected void Page_InitComplete (object sender, EventArgs e)
{
    // Everything has been initialized, this event can be used for tasks that require everything to first be initialized.
}
protected void Page_PreLoad(object sender, EventArgs e)
{
    // If you need to perform a task before the page load.
    // We are transitioning from the "Initialization stage" to the "Load stage".
}
protected void Page_Load(object sender, EventArgs e)
{
    // We are now in the load stage.
    // This is where you will perform most of your page related tasks such as
    // data binding to dropdowns and setting text.
    lblPageLoad.Text = "I set this text during the page load and this is where you will usually perform a task"
    
    if (Page.IsPostBack) // lần đầu load page thì ta sẽ không vô đây, chỉ khi submit form (post back)
        lblPost Back. Text = "I set this text when the page posted back"
}
protected void btnSubmit_Click(object sender, EventArgs e)
{
    // We are now in the Post Back Event Handling stage.
    lblButtonEvent.Text = "I set this text when my button OnClick"
}
protected void Page_LoadComplete(object sender, EventArgs e)
{
    // use this method when you need to perform a task after
}
protected void Page_PreRender (object sender, EventArgs e)
{
    // We are now in the rendering Phase.
    // Use this method if you need to modify a control's markup output before it is rendered.
}
protected void Page_Unload (object sender, EventArgs e)
{
    // We are now in the Unload Phase.
    // Use this method if you need to do final cleanup.
    // The lifecycle is complete.
}


<asp:Content ID="Content1" ContentPlaceHolder ID="MainContent" runat="server">
    <h2>Page Life Cycle Events</h2>
    <p>
        <asp: Label ID="lblInit" runat="server" />
    </p>
    <p>
        <asp: Label ID="lblPage Load" runat="server" />
    </p>
    <p>
        <asp:Label ID="1blPostBack" runat="server" />
    </p>
    <p>
        <asp: Label ID="1blButtonEvent" runat="server" />
    </p>
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary btn-large" OnClick="btnSubmit_Click"
</asp: Content>
```