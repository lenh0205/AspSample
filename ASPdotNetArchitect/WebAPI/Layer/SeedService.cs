/// <summary>
/// Seed Service:
/// </summary>
public interface ISeedService {
    IHoSoService HoSoService { get; }
    // ....
}

public class SeedService : ISeedService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public SeedService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerFactory loggerFactory, IConfiguration config) {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = loggerFactory.CreateLogger("logs");
        _config = config;
    }

    private IHoSoCongViecService? _hoSoCongViec { get; set; }
    public IHoSoCongViecService HoSoCongViecService => _hoSoCongViec ?? new HoSoCongViecService(_unitOfWork, _mapper, _logger,_config);
    // .....
}
