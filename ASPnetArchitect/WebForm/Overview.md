# ASP.NET Web Forms 
* It is built on top of the **.NET Framework**

* **a server-side web application framework** -> create dynamic `web pages`
* -> `Event Driven` pages
* -> with `server code, server controls, server events`

* utilize **Master Templates** - a centralized file
* => manage `header, footer, body` of site **all in one place**
* => `every active server page` is we create render `within master template` through a **single content placeholder tag**

* **Active server page**
* => utilize a **code behind** approach
* => provides a `clean break` of the **presentation markup** from the **page logic**
* => `presentation code` is the **aspx page**; `code behind` is an **aspx.cs**

* also feature **controls**
* => which are **object** on the `ASP.NET pages`
* => `render HTML` to the browser when the page is `requested`
* => textbox, button, drop-down list, ... -> can be wired up to an **event in the code behind**
* **runat="server"** is **`required`** for every server control

* **User Controls**
* => we create our own, `reusable controls` that can be used on mutilple pages

## Mechanism
* -> a user `requests` an ASP.NET Web Forms page 
* -> the `.aspx` file is compiled into an `assembly`
* -> executed by the .NET Framework
* -> The .NET Framework then creates an `instance` of the `page class` and calls the page's **Init, Load, Render methods**
* => **Init** method is called first and is used to initialize the `page's properties`
* => **Load** method is called next and is used to load the `page's data`. 
* => **Render** method is called last and is used to render the `page's HTML output`

## Ability
* includes a number of **built-in controls** (_buttons, text boxes, labels, and images_) to create your web pages. 
* create own `custom controls`
* is a server-side technology -> requires a web server to run -> if looking for a client-side technology, consider using ASP.NET MVC


