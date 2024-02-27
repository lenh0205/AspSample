> **.Net framework** has several technologies that allow you to create **HTTP services** such as **`Web Service`**, **`WCF`**, and **`Web API`**

=============================================================
# Web Service (SOAP)
* -> used to build **SOAP-based services** (for legacy systems)
* -> is based on **SOAP** and **`returns data in XML form`**; 
* -> it supports only **HTTP protocol**; can be **hosted only on IIS**
* -> is **not open source** but can be **`consumed by any client that understands XML`**

# WCF
* -> for building **general-purpose service framework**
* -> is also based on **SOAP** and **`returns data in XML form`** 
* -> is the **`evolution of the web service(ASMX)`** and supports **various protocols** like **`TCP, HTTP, HTTPS, Named Pipes, and MSMQ`**
* -> it can be host on **IIS**, **self-hosting** or using **WAS** (**`Window as a Service`**)
* -> the **main issue** is **`tedious and extensive configuration`**
* -> it is **not open source** but can be **`consumed by any client that understands XML`**

## WCF Rest
* -> for building **RESTful services over WCF**
* -> to use **`WCF as WCF Rest service`**, we have to enable **webHttpBindings**
* -> it supports **`HTTP GET and POST verbs`** by **[WebGet]** and **[WebInvoke]** attributes respectively
* -> to enable **other HTTP verbs**, we have to do some **`configuration in IIS`** to accept requests of that particular verb on **.svc files**
* -> **`passing data through parameters`** using a **WebGet** needs configuration. 
* -> the **UriTemplate** must be specified
* -> it supports **`XML, JSON, and ATOM data formats`**

## Web API
* -> building **HTTP services (RESTful)**
* -> **`open source framework`** for **`building HTTP services`** easily and simply; an ideal platform for building **RESTful services** over the **.NET Framework**
* -> unlike the `WCF Rest service`, it uses the **full features of HTTP** (like **`URIs`**, **`request/response headers`**, **`caching`**, **`versioning`**, and **`various content formats`**)
* -> it also **supports the MVC features** such as **`routing`**, **`controllers`**, **`action results`**, **`filter`**, **`model binders`**, **`IOC container`** or **`dependency injection`** 
* -> and **unit testing** makes it more simple and robust
* -> it can be host on **IIS** or **self-hosting** - **`hosted within the application or on IIS`**
* -> it is **lightweight architecture** and good for devices that have **`limited bandwidth`** like smartphones
* -> **Responses are formatted** by **`Web API’s MediaTypeFormatter`** into JSON, XML, or whatever format you want to add as a MediaTypeFormatter

=============================================================
# "MVC application" vs "MVC Web API"

## MVC
* -> developing **Web Applications** using **`ASP.NET Web forms`** 
* -> base class is **Controller** - defined in **System.Web.Mvc** assembly
* -> response as **`both views or data`** (other action helper objects); uses **JSONResult** to return **`data in JSON format`**
* -> MVC controllers selects the **`action`** based on the **URI path**

## Web API
* -> creates **`HTTP services`** and **`manages requests via HTTP protocols`**
* -> base class is **ApiController** - defined in the namespace **System.Web.Http**
* -> response **`raw objects`**; supports displaying **data in a variety of forms**, including **`XML and JSON, ATOM, ...`**
* -> features like **content negotiation** and **self-hosting** is **`exclusive`** to Web API
* -> facilitates the development of RESTful services
* -> Web API selects the **`action`** based on the **HTTP verb**

## Combining MVC with Web API 

* When we **self-host an application**
* -> integrate the **`MVC controller and the API`** into **`a single project`**
* -> this facilitates the **management of AJAX requests** and allows for the **return of responses in different format** like XML, JSON, ...

* To enable **application authorization**
* -> **`integrated MVC and Web API`**
* -> **create 2 filters** in it: **`one for MVC`** and **`one for the Web API`**