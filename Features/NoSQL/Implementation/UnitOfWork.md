
# Config

```cs - program.cs
services.AddScoped<IMongoDbContext, MongoDbContext>();
services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
```

```json - appsettings.json
{
    "MongoDB": {
        "ConnectionURI": "mongodb://localhost:27017/",
        "DatabaseName": "Test",
        "CollectionName": "Item"
    }
}
```

# DbContext

```cs
public interface IMongoDbContext
{
    void AddCommand(Func<Task> func);
    Task<int> SaveChanges();
    IMongoCollection<T> GetCollection<T>(string name);
    void Dispose();
}

public class MongoDbContext : IMongoDbContext
{
    private IMongoDatabase Database { get; set; }
    public IClientSessionHandle Session { get; set; }
    public MongoClient MongoClient { get; set; }
    private readonly List<Func<Task>> _commands;
    private readonly IConfiguration _configuration;

    public MongoDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _commands = new List<Func<Task>>();
    }

    private void ConfigureMongo()
    {
        if (MongoClient != null)
        {
            return;
        }

        MongoClient = new MongoClient(_configuration["MongoSettings:Connection"]);

        Database = MongoClient.GetDatabase(_configuration["MongoSettings:DatabaseName"]);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        ConfigureMongo();
        return Database.GetCollection<T>(name);
    }

    public async Task<int> SaveChanges()
    {
        ConfigureMongo();

        using (Session = await MongoClient.StartSessionAsync())
        {
            Session.StartTransaction();

            var commandTasks = _commands.Select(c => c());

            await Task.WhenAll(commandTasks);

            await Session.CommitTransactionAsync();
        }

        return _commands.Count;
    }

    public void Dispose()
    {
        Session?.Dispose();
        GC.SuppressFinalize(this);
    }

    public void AddCommand(Func<Task> func)
    {
        _commands.Add(func);
    }
}
```

# UnitOfWork

```cs
public interface IMongoUnitOfWork : IDisposable
{
    IProductRepository Product { get; }

    Task<bool> Commit();
}

public class MongoUnitOfWork : IMongoUnitOfWork
{
    private readonly IMongoDbContext _context;

    public MongoUnitOfWork(IMongoDbContext context)
    {
        _context = context;
    }

    public IProductRepository Product => new ProductRepository(_context);

    public async Task<bool> Commit()
    {
        var changeAmount = await _context.SaveChanges();

        return changeAmount > 0;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
```

# Repository

```cs - BaseRepository.cs
public interface IMongoRepository<TEntity> : IDisposable where TEntity : class
{
    void Add(TEntity obj);
    Task<TEntity> GetById(Guid id);
    Task<IEnumerable<TEntity>> GetAll();
    void Update(TEntity obj);
    void Remove(Guid id);
}

public abstract class BaseMongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class
{
    protected readonly IMongoDbContext Context;
    protected IMongoCollection<TEntity> DbSet;

    protected BaseMongoRepository(IMongoDbContext context)
    {
        Context = context;
        DbSet = Context.GetCollection<TEntity>(typeof(TEntity).Name);
    }

    public virtual void Add(TEntity obj)
    {
        Context.AddCommand(() => DbSet.InsertOneAsync(obj));
    }

    public virtual async Task<TEntity> GetById(Guid id)
    {
        var data = await DbSet.FindAsync(Builders<TEntity>.Filter.Eq("_id", id));
        return data.SingleOrDefault();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAll()
    {
        var all = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);
        return all.ToList();
    }

    public virtual void Update(TEntity obj)
    {
        Context.AddCommand(() => DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", obj.GetId()), obj));
    }

    public virtual void Remove(Guid id)
    {
        Context.AddCommand(() => DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id)));
    }

    public void Dispose()
    {
        Context?.Dispose();
    }
}
```

```cs - ProductRepository.cs
public interface IProductRepository : IMongoRepository<Product>
{
    string TestProductRepository();
}

// Implementation
public class ProductRepository : BaseMongoRepository<Product>, IProductRepository
{
    public ProductRepository(IMongoDbContext context) : base(context)
    {
    }

    public string TestProductRepository()
    {
        return "TestProductRepository";
    }
}
```