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
# Routing
* -> ta sẽ có 1 project ASP.NET Core Web API application có 1 endpoint là "http://localhost:5001/weatherforecast"

* -> h ta sẽ tạo thêm 1 project làm API Gateway để route đến endpoint đó
* -> tạo project với template là ASP.NET Core Web API (giả sử base url của nó là "https://localhost:5021/api/weather")
* -> cài **Ocelot** Nuget package

```cs - program.cs
builder.Services.AddOcelot();
app.UseOcelot().Wait();
```

* -> (most important) add the **`ocelot.json`** file sẽ cần **specifies where to route to** and **global configuration**

* -> create dev version **ocelot.dev.json** (lưu ý ta cần vào project properties -> Debug -> đổi giá trị của **ASPNETCORE_ENVIROMENT** thành **dev**)
```json - ocelot.dev.json
{
    "Routes": [ // an Array to have multiple routes equivalent to multipe services call
        {
            // the downstream application which we are going to call
            "DownstreamPathTemplate": "/weatherforecast",
            "DownstreamScheme": "https",
            "DownstreamHostAndPorts": [
                {
                    "Host": "localhost",
                    "Port": 5001
                }
            ],

            // Upstream is about the endpoint that we will be calling into the API Gateway
            "UpstreamPathTemplate": "/api/weather",
            "UpstreamHttpMethod": ["Get"]
        }
    ],
    "GlobalConfiguration": {
        // "BaseUrl" is very important declaration (to deal with headers for Ocelot)
        // it's URL where particular Ocelot API Gateway would be running
        "BaseUrl": "https://localhost:5021" // copy từ file "lauchSettings.json"
    }
}
```

```cs - add Ocelot config
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            var env = Enviroment.GetEnviromentVariables("ASPNETCORE_ENVIROMENT");
            webBuilder.UseStartup<Startup>();
            webBuilder.ConfigureAppConfiguration(config => 
                config.AddJsonFile($"ocelot.{env}.json"));

        })
        .ConfigureLogging(logging => logging.AddConsole());
```

* -> giờ ta có thể chạy project API Gateway của ta lên và truy cập "https://localhost:5021/api/weather" (kết quả sẽ giống như lúc ta truy cập "http://localhost:5001/weatherforecast")

=====================================================================
> ta sẽ authenticate ở API Gateway level thay vì individual service level

# Authentication
* -> when we have multiple services with APIs, services can be consume by external as well as internal processes
* -> for internal processes (which are running as cron job, background worker, ...) it's always overhead to authenticate for each service, instead these **`internal services can call each others directly`**
* -> whereas external services comming in, the **`authentication responsibility can be given to API Gateway`**
* => the services really **`doesn't have to worry about how to authenticate`** 

* (_the **Authorization** is different case, the services need to know the role and base on that to react, though the API Gateway can figure out the role and pass it to services as header_)

## Example
* -> Assume we're using JWT, ta sẽ cài **Microsoft.AspNetCore.Authentication.JwtBearer** NuGet package
```cs - program.cs
var secret = "Thisismytestprivatekey";
var key = Encoding.ASCII.GetBytes(secret);

services
    .AddAuthentication(option => {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = SymmetricSecurityKey(key);
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false
        }
    });

app.UseAuthentication();
```

* -> set Authentication configuration in Ocelot
```json - ocelot.dev.json
{
    "Routes": [
        {
            // .....,
            "AuthenticationOptions": {
                "AuthenticationProviderKey": "Bearer", // the scheme
                "AllowedScopes": [] // all scope
            }
        }
    ],
    // .....
}
```

* -> giờ ta truy cập "https://localhost:5021/api/weather" chắc chắn là không được
* -> giờ ta thử bỏ 1 JWT token vào và check lại thử

=====================================================================
> extremely simple

# Rate Limiting

```json - ocelot.dev.json
{
    "Routes": [
        {
            // .....,
           "RateLimitOptions": {
                "ClientWhitelist": [], // to whitelist some endpoints
                "EnableRateLimiting": true,
                "Period": "5s",
                "PeriodTimespan": 1,
                "Limit": 1 // allow 1 request for every 5 second
           }
        }
    ],
    // .....
}
```

* -> giờ ta thử gửi request liên tục sẽ không được và nhận mã lỗi **429 Too Many Requests**

=====================================================================
# Response Caching
* -> ta sẽ cần cài **`Ocelot.Cache.CacheManager`**

```cs - program.cs
services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());
```

```json - ocelot.dev.json
{
    "Routes": [
        {
            // .....,
           
           // -> enable caching + how many seconds we want to cache
           "FileCacheOptions:" { "TtlSeconds": 30 } // 30s
        }
    ],
    // .....
}
```