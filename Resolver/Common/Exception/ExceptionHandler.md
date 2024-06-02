========================================================

# Exception Middleware - catch errors globally
* -> **centralize error handling logic** (_keep `code clean and organized`_) - ensuring that all unhandled exceptions are processed in **`a uniform manner`**
* -> when an exception is thrown in the application, it is **`passed to the exception middleware`**
* -> which can then perform any **necessary logging** or **error handling** before **`sending a response to the client`**
* => ensure that our **`application continues to function smoothly`** - allow to **gracefully recover from errors** and **avoid crashes**, **better debugging** in the face of unanticipated errors

* => by using exception middleware to **`display custom error pages`**, it **provide a better user experience** when errors occur
* => in many case, we don't want to **expose the original exception message**; and also get **localized** (_i.e. users gets `messages based on their language`_) 

========================================================
# Materials

## Error Model
* -> acts as **a template for error**; then **`send this in response`** 
```cs
namespace ExceptionHandlingProject.Models
{
    public class ErrorModel
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Details { get; set; } // Stack Trace

        public ErrorModel(int statusCode, string? message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message ?? string.Empty;
            Details = details; 
        }
    }
}
```

## Crafting a Centralized API Response Format
* -> however, sometimes it's essential to establish **a standardized API response format** - used for **`both APIs and error handling`**

```cs
public class ApiResponse
{
    public bool Success { get; set; }
    public object Data { get; set; } // target resource if Success = true; Error Model if Success = false; 
}
```

## Custom Exception Handling Middleware
* -> **task**: **`intercepts exceptions`**, **`logs them`**, and **`sends a JSON-formatted error response to the client`**
* -> **constructor parameters**: **`ILogger`** is used for **logging exceptions**, and **`RequestDelegate`** represents the **next middleware** in the pipeline
* -> **InvokeAsync**: **`entry point`** of the middleware - **`catches exceptions`** occurring **during the execution of subsequent middlewares (the request handling pipeline)**

* -> **try** block - attempts to **`execute the next middleware`** by invoking _next(context)
* -> **catch** block - **`logging exception`** + **`invoke 'HandleCustomExceptionResponseAsync' method to manage the exception and send a tailored error response`**

* -> **HandleCustomExceptionResponseAsync** - crafting and transmitting the custom error response
* -> sets the **`response content type`** to **JSON** and the **`status code`** to **500 (Internal Server Error)**
* -> creates **an 'ErrorModel' object** - **`passing the status code`**, **`exception message`**, and **`stack trace (if available)`**
* -> **serialized 'ErrorModel' to JSON** using JsonSerializer and **sent as the HTTP response**

```cs
public class ExceptionMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            var error = new ErrorModel(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString());

            await context.Response.WriteAsJsonAsync(
                new ApiResponse { Success = false, Data = error });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleCustomExceptionResponseAsync(context, ex);
        }
    }

    private async Task HandleCustomExceptionResponseAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var error = new ErrorModel(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString());
        var response = new ApiResponse {  Success = false, Data = error };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}

// Testing
[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        throw new Exception("Exception in HomeController.");
    }
}
```

## Adding Custom Middleware to the Pipeline
* -> make _custom middleware_ becomes **`an integral part of the request handling flow`**
```cs
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
```

========================================================
# Abstract Middleware
* -> this should be **extended by each of the sub-projects** (_`backoffice project` and `user project` in this case_)
* -> to **`catch all possible exceptions`**, **log them**, and **`run an abstract method`** for **extracting the error message** and **HTTP status code** 
* -> the method should be **overridden** by each of the subprojects, and **`provide the logic for displaying the error message`** according to their needs

```cs
/// <summary>
/// Abstract handler for all exceptions
/// </summary>
public abstract class AbstractExceptionHandlerMiddleware
{
    // Enrich is a custom extension method that enriches the Serilog functionality - you may ignore it
    private static readonly ILogger Logger = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType).Enrich();

    /// <summary>
    /// This key should be used to store the exception in the <see cref="IDictionary{TKey,TValue}"/> of the exception data,
    /// to be localized in the abstract handler.
    /// </summary>
    public static string LocalizationKey => "LocalizationKey";

    private readonly RequestDelegate _next;

    /// <summary>
    /// Gets HTTP status code response and message to be returned to the caller.
    /// Use the ".Data" property to set the key of the messages if it's localized.
    /// </summary>
    /// <param name="exception">The actual exception</param>
    /// <returns>Tuple of HTTP status code and a message</returns>
    public abstract (HttpStatusCode code, string message) GetResponse(Exception exception);

    public AbstractExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            // log the error
            Logger.Error(exception, "error during executing {Context}", context.Request.Path.Value);
            var response = context.Response;
            response.ContentType = "application/json";
            
            // get the response code and message
            var (status, message) = GetResponse(exception);
            response.StatusCode = (int) status;
            await response.WriteAsync(message);
        }
    }
}
```

## Backoffice Handler 
* -> implement for the `backoffice project` (_project cung cấp logic về xử lý_)
* -> return **a tuple of HTTP status code**, and a simple **DTO for the response message** that has the **`exception's message as the returned data`**

```cs
public class BackofficeExceptionHandlerMiddleware : AbstractExceptionHandlerMiddleware
{
    public BackofficeExceptionHandlerMiddleware(RequestDelegate next) : base(next)
    {
    }

    public override (HttpStatusCode code, string message) GetResponse(Exception exception)
    {
        HttpStatusCode code;
        switch (exception)
        {
            case KeyNotFoundException
                or NoSuchUserException
                or FileNotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            case EntityAlreadyExists:
                code = HttpStatusCode.Conflict;
                break;
            case UnauthorizedAccessException
                or ExpiredPasswordException
                or UserBlockedException:
                code = HttpStatusCode.Unauthorized;
                break;
            case CreateUserException
                or ResetPasswordException
                or ArgumentException
                or InvalidOperationException:
                code = HttpStatusCode.BadRequest;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                break;
        }
        return (code, JsonConvert.SerializeObject(new SimpleResponse(exception.Message)));
    }
}
```

## Register Middleware
* _register our middleware on each of the sub-project’s **Program.cs**_

```cs
var app = builder.Build();
// must be the first middleware, to ensure exceptions at all levels are handled
app.UseMiddleware<BackofficeExceptionHandlerMiddleware>();
```

## User App Handler
* -> in `user application` (_project cung cấp logic về hiển thị_), we want to **hide details** and **translate errors based on the user’s language**
* _we can use `.NET localization` to achieve the error translations_

```cs
// insert the localization key to the Exception#Data property (constant)
public class UserExceptionHandlerMiddleware : AbstractExceptionHandlerMiddleware
{
    private readonly IStringLocalizer<UserExceptionHandlerMiddleware> _localizer;

    public UserExceptionHandlerMiddleware(RequestDelegate next,
        IStringLocalizer<UserExceptionHandlerMiddleware> localizer) : base(next)
    {
        _localizer = localizer;
    }

    public override (HttpStatusCode code, string message) GetResponse(Exception exception)
    {
        HttpStatusCode code;
        switch (exception)
        {
            case KeyNotFoundException
                or NoSuchUserException
                or FileNotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            case EntityAlreadyExists:
                code = HttpStatusCode.Conflict;
                break;
            case UnauthorizedAccessException
                or UserBlockedException:
                code = HttpStatusCode.Unauthorized;
                break;
            case OrderValidatorException
                or InvalidOperationException:
                code = HttpStatusCode.BadRequest;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                break;
        }
        // if exception has no localized message, use default message
        // default message can be something like "Something went wrong"
        var localizationKey = exception.Data[LocalizationKey]?.ToString() ?? LocalizerKeys.GeneralError;
        return (code, JsonConvert.SerializeObject(new SimpleResponse(_localizer[localizationKey])));
    }
}

// Example of a custom exception class:
public sealed class MyCustomException : Exception
{
    public OrderValidatorException(string message = "my custom exception", string? localizerKey = null) : base(message)
    {
        Data.Add(AbstractExceptionHandlerMiddleware.LocalizationKey, localizerKey);
    }
}
```

========================================================
# Global Exception Handlers in .NET Core

```cs
public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500; // Internal Server Error
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled exception.");
            // Respond with error details or a generic message
        });
    });
}
```

========================================================
# Dependency Injection for Exception Handling in case "Not using global Handler"
* -> **`manage exception handling strategies`** offers flexibility and **decouples error handling logic from business logic**
* -> this approach allows for **`more manageable code and the ability`** to **swap out handling strategies for different environments** (_e.g., development vs. production_)

```cs
public interface IErrorHandler
{
    void Handle(Exception exception);
}

public class GlobalErrorHandler : IErrorHandler
{
    public void Handle(Exception exception)
    {
        // Log the exception, send notifications, etc.
    }
}

// In Startup.cs
services.AddSingleton<IErrorHandler, GlobalErrorHandler>();

// In your application code
public class MyService
{
    private readonly IErrorHandler _errorHandler;

    public MyService(IErrorHandler errorHandler)
    {
        _errorHandler = errorHandler;
    }

    public void MyMethod()
    {
        try
        {
            // Potentially failing operation
        }
        catch (Exception ex)
        {
            _errorHandler.Handle(ex);
            throw; // Rethrow if necessary
        }
    }
}
```