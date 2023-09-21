
/// <summary>
/// Generic Service
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TContext"></typeparam>
public interface IGenericService<TEntity, TContext>
    where TEntity : class
    where TContext : DbContext
{
    List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null!, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!, string includeProperties = "", int PageIndex = 0, int PageSize = 0);
    TEntity GetByID(object id);
    void Insert(TEntity entity);
    void InsertRange(IEnumerable<TEntity> entities);
    void TryDelete(object id);
    void Update(TEntity entityToUpdate);
}
public class GenericService<TEntity, TContext> : IGenericService<TEntity, TContext>
    where TEntity : class
    where TContext : DbContext
{
    internal ILogger _logger;
    internal IUnitOfWork<TEntity, TContext> _unitOfWork;
    public GenericService(ILogger<GenericService<TEntity, TContext>> logger, IUnitOfWork<TEntity, TContext> unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null!, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!, string includeProperties = "", int PageIndex = 0, int PageSize = 0)
    {
        try
        {
            return _unitOfWork.Generic.GetList(filter, orderBy, includeProperties, PageIndex, PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"{nameof(GetList)} function error on {nameof(GenericService<TEntity, TContext>)}");
            throw;
        }
    }
    public TEntity GetByID(object id)
    {
        try
        {
            return _unitOfWork.Generic.GetByID(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"{nameof(GetByID)} function error on {nameof(GenericService<TEntity, TContext>)}");
            throw;
        }
    }
    public void Insert(TEntity entity)
    {
        try
        {
            _unitOfWork.Generic.Insert(entity);
            _unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"{nameof(Insert)} function error on {nameof(GenericService<TEntity, TContext>)}");
            throw;
        }
    }
    public void InsertRange(IEnumerable<TEntity> entitiesToDelete)
    {
        try
        {
            _unitOfWork.Generic.InsertRange(entitiesToDelete);
            _unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"{nameof(Insert)} function error on {nameof(GenericService<TEntity, TContext>)}");
            throw;
        }
    }
    public void TryDelete(object id)
    {
        try
        {
            _unitOfWork.Generic.TryDelete(id);
            _unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"{nameof(TryDelete)} function error on {nameof(GenericService<TEntity, TContext>)}");
            throw;
        }
    }
    public void Update(TEntity entityToUpdate)
    {
        try
        {
            _unitOfWork.Generic.Update(entityToUpdate);
            _unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"{nameof(Update)} function error on {nameof(GenericService<TEntity, TContext>)}");
            throw;
        }
    }

}

/// <summary>
/// CommonService - ta định nghĩa 1 số services dùng chung cho các service khác gọi tới
/// Các Class này ta sẽ không đăng ký trong ISeedService, mà config trong program.cs để "Injection"
/// </summary>
public interface ICommonServices
{
}
public class CommonServices : ICommonServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly Microsoft.Extensions.Logging.ILogger _logger;
    private readonly IConfiguration _configuration;
    public CommonServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _configuration = configuration;
    }
    // ....
}

/// <summary>
/// Specific Service
/// </summary>
public interface IHoSoCongViecService
{
    object? GetAll();
    object? GetEntity(Guid id);
    Task<object?> CreateEntity(HoSoCongViecCreateRequest request);
    Task<object?> UpdateEntity(HoSoCongViecUpdateRequest request);
    Task<object?> SoftDelete(Guid id);
    Task<object?> GetSearchBySearchGroupObject(SearchGroupObjectRequest search);
}

public class HoSoCongViecService : IHoSoCongViecService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly Microsoft.Extensions.Logging.ILogger _logger;
    private readonly IConfiguration _configuration;
    public HoSoCongViecService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _configuration = configuration;
    }
    
    public object? GetAll()
    {
        try
        {
            var entitys = _unitOfWork.HoSoCongViec.GetList();
            var result = _mapper.Map<IEnumerable<HoSoCongViec>, IEnumerable<HoSoCongViecResponse>>(entitys);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"{nameof(GetAll)} function error on {nameof(HoSoCongViecService)}");
            throw;
        }
    }

    public object? GetEntity(Guid id)
    {
        try
        {
            var entity = _unitOfWork.HoSoCongViec.GetByID(id);
            var result = _mapper.Map<HoSoCongViec, HoSoCongViecResponse>(entity);
            if (result.IsbaoQuanVinhVien == false)
            {
                var dateTime = result.NgayMo;
                result.NgayKetThuc = dateTime.Value.AddYears((int)result.ThoiGianBaoQuan);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"{nameof(GetEntity)} function error on {nameof(HoSoCongViecService)}");
            throw;
        }
    }

    public async Task<object?> CreateEntity(HoSoCongViecCreateRequest request)
    {
        try
        {
            var entity = _mapper.Map<HoSoCongViecCreateRequest, HoSoCongViec>(request);
            if (entity == null)
                return entity;
            await _unitOfWork.HoSoCongViec.InsertAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"{nameof(CreateEntity)} function error on {nameof(HoSoCongViecService)}");
            throw;
        }
    }

    public async Task<object?> UpdateEntity(HoSoCongViecUpdateRequest request)
    {
        try
        {

            var entity = _unitOfWork.HoSoCongViec.GetByID(request.Id);

            if (entity == null)
                return entity;

            _mapper.Map(request, entity);

            _unitOfWork.HoSoCongViec.Update(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"{nameof(UpdateEntity)} function error on {nameof(HoSoCongViecService)}");
            throw;
        }
    }

    public async Task<object?> SoftDelete(Guid id)
    {
        try
        {
            var entity = _unitOfWork.HoSoCongViec.GetByID(id);
            if (entity == null)
                return entity;
            entity.IsDeleted = true;

            _unitOfWork.HoSoCongViec.Update(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"{nameof(SoftDelete)} function error on {nameof(HoSoCongViecService)}");
            throw;
        }
    }
    
    public async Task<object?> GetSearchBySearchGroupObject(SearchGroupObjectRequest search)
    {
        try
        {
            var entity = await _unitOfWork.HoSoCongViec.GetSearchBySearchGroupObject(search);
            return new { TotalRow = entity.Item1, Data = entity.Item2 };
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message, $"{nameof(GetSearchBySearchGroupObject)} function error on {nameof(HoSoCongViecService)}");
            throw;
        }
    }

}
