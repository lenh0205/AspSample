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

## Problem nếu không dùng đúng life cycle
* we have `method` on `Page_Load` that bind data to a drop-down; 
* -> _Nếu_ ta không wrap nó trong `if(postback)` 
* -> _user selection_ will get remove when the form is submitted
* -> _because_ the `Page_Load` comes before `postback event` handling in the page lifecycle

## Life Cycle Page Event Methods
* -> _"Page_Load" is where magic happen_
* -> _theo đúng thứ tự này_
```C#
protected void Page_PreInit(object sender, EventArgs e)
{
    // The Start Stage is complete and Page 
}
```

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