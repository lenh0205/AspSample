
# Status Code
* -> **status code** is issued by the **`server`** based on the **`operation`**, **`input data`**, and some **`other parameters`**
* -> status code gives some useful information about the **`behavior of the response`**

## HTTP Status Codes Glossary
* Để xem chi tiết: https://www.webfx.com/web-development/glossary/http-status-codes/

* **1xx** – **`Informational`**
* **2xx** – **`Successful`**
* **3xx** – **`Redirection`**
* **4xx** – **`Client Error`**
* **5xx** – **`Server Error`**

* => `Asp.Net Core Web API` has some **built-in methods** to return the proper status code

## "StatusCode" method
* could use to **return any HTTP status code**

```C#
return StatusCode(418);
```

```C# - nếu chỉ muốn set StatusCode thôi thì
this.HttpContext.Response.StatusCode = 418;
```

## 200 status code
* -> the most common status code; 200 status code belongs to the **Successful category**
* -> _Asp.Net Core_ has **Ok()** method to return 200 status code

```C#
public IActionResult GetEmployees()  
{  
    var employees = EmployeeData();  
    return Ok(employees);  
}  
```

## 201 Status code
* -> indicates that **the new resource has been created successfully** 
* -> and the server will **`return the link`** to get that **`newly created resource`**
* ->  Asp.Net Core methods that return the _201 status code_: **Created**, **CreatedAtAction**, **CreatedAtRoute**
* => these method will add a **`new response header`** with a key **Location** and the value is the **`URL we pass`** to get this resource

* **Lưu ý**: đường link ta ta pass làm value trả về phải là 1 **`action method tồn tại trong Controller`**

```C# - VD: CreatedAtRoute
[HttpGet]    
[Route("{id}", Name = "getEmployeeRoute")]    
public IActionResult GetEmployeeById([FromRoute] int id)    
{    
    var employee = EmployeeData().Where(x => x.Id == id).FirstOrDefault();    
    return Ok(employee);    
}    
    
[HttpPost("")]    
public IActionResult AddEmployee([FromBody] Employee model)    
{    
    // code to add new employee here    
    int newEmployeeId = 1; // get this id from database.    
    return CreatedAtRoute("getEmployeeRoute", new { id = newEmployeeId }, model);    
}    
```

## 202 status code
* -> indicates that the **request has been accepted but the processing is not yet complete**
* -> Asp.Net Core methods that return the _202 status code_: **Accepted**, **AcceptedAtAction**, **AcceptedAtRoute**
* _ tức là the `server accepts the request`, but the `response cannot be sent immediately` (Ex: in batch processing)_

```C#
[HttpPost("")]  
public IActionResult AddEmployee([FromBody] Employee model)  
{  
    // code goes here  
    return Accepted();  
}  
```

## 400 Status Code
* -> indicated the **`bad request`** - means there is **something wrong in the request data**
* -> be able to return 400 status using the **BadRequest()** method

```C#
[HttpPost("")]  
public IActionResult AddEmployee([FromBody] Employee model)  
{  
    // code goes here  
    return BadRequest();  // returns 400 status code  
}
```

## 404 status code
* -> looking for **a resource that does not exist**
* -> using the **NotFound()** method

```C#
[HttpGet]  
[Route("{id}", Name = "getEmployeeRoute")]  
public IActionResult GetEmployeeById([FromRoute] int id)  
{  
    var employee = EmployeeData().Where(x => x.Id == id).FirstOrDefault();  
    if (employee == null)  
    {  
        return NotFound();  
    }  
    return Ok(employee);  
}  
```

## 301 & 302 Status code
* -> used for **local redirection** 
* -> means if you are trying to access an action method and from that action method you are **`redirecting the call to some other action method`** within the same application
* -> then in the response of this request there will be a **`301 or 302 status code`** and **`a new response header with name "Location"`**
* -> **client will send another request** on the value given in the **`Location header`**

* -> if **`redirecting permanently`** then the status code will be **301**; using the **LocalRedirectPermanent()** method
* -> if **`redirecting temporary`** then the status code will be **302**; using the **LocalRedirect()** method

```C#
[HttpGet]  
[Route("{id}", Name = "getEmployeeRoute")]  
public IActionResult GetEmployeeById([FromRoute] int id)  
{  
    return LocalRedirect("~/api/employees");  
}   

[HttpGet]  
[Route("{id}", Name = "getEmployeeRoute")]  
public IActionResult GetEmployeeById([FromRoute] int id)  
{  
    return LocalRedirectPermanent("~/api/employees");  
} 
```

## Other
* **204 No Content**: for successful operations that contain no data
* **304 Not Modified**: used for caching, indicating the resource is not modified
* **401 Unauthorized**: for failed operation due to unauthenticated requests
* **403 Forbidden**: for failed operation when the client is not authorized to perform
* **405 Method Not Allowed**: for failed operation when the HTTP method is not allowed for the requested resource
* **406 Not Acceptable**: for failed operation when the Accept header doesn’t match. Also can be used to refuse request
* **409 Conflict**: for failed operation when an attempt is made for a duplicate create operation
* **429 Too Many Requests**: for failed operation when a user sends too many requests in a given amount of time (rate limiting)
* **500 Internal Server Error**: for failed operation due to server error (generic)
* **502 Bad Gateway**: for failed operation when the upstream server calls fail (e.g. call to a third-party service fails)
* **503 Service Unavailable**: for a failed operation when something unexpected happened at the server (e.g. overload of service fails)

=======================================================
# REST API best practice for "Status Code"
* one of `REST` architectural constraints is **Uniform Interface** - use common and well-known **`HTTP methods`** and **`status codes`** in their APIs
* nhưng nên flexible trong 1 số trường hợp 

## GET
* **200 OK** - if **`successfully READ resource`** and **`response contains data`**
* **204 No Content** - if **`successfully READ resource`** **`and the response contains no data`**

## POST
* **201 Created** - **`successful CREATE operation`**
* **200 OK** - for **`successful READ operation`** (using POST) if the **`response contains data`** 
* -> _ sometimes we can't use GET to READ because `query string exceeds maximum allowed characters` or `contain sensitive information`_
* **204 No Content** - for **`successful READ operation`** if the response **`contains NO data`**

## PUT
* **200 OK** - if **`successfully UPDATE resource`** and the **`response contains data`**
* **204 No Content** - if **`successfully UPDATE resource`** and the **`response contains no data`**

## PATCH 
* **200 OK** - for **`successful partial UPDATE operation`**

## DELETE
* **204 No Content** - for **`successful DELETE operation`**




 