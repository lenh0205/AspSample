
# 'Elasticsearch' with 'ASP.NET Core Web API' using 'Entity Framework' and 'SQL Server'

* -> install Elasticsearch and run an Elasticsearch instance (_hoặc ta cũng có thể s/d **`cloud-hosted Elasticsearch service`**, ví dụ như **Elastic Cloud** cũng được_)
```bash
$ docker run -d --name elasticsearch -p 9200:9200 -e "discovery.type=single-node" -e "xpack.security.enabled=false" elasticsearch:8.5.0
```

* -> integrate for ASP.NET Core Web API
```bash
$ dotnet add package NEST # high-level client for working with Elasticsearch in .NET
$ dotnet add package Elasticsearch.Net # low-level client
```

* -> register Elasticsearch
```json
// appsettings.json
"Elasticsearch": {
  "Url": "http://localhost:9200",
  "Index": "products"
}
```
```cs
// Program.cs
using Elasticsearch.Net;
using Nest;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IElasticClient>(sp =>
{
    var settings = new ConnectionSettings(new Uri(builder.Configuration["Elasticsearch:Url"]))
        .DefaultIndex(builder.Configuration["Elasticsearch:Index"]);

    return new ElasticClient(settings);
});

builder.Services.AddScoped<ElasticsearchService>();

var app = builder.Build();
app.MapControllers();
app.Run();
```

* -> create an **elasticsearch service** to **`manage Elasticsearch operations`**
* -> sync SQL Server Data with Elasticsearch - require **`index existing SQL Server data into Elasticsearch`**
```cs
public class ElasticsearchService
{
    private readonly IElasticClient _elasticClient;
    private readonly string _indexName;

    public ElasticsearchService(IElasticClient elasticClient, IConfiguration config)
    {
        _elasticClient = elasticClient;
        _indexName = config["Elasticsearch:Index"] ?? "products";
    }

    public async Task IndexProductAsync(Product product)
    {
        await _elasticClient.IndexDocumentAsync(product);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        var response = await _elasticClient.SearchAsync<Product>(s => s
            .Index(_indexName)
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Name)
                    .Query(query)
                )
            )
        );

        return response.Documents;
    }

    // Add a method in ElasticsearchService.cs to index all products:
    public async Task IndexAllProductsAsync(List<Product> products)
    {
        foreach (var product in products)
        {
            await _elasticClient.IndexDocumentAsync(product);
        }
    }
}
```

* -> modify our Entity Model to be **searchable**
```cs
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }  
    public string Description { get; set; }
    public decimal Price { get; set; }
}
```

* -> usage
```cs
[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ElasticsearchService _elasticService;

    public ProductsController(ElasticsearchService elasticService)
    {
        _elasticService = elasticService;
    }

    // Call this first to sync products before actually searching
    // POST http://localhost:5000/api/products/sync-products
    [HttpPost("sync-products")]
    public async Task<IActionResult> SyncProducts([FromServices] ElasticsearchService elasticService, [FromServices] ApplicationDbContext dbContext)
    {
        var products = await dbContext.Products.ToListAsync();
        await elasticService.IndexAllProductsAsync(products);
        return Ok("Products indexed successfully.");
    }

    // Implement a Search API for users to search
    // GET http://localhost:5000/api/products/search?query=laptop
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string query)
    {
        var results = await _elasticService.SearchProductsAsync(query);
        return Ok(results);
    }
}
```