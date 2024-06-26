===========================================================================
# 'Developer Exception Page' in ASP.NET Web API - Development enviroment
* -> catch **`unhandled exceptions`** thrown in **`middleware pipeline`**
* -> ASP.NET Core apps **`enable the 'developer exception page'`** **by default** in the **Development environment**
* -> in other case, we shouldn't enable the 'developer exception page' (_by calling **`app.UseDeveloperExceptionPage`**_), because it's not suitable for **sharing detailed exception information publicly** when the app runs **in production**

* -> the _Developer Exception Page_ include **Stack trace**, **Query string parameters**, **Cookies**, **Headers**, **Endpoint metadata** about the exception

```r - Nếu ta gửi Request từ 1 cửa sổ mới trên Browser đến 1 Endpoint lỗi trên Server
// -> tức là request với header "Accept" là "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
// -> reponse sẽ là 1 trang "HTML"
```

```r - Nếu ta gửi 1 AJAX Request 
// -> request có header "Accept" là "text/plain" 
// -> reponse Body sẽ là 1 "plain text":
System.InvalidOperationException: Sample Exception
at WebApplicationMinimal.Program.<>c.<Main>b__0_0() in C:\Source\WebApplicationMinimal\Program.cs:line 12
at lambda_method1(Closure, Object, HttpContext)
at Microsoft.AspNetCore.Diagnostics.Developer
```

===========================================================================
# Exception Handling Middleware - Non-development environments
* -> produce **`an error payload`**

## Add 'Exception Handling Middleware'
```cs - program.cs
var app = builder.Build();
app.UseHttpsRedirection();

// Add Middleware
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error-development");
}
else
{
    app.UseExceptionHandler("/error");
}
```

## Configure a controller action to respond to the "/error" route
* -> **don't mark the error handler action method** with **`HTTP method attributes`** (_such as **`HttpGet`**); explicit verbs prevent some requests from reaching the action method
* -> and we should **`allow anonymous access to the method`** if **unauthenticated users should see the error**

```cs
[Route("/error")]
public IActionResult HandleError() => Problem(); // sends an "RFC 7807-compliant payload" to the client

[Route("/error-development")]
public IActionResult HandleErrorDevelopment([FromServices] IHostEnvironment hostEnvironment)
{
    if (!hostEnvironment.IsDevelopment())
    {
        return NotFound();
    }

    var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

    return Problem(
        detail: exceptionHandlerFeature.Error.StackTrace,
        title: exceptionHandlerFeature.Error.Message);
}
```

===========================================================================
# Use exceptions to modify the response
* -> the **contents of the response can be modified from outside of the controller** using **`a custom exception`** and **`an action filter`**:

## Custom Exception
* -> create **`a well-known exception type`** named **HttpResponseException**
```cs
public class HttpResponseException : Exception
{
    public HttpResponseException(int statusCode, object? value = null) =>
        (StatusCode, Value) = (statusCode, value);

    public int StatusCode { get; }

    public object? Value { get; }
}
```

## Action Filter
* -> **create an action filter** named **`HttpResponseExceptionFilter`**:
```cs
public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    // the filter specifies an "Order" of the maximum integer value minus 10
    // this "Order" allows other filters to run at the end of the pipeline
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is HttpResponseException httpResponseException)
        {
            context.Result = new ObjectResult(httpResponseException.Value)
            {
                StatusCode = httpResponseException.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }
}
```

* -> in **`Program.cs`**, **add the action filter to the filters collection**:
```cs
builder.Services.AddControllers(options =>
{
    options.Filters.Add<HttpResponseExceptionFilter>();
});
```

===========================================================================
# 'Validation failure' error response
* -> for **`web API controllers`**, MVC responds with a **`'ValidationProblemDetails' response type`** **when model validation fails**
* -> MVC uses the **results of 'InvalidModelStateResponseFactory'** to **`construct the error response`** for **`a validation failure`**

* -> example of **replacing the default factory** with an implementation that also **`supports formatting responses as XML`**, in Program.cs:
```cs
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
            new BadRequestObjectResult(context.ModelState)
            {
                ContentTypes =
                {
                    // using static System.Net.Mime.MediaTypeNames;
                    Application.Json,
                    Application.Xml
                }
            };
    })
    .AddXmlSerializerFormatters();
```

===========================================================================
# Client error response
* -> an **error result** is defined as **`a result with an HTTP status code of 400 or higher`**
* -> _for web API controllers_, MVC **`transforms an error result`** to produce a **ProblemDetails**

* -> the **`automatic creation of a 'ProblemDetails' for error status codes`** is **enabled by default**
* -> but **`error responses can be configured in following ways`**: use the **problem details service**, implement **ProblemDetailsFactory**, use **ApiBehaviorOptions.ClientErrorMapping**

* -> in **RFC7807**, _a problem detail_ is defined as a way to **`carry machine-readable details of errors in a HTTP response`** to **avoid the need to define new error response formats for HTTP APIs**
* -> Ex: _a problem details object can have the following members: `type`, `title`, `status`, `detail`, `instance`

## Default problem details response
* -> **`a "problem details" response`** **is (only) generated** when the "/api/values2/divide" endpoint is called with a zero denominator (_tức là chỉ khi ta trả về `BadRequest()` with no argument_)

* _tức là nếu có 1 unhandled exception hoặc no matching endpoint,... thì nó không trả về `Prolem Detail`_

```cs -  returns "BadRequest" when the input is invalid
[Route("api/[controller]/[action]")]
[ApiController]
public class Values2Controller : ControllerBase
{
    // /api/values2/divide/1/2
    [HttpGet("{Numerator}/{Denominator}")]
    public IActionResult Divide(double Numerator, double Denominator)
    {
        if (Denominator == 0)
        {
            return BadRequest();
        }
        return Ok(Numerator / Denominator);
    }
}
```

```json - 
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "00-84c1fd4063c38d9f3900d06e56542d48-85d1d4-00"
}
```

## Problem details service
* -> ASP.NET Core supports **`creating 'Problem Details' for HTTP APIs`** using the **IProblemDetailsService**
* -> we can configures it in **`program.cs`**, and the app to **`generate a "problem details response"`** for **all HTTP client** and **server error responses that don't have body content yet**

```cs - Configuration
ar builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails(); // this one

var app = builder.Build();

app.UseExceptionHandler(); // this one
app.UseStatusCodePages(); // this one

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // this one
}

app.MapControllers();
app.Run();
```

* _với ví dụ bên trên, `a problem details response` is generated when **an invalid input is supplied**, **the URI has no matching endpoint**, **an unhandled exception occurs**_

* -> to **disable the automatic creation of 'ProblemDetails' for error status codes**, we can set **SuppressMapClientErrors** property is **`set to true`**
```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressMapClientErrors = true;
    });
```
* _when an API controller returns `BadRequest`, an HTTP 400 response status is `returned with no response body`_ 
* _SuppressMapClientErrors prevents a ProblemDetails response from being created, even when calling `WriteAsync` for an API Controller endpoint_

## Customize problem details with 'CustomizeProblemDetails'
```cs - Configuration
builder.Services.AddControllers();

builder.Services.AddProblemDetails(options =>
        // uses 'ProblemDetailsOptions' to set 'CustomizeProblemDetails'
        options.CustomizeProblemDetails = (context) =>
        {
            var mathErrorFeature = context.HttpContext.Features.Get<MathErrorFeature>();
            if (mathErrorFeature is not null)
            {
                (string Detail, string Type) details = mathErrorFeature.MathError switch
                {
                    MathErrorType.DivisionByZeroError => (
                        "Divison by zero is not defined.",
                        "https://wikipedia.org/wiki/Division_by_zero"
                    ),
                    _ => (
                        "Negative or complex numbers are not valid input.",
                        "https://wikipedia.org/wiki/Square_root"
                    )
                };

                context.ProblemDetails.Type = details.Type;
                context.ProblemDetails.Title = "Bad Input";
                context.ProblemDetails.Detail = details.Detail;
            }
        }
    );
```

```cs - Controller Action
[HttpGet("{radicand}")]
public IActionResult Squareroot(double radicand)
{
    if (radicand < 0)
    {
        var errorType = new MathErrorFeature
        {
            MathError = MathErrorType.NegativeRadicandError
        };
        HttpContext.Features.Set(errorType);
        return BadRequest();
    }

    return Ok(Math.Sqrt(radicand));
}

// Custom Http Request Feature
class MathErrorFeature
{
    public MathErrorType MathError { get; set; }
}

// Custom math errors
enum MathErrorType
{
    DivisionByZeroError,
    NegativeRadicandError
}
```

* -> _với ví dụ trên thì `a problem details response is generated` when **`The "/divide" endpoint is called with a zero denominator`** or **the URI has no matching endpoint**_

* _the `problem details response body` contains the following when either "squareroot" endpoint is called with a radicand less than zero:_
```json
{
  "type": "https://en.wikipedia.org/wiki/Square_root",
  "title": "Bad Input",
  "status": 400,
  "detail": "Negative or complex numbers are not allowed."
}
```

## Implement 'ProblemDetailsFactory'
* -> MVC uses **ProblemDetailsFactory** to **`produce all instances`** of **ProblemDetails** and **ValidationProblemDetails**
* -> this factory is used for: **`Client error responses`**, **`Validation failure error responses`**, **ControllerBase.Problem** and **ControllerBase.ValidationProblem**

* -> to customize the problem details response, register **`a custom implementation`** of **ProblemDetailsFactory** in Program.cs:
```cs
builder.Services.AddControllers();
builder.Services.AddTransient<ProblemDetailsFactory, SampleProblemDetailsFactory>();
```

## Use 'ApiBehaviorOptions.ClientErrorMapping'
* -> use the **`ClientErrorMapping`** property to configure the contents of the **`ProblemDetails`** response. 

```cs - For example:
// the following code updates the "Link" property for 404 responses:
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.ClientErrorMapping[StatusCodes.Status404NotFound].Link =
            "https://httpstatuses.com/404";
    });
```