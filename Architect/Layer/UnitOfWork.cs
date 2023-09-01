
/// <summary>
/// Unit Of Work
/// </summary>
public interface IUnitOfWork : IDisposable
{
 
    IHoSoCongViecRepository HoSoCongViec { get; }
    // .....

    #region Danh Mục
    #endregion
    public Task CommitAsync();
    public void Commit();
    public void CommitTransaction<TContext>() where TContext : DbContext;
    public void RollbackTransaction<TContext>() where TContext : DbContext;
    public void BeginTransaction<TContext>() where TContext : DbContext;
}

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private IConfiguration _config;
    public UnitOfWork(IConfiguration config, ApplicationDbContext context)
    {
        _config = config;
        _context = context;
    }

    #region SqlServer Repositories
    private IHoSoCongViecRepository? _hoSoCongViec { get; set; }
    public IHoSoCongViecRepository HoSoCongViec => _hoSoCongViec ?? new HoSoCongViecRepository(_context);
    
    // ...

    #endregion
    private bool disposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _context.Dispose();
                _transaction?.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    public void Commit()
    {
        _context.SaveChanges();
    }
    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
    public void BeginTransaction<TContext>() where TContext : DbContext
    {
        if (typeof(TContext) == typeof(ApplicationDbContext))
            _transaction = _context.Database.BeginTransaction();
    }
    public void CommitTransaction<TContext>() where TContext : DbContext => _transaction?.Commit();
    public void RollbackTransaction<TContext>() where TContext : DbContext => _transaction?.Rollback();
}


/// <summary>
/// Generic UnitOfWork
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TContext"></typeparam>
public interface IUnitOfWork<TEntity, TContext> : IDisposable
    where TEntity : class
    where TContext : DbContext
{

    //SqlServerCCKL
    IGenericRepository<TEntity, TContext> Generic { get; }
    public Task<int> CommitAsync();
    public void Commit();
    public void CommitTransaction();
    public void RollbackTransaction();
    public void BeginTransaction();
}
public class UnitOfWork<TEntity, TContext> : IUnitOfWork<TEntity, TContext>
    where TEntity : class
    where TContext : DbContext

{
    private TContext _context;
    private IDbContextTransaction? _transaction;
    private IConfiguration _config;
    public UnitOfWork(IConfiguration config, TContext context)
    {
        _config = config;
        _context = context;
    }

    #region SqlServer Repositories
    private IGenericRepository<TEntity, TContext>? InitGenericRepository { get; }
    public IGenericRepository<TEntity, TContext> Generic => InitGenericRepository ?? new GenericRepository<TEntity, TContext>(_context);
    #endregion
    private bool disposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _context.Dispose();
                _transaction?.Dispose();
            }
        }
        this.disposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    public void Commit()
    {
        _context.SaveChanges();
    }
    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public void BeginTransaction()
    {
        if (typeof(TContext) == typeof(ApplicationDbContext))
            _transaction = _context.Database.BeginTransaction();
    }
    public void CommitTransaction() => _transaction?.Commit();
    public void RollbackTransaction() => _transaction?.Rollback();
}

