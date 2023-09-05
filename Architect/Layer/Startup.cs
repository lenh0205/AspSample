var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//add services to container
builder.Services.AddControllers()
               .AddJsonOptions(options =>
               {
                   //config relationship reference json
                   options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

                   options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
               });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SER", Version = "v1" });
});


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

// Configure the HTTP request pipeline.
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true));
app.UseSwagger();
app.UseSwaggerUI(c =>
{
});
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.Run();