====================================================================================
# Case: using multiple database in Single ASP.NET Core project

## Migration

```bash
# Add migrations of "ExternalClientContext" to "~/Migrations/ExternalClient/" folder
Add-Migration ExternalClientData -Context ExternalClientContext
Update-Database -Context ExternalClientContext
```

```bash
# Add migrations of "ExternalClientContext" to "~/Migrations/RepositoryData/" folder
Add-Migration RepositoryData -Context RepositoryContext
Update-Database -Context RepositoryContext
```

## Config
```cs - "program.cs" in "CompanyEmployees" project/assembly

services.AddScoped<IRepositoryManager, RepositoryManager>();

services.AddDbContext<RepositoryContext>(opts =>
    opts.UseSqlServer(
        configuration.GetConnectionString("sqlConnection"), 
        b => b.MigrationsAssembly("CompanyEmployees")));

services.AddDbContext<ExternalClientContext>(opts =>
    opts.UseSqlServer(
        configuration.GetConnectionString("externalClientConnection"), 
        b => b.MigrationsAssembly("CompanyEmployees")));
```

```json - appsettings.json
{
    "ConnectionStrings": {
        "sqlConnection": "server=.; database=CompanyEmployeeDb1; Integrated Security=true",
        "externalClientConnection": "server=.; database=CompanyEmployeeDb2; Integrated Security=true"
    }
}
```

```cs - DbContext 1
public class RepositoryContext : DbContext
{
    public RepositoryContext(DbContextOptions<RepositoryContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Employee> Employees { get; set; }
}

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasData
        (
            new Company
            {
                Id = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"),
                Name = "IT_Solutions Ltd",
                Address = "583 Wall Dr. Gwynn Oak, MD 21207",
                Country = "USA"
            },
            new Company
            {
                Id = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"),
                Name = "Admin_Solutions Ltd",
                Address = "312 Forest Avenue, BF 923",
                Country = "USA"
            }
        );
    }
}
```

```cs - DbContext 2
public class ExternalClientContext : DbContext
{
    public ExternalClientContext(DbContextOptions<ExternalClientContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ClientConfiguration());
    }

    public DbSet<Client> Clients { get; set; }
}

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasData
        (
            new Client
            {
                Id = new Guid("c1f33503-bb38-4fa1-98a0-6cfaf9986797"),
                Name = "External Client's Test Name"
            }
        );
    }
}
```

## Usage
```cs
// CompaniesController.cs
[Route("api/companies")]
[ApiController]
public class CompaniesController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetCompanies()
    {
        var companies = _repository.Company.GetAllCompanies(trackChanges: false);

        var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

        return Ok(companiesDto);
    }
}

// RepositoryManager.cs
public interface IRepositoryManager
{
    ICompanyRepository Company { get; }
    IClientRepository Client { get; }

    void Save();
}
public class RepositoryManager : IRepositoryManager
{
    private RepositoryContext _repositoryContext;
    private ExternalClientContext _externalClientContext;

    private ICompanyRepository _companyRepository;
    private IClientRepository _clientRepository;

    public RepositoryManager(RepositoryContext repositoryContext, ExternalClientContext externalClientContext)
    {
        _repositoryContext = repositoryContext;
        _externalClientContext = externalClientContext;
    }

    public ICompanyRepository Company
    {
        get
        {
            if (_companyRepository is null)
                _companyRepository = new CompanyRepository(_repositoryContext);

            return _companyRepository;
        }
    }
    public IClientRepository Client
    {
        get
        {
            if (_clientRepository is null)
                _clientRepository = new ClientRepository(_externalClientContext);

            return _clientRepository;
        }
    }

    public void Save() => _repositoryContext.SaveChanges();
}

// CompanyRepository.cs
public interface ICompanyRepository
{
    IEnumerable<Company> GetAllCompanies(bool trackChanges);
}
public class CompanyRepository : RepositoryBase<RepositoryContext, Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
        FindAll(trackChanges)
        .OrderBy(c => c.Name)
        .ToList();
}

// RepositoryBase.cs
public interface IRepositoryBase<T>
{
    IQueryable<T> FindAll(bool trackChanges);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
}
public abstract class RepositoryBase<TContext, T> 
    : IRepositoryBase<T> where T : class where TContext : DbContext
{
    protected TContext _context;

    public RepositoryBase(TContext context) => this._context = context;

    public IQueryable<T> FindAll(bool trackChanges) =>
        !trackChanges ?
            _context.Set<T>()
            .AsNoTracking() :
            _context.Set<T>();

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,
    bool trackChanges) =>
        !trackChanges ?
            _context.Set<T>()
            .Where(expression)
            .AsNoTracking() :
            _context.Set<T>()
            .Where(expression);

    public void Create(T entity) => _context.Set<T>().Add(entity);

    public void Update(T entity) => _context.Set<T>().Update(entity);

    public void Delete(T entity) => _context.Set<T>().Remove(entity);
}
```