=====================================================================
# API Gateway

## Problem
* -> for each of microservices we will have different endpoint
* -> when accessing these services from external calls, it doesn't make sense exposing multiple URL
* => we should have **`single entry point`** to all our service and then base on the route it will be routing these to different endpoints

* -> this can be done by **`traditional Load Balancer`**, but the feature of traditional load balancer is very limited to what it can do
* -> but **`API Gateway`** is way beyond just the **routing**

## Most common features
* -> Routing
* -> Request Aggregation
* -> Authentication and Authorization
* -> Rate Limiting
* -> Caching
* -> Load Balancing

## Ocelot
* -> is **`a ASP.NET Core API Gateway`** - **a Nuget package** that can be added to any ASP.NET Core application to **make it a API Gateway** 

=====================================================================
# Example
* -> ta sẽ có 1 project ASP.NET Core Web API application có 1 endpoint là "http://localhost:5001/weatherforecast"

* -> h ta sẽ tạo thêm 1 project làm API Gateway để route đến endpoint đó
* -> tạo project với template là ASP.NET Core Web API 
* -> cài **Ocelot** Nuget package

```cs - program.cs
builder.Services.AddOcelot();
app.UseOcelot().Wait();
```

* -> (most important) add the **`ocelot.json`** file sẽ cần **specifies where to route to** and **global configuration**

* -> create dev version **ocelot.dev.json** (lưu ý ta cần vào project properties -> Debug -> đổi giá trị của **ASPNETCORE_ENVIROMENT** thành **dev**)