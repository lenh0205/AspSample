# ASP.NET HTTP handler
* is the **process** (frequently referred to as the **endpoint**) that runs in **`response to a request made to an ASP.NET Web application`**

* The most common handler is an **ASP.NET page handler** that **`processes .aspx files`**
* -> When users request an .aspx file, the `request is processed by the page through the page handler`

* we can create our own HTTP handlers that render custom output to the browser:
* -> to create an **`RSS feed`** for a Web site
* -> a **`Web application to serve images`** in a variety of sizes



========================================================
# An HTTP module
* **an assembly** that is **`called on every request`** that is made to your application. 
* HTTP modules are called as **`part of the ASP.NET request pipeline`** and **`have access to life-cycle events`** throughout the request. 
* HTTP modules let us examine incoming and outgoing requests and take action based on the request

* Typical uses for HTTP modules include: 
* -> examine incoming requests for **`Security`**
* -> gather request **`statistics and log information`** in a centralized module, instead of in individual pages
* -> **`modify the outgoing response`**, we can insert content such as custom header information into every page or XML Web service response