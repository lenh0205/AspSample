> **REST APIs** are one of the most common kinds of **`web interfaces`** available today
> have to take into **`account security`**, **`performance`**, and **`ease of use`** for API consumers
> https://blog.postman.com/rest-api-examples/
> https://www.integrate.io/blog/how-to-make-a-rest-api/

===========================================================================
# REST API
* -> **`REST (representational state transfer)`** is a software architectural style commonly used for building web services such as APIs
* -> an **`API (application programming interface)`** is a collection of functions and protocols that enables two software applications or systems to communicate with each other

===========================================================================
> HATEOAS - the API response includes links that tell the client what actions it can take next (tức là ví dụ khi ta get resource thì reponse nên trả về thêm những thông tin về endpoint khác cũng như method để ta thao tác với resource này thay vì client phải hardcode)

# REST API standards 
* _to **`make an API service RESTful`**, six guiding constraints must be satisfied:_

## Uniform interface
* -> All clients should be able to interact with the REST API in the same manner, whether the client is a browser, a mobile app, or something else.
* _The REST API is usually accessible at a single URL (uniform resource locator) — for example, “https://api.example.com.”_
* _includes consistent resource naming_
* _using HTTP methods correctly (GET, POST, PUT, DELETE), and optional using hypermedia (HATEOAS)_

## Client-server architecture
* -> In REST APIs, the client and server are two separate entities. Concerns about the API interface are separate from concerns about how the underlying data is stored and retrieved

## Statelessness
* -> REST requests must be stateless; the server does not have to remember any details about the client’s state. This means that the client must include all necessary information within each API request it makes

## Cacheability
* -> REST servers can cache data by designating it as cacheable with the "Cache-Control" HTTP header. The cached result is ready for reuse when there is an equivalent request later on
* _the client can also plays a role in caching; HTTP headers like ETag, Cache-Control, and Expires help manage caching effectively_

## Layered system
* -> The REST client does not know (and does not need to know) if it is communicating with an intermediary layer in the architecture, or with the server itself

## Code on demand (Optional)
* -> The client can optionally download code such as a JavaScript script or Java applet in order to extend its functionality at runtime

===========================================================================
# ASP.NET Web API

## Uniform Interface
* -> consistent resource naming - by using routing convention (Ex: [Route])
* -> standard HTTP methods - [HttpGet], [HttpPost], [HttpPut], [HttpDelete]
```cs
[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        var product = new { Id = id, Name = "Sample Product" };
        return Ok(product);
    }
}
```

## Client-Server Architecture
* -> ASP.NET Core Web API separates UI (React, Angular, etc.) from backend logic
* -> Statelessness

## Statelessness
* -> each request is independent, client includes all necessary information - no session state stored on the server, uses JWT authentication or API keys instead
```cs
// JWT-based authentication in ASP.NET Core
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://your-auth-server.com";
        options.Audience = "your-api";
    });
```

## Cacheability
* -> supports **Response Caching Middleware** (Cache-Control, ETag)
* -> supports In-memory caching (IMemoryCache) and Distributed Caching (Redis, SQL Server, etc.)

```cs
services.AddResponseCaching();

[HttpGet("{id}")]
[ResponseCache(Duration = 60)] // Cache response for 60 seconds
public IActionResult GetProduct(int id)
{
    return Ok(new { Id = id, Name = "Cached Product" });
}
```

## Layered System
* -> can interact with **intermediary layers** (e.g., **`API Gateway`**, **`Load Balancers`**)
* -> works well with **`reverse proxies`** (NGINX, Cloudflare, etc.)
* -> supports **middleware pipelines** (authentication, logging, CORS, etc.) - handle request before reaching the actual API controller

## Code on demand
* -> can return script execution responses in JSON
```cs
[HttpGet("script")]
public IActionResult GetScript()
{
    return Content("console.log('Hello from API');", "application/javascript");
}
```

===========================================================================
# Best practices for REST API design
* https://stackoverflow.blog/2020/03/02/best-practices-for-rest-api-design/
