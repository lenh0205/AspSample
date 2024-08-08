
/// <summary>
/// Repository:
/// </summary>
public interface IHoSoCongViecRepository : IGenericRepository<HoSoCongViec, ApplicationDbContext>
{
    // Add Method
    Task<(int, object?)> GetSearchBySearchGroupObject(SearchGroupObjectRequest search);
}

public class HoSoCongViecRepository : GenericRepository<HoSoCongViec, ApplicationDbContext>, IHoSoCongViecRepository
{

    public HoSoCongViecRepository(ApplicationDbContext context) : base(context)
    {
    }
    // Implement Method
    public async Task<(int, object?)> GetGroupBySearch(HoSoCongViecBySearchRequest request)
    {
        //...
        
        // IQueryable result = <our_query_command>
        var totalRow = await result.CountAsync();
        return new(totalRow, await result.ToListAsync());
    }
}


/// <summary>
/// Generic Repository
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IGenericRepository<TEntity> where TEntity : class
{
    List<TEntity> GetList(
        Expression<Func<TEntity, bool>> filter = null!, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!, 
        string includeProperties = "", 
        int pageIndex = 0, 
        int pageSize = 0
    );
    
    IQueryable<TEntity> GetQuery(
        Expression<Func<TEntity, bool>> filter = null!, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!, 
        string includeProperties = ""
    );
    TEntity GetByID(object? id);
    
    void Insert(TEntity entity);
    
    void InsertRange(IEnumerable<TEntity> entities);
    
    void TryDelete(object id);
    
    void Delete(TEntity entity);
    
    public void DeleteRange(IEnumerable<TEntity> entities);

    void Update(TEntity entityToUpdate);
    Task InsertAsync(TEntity entity);
    Task InsertRangeAsync(IEnumerable<TEntity> entitiesToInsert);
}


// Get "List of entity": .Where(), .Include() , orderBy() , .Skip(), .Take(), .ToList()
// Create a "Query" to get List of entity
// Get "Entity" by ID value: .Find()
// Insert 1 Entity (Async): .Add()
// Insert 1 list Entity (Async): .AddRange()
// Xoá 1 Entity: .Find() , check .Entry() = Detach, .Attach(), .Remove()
// Xoá nhiều Entity: .RemoveRange()
// Update 1 Entity: .Attach(), .Entry() = Modified

using Microsoft.EntityFrameworkCore;
using SER.Infrastructure.IEntityRepositories.Base;
using System.Linq.Expressions;
namespace SER.Infrastructure.EntityRepositories.Base;

public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    internal TContext _context;
    private DbSet<TEntity> _dbSet;
    public GenericRepository(TContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public virtual IEnumerable<TEntity> GetWithRawSql(string query, params object[] parameters)
    {
        return dbSet.SqlQuery(query, parameters).ToList();
    }

    public virtual List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null!,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!,
        string includeProperties = "", int PageIndex = 0, int PageSize = 0)
    // có 5 tham số:
    {
        IQueryable<TEntity> query = _dbSet;

        // Filter: Expression<Func<MyEntity, bool>> filter = x => x.SomeProperty == "SomeValue";
        if (filter != null)
        {
            query = query.Where(filter);
        }

        // Eager Loading: string includeProperties = "RelatedEntity1,RelatedEntity2";
        // Assumpt: "RelatedEntity1" and "RelatedEntity2" is "navigation property" of Entity
        foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        // Order: Func<IQueryable<MyEntity>, IOrderedQueryable<MyEntity>> orderBy = q => q.OrderBy(x => x.SomeOtherProperty);
        if (orderBy != null)
        {
            query = orderBy(query);
        }

        // Pagination: int pageIndex = 1; int pageSize = 10;
        if (PageIndex > 0 && PageSize > 0)
        {
            return query.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList();
        }
        else
        {
            return query.ToList();
        }
    }

    public virtual IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter = null!,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!, string includeProperties = "")
    // "!" để tắt warning param "may be null", mặc dù "default value" của nó là null =))
    // có 3 tham số:
    {
        IQueryable<TEntity> query = _dbSet;

        // Filter:
        if (filter != null)
        {
            query = query.Where(filter);
        }

        // Eager Loading:
        foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        // Order:
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        return query;
    }

    public virtual TEntity GetByID(object? id)
    {
        // with List<T>, Tìm phần tử đầu tiên thoả điều kiện / trả về "giá trị mặc định" của kiểu
        // with DbSet<TEntity>, find an entity with the given "primary key" values / trả về "null"
        return _dbSet.Find(id)!; 
        // Find is method on DbSet; kiểm tra đã được load trong context chưa nếu rồi thì khỏi query database
        // FirstOrDefault is Extension method on IQueryable ; luôn query database dù entity đã được load
        
        // Find tìm dựa trên Id value; FirstOrDefault dựa trên condition và có thể include related data


        // FirstOrDefault có thể dùng đối số thứ 2 để thay đổi giá trị mặc định trả về
    }

    public virtual void Insert(TEntity entity)
    {
        _dbSet.Add(entity);
        // for Insert 1 Entity into the database when "SaveChanges" is called:
        // "primary key" exist -> thrown exception
        // "foreign key" not exist in related table -> thrown exception
    }
    public virtual void InsertRange(IEnumerable<TEntity> entitiesToInsert)
    {
        _dbSet.AddRange(entitiesToInsert);
        // for Insert multiple entities to the database at the same time when "SaveChanges" is called:
        // "primary key" exist -> thrown exception
        // "foreign key" not exist in related table -> thrown exception
    }
    public async virtual Task InsertAsync(TEntity entity) => await _dbSet.AddAsync(entity);
    public async virtual Task InsertRangeAsync(IEnumerable<TEntity> entitiesToInsert) => await _dbSet.AddRangeAsync(entitiesToInsert);

    public virtual void TryDelete(object id)
    {
        TEntity entityToDelete = _dbSet.Find(id)!;

        if (entityToDelete != null)
            Delete(entityToDelete);
    }
    public virtual void Delete(TEntity entityToDelete)
    {
        // this check'll use when the entity exist in database, but not detach to the context
        if (_context.Entry(entityToDelete).State == EntityState.Detached)
        {
            _dbSet.Attach(entityToDelete);
            // "Remove" method on DbSet can only be used to remove entities that are being tracked by the context
            // nếu không nó sẽ throw error
            // Nếu 1 entity đã attach mà ta ".Attach()" thì ko có ảnh hưởng gì
        } 
        _dbSet.Remove(entityToDelete);
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entitiesToDelete)
    {
        _dbSet.RemoveRange(entitiesToDelete);
    }

    public virtual void Update(TEntity entityToUpdate)
    {   // để update được cần lấy đúng entity với ID cụ thể
        _dbSet.Attach(entityToUpdate);
        _context.Entry(entityToUpdate).State = EntityState.Modified;
    }
}

