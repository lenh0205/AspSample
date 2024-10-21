> đúng là không nên viết hết logic trong controller vì như vậy khá khó nhìn vậy nên ta sẽ cho 1 lớp nữa là lớp Services
> lớp này sẽ sử dụng UnitOfWork tuỳ ý để truy cập đến các repository khác nhau; thiết nghĩ nên DI scope thằng này
> UnitOfWork sẽ cần được DI DbContext
> trong trường hợp có những thằng Services cần sử dụng những logic lặp đi lặp lại thì sao - thiết nghĩ ta nên tạo 1 ShareService để kế thừa

> cái gì làm nên sự phân biệt giữa controller này và controller kia ? (không lẽ là model)
> https://github.com/ardalis/CleanArchitecture/tree/main
> https://github.com/iayti/CleanArchitecture/tree/master/src/Apps/CleanArchitecture.Api
> https://github.com/iammukeshm/CleanArchitecture.WebApi/blob/master/Application/Interfaces/IAccountService.cs

```cs
public interface IBusinessService<T> {}

public class VanBanDenService : IBusinessService<VanBanDen>, IVanBanDenService
{
    
}
```

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