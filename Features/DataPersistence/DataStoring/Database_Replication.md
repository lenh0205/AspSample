> Database Replication là công việc thuộc về Database Administrator (DBA) or DevOps role
> nhưng 1 .NET Developer sẽ cần hiểu và biết cách làm việc với replication  
> https://www.fivetran.com/learn/database-replication#:~:text=Database%20replication%20is%20the%20process,%2Dto%2Ddate)%20data.
> https://www.scylladb.com/glossary/database-replication/
> https://www.geeksforgeeks.org/database-replication-and-their-types-in-system-design/
> https://www.geeksforgeeks.org/data-replication-in-dbms/
> https://viblo.asia/p/tang-performance-cua-sql-database-voi-replication-phan-1-aNj4vxlOL6r
> https://www.ibm.com/think/topics/data-replication
===================================================================================
# Database Replication 
* -> is the process of **`creating copies of a database`** and **`storing them across various on-premises or cloud destinations`**
* -> replication is **`an ongoing 24/7 process`** - when a user changes the source database, those **changes are synchronized to the replicated databases**
* => ensures that all users connected to the system can access the same up-to-date data copies, always work with the latest and most accurate data
* => can shift workloads and data storage from your primary database to a cloud environment
* => ensure company's ability to scale processes and perform real-time analytics, transformations and visualizations

## Database Replication configuration
* _Databases can be replicated once, in scheduled batches or continuously_
* _Database replication can be configured in various ways to meet different needs_
* -> **`Active/Active Replication`**: Both databases can process data changes and synchronize bidirectionally, ideal for load balancing and high availability
* -> **`Read-Only Replication`**: The primary database pushes updates to replicas that are read-only, useful for data democratization and reporting

## Database replication vs. mirroring

## Database Replication vs. Data Replication

===================================================================================
# How does database replication work - CDC (change data capture)

## CDC
* -> a sophisticated method that **`monitors and logs every change made in the source database`**, including updates, inserts and deletions
* => the CDC approach to database replication **enhances the speed of data integration**, a key advantage for businesses that aim to **optimize operational responsiveness**

## Accuracy
* -> the process **`captures a complete snapshot of all changes, recording each update, insert and deletion`**
* => allows to **`accurately replicate a database without the need to execute replication-specific queries against the database`**

## Data integrity
* -> CDC **relies on log files that `sequentially record` all changes**
* -> additionally, CDC captures every modification made so that **`no updates are skipped between synchronization intervals`**
* => this approach **`preserves the order in which changes occur`**, maintaining data integrity across replicated databases

## Performance
* -> CDC minimizes the impact on the source database by **`operating asynchronously`** — changes are captured from the logs as they occur
* => **`significantly reduces the database server load`** by **avoiding frequent querying for replication purposes**
* -> additionally, CDC can be configured to filter and replicate only relevant data changes, further optimizing the replication process

## Real-time tracking
* -> rapidly replicate database changes, **`enabling real or near-real-time updates`** to the target system
* => this keeps all replicated environments accurately synchronized, **allowing users on various platforms to access updated information**
* => rapid responsiveness enhances **`applications reliant on timely data for decision-making`**, especially in sectors like **financial services**

===================================================================================
# Benefits

## Improved disaster recovery
* -> by replicating multiple copies of your database to BCP/DR infrastructure, you can create a high-availability environment that ensures your data is always accessible

## Reduced server load
* -> Network performance suffers when capacity on the database server is lowered due to significant data storage or CPU processing.
* -> replicating our database to a destination offloads some of this burden, freeing up space on our production database and keeping system performance levels optimal

## Enhanced data analytics
* -> Database replication boosts analytics by creating an isolated environment for data queries, leaving core system performance unaffected.
* -> Analysts can execute complex queries in this replicated setting without slowing down operational systems, gaining real-time access to large datasets.
* -> This enables quicker insights and more fluid data exploration

## Real-time business intelligence 
* -> Real-time data access across all business units enhances accuracy in reporting and decision-making.
* -> BI tools retrieve the latest data without delays, enabling more informed business strategies.
* -> Additionally, this approach simplifies the integration of disparate data sources, fostering a more cohesive BI strategy.

## Predictive AI/ML applications

===================================================================================
# Risk/Challenges

## Ensuring data consistency

## Managing multiple servers and destinations

## Implementing a Backup Strategy

===================================================================================
# Steps to database replication
* -> Identify your data source
* -> Determine the scope of your database replication
* -> Decide on a database replication frequency
* -> Choose a database replication type and method
* -> Use a database replication tool

===================================================================================
# Types of database replication
* -> Full-table replication
* -> Key-based incremental replication
* -> Log-based replication

===================================================================================
# Data Replication with .NET

## Your Role as a .NET Developer
Even though you don’t set up replication, you might need to:

Ensure application compatibility – Your code should handle scenarios like read replicas (e.g., directing read queries to replicas).
Understand replication impact – Be aware of latency issues and eventual consistency in certain replication models.
Work with DBAs – Communicate application requirements to DBAs if replication affects performance or data consistency.

## Common pattern to work with database replication
* -> Use the primary (master) database for write operations (INSERT, UPDATE, DELETE).
* -> Use read replicas (secondary/slave databases) for read operations (SELECT queries) to reduce load on the primary database.

## Example: Implement Read-Write Splitting in an ASP.NET Core Web API
* _In a real-world ASP.NET Core Web API application, you can configure your DbContext to use different connection strings for read and write operations_

* -> **program.cs** - configure our 2 DbContext for **read database** and **write Database**
```cs
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services for both read and write contexts
builder.Services.AddScoped<ApplicationDbContext>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    return new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>(), config, false); // Default to Write DB
});

builder.Services.AddScoped<ApplicationDbContext>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    return new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>(), config, true); // Read DB
});

var app = builder.Build();
app.Run();
```

* -> **ApplicationDbContext** - class to support both read and write operations base on parameter
```cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly bool _isReadOnly;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration, bool isReadOnly = false)
        : base(options)
    {
        _configuration = configuration;
        _isReadOnly = isReadOnly;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = _isReadOnly 
            ? _configuration.GetConnectionString("ReadDb")  // Use Read Replica
            : _configuration.GetConnectionString("WriteDb"); // Use Primary DB

        optionsBuilder.UseSqlServer(connectionString);
    }

    public DbSet<Product> Products { get; set; }  // Example entity
}
```

* -> **appsettings.json**
```json
{
  "ConnectionStrings": {
    "WriteDb": "Server=PrimaryDBServer;Database=MyAppDB;User Id=sa;Password=yourpassword;",
    "ReadDb": "Server=ReplicaDBServer;Database=MyAppDB;User Id=sa;Password=yourpassword;"
  }
}
```

* -> **Repository**
```cs
public class ProductRepository
{
    private readonly ApplicationDbContext _writeDbContext;
    private readonly ApplicationDbContext _readDbContext;

    public ProductRepository(ApplicationDbContext writeDbContext, ApplicationDbContext readDbContext)
    {
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
    }

    // Read from Replica
    public async Task<List<Product>> GetProductsAsync()
    {
        return await _readDbContext.Products.ToListAsync();
    }

    // Write to Primary DB
    public async Task AddProductAsync(Product product)
    {
        _writeDbContext.Products.Add(product);
        await _writeDbContext.SaveChangesAsync();
    }
}
```

* -> **Controller**
```cs
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductRepository _productRepository;

    public ProductsController(ProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productRepository.GetProductsAsync();
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] Product product)
    {
        await _productRepository.AddProductAsync(product);
        return Ok(new { message = "Product added successfully" });
    }
}
```

## Further Optimizations
Load Balancing for Read Queries: If multiple read replicas exist, a load balancer (like HAProxy) can distribute read requests among replicas.
Handling Replication Lag: Since replication is asynchronous in some setups, ensure your app can tolerate slight delays in data consistency.
Retry Policies: If a replica is down, configure retries or fallback to the primary database

===================================================================================
> https://medium.com/@vijmoorthy/practical-ways-to-implement-cqrs-f84a577c7bd2
> https://tech.cybozu.vn/cqrs-thiet-ke-he-thong-chiu-tai-lon-va-de-bao-tri-99f4b/
> https://stackoverflow.com/questions/75751508/cqrs-vs-database-replica-set

# 'CQRS' pattern with Replication
* -> **`CQRS fits naturally with database replication`** but doesn't require it (_we can use CQRS even without replication_)

* -> install **MediatR** packages
```bash
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
```

* -> register MediatR in **Program.cs**
```cs
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WriteDb")));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReadDb")));

builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();
app.Run();
```

* -> define CQRS Queries and Commands
```cs
// ---------> Query (Read from Replica)

using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetProductsQuery : IRequest<List<Product>> {}

public class GetProductsHandler : IRequestHandler<GetProductsQuery, List<Product>>
{
    private readonly ApplicationDbContext _readDbContext;

    public GetProductsHandler(ApplicationDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<List<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await _readDbContext.Products.ToListAsync();
    }
}
```
```cs
// ---------> Query (Read from Replica)

public class AddProductCommand : IRequest
{
    public string Name { get; set; }
}

public class AddProductHandler : IRequestHandler<AddProductCommand>
{
    private readonly ApplicationDbContext _writeDbContext;

    public AddProductHandler(ApplicationDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task<Unit> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product { Name = request.Name };
        _writeDbContext.Products.Add(product);
        await _writeDbContext.SaveChangesAsync();
        return Unit.Value;
    }
}
```

* -> usage in Controller
```cs
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _mediator.Send(new GetProductsQuery());
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] AddProductCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Product added successfully" });
    }
}
```
