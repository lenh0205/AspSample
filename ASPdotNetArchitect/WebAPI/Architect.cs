/// Architect
// -> Controller (derive Generic Controller) use Seed Service 
// -> Seed Service access Service property 
// -> Service property return Service
// -> Service has methods use Unit Of Work
// -> Unit Of Work access Repository property 
// -> Repository property return Repository 
// -> Repository (derive Generic Repository) has methods solve DAL


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