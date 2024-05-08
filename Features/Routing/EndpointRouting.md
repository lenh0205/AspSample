> https://aregcode.com/blog/2019/dotnetcore-understanding-aspnet-endpoint-routing/

# "Endpoint Routing" feature of ASP.NET Core 3.0

## Prior to "Endpoint Routing" - the ASP.NET Core MVC middleware
* -> before the **`routing resolution for an ASP.NET Core application`** was done in the **`ASP.NET Core MVC middleware`** **at the end of the HTTP request processing pipeline**

* -> means that **route information** (_Ex: controller, action, ... would be executed_), **was not available to middleware** that **`processed the request before the MVC middleware`**
* -> it's _particularly useful to have this route information_ available in a **`CORS`** or **`authorization`** middleware to use the information as a factor in the authorization process 

* -> _Endpoint routing_ also allows to **decouple the route matching logic** from the **`MVC middleware`** and have its **`own middleware`**
* -> it allows the **MVC middleware** to focus on its responsibility of **`dispatching the request to the particular controller action method`** that is **`resolved`** by the **endpoint routing middleware**

## The Endpoint Routing middleware
* -> allow **the route resolution to happen earlier** in the pipeline in **`a separate endpoint routing middleware`**
* => this middleware can be placed at any point in the pipeline, after which **other middleware in the pipeline can access the resolved route data**

=================================================================
# Main concepts of "Enpoint Routing"

## Endpoint route resolution
* -> the concept of **looking at the incoming request** and **mapping the request to an endpoint** using **`route mappings`**
* -> **`an endpoint`** represents the **controller action** that **`the incoming request resolves to`**, along with **other metadata** **`attached to the route`** that matches the request 

* -> _the job of the route resolution middleware_ is to **construct "Endpoint" object** using the **`route information`** from **`the route that it resolves based on the route mappings`**
* -> the middleware then **places "Endpoint" object into the http context** 
* => where _other middleware the `come after the endpoint routing middleware` in the pipeline_ **can access the Endpoint object** and **use the route information within**

* -> Prior to endpoint routing, route resolution was done in the MVC middleware at the end of the middleware pipeline. The current 2.2 version of the framework adds a new endpoint route resolution middleware that can be placed at any point in the pipeline, but keeps the endpoint dispatch in the MVC middleware. This will change in the 3.0 version where the endpoint dispatch will happen in a separate endpoint dispatch middleware that will replace the MVC middleware

## Endpoint dispatch

## Endpoint route mapping