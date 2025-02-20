===============================================================
# Overview

## Reason
* -> improve **`code quality`**: require our code cover various scenarios; the code also need to be modular and maintainable for easily testing
* -> ensure making **`code changes`** did not break existing functionality

## Cách test
* -> ta chỉ test những gì **`critical`** và **`liên tục thay đổi`**; không cần test model, static things

* _ta sẽ cần nghĩ how would we actually test this ? what would we be testing for? why would we even have a test?_
* _cái ta cần suy nghĩ là kết quả mong muốn tương ứng với tham số truyền vào_
* _ý tưởng của Unit Test là việc ta chỉ cần bấm một nút và nó sẽ chạy cả ngàn Unit Test, xem sau khi ta thay đổi có test nào bị fail không_

===============================================================
# No Unit Test Framework

## AAA
* _1 test sẽ bao gồm **triple A**:_
* -> **`Arrange`** - preparing, go get your variables, your classes, functions, whatever you need for this test to run 
* ->  **`Act`** - Execute this function
* ->  **`Assert`** - whatever is returned is it what you want

## Note
* -> khi đặt tên cho test (method không có kiểu trả về) cần tuân theo convention: **`ClassName_MethodName_ExpectedResult`**
* -> ta sẽ cần điều chỉnh số lượng **Assertion** cho hợp lý, nếu quá nhiều thì nhiều khi gây khó chịu
* -> ta có thể dựa vào **fluent assertions** khi viết test, ví dụ viết test case cho method trả về string thì **`https://fluentassertions.com/strings/`**

```cs
// class with function we want to test
internal class DumpestFunction
{
    public string ReturnsPikachuIfZero(int num)
    {
        if (num == 0)
        {
            return "Pikachu";
        }
        else
        {
            return "Squirtle";
        }
    }
    
}
// -> Problem: what if another dev come in and change condition to "num == 1"

// write a test
 public class DumpestFunctionTests
 {
     public static void DumpestFunction_ReturnsPikachuIfZero_ReturnString()
     {
         // Arrange
         int num = 0;
         DumpestFunction func = new DumpestFunction();

         // Act
         string result = func.ReturnsPikachuIfZero(num);

         // Assert
         if (result == "Pikachu")
         {
             Console.WriteLine("PASSED: DumpestFunction_ReturnsPikachuIfZero_ReturnString");
         }
         else
         {
             Console.WriteLine("FAILED: DumpestFunction_ReturnsPikachuIfZero_ReturnString");
         }
     }
 }
```

===============================================================
# XUnit and Fluent Assertions
* -> **XUnit** cho phép ta truy cập vào **`Test Explorer`** (Visual Studio -> Test -> Test Explorer)
* -> **XUnit** có hỗ trợ ta **`Assert.`** để viết assertions, nhưng NuGet package **`FluentAssertions`** sẽ giúp việc viết assertions dễ hơn rất nhiều

## Create Unit Test project
* -> Trong solution, ta sẽ có project của ta rồi tạo thêm 1 project có template là **`xUnit Test Project`** với tên là tên của project cộng thêm **`.Test`** và reference đến project đó
* -> ta nên để Folder Structure giống như project gốc và thêm chữ Tests đằng sau
* -> giờ ta **Run All Tests** để xem project Test của ta đã hook vào được chưa

## Chạy Test
* -> ta sẽ **`VS -> Test -> Run All Tests`** để chạy
* -> khi ta chạy test, nó sẽ tự động tìm tất cả các Test và chạy cho ta

## Viết Test
* -> để đánh dấu 1 Test (method) trong xUnit ta cần có **`[Fact]`** hoặc **`[Theory]`** (nếu muốn pass data) attribute 
* -> **`InlineData`** (dùng với **[Theory]**) cho phép ta pass data vào param của Test method theo thứ tự

## Debug Test
* -> để debug a test, ta sẽ đặt break point bên trong test sau đó right-click vào test chọn **`Debug Tests`**

## Example

```cs - Test string, int, Date, object, IEnumerable
// test project
public class NetworkServiceTests
{
    private readonly NetworkService _pingService;
    public NetworkServiceTests()
    {
        _pingService = new NetworkService();
    }

    // test string
    [Fact]
    public void NetworkService_SendPing_ReturnString()
    {
        // Arrange - variables, classes, mocks

        // Act
        var result = _pingService.SendPing();

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Be("Success: Ping Sent");
        result.Should().Contain("Success", Exactly.Once());
    }

    // test int
    [Theory]
    [InlineData(1, 1, 2)]
    [InlineData(2, 2, 4)]
    public void NetworkService_PingTimeout_ReturnInt(int a, int b, int expected)
    {
        // Arrange

        // Act
        var result = _pingService.PingTimeout(a, b);

        // Assert
        result.Should().Be(expected);
        result.Should().BeGreaterOrEqualTo(2);
        result.Should().NotBeInRange(-1000, 0);
    }

    // test Date
    [Fact]
    public void NetworkService_LastPingDate_ReturnDate()
    {
        // Arrange - variables, classes, mocks

        // Act
        var result = _pingService.LastPingDate();

        // Assert
        result.Should().BeAfter(1.January(2018));
        result.Should().BeBefore(1.January(2030));
    }

    // test object
    [Fact]
    public void NetworkService_GetPingOptions_ReturnObject()
    {
        // Arrange
        var expected = new PingOptions()
        {
            DontFragment = true,
            Ttl = 1
        };

        // Act
        var result = _pingService.GetPingOptions();

        // Assert
        result.Should().BeOfType<PingOptions>();
        result.Should().BeEquivalentTo(expected);
        result.Ttl.Should().Be(1);
    }

    // test IEnumerable
    [Fact]
    public void NetworkService_MostRecentPings_ReturnIEnumerable()
    {
        // Arrange
        var expected = new PingOptions()
        {
            DontFragment = true,
            Ttl = 1
        };

        // Act
        var result = _pingService.MostRecentPings();

        // Assert
        result.Should().BeOfType<PingOptions[]>();
        result.Should().ContainEquivalentOf(expected);
        result.Should().Contain(x => x.DontFragment == true);
    }
}

/// origin project
public class NetworkService
{
    public string SendPing()
    {
        return "Success: Ping Sent";
    }

    public int PingTimeout(int a, int b)
    {
        return a + b;
    }

    public DateTime LastPingDate()
    {
        return DateTime.Now;
    }

    public PingOptions GetPingOptions()
    {
        return new PingOptions()
        {
            DontFragment = true,
            Ttl = 1
        };
    }

    public IEnumerable<PingOptions> MostRecentPings()
    {
        IEnumerable<PingOptions> pingOptions = new[]
        {
            new PingOptions()
            {
                DontFragment = true,
                Ttl = 1
            },
            new PingOptions()
            {
                DontFragment = true,
                Ttl = 1
            },
            new PingOptions()
            {
                DontFragment = true,
                Ttl = 1
            }
        };
        return pingOptions;
    }
}
```

===============================================================
# Mocking Framework - FakeItEasy
* -> cho phép ta fake các DI để khởi tạo instance từ class
* -> đảm bảo **consistence**: dữ liệu trên database là luôn thay đổi vậy nên lấy dữ liệu thực từ database để test là không thể (cùng input nhưng có thể khác output)

* -> it's almost impossible to unit test những abstraction như Repository layer nếu không có **mocking framework**
* -> ta sẽ không muốn mocking framework đụng tới database hay vào sâu trong abstraction (cái đó thuộc về integration testing hay end-to-end testing)
* -> 
* -> công việc của mocking framework là thay thế data ta cần từ abstraction và test method chính đang sử dụng data đó

## Step
* -> install **FakeItEasy**
* -> ta cần sử dụng **`A.Fake<T>()`** để fake các dependencies, rồi khởi tạo thủ công class ta cần test bằng những Dependencies này
* -> ta cần lưu ý là vì ta đã mock các dependency này, nên việc gọi vào các method này là không (Ex: dù method luôn trả về false nhưng khi test nó sẽ trả về mặc định là false vì thực tế nó không truy cập được)
* => nên để truy cập method này thì trong test ta cần fake nó, s/d **`A.CallTo(() => Dependency.Method()).Returns(ExpectedReturn)`**
* => vậy nên tất cả method ta muốn access từ Dependency được inject đều phải Fake nếu không lúc chạy test sẽ lỗi

```cs
// test project
public class NetworkServiceTests
{
    private readonly NetworkService _pingService;
    private readonly IDNS _dNS;
    public NetworkServiceTests()
    {
        // Dependencies
        _dNS = A.Fake<IDNS>();

        // SUT
        _pingService = new NetworkService(_dNS);
    }

    // test string
    [Fact]
    public void NetworkService_SendPing_ReturnString()
    {
        // Arrange - variables, classes, mocks
        A.CallTo(() => _dNS.SendDNS()).Returns(true);

        // Act
        var result = _pingService.SendPing();

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Be("Success: Ping Sent");
        result.Should().Contain("Success", Exactly.Once());
    }
}

// origin project
public class NetworkService
{
    private readonly IDNS _dNS;

    public NetworkService(IDNS dNS) 
    { 
        _dNS = dNS;
    }

    public string SendPing()
    {
        var dnsSuccess = _dNS.SendDNS();
        if (dnsSuccess)
        {
        return "Success: Ping Sent";
        }
        else
        {
            return "Failed: Ping not sent";
        }
    }
}

public interface IDNS
{
    public bool SendDNS();
}

public class DNSService : IDNS
{
    public bool SendDNS()
    {
        return true;
    }
}
```

===============================================================
# Unit Test for ASP.NET Core Web API
* -> ta cũng tạo project **`xUnit Test Project`** reference đến web API


```cs - Test Controller
// test project
public class PokemonControllerTests
{
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;
    public PokemonControllerTests()
    {
        _pokemonRepository = A.Fake<IPokemonRepository>();
        _reviewRepository = A.Fake<IReviewRepository>();
        _mapper = A.Fake<IMapper>();
    }

    [Fact]
    public void PokemonController_GetPokemons_ReturnOK()
    {
        //Arrange
        var pokemons = A.Fake<ICollection<PokemonDto>>();
        var pokemonList = A.Fake<List<PokemonDto>>();
        A.CallTo(() => _mapper.Map<List<PokemonDto>>(pokemons)).Returns(pokemonList);
        var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

        //Act
        var result = controller.GetPokemons();

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(OkObjectResult));
    }

    [Fact]
    public void PokemonController_CreatePokemon_ReturnOK()
    {
        //Arrange
        int ownerId = 1;
        int catId = 2;
        var pokemonMap = A.Fake<Pokemon>();
        var pokemon = A.Fake<Pokemon>();
        var pokemonCreate = A.Fake<PokemonDto>();
        A.CallTo(() => _pokemonRepository.GetPokemonTrimToUpper(pokemonCreate)).Returns(pokemon);
        A.CallTo(() => _mapper.Map<Pokemon>(pokemonCreate)).Returns(pokemon);
        A.CallTo(() => _pokemonRepository.CreatePokemon(ownerId, catId, pokemonMap)).Returns(true);
        var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

        //Act
        var result = controller.CreatePokemon(ownerId, catId, pokemonCreate);

        //Assert
        result.Should().NotBeNull();
    }
}

// origin project
[Route("api/[controller]")]
[ApiController]
public class PokemonController : Controller
{
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public PokemonController(IPokemonRepository pokemonRepository,
        IReviewRepository reviewRepository,
        IMapper mapper)
    {
        _pokemonRepository = pokemonRepository;
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    public IActionResult GetPokemons()
    {
        var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(pokemons);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto pokemonCreate)
    {
        if (pokemonCreate == null)
            return BadRequest(ModelState);

        var pokemons = _pokemonRepository.GetPokemonTrimToUpper(pokemonCreate);

        if (pokemons != null)
        {
            ModelState.AddModelError("", "Owner already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

        
        if (!_pokemonRepository.CreatePokemon(ownerId, catId, pokemonMap))
        {
            ModelState.AddModelError("", "Something went wrong while savin");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }
}
```

===============================================================
# Test EntityFramework Repository - Use InMemory Database
* -> để sử dụng **in-memory database** ta sẽ cần cài NuGet package **`Microsoft.EntityFrameworkCore.InMemory`**
* -> ta sẽ có **`1 private method sẽ create database context for our in-memory database`** 

```cs
// test project
public class PokemonRepositoryTests
{

    private async Task<DataContext> GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var databaseContext = new DataContext(options);
        databaseContext.Database.EnsureCreated();

        if (await databaseContext.Pokemon.CountAsync() <= 0)
        {
            for (int i = 1; i <= 10; i++)
            {
                databaseContext.Pokemon.Add(
                new Pokemon()
                {
                    Name = "Pikachu",
                    BirthDate = new DateTime(1903, 1, 1),
                    PokemonCategories = new List<PokemonCategory>()
                        {
                            new PokemonCategory { Category = new Category() { Name = "Electric"}}
                        },
                    Reviews = new List<Review>()
                        {
                            new Review { Title="Pikachu",Text = "Pickahu is the best pokemon, because it is electric", Rating = 5,
                            Reviewer = new Reviewer(){ FirstName = "Teddy", LastName = "Smith" } },
                            new Review { Title="Pikachu", Text = "Pickachu is the best a killing rocks", Rating = 5,
                            Reviewer = new Reviewer(){ FirstName = "Taylor", LastName = "Jones" } },
                            new Review { Title="Pikachu",Text = "Pickchu, pickachu, pikachu", Rating = 1,
                            Reviewer = new Reviewer(){ FirstName = "Jessica", LastName = "McGregor" } },
                        }
                });
                await databaseContext.SaveChangesAsync();
            }
        }
        return databaseContext;
    }

    [Fact]
    public async void PokemonRepository_GetPokemon_ReturnsPokemon()
    {
        //Arrange
        var name = "Pikachu";
        var dbContext = await GetDatabaseContext();
        var pokemonRepository = new PokemonRepository(dbContext);

        //Act
        var result = pokemonRepository.GetPokemon(name);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Pokemon>();
    }

    [Fact]
    public async void PokemonRepository_GetPokemonRating_ReturnDecimalBetweenOneAndTen()
    {
        //Arrange
        var pokeId = 1;
        var dbContext = await GetDatabaseContext();
        var pokemonRepository = new PokemonRepository(dbContext);

        //Act
        var result = pokemonRepository.GetPokemonRating(pokeId);

        //Assert
        result.Should().NotBe(0);
        result.Should().BeInRange(1, 10);
    }
}

// origin project
public class PokemonRepository : IPokemonRepository
{
    private readonly DataContext _context;

    public PokemonRepository(DataContext context)
    {
        _context = context;
    }

    public Pokemon GetPokemon(string name)
    {
        return _context.Pokemon.Where(p => p.Name == name).FirstOrDefault();
    }
    public decimal GetPokemonRating(int pokeId)
    {
        var review = _context.Reviews.Where(p => p.Pokemon.Id == pokeId);

        if (review.Count() <= 0)
            return 0;

        return ((decimal)review.Sum(r => r.Rating) / review.Count());
    }
}

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Pokemon> Pokemon { get; set; }
    public DbSet<PokemonOwner> PokemonOwners { get; set; }
    public DbSet<PokemonCategory> PokemonCategories { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Reviewer> Reviewers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PokemonCategory>()
                .HasKey(pc => new { pc.PokemonId, pc.CategoryId });
        modelBuilder.Entity<PokemonCategory>()
                .HasOne(p => p.Pokemon)
                .WithMany(pc => pc.PokemonCategories)
                .HasForeignKey(p => p.PokemonId);
        modelBuilder.Entity<PokemonCategory>()
                .HasOne(p => p.Category)
                .WithMany(pc => pc.PokemonCategories)
                .HasForeignKey(c => c.CategoryId);

        modelBuilder.Entity<PokemonOwner>()
                .HasKey(po => new { po.PokemonId, po.OwnerId });
        modelBuilder.Entity<PokemonOwner>()
                .HasOne(p => p.Pokemon)
                .WithMany(pc => pc.PokemonOwners)
                .HasForeignKey(p => p.PokemonId);
        modelBuilder.Entity<PokemonOwner>()
                .HasOne(p => p.Owner)
                .WithMany(pc => pc.PokemonOwners)
                .HasForeignKey(c => c.OwnerId);
    }
}
```

===============================================================
# Mocking Framework - NSubstitute
