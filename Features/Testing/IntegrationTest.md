=============================================================================
# Test Containers

# WebApplicationFactory

=============================================================================
# Test "external APIs" 
* -> this would be a problem if these external API is unavailable or have some lincenses that require us to pay for calls it
* -> especially when we're running a pretty large test or running test in parallel
* -> mocking an HTTP Handle might seem like a good idea, but we can't really simulate pragmatically all the real world scenarios for production like network, delays, timeouts, service unavailable, ....

* => we **don't necessarily need to call those external API**, we can **call a mocked out instance of the API** that will behave in a similar fashion to the one that we need
* => this is where **`WireMock`** comes in - basically by using this library we can setup **InMemory API** that will behave like the real instance
* -> it can response to URL, paths, headers, cookie, ...

## Step
https://www.youtube.com/watch?v=2FWt_v0YB3U&list=WL&index=6

* -> the firt thing we'll need is an **HttpClient**

```cs
public class PaymentServiceClient
{
    private readonly HttpClient _httpClient;

    public PaymentServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaymentStatusResponse> RetrievePaymentStatusAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/v1/payments/{id}/status");

        if (response.IsSuccessStatusCode)
        {
            var paymentStatus = await response.Content.ReadFromJsonAsync<PaymentStatusReponse>();
            return paymentStatus;
        }

        return default;
    }
}

public class PaymentStatusResponse
{
    public string Status { get; set; }
}

public class PaymentServiceEndpointDefinition : IEndpointsDefinition
{
    public static void Configuration
}
```
========================================================================
# Step
* -> Đầu tiên tạo project với templace **`Unit Test + XUnit`** và các packages bao gồm **`Fluent Assertion`**, **`Microsoft.AspNetCore.Mvc.Testing`**, **`Microsoft.NET.Test.Sdk`**
* -> reference tới project chính

```cs 
// we will test only the "create habit" endpoint

// the 'IClassFixture' is use to avoiding creating new instance of WebApplicationFactory<IApiMaker> for every test that we create
// 'IAsyncLifetime' require implement 2 method 'InitializeAsync' và 'DisposeAsync'
public class CreateHabitEndpointTests 
    : IClassFixture<WebApplicationFactory<IApiMaker>>
    : IAsyncLifetime
{
    

    // we'll use the 'WebApplicationFactory'
    // -> because for integration testing, we need an instance of our "Habit tracker" API in memory
    // -> and also offer us possibility to create 'HttpClient'
    private readonly WebApplicationFactory<IApiMaker> _webApplicationFactory;

    private List<int> _habitIds;

    public CreateHabitEndpointTests(WebApplicationFactory<IApiMaker> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
        _habitIds = new List<int>();
    }

    [Fact]
    public async Task GivenValidHabit_CreateHabit()
    {
        // Arrange
        var httpClient = _webApplicationFactory.CreateClient();
        var habit = new Habit // a valid habit
        {
            Name = "First integration test"
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/habits", habit);
        var createdHabit = await response.Content.ReadFromJsonAsync<Habit>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdHabit.Should().NotBeNull();
        createdHabit.Name.Should().Be(habit.Name);
        response.Headers.Location.AbsolutePath.Should().Be($"/api/v1/habits/{createdHabit.Id}");
        _habitIds.Add(createdHabit.Id);
    }

    [Fact]
    public async Task GivenInvalidHabit_ReturnsProblemDetails()
    {
        // Arrange
        var httpClient = _webApplicationFactory.CreateClient();
        var habit = new Habit();

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/habits", habit);
        var problem = await response.Content.ReadFromJsonAsync<Habit>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        problem.Should().NotBeNull();
        problem.Errors.Should().NotBeEmpty(); 
        problem.Errors
            .Any(x => x.Key == "Name" && x.Value.Any(y => y == "'Name' must not be empty."))
            .Should()
            .BeTrue();

    }

    // 'InitializeAsync' thường được dùng thay cho constructor khi có một số setup methods cần được call in async fashion
    // for set up things before specific test
    public Task InitializeAsync() => Task.CompletedTask; // trong trường hợp ta sẽ không làm gì cả

    // vấn đề hiện tại là mỗi lần ta chạy test "GivenValidHabit_CreateHabit" thì nó sẽ thêm 1 record vào database
    // ta sẽ dùng thằng này clean up the database after running the test 
    // nó sẽ gửi 1 request để delete sau khi chạy 1 test  
    public async Task DisposeAsync()
    {
        var httpClient = _webApplicationFactory.CreateClient();
        foreach (var habitId in _habitIds)
        {
            await httpClient.DeleteAsync($"api/v1/habits/{habitId}");
        }
    }
}
```

* -> tạo 1 interface tại root trong project chính:
```cs
// thằng IApiMaker này là để dùng cho 'WebApplicationFactory', ta có thể dùng program.cs cũng được
public interface IApiMarker
{
}
```

* -> there is an infrastructure provision issue here, we have to make sure the database and all infrastructure was up and running and the data was in a specific state before we were able to run the tests
* -> also if multiple build pipelines were running in parallel, we had interference between the tests and the tests were flanky 
* => the Test Container library - allow us to bootstrap our integration tests against a real instance of a service running in a Docker container 

# Infrastructure provision - Test Container
* -> đầu tiên ta cần có custom **`WebApplicationFactory`**
```cs
public class ApiFactory : WebApplicationFactory<IApiMaker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // we can override things that we have wired up in our program.cs
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                service.Remove(descriptor);
            }
            var connectionString = string.Empty;
            services.AddDbContext<ApplicationDbContext>(opts => opts.UseSqlServer(connectionString));
        });
    }
}
```

=============================================================================
# NUnit for Integration Test -  Test a repository that queries data using Entity Framework Core

```cs - origin project
public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public interface IProductRepository
{
    Product GetById(int productId);
    void Add(Product product);
    void SaveChanges();
}
public class ProductRepository : IProductRepository
{
    private ProductContext context;

    public ProductRepository(ProductContext context)
    {
        this.context = context;
    }

    public Product GetById(int productId)
    {
        return context.Products.Find(productId);
    }

    public void Add(Product product)
    {
        context.Products.Add(product);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}
```

```cs - test project
using System.Data.Entity;
using System.Linq;
using Moq;
using NUnit.Framework;

public class ProductIntegrationTests
{
    [Test]
    public void AddProductAndRetrieveFromDatabase()
    {
        // Arrange
        var fixture = new Fixture();
        var product = fixture.Create<Product>(); // use 'AutoFixture' library that allows us to create our test objects easily
        
        // simulate a database using Entity Framework Core
        var dbContextOptions = new DbContextOptionsBuilder<ProductContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        // added the product to the database
        using (var context = new ProductContext(dbContextOptions))
        {
            var productRepository = new ProductRepository(context);
            productRepository.Add(product);
            productRepository.SaveChanges();
        }

        using (var context = new ProductContext(dbContextOptions))
        {
            var productRepository = new ProductRepository(context);

            // Act
            var retrievedProduct = productRepository.GetById(product.ProductId);

            // Assert
            Assert.IsNotNull(retrievedProduct);
            Assert.AreEqual(product.ProductId, retrievedProduct.ProductId);
            Assert.AreEqual(product.Name, retrievedProduct.Name);
            Assert.AreEqual(product.Price, retrievedProduct.Price);
        }
    }
}
```
