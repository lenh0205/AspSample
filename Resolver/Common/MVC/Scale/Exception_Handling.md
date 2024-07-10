====================================================================
# Exception Handling in ASP.NET MVC

## Problem
* -> we may **`handle all possible exceptions`** in the action methods using **try-catch blocks**
* -> however, there can be **some unhandled exceptions** that we want to **log** and **display custom error messages or custom error pages to `users`**

* -> _when we create an MVC application in Visual Studio_, it **`doesn't implement any exception handling technique out of the box`**
* -> it will **`display an error page when an exception occurred`** - **Yellow Screen of Death** 
* -> shows _exception details_ such as **`exception type, line number and file name where the exception occurred, and stack trace`**

## Solution:
* _`ASP.NET` provides some ways to handle exceptions:_
* -> using **<customErrors> element in `web.config`**
* -> using **HandleErrorAttribute**
* -> **overriding Controller.OnException** method
* -> using **`Application_Error` event of HttpApplication**

====================================================================
# <customErrors> Element in web.config
