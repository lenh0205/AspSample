==================================================================================
# Fundermental

## Lifetime
* _for in-depth understanding: `https://web.archive.org/web/20200724100912/http://stephenwalther.com/archive/2008/03/18/asp-net-mvc-in-depth-the-life-of-an-asp-net-mvc-request`_
* _but in overview, the life-cycle steps will include:_

1. the **`RouteTable`** is Created - this first step happens **only once when an ASP.NET application first starts** (**`singleton`**) (_the RouteTable maps URLs to handlers_)

2. **whenever a request is made** (basically **`scoped`**), The **`UrlRoutingModule`** intercepts every request, creates and executes the right handler

3. the **`MvcHandler`** (basically **`scoped`**) **creates a controller**, passes the controller a ControllerContext, and executes the controller

4. the **`Controller`** (basically **`scoped`**) determines which method to execute, builds a list of parameters, and executes the method

5. typically, a controller method calls RenderView() to render content back to the browser. The Controller.RenderView() method delegates its work to a particular ViewEngine

## Summary 
> đúng là không nên viết hết logic trong controller vì như vậy khá khó nhìn vậy nên ta sẽ cho 1 lớp nữa là lớp Services
> lớp Service này đơn giản là tách logic ra cho dễ nhìn nên ta sẽ không viết interface và config DI từng thằng vào controller sẽ không hợp lý
> vậy nên ta sẽ tạo 1 thằng Factory để tạo từng Service cần thiết tương ứng với từng Controller cụ thể; vì controller là Scoped nên ta cũng sẽ DI Factory là Scoped
> ta sẽ truy cập cập service trong Factory dưới dạng property, nhưng ta sẽ không tạo sẵn instance cho các service (s/d **=**) mà chỉ tạo instance khi access property (s/d **=>**)
> Interface của Factory sẽ có property với type là interface của Service, còn Implement của Factory sẽ có property với type là interface của service nhưng ta sẽ gán giá trị bằng implement của service
> đồng thời khi khởi tạo instance cho service ta cần pass IServiceProvider mà Factory nhận được thông qua DI; đồng thời cache lại instance để tránh tạo lại mỗi lần access property trong 1 Scope
> các logic method, property xài chung giữa các service thì ta sẽ bỏ vào abstract class BaseService  
> còn những member riêng thì ta sẽ để riêng - việc này đảm bảo khi ta truy cập service từ Factory nó sẽ cho ta biết method cụ thể của service đó

> các lớp service cần có khả năng sử dụng UnitOfWork để truy cập bất cứ repository nào 1 cách tuỳ ý; vậy nên ta sẽ DI scope thằng này
> UnitOfWork sẽ cần được DI DbContext
> trong trường hợp có những thằng Services cần sử dụng những logic lặp đi lặp lại thì sao - thiết nghĩ ta nên tạo 1 ShareService để kế thừa

> nếu ApplicationDbContext : DbContext vậy thì typeof(ApplicationDbContext) == typeof(DbContext) ? có thể pass ApplicationDbContext instance cho method(DbContext db) ?

## Note
* -> đối với việc DI 1 Factory, ta cần xem Factory này sẽ tạo ra những instances với lifetime như thế nào ?

## Reference
> cái gì làm nên sự phân biệt giữa controller này và controller kia ? (không lẽ là model)
> https://github.com/ardalis/CleanArchitecture/tree/main
> https://github.com/iayti/CleanArchitecture/tree/master/src/Apps/CleanArchitecture.Api
> https://github.com/iammukeshm/CleanArchitecture.WebApi/blob/master/Application/Interfaces/IAccountService.cs

==================================================================================
# Flow
* -> Controller (derive Generic Controller) use Seed Service 
* -> Seed Service access Service property 
* -> Service property return Service
* -> Service has methods use Unit Of Work
* -> Unit Of Work access Repository property 
* -> Repository property return Repository 
* -> Repository (derive Generic Repository) has methods solve DAL

```cs
/// <summary>
/// Controller (Apply for direct interaction with "T" model in Database)
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class DanhSachHoSoController : GenericController<Guid, DanhSachHoSo, ApplicationDbContext>
{
    private readonly ISeedService _service;
    public DanhSachHoSoController(
        ISeedService service, 
        IGenericService<DanhSachHoSo, ApplicationDbContext> genericService
    ) : base(genericService) 
    {
        _service = service;
    }

    // inherit Method from "genericService" -> using generic "UnitOfWork" -> using "GenericRepository"

    // ... write individual Method use "DanhSachHoSoService" from "SeedService"
    // -> có thể s/d "base.Create(entity);" để tái sử dụng method của genericController 
}

/// <summary>
/// Service
/// </summary>
public class DanhSachHoSoService : IDanhSachHoSoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    public DanhSachHoSoService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger, IConfiguration configuration, ICommonServices CommonServices)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _configuration = configuration;
    }

    // ... write Method use "DanhSachHoSo" repository from "UnitOfWork" 
}

/// <summary>
/// Repository
/// </summary>
public class DanhSachHoSoRepository 
{
    public DanhSachHoSoRepository(ApplicationDbContext context)
    {
    }
    // Implement Method
}


/// <summary>
/// Controller (Apply for "T" model of connective service)
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class VanBanDiController : ControllerBase
{
    private readonly ISeedService _service;
    public VanBanDiController(ISeedService service)
    {
        _service = service;
    }

    //....
}
```