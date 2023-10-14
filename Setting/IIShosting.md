========================================================
# IIS
* **`a Windows Service`**
* allows to **`host(or store), manage, test`** _websites / web application_ **on Windows Systems**

* => IIS **does all** the processing and managing of our websites (_PHP websites, static websites, asp.net websites, java websites_)

* the same function as **`Apache Server does in Linux`**

=========================================================
# Mechanism of hosting an ASP.NET Core application on IIS
## Overview
* `the IIS server` acts as a **`reverse proxy`** that forwards incoming requests to the ASP.NET Core application. 
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


==============================
# Hosting in deployment
* Trong **Client-side rendering**, **Frontend** communicate **Backend** throght `API`
* **Browser** 
* -> query `data json` from **Backend**
* -> query `html/css/js` from **Frontend**
* => hiển thị ra trang web cho user

* Khi development, tất cả thành phần **Frontend**, **Backend**, **Browser** đều nằm trên máy local của ta

* Khi deploy **Backend**, **Frontend** lên cùng 1 server; `Frontend` không thể gọi `Backend` thông qua **locahost**
* -> `Browser` chỉ nhận `html/css/js` từ `Frontend`
* -> khi `Browser` gọi `localhost` nó sẽ hiểu là `localhost` trên máy **client**