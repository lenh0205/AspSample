# ASP.NET Web Forms 
* **a server-side web application framework** -> create dynamic `web pages`
* It is built on top of the `.NET Framework`
* uses a `page-based` model; each page is a **.aspx** file that contains a `HTML, CSS, C#` code. 
* The C# code in an .aspx file is used to define the page's layout and behavior.

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


