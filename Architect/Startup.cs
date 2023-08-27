/// Architect
// -> Controller (derive Generic Controller) use Seed Service 
// -> Seed Service access Service property 
// -> Service property return Service
// -> Service has methods use Unit Of Work
// -> Unit Of Work access Repository property 
// -> Repository property return Repository 
// -> Repository (derive Generic Repository) has methods solve DAL


/// Startup:
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
    //sử dụng cho kiểu geometry
    //x => x.UseNetTopologySuite());
});
var config = new MapperConfiguration(cfg =>
{
    cfg.AllowNullCollections = true;
    cfg.AddProfile(new MappingProfile());
    //cfg.AddProfile<MappingProfile>();
});
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services
.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>))
.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>))
.AddScoped(typeof(IUnitOfWork<,>), typeof(UnitOfWork<,>))
.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork))
.AddScoped(typeof(ISeedService), typeof(SeedService));
builder.Services.AddScoped<IExternalApiServices, ExternalApiServices>();

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var myDependency = services.GetRequiredService<IExternalApiServices>();

    myDependency.GetDataFromConfig();
    //Use the service
    myDependency.ReceiveExternalData();

}



