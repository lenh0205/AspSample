# hosting an ASP.NET Core application on IIS

## Overview
* `the IIS server` acts as a _`reverse proxy`_ that forwards incoming requests to the ASP.NET Core application. 
* The `application` is hosted using the **Kestrel web server**, which is included with `the .NET Core runtime`

## IIS
* -> **IIS** receives a request for your application,

* -> it first checks if the request can be handled by one of its `native modules` 
* -> If not, the request is forwarded to the **ASP.NET Core Module** - a native IIS module that handles communication between IIS and the Kestrel server

* -> after finish, **IIS** then sends the response back to the client

## ASP.NET Core Module
* The **ASP.NET Core Module** reads the **web.config** file in our `published application’s root directory` to determine:
* -> how to start the `Kestrel server` 
* -> which arguments to pass to it
* -> **web.config** file also contains `other settings` that control how your application behaves when hosted on IIS

* The **ASP.NET Core Module**:
* -> forwards requests from IIS to the **Kestrel server**
* -> then processes the request and sends a response back to IIS

## Kestrel
* Once the **Kestrel server** is started, it listens for `incoming requests` on a `specific port`


