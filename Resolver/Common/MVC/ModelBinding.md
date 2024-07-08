# Model in ASP.NET MVC
* -> represents the **`shape of the domain-specific data as public properties`** and **`business logic as methods`** in MVC application
* -> the model class can be **`used in the view to populate the data`**, as well as **`sending data to the controller`**

* _usually, we put these model in `~/Model` folder_

# Views
* -> a view is used to **display data using the `model` class object**
* -> derived from **WebViewPage** class included in **`System.Web.Mvc`** namespace
* -> these view are contained inside **Views** folder 

* -> the MVC framework requires **`a separate sub-folder for each controller with the same name as a controller, under the "Views" folder`**
* -> it's good practice to **`keep the view name the same as the action method name`** so that you don't have to **explicitly specify the view name in the action method** while returning the view

* -> the **Shared** folder contains **`views, layout views, and partial views`** - which will be **shared among multiple controllers**

# Razor View Engine
* -> to **compile a view with a mix of HTML tags and server-side code (C#)** - maximizes the speed of writing code
* -> the razor view uses **@** character to **`include the server-side code`** instead of the traditional **<% %>** of ASP
* -> the **`C# Razor view`** is **.cshtml** file

# Integrate Model, View, Controller
* -> the **View()** method is **`defined in the BaseController class`**, which **automatically binds a model object to a view**
