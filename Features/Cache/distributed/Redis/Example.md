https://www.youtube.com/watch?v=fMUHhuGfqwg
https://www.youtube.com/watch?v=Tt5zIKVMMbs
https://www.youtube.com/watch?v=UrQWii_kfIE&t=492s
https://www.youtube.com/watch?v=QDFxi7veYps
https://www.youtube.com/watch?v=jwek4w6als4

================================================================================
# Demo: using 'Redis' Cache in an 'ASP.NET Core Web API' project

## Overviews
* -> this approach in Demo improves performance by reducing database queries
* -> the way it is implemented just like **Memory Cache (IMemoryCache)**, but memory cache only efficient for single a app on a single server
* -> Redis still the best choice for **distributed system** (_scalable applications or microservices_)
* => allows multiple app instances need to **`share cache data`**;
* => **`caching large datasets`** (Redis can handle more than what memory allows);
* => **`persistent cache storage`** even if the app restarts;
* => **`advanced features`** (_eviction policies, data replication, and pub/sub messaging_)

## Steps
* -> install **`Redis`** on your system (e.g., using Docker: docker run --name redis -d -p 6379:6379 redis).
* -> install the **`StackExchange.Redis`** NuGet package to ASP.NET Core Web API project:
```bash
$ dotnet add package StackExchange.Redis
```

* -> configure Redis in **`appsettings.json`**:
```json
{
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

* -> register Redis in **`Program.cs`**
```cs
using StackExchange.Redis;

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]));
```

* -> implement Redis Caching in a Controller
```cs
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _cache;

    public ProductController(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _cache = _redis.GetDatabase();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        string cacheKey = $"product:{id}";
        
        // Check if the product is in Redis cache
        var cachedProduct = await _cache.StringGetAsync(cacheKey);
        if (!cachedProduct.IsNullOrEmpty)
        {
            var product = JsonSerializer.Deserialize<Product>(cachedProduct);
            return Ok(new { Source = "Cache", Product = product });
        }

        // Simulate fetching from database
        var productFromDb = new Product { Id = id, Name = $"Product {id}", Price = 100 + id };

        // Store data in Redis cache with an expiration time of 1 minute
        await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(productFromDb), TimeSpan.FromMinutes(1));

        return Ok(new { Source = "Database", Product = productFromDb });
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

* -> Run and Test
```bash
GET http://localhost:5149/api/product/1
# First request: Data comes from the database
# Second request: Data comes from Redis Cache
```
