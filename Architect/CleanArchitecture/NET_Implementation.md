
========================================================================

# Triển khai với ASP.NET
* -> _thường thì 4 layers khi triển khai 'Clean Architect" trong ASP.NET sẽ là: **Domain**, **Application**, **Infrastructure**, **Presentation**_
* -> Clean Architecture enforces strict layering with **`inward-pointing dependencies`**
* _**Application Layer** và **Domain Layer** are always **`the core`** of system's design_
* _the outer layers (**Presentation** or **Infrastructure**) **`depend on abstractions defined by the inner layers`** ( **Application** or **Domain**)_

<img src="../nonrelated/clean_architect_asp.jpg">

## Domain Layer ('Entities' Layer in Clean Architecture)
* -> contains the **business logic** (_like the **`entities`** and **`specifications`**_)
* -> the **application entities** - which are the **`application model classes`** or **`database model classes`**
* -> with **`no dependencies on other layers`**

## Application Layer
* -> contains all **application logic**
* -> in this layer, **`services interfaces`** are kept separate from their **`implementation`** (_for loose coupling and separation of concerns_)
* -> also **`define interfaces for the outer layers`**
* -> **`depends on the Domain layer`**, but **`not directly on Presentation or Infrastructure layers`**

## Infrastructure Layer ('Framework & Driver Layer' in Clean Architect)
* -> **`implements interfaces from the Application layer`** - dealing with data access, file systems, network,...

## Presentation Layer ('Framework & Driver Layer' in Clean Architect)
* -> presents us the object data from the database using the HTTP request in the form of JSON Object
* -> but in the case of front-end applications, we present the data using the UI by consuming the APIS
* -> **`interacting with the Application layer`**

https://medium.com/codenx/code-in-clean-vs-traditional-layered-architecture-net-31c4cad8f815#:~:text=Domain%20Layer%3A%20Contains%20the%20business,on%20Presentation%20or%20Infrastructure%20layers.
https://medium.com/c-sharp-progarmming/tutorial-net-5-clean-architecture-4cc900f7945b

========================================================================
# Triển khai "Domain Layer" in ASP.NET
* -> tạo 1 project mới với template là **Class Library**

## "Models" folder
* -> create the **`database entities`**

## "Interface" folder
* -> add the **`interfaces of the entities`** that we want to add the specific methods in our interface

* _VD: IUnitOfWork.cs, IGenericRepository, IProductRepository, ITokenService.cs, IOrderService, ICustomerBasket.cs_
```cs
public interface ICustomerBasket
{
    Task<CustomerBasket> GetBasketAsync(string basketId);
    Task<CustomerBaseket> UpdateBasketAsync(CustomerBasket basket);
    Task<bool> DeleteBasketAsync(string basketId);
}
```

## "Specification" folder
* -> add all the **`specifications`**

```r - Ex: 
// if we want the result of the API  
// in ascending or in descending Order
// or want the result in the specific criteria
// or want the result in the form of pagination then we need to add the specification class
```

```cs - Ex: ISpecifications.cs 
public interface ISpecifications < T > {
    Expression <Func<T,bool>> Criteria { get; }
    List <Expression<Func<T, object>>> Includes { get; }
    Expression<Func<T, object>> OrderBy { get; }
    Expression<Func<T, object>> OrderByDescending { get; }
    int Take { get; }
    int Skip { get; }
    bool isPagingEnabled { get; }
}
```
```cs - Ex: BaseSpecification.cs
public class BaseSpecification < T > : ISpecifications < T > 
{
    public Expression<Func<T,bool >> Criteria { get; }
    public BaseSpecification() {}
    public BaseSpecification(Expression < Func < T, bool >> Criteria) {
        this.Criteria = Criteria;
    }
    public List<Expression<Func <T, object>>> Includes { get; } 
        = new List <Expression<Func<T,object>>> ();
    public Expression <Func<T,object >> OrderBy { get; private set;}
    public Expression <Func<T, object>> OrderByDescending { get; private set; }

    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool isPagingEnabled { get; private set; }

    protected void AddInclude(Expression < Func < T, object >> includeExpression) {
        Includes.Add(includeExpression);
    }
    public void AddOrderBy(Expression < Func < T, object >> OrderByexpression) {
        OrderBy = OrderByexpression;
    }
    public void AddOrderByDecending(Expression < Func < T, object >> OrderByDecending) {
        OrderByDescending = OrderByDecending;
    }
    public void ApplyPagging(int take, int skip) {
        Take = take;
        //Skip = skip;
        isPagingEnabled = true;
    }
}
```

========================================================================
# Triển khai "Application Layer" in ASP.NET 
* -> tạo 1 project mới với template là **Class Library**
* -> đồng thời **add reference of the Domain Layer** in the application layer

## 'ICustomServices'
* -> the **ICustomServices** folder contain **`ICustomService interface`** that will be **`inherited by all the services`** we will add in our **`CustomerService folder`**

```cs
public interface ICustomService <T> {
    IEnumerable <T> GetAll();
    void FindById(int Id);
    void Insert(T entity);
    Task <T> Update(T entity);
    void Delete(T entity);
}
```

## 'CustomService' folder
* -> the **CustomServices** folder used to add the custom services to our system
* _VD: TokenService.cs, OrderService.cs_

========================================================================
# Triển khai "Infrastructure Layer" in ASP.NET 
* -> tạo 1 project mới với template là **Class Library**
* -> đồng thời **add reference of the Domain Layer** in the application layer

## "Data" folder
* -> used to add **`database context class`**

```cs
public class StoreContext: DbContext {
    public StoreContext(DbContextOptions < StoreContext > options): base(options) {}
    public DbSet <Products> Products { get; set; }
    public DbSet <ProductType> ProductTypes { get; set; }
    public DbSet <ProductBrand> ProductBrands { get; set; }
    public DbSet <Order> Orders { get; set; }
    public DbSet <DeliveryMethod> DeliveryMethods { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        if (Database.ProviderName == "Microsoft.EntityFramework.Sqlite") {
            foreach(var entity in modelBuilder.Model.GetEntityTypes()) {
                var properties = entity.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal));
                var dateandtimepropertise = entity.ClrType.GetProperties().Where(t => t.PropertyType == typeof(DateTimeOffset));
                foreach(var property in properties) {
                    modelBuilder.Entity(entity.Name).Property(property.Name).HasConversion < double > ();
                }
                foreach(var property in dateandtimepropertise) {
                    modelBuilder.Entity(entity.Name).Property(property.Name).HasConversion(new DateTimeOffsetToBinaryConverter());
                }
            }
        }
    }
}
```

## "Repositories" folder
* -> is used to **`add the repositories of the domain classes`**, because we are going to implement the **repository pattern** in our solution

```cs - BasketRepository.cs
public class BasketRepository: ICustomerBasket {
    private readonly IDatabase _database;
    public BasketRepository(IConnectionMultiplexer radis) {
        _database = radis.GetDatabase();
    }
    public async Task < bool > DeleteBasketAsync(string basketId) {
        return await _database.KeyDeleteAsync(basketId);
    }
    public async Task < CustomerBasket > GetBasketAsync(string basketId) {
        var data = await _database.StringGetAsync(basketId);
        return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize < CustomerBasket > (data);
    }
    public async Task < CustomerBasket > UpdateBasketAsync(CustomerBasket basket) {
        var created = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(15));
        if (!created) {
            return null;
        }
        return await GetBasketAsync(basket.Id);
    }
}
```

## "Migrations" folder
* -> create by using the package manager console and run the command Add-Migration

========================================================================
# Triển khai "Presentation Layer" in ASP.NET
* -> create project with template **ASP.NET Core Web API** 

## "Extensions" folder
* -> used for extension methods/classes

* _create `ApplicationServicesExtensions.cs` create the extension method for **`registering all services`** we have created during the entire project_
```cs
public static class ApplicationServicesExtensions {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
        services.AddScoped <ITokenService, TokenService> ();
        services.AddScoped <StoreContext, StoreContext> ();
        services.AddScoped <StoreContextSeed, StoreContextSeed> ();
        services.AddScoped <IProductRepository, ProductRepository> ();
        services.AddScoped <ICustomerBasket, BasketRepository> ();
        services.AddScoped <IUnitOfWork, UnitOfWork> ();
        services.AddScoped <IOrderService, OrderService> ();
        services.AddScoped(typeof(IGenericRepository <> ), typeof(GenericRepository <> ));
        services.Configure <ApiBehaviorOptions> (options => 
            options.InvalidModelStateResponseFactory = ActionContext => {
                var error = ActionContext.ModelState.Where(e => e.Value.Errors.Count > 0).SelectMany(e => e.Value.Errors).Select(e => e.ErrorMessage).ToArray();
                var errorresponce = new APIValidationErrorResponce {
                    Errors = error
                };
                return new BadRequestObjectResult(error);
            });
        return services;
    }
}

// Startup.cs
services.AddApplicationServices();
```

## "Helper" folder
* ->  create the **`auto mapper profiles`** - used for mapping the entities with each other

```cs
public class MappingProfiles: Profile {
    public MappingProfiles() {
        CreateMap < Products, ProductDto > ().
        ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name)).ForMember(p => p.ProductType, pt => pt.MapFrom(p => p.ProductType.Name)).ForMember(p => p.PictureUrl, pt => pt.MapFrom < ProductUrlResolvers > ());
        CreateMap < Core.Entities.Identity.Address, AddressDto > ();
        CreateMap < CustomerBasket, CustomerbasketDto > ();
        CreateMap < BasketItem, BasketItemDto > ();
        CreateMap < AddressDto, Core.Entities.OrderAggregate.Address > ();
    }
}
```

## DTOs
* -> create the **data transfer object** class for **`mapping the incoming request data`**

```cs
public class BasketItemDto {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
    }
```

## "Controllers" folder
* -> handling the HTTP request - interact with service layer and display the data to the users

```cs
public class BasketController: BaseApiController {
    private readonly ICustomerBasket _customerBasket;
    private readonly IMapper _mapper;
    public BasketController(ICustomerBasket customerBasket, IMapper mapper) {
            _customerBasket = customerBasket;
            _mapper = mapper;
        }

    [HttpGet(nameof(GetBasketElement))]
    public async Task <ActionResult<CustomerBasket>> GetBasketElement([FromQuery] string Id) {
        var basketelements = await _customerBasket.GetBasketAsync(Id);
        return Ok(basketelements ?? new CustomerBasket(Id));
    }

    [HttpPost(nameof(UpdateProduct))]
    public async Task <ActionResult<CustomerBasket>> UpdateProduct(CustomerBasket product) {
        //var customerbasket = _mapper.Map<CustomerbasketDto, CustomerBasket>(product);
        var data = await _customerBasket.UpdateBasketAsync(product);
        return Ok(data);
    }
    
    [HttpDelete(nameof(DeleteProduct))]
    public async Task DeleteProduct(string Id) {
        await _customerBasket.DeleteBasketAsync(Id);
    }
}
```