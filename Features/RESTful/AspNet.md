
# An ASP.NET Web API controller action can return:
* **void** - return **`empty 204 (No Content)`**
* **HttpResponseMessage** - **`convert directly`** to an **`HTTP response message`**
* **IHttpActionResult** - call **`ExecuteAsync`** to create an **`HttpResponseMessage`**, then convert to an HTTP response message
* **Some other type** - write the **`serialized return value`** into the **`response body`**; return 200 (OK)
* chi tiết: _https://learn.microsoft.com/en-us/aspnet/web-api/overview/getting-started-with-aspnet-web-api/action-results_