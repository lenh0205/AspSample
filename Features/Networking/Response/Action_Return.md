==========================================================================
# An ASP.NET Web API controller action can return:
* **void** - return **`empty 204 (No Content)`**
* **HttpResponseMessage** - **`convert directly`** to an **`HTTP response message`**
* **IHttpActionResult** - call **`ExecuteAsync`** to create an **`HttpResponseMessage`**, then convert to an HTTP response message
* **Some other type** - write the **`serialized return value`** into the **`response body`**; return 200 (OK)
* chi tiết: _https://learn.microsoft.com/en-us/aspnet/web-api/overview/getting-started-with-aspnet-web-api/action-results_

```C#
HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
httpResponseMessage.Content.Headers...;
httpResponseMessage.StatusCode = HttpStatusCode.OK;
```

==========================================================================
# ActionResult
* -> **Action** method - is **`public methods of the Controller class`** that **cannot be private, protected, static and overloaded**
* -> ASP.NET MVC support **`various Result classes`** - which can be **returned from an action method**
* -> **ActionResult** class is a **`base class of all the result classes`**; so it can be the return type of all action method that use result class

* -> however, we can **`specify the appropriate result class as a return type of action method`** (**derived from the 'ActionResult' class**):
```js
_______________________________________________________________________________________________
|     Result Class       |               Description	             | Base Controller Method |
|------------------------|-------------------------------------------|------------------------|
| ViewResult             | Represents HTML and markup                | View()                 |
|------------------------|-------------------------------------------|------------------------|
| EmptyResult            | Represents No response                    |	                      |
|------------------------|-------------------------------------------|------------------------|
| ContentResult          | Represents string literal                 | Content()              |
|------------------------|-------------------------------------------|------------------------|
| FileContentResult,     |                                           |                        |
| FilePathResult,        |                                           |                        |
| FileStreamResult       | Represents the content of a file	         | File()                 |
|------------------------|-------------------------------------------|------------------------|
| JavaScriptResult       | Represents a JavaScript script            | JavaScript()           |
|------------------------|-------------------------------------------|------------------------|
| JsonResult             | Represents JSON that can be used in AJAX  | Json()                 |
|------------------------|-------------------------------------------|------------------------|
| RedirectResult         | Represents a redirection to a new URL     | Redirect()             |
|------------------------|-------------------------------------------|------------------------|
| RedirectToRouteResult  | Represents redirection to another route   | RedirectToRoute()      |
|------------------------|-------------------------------------------|------------------------|
| PartialViewResult      | Represents the partial view result        | PartialView()          |
|------------------------|-------------------------------------------|------------------------|
| HttpUnauthorizedResult | Represents HTTP 403 response              |	                      |
|------------------------|-------------------------------------------|------------------------|
```

# https://tedu.com.vn/lap-trinh-aspnet-core/action-result-trong-aspnet-core-237.html