===========================================================
# ASP.NET Core 6.0
* https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-8.0

```cs
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://example.com",
                                              "http://www.contoso.com");
                      });
});

// services.AddResponseCaching();

builder.Services.AddControllers();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
```

===========================================================
# Enable CORS with "CORS middleware"
```cs - setting up the WebHost:
services.AddCors(
    options =>
    {
        // Created a custom CORS policy named AllowCors
        options.AddPolicy("AllowCors", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .WithMethods(
                        HttpMethod.Get.Method,
                        HttpMethod.Put.Method,
                        HttpMethod.Post.Method,
                        HttpMethod.Delete.Method
                    )
                    .AllowAnyHeader()
                    .WithExposedHeaders("CustomHeader");
            });
    });

// Added the CORS middleware to the pipeline, which uses 'AllowCors' as its "policyName"
// apply CORS "AllowCors" policy for every requests
app.UseCors("AllowCors");
```

```cs - a snippet of the "Invoke" function that gets called for the "CORS middleware"
public async Task Invoke(HttpContext context)
{
    if (context.Request.Headers.ContainsKey(CorsConstants.Origin))
    {
        // "_policy" is null and "_corsPolicyName" is AllowCors.
        var corsPolicy = _policy ?? await _corsPolicyProvider?.GetPolicyAsync(context, _corsPolicyName);
        if (corsPolicy != null)
        {
            var corsResult = _corsService.EvaluatePolicy(context, corsPolicy);
            _corsService.ApplyResult(corsResult, context.Response);
            // ...
        }
        //....
    }
    // ...
}
// -> because 'AllowCors' is the name of a valid policy that was added using "AddCors"
// -> this results in the CORS middleware applying the revelant CORS headers for all requests
```

# Enable CORS with MVC middleware
* -> use **`MVC authorization filters`** - **[EnableCors] and [DisableCors] attributes**
* -> this make **`MVC to take care of CORS`**, which is **independent of the 'CORS middleware'** in the WebHost's pipeline

```cs
services.AddMvcCore()
app.UseMvc();

[EnableCors("AllowCors")]
public class ControllerA1: Controller
```

## Note:
* -> so the **`combination of MVC and CORS middleware`** in handling CORS may cause **`unexpected results`**
* -> the _CORS middleware_ **add the CORS headers to request** regardless of whether or not we're asking it not to by **`using the [DisableCors] attribute`**
* -> the CORS middleware has no idea that this MVC concept (a filter) even exists
* -> ta có thể lựa chọn bỏ **UseCors()** và apply CORS cụ thể cho Controller/Action với **EnableCors attribute**; hoặc sử dụng **UseCors()** và apply policy cho toàn bộ request
